using MongoDockerSample.Core.Domain.Exceptions.Custom;
using System.Net;

namespace MongoDockerSample.Infrastructure.Repository.Exceptions
{
    public class RepositoryCustomException : CustomException<RepositoryCustomError>
    {
        public RepositoryCustomException(RepositoryCustomError error) : base(error)
        {
        }
    }

    public class RepositoryCustomError : CustomError
    {
        public static RepositoryCustomError UnavailableKey =>
            new RepositoryCustomError(HttpStatusCode.BadRequest, "Could not find a valid key for the record");

        public static RepositoryCustomError TimeOutServer =>
            new RepositoryCustomError(HttpStatusCode.InternalServerError, $"Timeout while attempting to connect to MongoDB server");
        
        protected RepositoryCustomError(HttpStatusCode statusCode, string error) : base(statusCode, error)
        {
        }
    }
}