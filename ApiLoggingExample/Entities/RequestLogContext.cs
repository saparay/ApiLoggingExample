namespace ApiLoggingExample.Entities
{
    public class RequestLogContext
    {
        public List<string> Logs { get; } = new();

        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }

}
