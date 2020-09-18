var fs = require('fs');
var utility = require('./utility');
const path = require('path');
const TaskInfo = require('./module/TaskInfo');

var root_dbDir = path.join(__dirname, "./database");
var dbid = utility.platformPath(`${root_dbDir}/dbId.txt`);
var dbPath = utility.platformPath(`${root_dbDir}/db.json`);

var dbList = [new TaskInfo(0)]

function GetLastId() {
    var exists = fs.existsSync(dbid);
    if (exists) {
        var lastID = parseInt(fs.readFileSync(dbid)) + 1;
        SaveIdIndex(lastID);
        return lastID;
    } else {
        var defaultID = 1;
        SaveIdIndex(defaultID);
        return defaultID
    };
}

function SaveIdIndex(id) {
    fs.writeFileSync(dbid, id);
}

function GetInfoById(id) {
    if (dbList == null || dbList.length == 0)
        return null;
    var info = null;
    for (let i = 0; i < dbList.length; i++) {
        const t = dbList[i];
        if (t != null && t.id == id) {
            info = t;
            break;
        }
    }
    return info;
}

function Insert2DB(taskinfo) {
    dbList.push(taskinfo);
    Save();
}

function Load() {
    utility.createDirIfNotExist(root_dbDir);
    dbList.splice(0, dbList.length);
    var exists = fs.existsSync(dbPath);
    if (exists) {
        var data = fs.readFileSync(dbPath);
        dbList = JSON.parse(data.toString());
    }
    console.log("db data length:", dbList.length);
}
function Save() {
    if (dbList == null)
        return;
    fs.writeFileSync(dbPath, JSON.stringify(dbList));
}

function Count() {
    return dbList == null ? 0 : dbList.length;
}

function GetInfos(start, end) {
    return dbList == null ? null : dbList.slice(start, end);
}

function Clear() {
    var exists = fs.existsSync(dbPath);
    if (exists) {
        fs.unlinkSync(dbPath);
    }
    dbList.splice(0, dbList.length);
    Save();
    console.log("db data length:", dbList.length);
}

exports.GetLastId = GetLastId;
exports.GetInfoById = GetInfoById;
exports.Insert2DB = Insert2DB;
exports.Load = Load;
exports.Save = Save;
exports.Count = Count;
exports.GetInfos = GetInfos;
exports.Clear = Clear;

Load();