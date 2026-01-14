namespace ApiLoggingExample.Data
{
    public class LogWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogQueue _queue;

        public LogWorker(IServiceScopeFactory scopeFactory, ILogQueue queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var log))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();

                    try
                    {
                        db.ApiLogs.Add(log);
                        await db.SaveChangesAsync(stoppingToken);
                    }
                    catch
                    {
                        // swallow — logging must never crash app
                    }
                }

                await Task.Delay(100, stoppingToken);
            }
        }
    }

}
