function say(word)
{
    console.log(word);
}
function execute(someFunc,val){
    someFunc(val);
}

execute(function(word){console.log(word);},"Hi，jjjj!");