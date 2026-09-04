using System.Net;

namespace Shared.UnifiedResult
{
    public class Result
    {
        public Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; set; }
        public Error Error { get; set; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<T> Success<T>(T data) => new(true, Error.None, data);

        public static Result<T> Failure<T>(Error error) => new(false, error, default);
        //
        //public int? ResponseCode { get; set; }
        
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        //public ErrorCode ErrorCode { get; set; } = ErrorCode.NoErrorCode;
    }
}
