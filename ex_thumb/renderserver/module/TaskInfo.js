var State = require('./State');
var TimeSegs = require('./TimeSegs');

class TaskInfo {
  constructor(id) {
    this.id = id;
    this.state = State.Idle;
    this.msg = '';
    this.timestamp;
    this.pipename = "";
    this.runtime = 0;
    this.segments = [new TimeSegs(0, Date.now(), "ToWaiting")];
  };
  ChangeState(_state) {
    this.state = _state;
    return this;
  };
  SetTimeStamp(_stamp) {
    this.timestamp = _stamp;
    return this;
  }
  static ProcessStamp(segs) {
    if (segs != null) {
      var length = segs.length;
      var resShows = [];
      if (length > 1) {
        for (var i = 1; i < length; i++) {
          var show = {
            "id": segs[i - 1].id,
            "desc": segs[i - 1].desc.replace("To", ""),
            "time": TimeSegs.Stamp2Time(segs[i], segs[i - 1])
          };
          resShows.push(show);
        }
      }
    }
    return resShows;
  }
  static ProcessStamp2String(shows) {
    var res = "";
    if (shows != null) {
      shows.forEach(t => {
        res += `${t.desc}:${t.time} `
      })
    }
    return res;
  }
  static ProcessSegs(segInfos) {
    var res = [];
    if (segInfos == null || segInfos.length == 0)
      return res;
    var waiting = [], running = [], playing = [], render = [], download = [], upload = [];
    segInfos.forEach(t => {
      switch (t.id) {
        case 0:
          waiting.push(t);
          break;
        case 1:
          running.push(t);
          break;
        case 2:
          playing.push(t);
          break;
        case 3:
          render.push(t);
          break;
        case 4:
          download.push(t);
          break;
        case 5:
          upload.push(t);
          break;
      }
    })
    res.push(TaskInfo.ProcessSingleSeg(waiting));
    res.push(TaskInfo.ProcessSingleSeg(running));
    res.push(TaskInfo.ProcessSingleSeg(playing));
    res.push(TaskInfo.ProcessSingleSeg(render));
    res.push(TaskInfo.ProcessSingleSeg(download));
    res.push(TaskInfo.ProcessSingleSeg(upload));
    return res;
  }

  static ProcessSingleSeg(segStatings) {
    if (segStatings == null || segStatings.length == 0)
      return "";
    var w_res = {
      "desc:": "",
      "max": 0,
      "min": 0,
      "sum": 0,
      "average": 0,
      tostring: function () {
        return `${this.desc}:max->${this.max} min->${this.min} average->${this.average}`;
      }
    };
    w_res.desc = segStatings[0].desc == null ? "nothing" : segStatings[0].desc;
    w_res.min = segStatings[0].time == null ? 0 : segStatings[0].time;
    segStatings.forEach(w => {
      w_res.sum += w.time;
      w_res.max = w.time > w_res.max ? w.time : w_res.max;
      w_res.min = w.time < w_res.min ? w.time : w_res.min;
    })
    w_res.average = w_res.sum / segStatings.length;
    return w_res.tostring();
  }
}
module.exports = TaskInfo;