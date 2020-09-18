var express = require('express');
const path = require('path');
var utility = require('./utility');
var router = require("./router")
var app = express();

app.use(express.static(utility.platformPath(path.join(__dirname, 'public'))));
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*')
    res.header('Access-Control-Allow-Headers', 'Authorization,X-API-KEY, Origin, X-Requested-With, Content-Type, Accept, Access-Control-Request-Method')
    res.header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PATCH, PUT, DELETE')
    res.header('Allow', 'GET, POST, PATCH, OPTIONS, PUT, DELETE')
    next();
});

//主页
app.get('/', router.home);
//当前服务状态
app.get('/state', router.state);
//服务端数据查询
app.get('/query', router.query);
//服务端清除缓存
app.get('/clear', router.clear);
//渲染接口
var bodyParser = require('body-parser');
var urlencodeParser = bodyParser.urlencoded({ extended: false });
app.post('/renderblock', urlencodeParser, router.renderblock);
//渲染文件接口
var multer = require('multer');
app.use(multer({ dest: '/tmp/' }).array('config'));
app.post('/renderfile', router.renderfile);
//轮询接口
app.get('/querystate', router.querystate);
//渲染完成接口
app.get('/done', router.done);
//时间打点
app.get('/segdot', router.segdot);

var ipAddress = router.ipAddress();
app.listen(ipAddress.port);