using EJournalAutomate.Services.Logger;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using EJournalAutomate.Models.Domain;
using Microsoft.Extensions.Options;
using File = System.IO.File;

namespace EJournalAutomate.Services.Storage.Settings
{
    public class SettingsStorage : ISettingsStorage
    {
        private readonly IOptionsMonitor<SettingsModel> _settingsMonitor;
        private readonly string _settingsPath = Path.Combine(Environment.CurrentDirectory, "settings.json");
        private readonly ILogger<SettingsStorage> _logger;

        public SettingsStorage(IOptionsMonitor<SettingsModel> settingsMonitor, ILogger<SettingsStorage> logger)
        {
            _settingsMonitor = settingsMonitor;
            _logger = logger;
            _logger.LogInformation("SettingsStorage инициализирована");
        }

        public async Task SaveSettings()
        {
            _logger.LogInformation("Попытка сохранения настроек");
            try
            {
                await File.WriteAllTextAsync(_settingsPath, JsonSerializer.Serialize(_settingsMonitor.CurrentValue));
                _logger.LogInformation("Настройки успешно сохранены");
            }
            catch (Exception ex)
            {
                var exception = new Exception("Не удалось сохранить настройки", ex);
                _logger.LogCritical(ex, "Не удалось сохранить настройки");
                throw exception;
            }

        }

        public void SetSavePath(string savePath)
        {
            _settingsMonitor.CurrentValue.SavePath = savePath;
        }

        public void SetSaveDate(bool saveDate)
        {
            _settingsMonitor.CurrentValue.SaveDate = saveDate;
        }

        public void SetSaveLogs(bool saveLogs)
        {
            _settingsMonitor.CurrentValue.SaveLogs = saveLogs;
        }

        public void SetSaveFullName(bool saveFullName)
        {
            _settingsMonitor.CurrentValue.SaveFullName = saveFullName;
        }

        public void SetVendor(string vendor)
        {
            _settingsMonitor.CurrentValue.Vendor = vendor;
        }

        private bool IsValidPath(string path)
        {
            try
            {
                Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
