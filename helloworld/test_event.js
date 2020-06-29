var events=require('events');

var eventEmitter=new events.EventEmitter();

var connectHandle=function connected()
{
    console.log('连接成功。');
    eventEmitter.emit('data_received');
}

eventEmitter.on('connection',connectHandle);

eventEmitter.on('data_received',function(){
    console.log('数据接收成功。');
})

setTimeout(() => {    
    eventEmitter.emit('connection');
}, 1000);
console.log(eventEmitter.listenerCount('connection'));
// eventEmitter.emit('error');
console.log('程序执行完毕!');