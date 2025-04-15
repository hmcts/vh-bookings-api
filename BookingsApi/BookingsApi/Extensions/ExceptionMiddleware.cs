using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenTelemetry.Trace;
using BookingsApi.Common.Logging;

namespace BookingsApi.Extensions;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (DomainRuleException ex)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddDomainRuleErrors(ex.ValidationFailures);
            var problemDetails = new ValidationProblemDetails(modelState);

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (EntityNotFoundException ex)
        {
            logger.LogErrorUnexpected(ex);
            await HandleExceptionAsync(httpContext, HttpStatusCode.NotFound, ex);
            TraceException("Entity Not Found", ex);
        }
        catch (Exception ex)
        {
            logger.LogErrorUnexpected(ex);
            await HandleExceptionAsync(httpContext, HttpStatusCode.InternalServerError, ex);
            TraceException("API Exception", ex);
        }
    }
        
    private static void TraceException(string eventTitle, Exception exception)
    { 
        var activity = Activity.Current;
        if (activity == null) return;
        activity.DisplayName = eventTitle;
        activity.RecordException(exception);
        activity.SetStatus(ActivityStatusCode.Error);
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