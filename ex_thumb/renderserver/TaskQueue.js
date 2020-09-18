var fs = require('fs');
const path = require('path');
var TaskInfo = require("./module/TaskInfo");
var DB = require('./DB');
var Unity = require('./Unity');
var utility = require('./utility');
const State = require("./module/State");
var Message = require("./module/Message");
var TimeSegs = require('./module/TimeSegs');
var delay = utility.sleep;

const timeout = 300;//超时设置 单位:s

var tasks = []
var curTasks = [];
var checkInterval = 1;
var root_cacheDir = path.join(__dirname, './cache');

function Enqueue(config) {
    //1.分配id
    var taskInfo = new TaskInfo(DB.GetLastId());
    DB.Insert2DB(taskInfo);
    //2.包装所需信息
    var resultInfo = {
        'id': taskInfo.id,
        "info": config
    }
    var timeStamp = Date.now();    
    utility.createDirIfNotExist(root_cacheDir);
    var fullPath = utility.platformPath(`${root_cacheDir}/${timeStamp}.json`);
    console.log(fullPath);
    fs.writeFile(fullPath, JSON.stringify(resultInfo), (err) => {
        if (err) {
            console.log(err);
        }
        taskInfo.ChangeState(State.Waiting)
            .SetTimeStamp(timeStamp);
        DB.Save();
        tasks.push(taskInfo);
    });
    return resultInfo;
}

function getStateData() {
    return {
        "tCount": curTasks == null ? 0 : curTasks.length,
        'rCount': Unity.rundantCount(curTasks.length)
    };
}

function Execute() {
    if (Unity.hasRundant(curTasks.length) && tasks != null && tasks.length > 0) {
        curTasks.push(tasks.shift());
        console.log("当前任务数:", curTasks.length, "当前闲置Unity数:", Unity.rundantCount(curTasks.length));
    }
    curTasks.forEach(cTask => {
        if (cTask == null)
            return;
        switch (cTask.state) {
            case State.Waiting:
                Waiting(cTask);
                break;
            case State.Runnig:
                Running(cTask);
                break;
            case State.Fail:
            case State.Success:
                Finish(cTask);
                break;
            default:
                console.log("未知状态:", cTask.state);
                break;
        }
    })
}

function Waiting(cTask) {
    cTask.runtime = 0;
    var rundantProName = Unity.getRundantProName();
    cTask.pipename = rundantProName;
    cTask.state = State.Runnig;
    cTask.segments.push(new TimeSegs(1, Date.now(), "ToRunning"))
    DB.Save();
    console.log('开启任务！', cTask.id, cTask.pipename);
    //1.杀掉Unity
    Unity.Kill(rundantProName);
    //2.等待0.3s启动Unity 传参
    delay().then(function () {
        return delay(300);
    }).then(function () {
        Unity.Start(cTask.id, cTask.timestamp, cTask.pipename);
    });
}

function Running(cTask) {
    //上一个任务未完成
    cTask.runtime += checkInterval;
    if (cTask.runtime >= timeout) {
        //任务超时
        console.log(cTask.id + '=>' + Message.Error_Timeout)
        cTask.state = State.Fail;
        cTask.msg = Message.Error_Timeout;
        DB.Save();
    }
}

function Finish(cTask) {
    curTasks.splice(curTasks.indexOf(cTask), 1);
    //移除UnityPip占用
    Unity.Kill(cTask.pipename);
    Unity.freedPipe(cTask.pipename);
    cTask = null;
}

exports.Enqueue = Enqueue;
exports.getStateData = getStateData;
exports.timeout = timeout;

setInterval(Execute, checkInterval * 1000);

