﻿using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace EJournalAutomate.Services.Logger
{
    public class LoggingService : ILogger
    {
        private static string _logFilePath;
        private readonly string _categoryName;
        private static readonly ConcurrentQueue<string> _memoryLogs = new();
        private static readonly object _fileLock = new object();

        private static bool _saveLogsToFile;
        private static bool _settingsLoaded = false;

        private const int MaxMemoryLogs = 1000;

        public LoggingService(string path, string categoryName)
        {
            _logFilePath = path;
            _categoryName = categoryName;
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

            if (_saveLogsToFile)
            {
                lock (_fileLock)
                {
                    var logDir = Path.GetDirectoryName(_logFilePath);
                    if (!Directory.Exists(logDir) && logDir != null)
                        Directory.CreateDirectory(logDir);

                    File.AppendAllText(_logFilePath, message + Environment.NewLine);
                }
            }
        }

        public static void SetSettingsSaveLogs(bool saveToFile)
        {
            lock (_fileLock)
            {
                _saveLogsToFile = saveToFile;

                if (_saveLogsToFile)
                {
                    try
                    {
                        var logDir = Path.GetDirectoryName(_logFilePath);
                        if (!Directory.Exists(logDir) && logDir != null)
                            Directory.CreateDirectory(logDir);

                        using var writer = File.AppendText(_logFilePath);
                        foreach (var message in _memoryLogs)
                        {
                            writer.WriteLine(message);
                        }
                    }
                    catch { }
                }

                _settingsLoaded = true;
            }
        }

        public static void SaveLogsOnCrash(string path)
        {
            lock (_fileLock)
            {
                var logDir = Path.GetDirectoryName(path);
                if (!Directory.Exists(logDir) && logDir != null)
                    Directory.CreateDirectory(logDir);

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
