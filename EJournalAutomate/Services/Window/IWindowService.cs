namespace EJournalAutomate.Services.Window;

public interface IWindowService
{
    void ShowAboutWindow();
    string? ShowDirectorySettingsWindow(string currentDirectory);
    string? ShowDevKeySettingWindow();
}