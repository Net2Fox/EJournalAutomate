﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomate.Services.Logger
{
    public class LoggingService : ILogger
    {
        private readonly string _logFilePath;
        private readonly string _categoryName;
        private static readonly ConcurrentQueue<string> _memoryLogs = new(capacity: 1000);
        private static readonly object _fileLock = new object();

        private static bool _saveLogsToFile;

        private const int MaxMemoryLogs = 1000;

        public LoggingService(string path, string categoryName)
        {
            _logFilePath = path;
            _categoryName = categoryName;

            var logDir = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(logDir) && logDir != null)
                Directory.CreateDirectory(logDir);
        }

        public static bool SaveLogsToFile
        {
            get => _saveLogsToFile;
            set => _saveLogsToFile = value;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_categoryName}: {formatter(state, exception)}";

            if (exception != null)
                message += $"{Environment.NewLine}    Exception: {exception}";

            _memoryLogs.Enqueue(message);
            while (_memoryLogs.Count > MaxMemoryLogs && _memoryLogs.TryDequeue(out _)) { }

            if (_saveLogsToFile || logLevel >= LogLevel.Critical)
            {
                lock (_fileLock)
                {
                    File.AppendAllText(_logFilePath, message + Environment.NewLine);
                }
            }
        }

        public static void SaveLogsOnCrash(string path)
        {
            lock (_fileLock)
            {
                var allLogs = new StringBuilder();
                foreach (var log in _memoryLogs)
                {
                    allLogs.AppendLine(log);
                }

                try
                {
                    File.WriteAllText(path, allLogs.ToString());
                }
                catch { }
            }
        }
    }
}
