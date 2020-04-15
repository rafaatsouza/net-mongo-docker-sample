using MongoDockerSample.Core.Domain.Exceptions;
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
        public static RepositoryCustomError KeyNotInformed =>
            new RepositoryCustomError(HttpStatusCode.BadRequest, "Informed key is null or empty");

        public static RepositoryCustomError RegisterNotFound =>
            new RepositoryCustomError(HttpStatusCode.NotFound, "Register not found");

        public static RepositoryCustomError UnavailableKey =>
            new RepositoryCustomError(HttpStatusCode.BadRequest, "Could not find a valid key for the register");

        public static RepositoryCustomError TimeOutServer(string server)
        {
            return new RepositoryCustomError(HttpStatusCode.InternalServerError, $"Timeout while attempting to connect to server at {server}");
        }

        protected RepositoryCustomError(HttpStatusCode statusCode, string error) : base(statusCode, error)
        {
        }
    }
}