var mysql=require('mysql');
var connection=mysql.createConnection({
    host:'localhost',
    user:'root',
    password:'1024',
    database:'test'
});
connection.connect();

connection.query('SELECT 1+1 AS solution',function(err,results,fields){
    if(err) throw err;
    console.log('The solution is:%s',results[0].solution);
});

connection.end();