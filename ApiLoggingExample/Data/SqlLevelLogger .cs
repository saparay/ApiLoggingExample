using ApiLoggingExample.Entities;

namespace ApiLoggingExample.Data
{
    public class SqlLevelLogger : ILogger
    {
        private readonly string _category;
        private readonly IHttpContextAccessor _http;
        private readonly RequestLogContext _logContext;

        public SqlLevelLogger(
            string category,
            IHttpContextAccessor http,
            RequestLogContext logContext)
        {
            _category = category;
            _http = http;
            _logContext = logContext;
        }

        public bool IsEnabled(LogLevel logLevel)
            => logLevel >= LogLevel.Information;

        public IDisposable BeginScope<TState>(TState state) => null;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            if (_category.Contains("SqlLogger")) return;

            var message = formatter(state, exception);
            _logContext.Logs.Add($"{logLevel}: {message}");
        }
    }

}
