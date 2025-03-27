using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomate.Services.Logger
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private readonly string _categoryName;
        private static readonly object _lock = new object();

        public FileLogger(string path, string categoryName)
        {
            _logFilePath = path;
            _categoryName = categoryName;

            var logDir = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(logDir) && logDir != null)
                Directory.CreateDirectory(logDir);
        }

        public IDisposable BeginScope<TState>(TState state) => default!;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_categoryName}: {formatter(state, exception)}";

            if (exception != null)
                message += $"{Environment.NewLine}    Exception: {exception}";

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, message + Environment.NewLine);
            }
        }
    }
}
