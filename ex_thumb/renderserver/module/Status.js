const Status = {
    OK: 200,
    NotFound: 404,
    RenderError: 6000,
    FormatError: 6001,
    ParameterMissing: 6002,
    ParameterNotFound: 6003,
}
Object.freeze(Status);
module.exports = Status;