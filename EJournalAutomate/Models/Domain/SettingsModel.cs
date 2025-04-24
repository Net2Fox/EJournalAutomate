using System.IO;

namespace EJournalAutomate.Models.Domain
{
    public class SettingsModel
    {
        public string SavePath { get; set; } = Path.Combine(Environment.CurrentDirectory, "Письма");
        public bool SaveDate { get; set; } = false;
        public bool SaveLogs { get; set; } = false;
        public bool SaveFullName { get; set; } = true;
        public string Vendor { get; set; } = string.Empty;


    }
}
