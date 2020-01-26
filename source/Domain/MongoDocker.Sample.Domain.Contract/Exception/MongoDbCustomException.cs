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

        protected MongoDbCustomError(HttpStatusCode statusCode, string error) : base (statusCode, error)
        {                
        }
    }
}
