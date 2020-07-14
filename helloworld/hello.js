
exports.world=function(){
    console.log('Hello world.');
}

function Hello(){
    var name;
    this.setName=function(theName){
        name=theName;
    };   
    this.sayHello=function(){
        console.log('Hello '+name);
    };
}

module.exports=Hello;