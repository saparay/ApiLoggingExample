namespace ApiLoggingExample.Data
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SqlLoggerProvider(
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SqlLogger(categoryName, _serviceProvider, _httpContextAccessor);
        }

        public void Dispose() { }
    }

}
