var net=require('net');
console.log('开始连接...')
var client=net.connect({port:8080},function(){
    console.log("连接到服务器!");
    client.write('Hello server!');
});
client.on('data',function(data){
    console.log('Client receive:%s',data.toString());    
})

client.on('end',function(){
    console.log('断开服务器连接。');
})