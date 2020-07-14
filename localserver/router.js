var fs=require('fs');

function route(pathname,callback){
    console.log("route:"+pathname);
    if(pathname==='/' || pathname==='/index')
    {
        fs.readFile('./defaultmain.html','utf-8',function(err,data){
            if(err)
            {
                throw err;
            }
            callback(data);
        })        
    }else
    {
        callback('404 not fond!');
    }
}

exports.route=route;