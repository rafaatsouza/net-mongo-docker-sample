using MongoDockerSample.Core.Domain.Exceptions.Custom;
using System.Net;

namespace MongoDockerSample.Core.Domain.Exceptions
{
    public class RecordCustomException : CustomException<RecordCustomError>
    {
        public RecordCustomException(RecordCustomError error) : base(error)
        { }
    }

    public class RecordCustomError : CustomError
    {
        public static RecordCustomError KeyNotInformed =>
            new RecordCustomError(HttpStatusCode.BadRequest, "Informed key is null or empty");

        public static RecordCustomError RecordNotFound =>
            new RecordCustomError(HttpStatusCode.NotFound, "Record not found");

        public static RecordCustomError ValueNotInformed =>
            new RecordCustomError(HttpStatusCode.BadRequest, "Informed value is null or empty");
        
        protected RecordCustomError(HttpStatusCode statusCode, string error) : base(statusCode, error)
        { }
    }
}