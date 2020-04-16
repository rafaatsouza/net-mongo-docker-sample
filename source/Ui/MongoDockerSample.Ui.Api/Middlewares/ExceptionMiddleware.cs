using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Net;
using System;
using System.Threading.Tasks;
using MongoDockerSample.Core.Domain.Exceptions;

namespace MongoDockerSample.Ui.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory?.CreateLogger<ExceptionMiddleware>()
                ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.next = next
                ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                logger.LogDebug("New http request: {@request}", httpContext.Request);

                await next(httpContext);

                logger.LogDebug("Response: {@request}", httpContext.Response);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError;

            var customException = exception as CustomException;

            if (customException != null)
            {
                logger.LogWarning("Custom exception: {@ex}", customException);
                httpStatusCode = customException.StatusCode;
            }
            else
            {
                logger.LogError("Unhandled exception: {@ex}", exception);
            }

            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)httpStatusCode;

            return context.Response.WriteAsync(exception.Message);
        }
    }
}