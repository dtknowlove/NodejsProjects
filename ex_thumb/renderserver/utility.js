var os = require('os');
var fs = require('fs');
const { exec } = require('child_process');

/**
 * Get IPv4 address of local machine.
 * @param {'number'} v 0->IPv4 or 1->IPv6
 */
function getIPAdress(v = 0) {
    var interfaces = os.networkInterfaces();
    var ipType = v == 0 ? "IPv4" : "IPv6";
    for (var devName in interfaces) {
        var iface = interfaces[devName];
        for (let i = 0; i < iface.length; i++) {
            const alias = iface[i];
            if (alias.family == ipType && alias.address != '127.0.0.1' && !alias.internal) {
                return alias.address;
            }
        }
    }
}
/**
 * Get the current platform.
 */
function platform() {
    return os.platform();
}

/**
 * Get a path of the current platform.
 * @param {string} path a path of file or directory
 */
function platformPath(path) {
    return platform() == "win32" ? replaceAll(path, "/", "\\") : path;
}

/**
 * get the kill cmd by pid of process.
 * @param {string} pid 
 */
function killProcessCmd(pid) {
    return platform() == "win32" ? `taskkill /pid ${pid} -f` : `kill -9 ${pid}`;
}

/**
 * Replace all searchValue in str to replaceValue.
 * @param {string} str 
 * @param {string} searchValue 
 * @param {string} replaceValue 
 */
function replaceAll(str, searchValue, replaceValue) {
    return str.split(searchValue).join(replaceValue);
}

/**
  * Aync delay.
  * @param {number} time delay time, unit:ms
  */
function sleep(time = 0) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            resolve();
        }, time);
    })
};

/**
 * The string is not null and empty.
 * @param {string} str 
 */
function isNotNullAndEmpty(str) {
    return typeof (str) == 'string' && str != null && str != "";
}

/**
 * Delete directory.
 * @param {string}} path directory name
 * @param {boolean} ?is recurse delete
 */
function deleteDir(path, recurse = true) {
    let files = [];
    if (fs.existsSync(path)) {
        files = fs.readdirSync(path);
        files.forEach(file => {
            let curPath = path + "/" + file;
            if (recurse && fs.statSync(curPath).isDirectory()) {
                deleteDir(curPath); //recurse
            } else {
                fs.unlinkSync(curPath); //delete file
            }
        });
        fs.rmdirSync(path);
    }
}

/**
 * Delete directory by cmd.
 * @param {string}} path directory name
 * @param {functon} callback
 */
function deleteDirByCmd(path, callback = null) {
    if (!fs.existsSync(path)) {
        return
    }
    var delCmd = platform() == "win32" ? `rmdir /S/Q ${path}` : `rm -rf ${path}`;
    exec(delCmd, (error, stdout, stderr) => {
        if (callback != null)
            callback(error);
    });
}

/**
 * Create directory if the dir is not exist.
 * @param {string} dir A directory name.
 */
function createDirIfNotExist(dir) {
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir);
    }
}



exports.getIPAdress = getIPAdress;
exports.platform = platform;
exports.platformPath = platformPath;
exports.killProcessCmd = killProcessCmd;
exports.replaceAll = replaceAll;
exports.sleep = sleep;
exports.isNotNullAndEmpty = isNotNullAndEmpty;
exports.deleteDir = deleteDir;
exports.deleteDirByCmd = deleteDirByCmd;
exports.createDirIfNotExist = createDirIfNotExist;