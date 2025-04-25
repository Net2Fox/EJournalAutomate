namespace EJournalAutomate.Services.Storage.Settings
{
    public interface ISettingsStorage
    {
        string SavePath { get; }
        bool SaveDate { get; }
        bool SaveLogs { get; }
        bool SaveFullName { get; }
        string Vendor { get; }

        Task SaveSettings();
        Task LoadSettings();
        void SetSavePath(string savePath);
        void SetSaveDate(bool saveDate);
        void SetSaveLogs(bool saveLogs);
        void SetSaveFullName(bool saveFullName);
        void SetVendor(string vendor);
    }
}
