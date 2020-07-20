console.log(new Date().toDateString());
var util=require('util');

function Base(){
    this.name='base';
    this.birth=1992;
    this.sayHello=function(){
        console.log('Hello %s',this.name);
    };
}

Base.prototype.showName=function(){
    console.log(this.name);
};

function Sub(){
    this.name='Sub';
}

util.inherits(Sub,Base);
var objBase=new Base();
objBase.showName();
objBase.sayHello();
console.log(util.inspect(objBase));
console.log(util.inspect(objBase,true,2,true));

var objSub=new Sub();
// objSub.sayHello();
objSub.showName();
console.log(objSub);
