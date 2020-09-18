var PipeState = require("./PipeState");

class UnityPipe {
    constructor(proname) {
        this.taskid = 0;
        this.state = PipeState.Idle;
        this.pro_name = proname;
        this.process;
    };
    IsBusy() {
        return this.state == PipeState.Busy;
    };
    SetState(_state) {
        this.state = _state;
        return this;
    };
}

module.exports = UnityPipe;