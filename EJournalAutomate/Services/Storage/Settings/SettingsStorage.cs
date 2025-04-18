﻿using EJournalAutomate.Services.Logger;
using Microsoft.Extensions.Logging;
using System.IO;

namespace EJournalAutomate.Services.Storage.Settings
{
    public class SettingsStorage : ISettingsStorage
    {
        private readonly string _settingsPath = Path.Combine(Environment.CurrentDirectory, "settings");
        private readonly ILogger<SettingsStorage> _logger;

        private string _savePath = Path.Combine(Environment.CurrentDirectory, "Письма");
        public string SavePath => _savePath;

        private bool _saveDate = false;
        public bool SaveDate => _saveDate;

        private bool _saveLogs = false;
        public bool SaveLogs => _saveLogs;

        private string _vendor = string.Empty;

        public string Vendor => _vendor;

        public SettingsStorage(ILogger<SettingsStorage> logger)
        {
            _logger = logger;

            _logger.LogInformation("SettingsStorage инициализирована");
        }

        public async Task LoadSettings()
        {
            _logger.LogInformation("Попытка загрузить настройки");
            if (File.Exists(_settingsPath))
            {
                string settingsContent = await File.ReadAllTextAsync(_settingsPath);
                string[] settings = settingsContent.Split("\n");

                if (settings.Length == 4)
                {
                    if (!string.IsNullOrEmpty(settings[0]) && IsValidPath(settings[0]))
                    {
                        _savePath = settings[0];
                    }

                    if (!string.IsNullOrEmpty(settings[1]))
                    {
                        if (bool.TryParse(settings[1], out bool result))
                        {
                            _saveDate = result;
                        }
                    }

                    if (!string.IsNullOrEmpty(settings[2]))
                    {
                        if (bool.TryParse(settings[2], out bool result))
                        {
                            _saveLogs = result;
                        }
                    }

                    if (!string.IsNullOrEmpty(settings[3]))
                    {
                        if (!string.IsNullOrWhiteSpace(settings[3]))
                        {
                            _vendor = settings[3];
                        }
                    }

                    _logger.LogInformation("Настройки успешно загружены");
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
                await File.WriteAllTextAsync(_settingsPath, $"{_savePath}\n{_saveDate}\n{_saveLogs}\n{_vendor}");
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
            _savePath = savePath;
        }

        public void SetSaveDate(bool saveDate)
        {
            _saveDate = saveDate;
        }

        public void SetSaveLogs(bool saveLogs)
        {
            _saveLogs = saveLogs;
            LoggingService.SetSettingsSaveLogs(saveLogs);
        }

        public void SetVendor(string vendor)
        {
            _vendor = vendor;
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
