using System.Net;

namespace MongoDocker.Sample.Domain.Contract.Exception.Base
{
    public abstract class CustomError
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; }

        public CustomError(HttpStatusCode statusCode, string error)
        {
            StatusCode = statusCode;
            Message = error;
        }
    }
}