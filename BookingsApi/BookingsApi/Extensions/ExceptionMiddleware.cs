using Microsoft.AspNetCore.Http;
using BookingsApi.Common.Helpers;
using System.Text;

namespace BookingsApi.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;
        

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (BadRequestException ex)
            {
                ApplicationLogger.TraceException(TraceCategory.APIException.ToString(), "400 Exception", ex, null, null);
                await HandleExceptionAsync(httpContext, HttpStatusCode.BadRequest, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                ApplicationLogger.TraceException(TraceCategory.APIException.ToString(), "API Exception", ex,null, null);
                await HandleExceptionAsync(httpContext, HttpStatusCode.InternalServerError, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, Exception exception)
        {
            context.Response.StatusCode = (int) statusCode;
            var sb = new StringBuilder(exception.Message);
            var innerException = exception.InnerException;
            while (innerException != null)
            {
                sb.Append($" {innerException.Message}");
                innerException = innerException.InnerException;
            }
            return context.Response.WriteAsJsonAsync(sb.ToString());
        }

    }
}
