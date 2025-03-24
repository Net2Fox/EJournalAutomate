using System.IO;

namespace EJournalAutomate.Services.Storage.Settings
{
    public class SettingsStorage : ISettingsStorage
    {
        private readonly string _settingsPath = Path.Combine(Environment.CurrentDirectory, "settings");

        private string _savePath = Path.Combine(Environment.CurrentDirectory, "Письма");
        public string SavePath => _savePath;

        private bool _saveDate = false;
        public bool SaveDate => _saveDate;

        public async Task LoadSettings()
        {
            if (File.Exists(_settingsPath))
            {
                string settingsContent = await File.ReadAllTextAsync(_settingsPath);
                string[] settings = settingsContent.Split("\n");

                if (settings.Length == 2)
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

                }
            }
        }

        public async Task SaveSettings()
        {
            await File.WriteAllTextAsync(_settingsPath, $"{_savePath}\n{_saveDate}");
        }

        public void SetSavePath(string savePath)
        {
            _savePath = savePath;
        }

        public void SetSaveDate(bool saveDate)
        {
            _saveDate = saveDate;
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
