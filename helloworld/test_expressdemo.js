var express=require('express');
var app=express();


app.use('/defaultmain_files',express.static('defaultmain_files'));

app.get('/',function(req,res){
    res.send('Hello world!');
});

app.get('/png',function(req,res){
    res.sendFile(__dirname+'/defaultmain_files/result.png');
});

//Get 表单
app.get('/index.html',function(req,res){
    res.sendFile(__dirname+'/index.html');
});
app.get('/process_get',function(req,res){
    var response={
        'first_name':req.query.first_name,
        'last_name':req.query.last_name
    };
    console.log(response);
    res.end(JSON.stringify(response));
});

//Post 表单
var bodyParser=require('body-parser');
var urlencodeParser=bodyParser.urlencoded({extended:false});
app.get('/index2.html',function(req,res){
    res.sendFile(__dirname+'/index2.html');
});
app.post('/process_post',urlencodeParser, function(req,res){
    var response={
        'first_name':req.body.first_name,
        'last_name':req.body.last_name
    };
    console.log(response);
    res.end(JSON.stringify(response));
});

//Post 上传文件
var fs=require('fs');
var multer=require('multer');
app.use(multer({dest:'/tmp/'}).array('image'));
app.get('/upload.html',function(req,res){
    res.sendFile(__dirname+'/upload.html');
});
app.post('/file_upload', function(req,res){
    console.log(req.files[0]);
    var des_file=__dirname+'/'+req.files[0].originalname;
    fs.readFile(req.files[0].path,function(err,data){
        fs.writeFile(des_file,data,function(err){
            if(err){
                console.log(err);
            }else{
                response={
                    message:'File upload successfully.',
                    filename:req.files[0].originalname
                };
            }
            console.log(response);
            res.end(JSON.stringify(response));
        });
    });
});

app.post('/',function(req,res){
    res.send('hello post!');
})

app.get('/del_usr',function(req,res){
    res.send('删除页面!');
})

app.get('/list_usr',function(req,res){
    res.send('用户列表页面!');
})

app.get('/ab*cd',function(req,res){
    res.send('正则匹配!');
})

var server=app.listen(8081,function(){
    var host=server.address().address;
    var port=server.address().port;
    console.log(host);
    console.log('应用实例，访问地址为 http://%s:%s',host,port);
});