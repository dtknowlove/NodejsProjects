// -------------写操作-------------
// var fs=require('fs');
// var data="这是一个测试node.js的故事!";
// var writeStream=fs.createWriteStream('output.txt');
// writeStream.write(data,'utf8',function(err){console.log(err.stack);});
// writeStream.end();
// writeStream.on('finish',function(){console.log('写入完成');});

// -------------读操作-------------
// var fs=require('fs');
// var data=''
// var readStream=fs.createReadStream('output.txt','utf8');
// readStream.on('data',function(chunk){data+=chunk;});
// readStream.on('end',function(){console.log(data);});
// readStream.on('error',function(err){console.log(err.stack);});

// -------------管道操作-------------
// var fs=require('fs');
// var rStream=fs.createReadStream('helloworld.js');
// var wStream=fs.createWriteStream("output.txt");
// rStream.pipe(wStream);

// -------------链式管道操作-------------
var fs=require('fs');
var zlib=require('zlib');
// 压缩
// fs.createReadStream('output.txt')
//     .pipe(zlib.createGzip())
//     .pipe(fs.createWriteStream('output.txt.gz'));
// 解压缩
fs.createReadStream('output.txt.gz')
    .pipe(zlib.createGunzip())
    .pipe(fs.createWriteStream('output.txt'));

console.log('程序执行完毕');
