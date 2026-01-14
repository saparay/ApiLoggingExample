namespace ApiLoggingExample.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.TraceIdentifier ??= Guid.NewGuid().ToString();

            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            context.Items["RequestBody"] = requestBody;
            _logger.LogInformation("Incoming Request: {method} {path} {body}",
                context.Request.Method,
                context.Request.Path,
                requestBody);

            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            memStream.Position = 0;
            var responseBody = await new StreamReader(memStream).ReadToEndAsync();
            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);
            context.Items["ResponseBody"] = responseBody;
            _logger.LogInformation("Outgoing Response: {statusCode} {body}",
                context.Response.StatusCode,
                responseBody);
        }
    }

}
