using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BookingsApi.Extensions
{
    /// <summary>
    /// Middleware to intercept the response we're sending to the client in order to use for logging
    /// </summary>
    public class LogResponseBodyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ItemOfInterest = "responseBody";


        public LogResponseBodyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Stream originalBody = context.Response.Body;

            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await _next(context);

                    memStream.Position = 0;
                    var responseBody = new StreamReader(memStream).ReadToEnd();

                    // Rewind the stream for the client
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);

                    // Stow away the response body
                    context.Items[ItemOfInterest] = responseBody;
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}