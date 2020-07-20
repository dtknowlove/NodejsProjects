// console.log(__filename);
// console.time('打印数据');
// var t= setTimeout(()=>{
//     console.log(__dirname);
//     console.timeEnd('打印数据');
// },2000);
// // clearTimeout(t);
// var counter=0;
// setInterval(() => {
//     counter++;
//     console.log('执行次数:%d',counter);
// }, 2000);

process.on('exit',function(code){
    setTimeout(() => {
        console.log('不执行！');
    }, 1);
    console.log('退出码为:%d',code);
})

process.stdout.write('hello world!\n');
process.argv.forEach(function(val,index){
    console.log(index+':'+val);
})
console.log(process.execPath);
console.log(process.platform);
console.log(process.memoryUsage());

const{ exec }=require('child_process');
exec('node helloworld.js',(err,stdout,stderr)=>{
    if(err){
        console.log(err);
        return;
    }
    console.log(`stdout: ${stdout}`);
    console.log(`stderr: ${stderr}`);
})


console.log('结束');