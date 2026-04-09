using EJournalAutomate.Models.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EJournalAutomate.Services.Logger
{
    public class LoggingServiceProvider : ILoggerProvider
    {
        private readonly string _logFilePath;
        private readonly IDisposable? _onChangeListener;

        public LoggingServiceProvider(string path, IOptionsMonitor<SettingsModel> settingsMonitor)
        {
            _logFilePath = path;
            LoggingService.SetSettingsSaveLogs(settingsMonitor.CurrentValue.SaveLogs);

            _onChangeListener = settingsMonitor.OnChange(settings =>
            {
                LoggingService.SetSettingsSaveLogs(settings.SaveLogs);
            });
        }

        public ILogger CreateLogger(string categoryName) => new LoggingService(_logFilePath, categoryName);

        public void Dispose()
        {
            _onChangeListener?.Dispose();
        }
    }
}
