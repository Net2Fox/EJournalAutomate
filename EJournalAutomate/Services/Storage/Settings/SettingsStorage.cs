using EJournalAutomate.Services.Logger;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace EJournalAutomate.Services.Storage.Settings
{
    public class SettingsStorage : ISettingsStorage
    {
        private readonly string _settingsPath = Path.Combine(Environment.CurrentDirectory, "settings.json");
        private readonly string _oldSettingsPath = Path.Combine(Environment.CurrentDirectory, "settings");
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

        public async Task LoadSettings()
        {
            _logger.LogInformation("Попытка загрузить настройки");
            if (File.Exists(_oldSettingsPath))
            {
                LoadOldSettings();
                return;
            }

            if (File.Exists(_settingsPath))
            {
                string settingsContent = await File.ReadAllTextAsync(_settingsPath);
                if (!string.IsNullOrWhiteSpace(settingsContent))
                {
                    _settings = JsonSerializer.Deserialize<Models.Domain.SettingsModel>(settingsContent);
                }
                else
                {
                    _logger.LogInformation("Файл с настройками отсутствует, создание нового файла");
                    await SaveSettings();
                }
            }
            else
            {
                _logger.LogInformation("Файл с настройками неполный или повреждён, создание нового файла");
                await SaveSettings();
            }
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
            LoggingService.SetSettingsSaveLogs(saveLogs);
        }

        public void SetSaveFullName(bool saveFullName)
        {
            _settings.SaveFullName = saveFullName;
        }

        public void SetVendor(string vendor)
        {
            _settings.Vendor = vendor;
        }

        private async void LoadOldSettings()
        {
            _logger.LogInformation("Попытка загрузить старые настройки");

            string settingsContent = await File.ReadAllTextAsync(_oldSettingsPath);
            string[] settings = settingsContent.Split("\n");

            if (settings.Length == 4)
            {
                if (!string.IsNullOrEmpty(settings[0]) && IsValidPath(settings[0]))
                {
                    _settings.SavePath = settings[0];
                }

                if (!string.IsNullOrEmpty(settings[1]))
                {
                    if (bool.TryParse(settings[1], out bool result))
                    {
                        _settings.SaveDate = result;
                    }
                }

                if (!string.IsNullOrEmpty(settings[2]))
                {
                    if (bool.TryParse(settings[2], out bool result))
                    {
                        _settings.SaveLogs = result;
                    }
                }

                if (!string.IsNullOrEmpty(settings[3]))
                {
                    if (!string.IsNullOrWhiteSpace(settings[3]))
                    {
                        _settings.Vendor = settings[3];
                    }
                }

                await SaveSettings();

                File.Delete(_oldSettingsPath);

                _logger.LogInformation("Старые настройки успешно загружены");
            }
            else
            {
                _logger.LogInformation("Файл с настройками неполный или повреждён, создание нового файла");
                await SaveSettings();
            }
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
