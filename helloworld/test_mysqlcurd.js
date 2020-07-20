var mysql=require('mysql');
var connection=mysql.createConnection({
    host:'localhost',
    user:'root',
    password:'1024',
    database:'test'
});
connection.connect();

var query=function(sql,log,params){
    connection.query(sql,params,function(err,result){
        if(err){
            console.log('[SELECT ERROR]-',err.message);
            return;
        }
        console.log('--------------------------SELECT----------------------------');
        log(result);
        console.log('------------------------------------------------------------');
    });
}

//增
// var addsql='INSERT INTO websites(id,name,url,alexa,country) VALUES(0,?,?,?,?)';
// var addsqlparams=['项目管理工具','https://www.trello.com','24578','CN'];
// query(addsql,function(result){
//     console.log('INSERT ID:',result);
// },addsqlparams);

//更新
// var modsql='UPDATE websites SET name=?,url=?,alexa=? WHERE id=?';
// var modeparams=['百度一下','https://www.baidu.com',12333,7];
// query(modsql,function(result){
//     console.log('UPDATE affectedRows',result.affectedRows);
// },modeparams);

//删
var delsql='DELETE FROM websites WHERE id=7';
query(delsql,function(result){
    console.log('DELETE affectedRows',result.affectedRows);
});

//查
var showsql='SELECT * FROM websites';
query(showsql,function(result){
    console.log(result);
});

connection.end();