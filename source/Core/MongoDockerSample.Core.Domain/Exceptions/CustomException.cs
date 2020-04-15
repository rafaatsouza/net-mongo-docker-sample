using System.Net;

namespace MongoDockerSample.Core.Domain.Exceptions
{
    public abstract class CustomException<T> : System.Exception where T : CustomError
    {
        public HttpStatusCode StatusCode { get; protected set; }

        protected CustomException(CustomError error) : base(error.Message)
        {
            StatusCode = error.StatusCode;
        }
    }
}