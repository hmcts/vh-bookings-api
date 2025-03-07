using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BookingsApi.Extensions;

public class RequestBodyLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;

        // Ensure the request body can be read multiple times
        context.Request.EnableBuffering();

        // Only if we are dealing with POST or PUT, GET and others shouldn't have a body
        if (context.Request.Body.CanRead && (method == HttpMethods.Post || method == HttpMethods.Put || method == HttpMethods.Patch))
        {
            // Leave stream open so next middleware can read it
            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 512, leaveOpen: true);

            var requestBody = await reader.ReadToEndAsync();

            // Reset stream position, so next middleware can read it
            context.Request.Body.Position = 0;

            // Write request body to App Insights
            var activity = Activity.Current;
            activity?.SetTag("http.request.body", requestBody);
            activity?.SetTag("http.request.method", method);
            activity?.SetTag("http.request.path", context.Request.Path);
        }

        // Call next middleware in the pipeline
        await next(context);
    }
}