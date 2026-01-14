using ApiLoggingExample.Entities;
using System.Text.Json;

namespace ApiLoggingExample.Data
{
    public class SqlLogger : ILogger
    {
        private readonly string _category;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SqlLogger(
            string category,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _category = category;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var http = _httpContextAccessor.HttpContext;
            if (http == null) return;

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();
            var requestBody = http.Items["RequestBody"]?.ToString();
            var responseBody = http.Items["ResponseBody"]?.ToString();
            var log = new ApiLog
            {
                TraceId = http.TraceIdentifier,
                LogLevel = logLevel.ToString(),
                Message = formatter(state, exception),
                Exception = exception?.ToString(),
                Path = http.Request.Path,
                Method = http.Request.Method,
                StatusCode = http.Response?.StatusCode,
                IpAddress = http.Connection.RemoteIpAddress?.ToString(),
                Device = http.Request.Headers["sec-ch-ua-platform"],
                Browser = http.Request.Headers["User-Agent"],
                CreatedAt = DateTime.UtcNow,
                RequestBody = requestBody,
                ResponseBody = responseBody
                
            };


            db.ApiLogs.Add(log);
            db.SaveChanges();
        }
    }

}
