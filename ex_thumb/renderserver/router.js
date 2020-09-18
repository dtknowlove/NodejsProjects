var fs = require('fs');
const path = require('path');
const State = require('./module/State');
const Status = require('./module/Status');
var DB = require('./DB');
var TaskQueue = require('./TaskQueue');
var Message = require('./module/Message');
var utility = require('./utility');
var TaskInfo = require('./module/TaskInfo');
var TimeSegs = require('./module/TimeSegs');

var ip = utility.getIPAdress();
var port = 8081;

function home(req, res) {
    res.sendFile(utility.platformPath(__dirname + "/html/index.html"));
}

function state(req, res) {
    var r = TaskQueue.getStateData();
    res.end(`Current tasks count:${r.tCount} Current rundant pip:${r.rCount}`);
}

function query(req, res) {
    console.log("query:", req.query);
    var num = req.query['num'];
    var endInfo = {
        "dblength": DB.Count(),
        "num": num,
        "average": "",
        "max": "",
        "min": "",
        "segsStatistics": "",
        "items": null,
        "message": ""
    };
    if (num) {
        var showLength = num > endInfo.dblength ? endInfo.dblength : num;
        var start = endInfo.dblength - showLength;
        var infos = DB.GetInfos(start, endInfo.dblength);
        var showItems = [];
        var segInfos = [];
        if (infos) {
            //总时间统计
            var sum = 0, max = 0;
            min = infos.length > 0 ? (infos[0].runtime != null ? infos[0].runtime : 0) : 0;
            infos.forEach(t => {
                var runtime = t.runtime == null ? 0 : t.runtime;
                if (runtime != 0 && runtime < TaskQueue.timeout) {
                    max = max < runtime ? runtime : max;
                    min = min > runtime ? runtime : min;
                    sum += runtime;
                    //分段时间处理
                    var segs = TaskInfo.ProcessStamp(t.segments);
                    if (segs != null) {
                        segs.forEach(f => { segInfos.push(f) });
                    }
                    showItems.push({ "id": t.id, "time": runtime, "segs": TaskInfo.ProcessStamp2String(segs) });
                }
            });
            endInfo.items = showItems;
            endInfo.average = `Average times:${sum / showItems.length}s`;
            endInfo.max = `Max times:${max}s`;
            endInfo.min = `Min times:${min}s`;
            endInfo.segsStatistics = TaskInfo.ProcessSegs(segInfos);
        }
    } else {
        endInfo.message = "Missing parameter:num";
    }
    res.end(JSON.stringify(endInfo));
}

function clear(req, res) {
    console.log('Receive clear...');
    var endInfo = {
        'code': Status.OK,
        'message': ""
    };
    var sta = TaskQueue.getStateData();
    if (sta.tCount != 0) {
        endInfo.message = "The server is executing tasks,please retry later!";
        res.end(JSON.stringify(endInfo));
    } else {
        //clear db
        DB.Clear();
        //clear dir
        var dirs = [utility.platformPath(__dirname + "/public/"), utility.platformPath(__dirname + "/cache/")];
        var count = 0;
        var call = function (err) {
            count++;
            console.log("清理进度:", `${count / dirs.length * 100}%`);
            if (err || count >= dirs.length) {
                endInfo.message = err == null ? "Clear cache success!" : err;
                res.end(JSON.stringify(endInfo));
            }
        }
        dirs.forEach(t => {
            clearDir(t, call);
        });
    }
}

function renderblock(req, res) {
    console.log("Receive renderblock..");
    var postData = '';
    req.on('data', chunk => {
        postData += chunk.toString()
    });
    req.on('end', () => {
        var endInfo = {
            'code': Status.OK,
            'id': null,
            'message': ""
        }
        try {
            var blockInfo = JSON.parse(postData);
            if (blockInfo.items.length == 0) {
                endInfo.code = Status.FormatError;
                endInfo.message = Message.FormatError;
            } else {
                var resultInfo = TaskQueue.Enqueue(blockInfo);
                endInfo.id = resultInfo.id;
                endInfo.message = Message.ReceiveFileOK;
            }
        } catch (error) {
            endInfo.code = Status.FormatError;
            endInfo.message = Message.FormatError;
        } finally {
            res.end(JSON.stringify(endInfo));
        }
    });
}

function renderfile(req, res) {
    console.log("Receive file for renderblock..");
    var endInfo = {
        'code': Status.OK,
        'id': null,
        'message': ""
    };
    if (req.files == null || req.files.length == 0) {
        endInfo.message = "Receive nothing."
        res.end(JSON.stringify(endInfo));
    } else {
        var reciveFile = req.files[0];
        if (reciveFile) {
            fs.readFile(reciveFile.path, function (err, data) {
                if (err) {
                    endInfo.message = err;
                    res.end(JSON.stringify(endInfo));
                } else {
                    var postData = data.toString();
                    try {
                        var blockInfo = JSON.parse(postData);
                        if (blockInfo.items.length == 0) {
                            endInfo.code = Status.FormatError;
                            endInfo.message = Message.FormatError;
                        } else {
                            var resultInfo = TaskQueue.Enqueue(blockInfo);
                            endInfo.id = resultInfo.id;
                            endInfo.message = Message.ReceiveFileOK;
                        }
                    } catch (error) {
                        endInfo.code = Status.FormatError;
                        endInfo.message = Message.FormatError;
                    } finally {
                        res.end(JSON.stringify(endInfo));
                    }
                }
            });
        } else {
            endInfo.message = "Receive nothing."
            res.end(JSON.stringify(endInfo));
        }
    }
}

function querystate(req, res) {
    console.log('querystate:', req.query);
    var id = req.query["id"];
    var endInfo = {
        'code': Status.OK,
        'id': null,
        'finish': false,
        'items': null,
        'unitylog': null,
        'message': '',
    }
    if (id) {
        var id = parseInt(id);
        endInfo.id = id;
        var info = DB.GetInfoById(id);
        if (info != null) {
            console.log(info.state);
            var fileInfo = GetFileUrl(info);
            switch (info.state) {
                case State.Idle:
                case State.Waiting:
                    endInfo.message = Message.RenderWaiting;
                    break;
                case State.Runnig:
                    endInfo.unitylog = fileInfo.log;
                    endInfo.message = Message.RenderUndone;
                    break;
                case State.Success:
                    endInfo.finish = true;
                    endInfo.items = fileInfo.items;
                    endInfo.unitylog = fileInfo.log;
                    break;
                case State.Fail:
                    endInfo.code = Status.RenderError;
                    endInfo.finish = true;
                    endInfo.unitylog = fileInfo.log;
                    endInfo.message = info.msg;
                    break;
            }
        } else {
            endInfo.code = Status.ParameterNotFound;
            endInfo.message = Message.ParameterNotExists_Id;
        }
    } else {
        endInfo.code = Status.ParameterMissing;
        endInfo.message = Message.ParameterMissing_Id;
    }
    res.end(JSON.stringify(endInfo));
}

function done(req, res) {
    console.log("done:", req.query);
    var id = req.query['id'];
    var msg = req.query['msg'];
    var message = "";
    if (id) {
        var info = DB.GetInfoById(parseInt(id));
        if (info) {
            var isFailed = utility.isNotNullAndEmpty(msg);
            if (isFailed) {
                info.state = State.Fail;
                info.msg = msg;
            } else {
                info.state = State.Success;
            }
            DB.Save();
            message = "success!";
        } else {
            message = id + " not exsit.";
        }
    } else {
        message = "Missing parameters:id";
    }
    res.end(message);
}

function segdot(req, res) {
    console.log("segdot:", req.query);
    var id = req.query['id'];
    var segid = req.query['segid'];
    var segdesc = req.query['segdesc'];
    var message = "";
    if (id) {
        var info = DB.GetInfoById(parseInt(id));
        if (info) {
            info.segments.push(new TimeSegs(parseInt(segid), Date.now(), segdesc));
            DB.Save();
            message = "seg dot success!";
        } else {
            message = id + " not exsit.";
        }
    } else {
        message = "Missing parameters:id";
    }
    res.end(message);
}

function ipAddress() {
    console.log('应用实例，访问地址为 http://%s:%s', ip, port);
    return {
        'ip': ip,
        'port': port
    }
}

function GetFileUrl(info) {
    var stamp = info.timestamp.toString();
    var id = info.id;
    var log = `http://${ip}:${port}/log/${id}_${stamp}.txt`;
    var dataPath = utility.platformPath(path.join(__dirname, `./public/${id}/${id}.json`));
    var exists = fs.existsSync(dataPath);
    var list = [];
    if (exists) {
        var uploadData = fs.readFileSync(dataPath);
        var block = JSON.parse(uploadData);
        list = block.items;
    }
    var result = {
        'id': id,
        'items': list,
        'log': log
    }
    return result;
}

function clearDir(dir, callback) {
    utility.deleteDirByCmd(dir, (error) => {
        utility.createDirIfNotExist(dir);
        if (callback != null)
            callback(error);
    });
}

exports.home = home;
exports.state = state;
exports.query = query;
exports.clear = clear;
exports.renderblock = renderblock;
exports.renderfile = renderfile;
exports.querystate = querystate;
exports.done = done;
exports.segdot = segdot;
exports.ipAddress = ipAddress;