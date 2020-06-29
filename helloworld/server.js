var http= require('http');
var fs=require('fs');

var server= http.createServer();
server.listen(8888,function(){
    console.log('Sever running at http://127.0.0.1:8888');
});
server.on('request',function(request,response){
    var url=request.url;
    if(url==='/' || url==='/index')
    {
        response.writeHead(200,{'Content-Type':'text/html'});
        fs.readFile('./defaultmain.html','utf-8',function(err,data){
            if(err)
            {
                throw err;
            }
            response.end(data);
        })        
    }else
    {
        response.writeHead(200,{'Content-Type':'text/plain'});
        response.end('404 not fond!');
    }
});