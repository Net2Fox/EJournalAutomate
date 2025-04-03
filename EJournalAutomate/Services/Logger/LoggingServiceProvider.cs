using Microsoft.Extensions.Logging;

namespace EJournalAutomate.Services.Logger
{
    public class LoggingServiceProvider : ILoggerProvider
    {
        private readonly string _logFilePath;

        public LoggingServiceProvider(string path)
        {
            _logFilePath = path;
        }

        public ILogger CreateLogger(string categoryName) => new LoggingService(_logFilePath, categoryName);
        public void Dispose() { }
    }
}
