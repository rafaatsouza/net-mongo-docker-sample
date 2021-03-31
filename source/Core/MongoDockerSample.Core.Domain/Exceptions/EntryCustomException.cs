using MongoDockerSample.Core.Domain.Exceptions.Custom;
using System.Net;

namespace MongoDockerSample.Core.Domain.Exceptions
{
    public class EntryCustomException : CustomException<EntryCustomError>
    {
        public EntryCustomException(EntryCustomError error) : base(error)
        { }
    }

    public class EntryCustomError : CustomError
    {
        public static EntryCustomError KeyNotInformed =>
            new EntryCustomError(HttpStatusCode.BadRequest, "Informed key is null or empty");

        public static EntryCustomError RecordNotFound =>
            new EntryCustomError(HttpStatusCode.NotFound, "Entry not found");

        public static EntryCustomError ValueNotInformed =>
            new EntryCustomError(HttpStatusCode.BadRequest, "Informed value is null or empty");
        
        protected EntryCustomError(HttpStatusCode statusCode, string error) : base(statusCode, error)
        { }
    }
}