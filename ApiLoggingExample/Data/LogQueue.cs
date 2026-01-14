using ApiLoggingExample.Entities;
using System.Collections.Concurrent;

namespace ApiLoggingExample.Data
{
    public class LogQueue : ILogQueue
    {
        private readonly ConcurrentQueue<ApiLog> _queue = new();

        public void Enqueue(ApiLog log) => _queue.Enqueue(log);

        public bool TryDequeue(out ApiLog log)
            => _queue.TryDequeue(out log);
    }

}
