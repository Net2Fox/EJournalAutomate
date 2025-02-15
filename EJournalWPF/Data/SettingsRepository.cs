using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomate.Data
{
    internal class SettingsRepository
    {
        private string _saveDirectory = $"{Environment.CurrentDirectory}\\Письма";

        private bool _saveDateTime = false;

        internal string GetSaveDirectory { get { return _saveDirectory; } }
        internal bool GetSaveDateTime { get { return _saveDateTime; } }

        public SettingsRepository()
        {
            LoadSettings();
        }

        internal void SaveSettings()
        {
            System.IO.File.WriteAllText($"{Environment.CurrentDirectory}\\settings", $"{_saveDirectory}\n{_saveDateTime}");
        }

        private void LoadSettings()
        {
            if (System.IO.File.Exists($"{Environment.CurrentDirectory}\\settings"))
            {
                string[] settings = System.IO.File.ReadAllText($"{Environment.CurrentDirectory}\\settings").Split('\n');
                if (settings.Length > 0 && settings.Length < 2)
                {
                    _saveDirectory = settings[0];
                }
                else
                {
                    _saveDirectory = settings[0];
                    _saveDateTime = Convert.ToBoolean(settings[1]);
                }

            }
        }

        internal void SetSaveDirectory(string directory)
        {
            _saveDirectory = directory;
        }

        internal void SetDateTimeSave()
        {
            _saveDateTime = !_saveDateTime;
        }
    }
}
