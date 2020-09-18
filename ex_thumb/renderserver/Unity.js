var fs = require('fs');
const path = require('path');
const { exec } = require('child_process');
var utility = require('./utility');
var UnityPipe = require('./module/UnityPipe');
var PipeState = require('./module/PipeState');

var unity_path = getUnityPath();
var pro_root = utility.platformPath(path.join(__dirname, './../'));
var method_name = "RenderEditor.ExecCmd";
var method_param = "";//文件名,分隔符为:
var pipes = [new UnityPipe("111")];

function Start(id, filename, _proName) {
    var pipe = getPipeByName(_proName);
    if (pipe == null) {
        console.log("分配工程名称未找到对应pipe:", _proName);
        return;
    }
    var log_dir = utility.platformPath(path.join(__dirname, './public/log'));
    utility.createDirIfNotExist(log_dir);
    var log_path = utility.platformPath(`${log_dir}/${id}_${filename}.txt`);
    method_param = filename;
    var pro_path = pro_root + pipe.pro_name;
    console.log(pro_path);
    var openCmd = `${unity_path} -projectPath ${pro_path} -batchmode -noUpm -buildTarget standalone -executeMethod ${method_name} -logFile "${log_path}"  -openpara ${method_param}`;
    pipe.process = exec(openCmd);
}

function Kill(_proName) {
    var pipe = getPipeByName(_proName);
    if (pipe && pipe.process) {
        pipe.process.kill();
    }
    var pid_path = utility.platformPath(pro_root + pipe.pro_name + "/Library/EditorInstance.json");
    var exists = fs.existsSync(pid_path);
    if (!exists)
        return;
    var data = fs.readFileSync(pid_path);
    var pid = JSON.parse(data.toString());
    var killcmd = utility.killProcessCmd(pid.process_id.toString());
    exec(killcmd);
}

function hasRundant(count) {
    return count < pipes.length;
}

function rundantCount(count) {
    return pipes.length - count;
}

function getRundantProName() {
    var pipe = pipes.find(t => !t.IsBusy());
    if (pipe == null)
        return null;
    pipe.SetState(PipeState.Busy);
    return pipe.pro_name;
}

function freedPipe(_proName) {
    var pipe = getPipeByName(_proName);
    if (pipe == null)
        return;
    pipe.SetState(PipeState.Idle);
}

function init() {
    pipes.splice(0, pipes.length);
    var res = fs.readdirSync(pro_root);
    res.forEach(t => {
        if (t.startsWith("client_unity"))
            pipes.push(new UnityPipe(t));
    });
    console.log("unity pipe个数:", pipes.length);
}

function getUnityPath() {
    var path = '';
    var platform = utility.platform();
    if (platform == "darwin")
        path = "/Applications/Unity2018.4.9f1/Unity.app/Contents/MacOS/Unity";
    if (platform == "win32")
        path = utility.platformPath("E:/Unity2018/Editor/Unity.exe");
    return path;
}

function getPipeByName(_proName) {
    return pipes.find(t => t.pro_name == _proName);
}


exports.Start = Start;
exports.Kill = Kill;
exports.hasRundant = hasRundant;
exports.rundantCount = rundantCount;
exports.getRundantProName = getRundantProName;
exports.freedPipe = freedPipe;

init();