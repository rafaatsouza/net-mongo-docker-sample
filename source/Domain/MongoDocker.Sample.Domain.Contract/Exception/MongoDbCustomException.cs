using MongoDocker.Sample.Domain.Contract.Exception.Base;
using System.Net;

namespace MongoDocker.Sample.Domain.Contract.Exception
{
    public class MongoDbCustomException : CustomException<MongoDbCustomError>
    {
        public MongoDbCustomException(MongoDbCustomError error) : base(error)
        {
        }
    }

    public class MongoDbCustomError : CustomError
    {
        public static MongoDbCustomError RegisterNotFound => 
            new MongoDbCustomError(HttpStatusCode.NotFound, "Register not found");

        public static MongoDbCustomError UnavailableKey =>
            new MongoDbCustomError(HttpStatusCode.BadRequest, "Could not find a valid key for the register");

        public static MongoDbCustomError TimeOutServer(string server)
        {
            return new MongoDbCustomError(HttpStatusCode.InternalServerError, $"Timeout while attempting to connect to server at {server}");
        }            

        protected MongoDbCustomError(HttpStatusCode statusCode, string error) : base (statusCode, error)
        {                
        }
    }
}
