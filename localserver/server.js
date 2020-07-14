var http=require("http");
var url=require("url");

function start(route){
    function onRequest(request,response){
        var pathname=url.parse(request.url).pathname;
        console.log('received:'+pathname);

        response.writeHead(200,{"Content-Type":"text/html"});
        route(pathname,(data)=>{
            response.end(data);
        });
    }    
    http.createServer(onRequest).listen(8888);
    console.log('Sever has started');
}

exports.start=start;