using ApiLoggingExample.Entities;

namespace ApiLoggingExample.Data
{
    public interface ILogQueue
    {
        void Enqueue(ApiLog log);
        bool TryDequeue(out ApiLog log);
    }

}
