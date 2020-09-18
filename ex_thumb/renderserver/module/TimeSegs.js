class TimeSegs {
  constructor(id, stamp = 0, desc = "") {
    this.id = id,
      this.stamp = stamp,
      this.desc = desc
  };
  static Stamp2Time(_curSeg, _preSeg) {
    return (_curSeg.stamp - _preSeg.stamp) / 1000;
  };
  static ToString(seg) {
    return `${seg.desc}:${seg.time}`;
  }
}
module.exports = TimeSegs;