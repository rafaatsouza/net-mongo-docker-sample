using System;
using System.Collections.Generic;
using System.Net;

namespace MongoDockerSample.Core.Domain.Exceptions
{
    public abstract class CustomException : Exception
    {
        protected CustomException(string message) : base(message)
        {
        }

        public abstract HttpStatusCode StatusCode { get; protected set; }
    }

    public abstract class CustomException<T> : CustomException where T : CustomError
    {
        public override HttpStatusCode StatusCode { get; protected set; }

        protected CustomException(CustomError error) : base(error.Message)
        {
            StatusCode = error.StatusCode;
        }
    }
}