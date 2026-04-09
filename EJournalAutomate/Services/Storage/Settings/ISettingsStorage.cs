namespace EJournalAutomate.Services.Storage.Settings
{
    public interface ISettingsStorage
    {
        Task SaveSettings();
        void SetSavePath(string savePath);
        void SetSaveDate(bool saveDate);
        void SetSaveLogs(bool saveLogs);
        void SetSaveFullName(bool saveFullName);
        void SetVendor(string vendor);
    }
}
