using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Net;
using System;
using System.Threading.Tasks;

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
                logger.LogError("Unhandled exception: {@ex}", ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(exception.Message);
        }
    }
}