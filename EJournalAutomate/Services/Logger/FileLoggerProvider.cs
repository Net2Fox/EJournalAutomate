using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomate.Services.Logger
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _logFilePath;

        public FileLoggerProvider(string path)
        {
            _logFilePath = path;
        }

        public ILogger CreateLogger(string categoryName) => new FileLogger(_logFilePath, categoryName);
        public void Dispose() { }
    }
}
