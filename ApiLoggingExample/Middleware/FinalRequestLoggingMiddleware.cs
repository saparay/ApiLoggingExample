using ApiLoggingExample.Data;
using ApiLoggingExample.Entities;

namespace ApiLoggingExample.Middleware
{
    public class FinalRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogQueue _queue;

        public FinalRequestLoggingMiddleware(
            RequestDelegate next,
            ILogQueue queue)
        {
            _next = next;
            _queue = queue;
        }

        public async Task Invoke(HttpContext context)
        {
            var logContext = context.RequestServices
                .GetRequiredService<RequestLogContext>();

            await _next(context);

            var log = new ApiLog
            {
                TraceId = context.TraceIdentifier,
                Path = context.Request.Path,
                Method = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                Message = string.Join(" | ", logContext.Logs),
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                Browser = context.Request.Headers["User-Agent"],
                Device = context.Request.Headers["sec-ch-ua-platform"],
                CreatedAt = DateTime.UtcNow
            };

            _queue.Enqueue(log);
        }
    }


}
