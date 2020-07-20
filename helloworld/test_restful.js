var express=require('express');
var app=express();
var fs=require('fs');

var adduser = {
    "user4" : {
       "name" : "mohit",
       "password" : "password4",
       "profession" : "teacher",
       "id": 4
    }
 }
 var delid=2;
app.get('/listUsers',function(req,res){
    fs.readFile(__dirname+'/usr.json','utf8',function(err,data){
        console.log(data);
        res.send(JSON.stringify(data));
    });    
});
 app.get('/addUser',function(req,res){
    fs.readFile(__dirname+'/usr.json','utf8',function(err,data){
        data=JSON.parse(data);
        data['user4']=adduser['user4'];
        console.log(data);
        res.send(JSON.stringify(data));
    });
});
app.get('/:id',function(req,res){
    fs.readFile(__dirname+'/usr.json','utf8',function(err,data){
        data=JSON.parse(data);
        var user=data['user'+req.params.id];
        console.log(user);
        res.send(JSON.stringify(user));
    });
});

app.get('/del',function(req,res){
    fs.readFile(__dirname+'/usr.json','utf8',function(err,data){
        data=JSON.parse(data);
        delete data['user'+delid];
        console.log(data);
        res.send(JSON.stringify(data));
    });
});

var server=app.listen(8081,function(){
    var host = server.address().address
  var port = server.address().port

  console.log("应用实例，访问地址为 http://%s:%s", host, port)
});