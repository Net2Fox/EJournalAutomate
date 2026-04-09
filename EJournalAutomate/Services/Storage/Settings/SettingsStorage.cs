using EJournalAutomate.Services.Logger;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace EJournalAutomate.Services.Storage.Settings
{
    public class SettingsStorage : ISettingsStorage
    {
        private readonly string _settingsPath = Path.Combine(Environment.CurrentDirectory, "settings.json");
        private readonly ILogger<SettingsStorage> _logger;

        private Models.Domain.SettingsModel _settings;
        public string SavePath => _settings.SavePath;
        public bool SaveDate => _settings.SaveDate;
        public bool SaveLogs => _settings.SaveLogs;
        public bool SaveFullName => _settings.SaveFullName;
        public string Vendor => _settings.Vendor;

        public SettingsStorage(ILogger<SettingsStorage> logger)
        {
            _logger = logger;
            _settings = new();
            _logger.LogInformation("SettingsStorage инициализирована");
        }

        public async Task SaveSettings()
        {
            _logger.LogInformation("Попытка сохранения настроек");
            try
            {
                await File.WriteAllTextAsync(_settingsPath, JsonSerializer.Serialize<Models.Domain.SettingsModel>(_settings));
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
            _settings.SavePath = savePath;
        }

        public void SetSaveDate(bool saveDate)
        {
            _settings.SaveDate = saveDate;
        }

        public void SetSaveLogs(bool saveLogs)
        {
            _settings.SaveLogs = saveLogs;
        }

        public void SetSaveFullName(bool saveFullName)
        {
            _settings.SaveFullName = saveFullName;
        }

        public void SetVendor(string vendor)
        {
            _settings.Vendor = vendor;
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
