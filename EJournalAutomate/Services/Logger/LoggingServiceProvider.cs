using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
