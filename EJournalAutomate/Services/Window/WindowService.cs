using EJournalAutomate.Views.Windows;
using Microsoft.Extensions.Logging;

namespace EJournalAutomate.Services.Window;

public class WindowService : IWindowService
{
    private readonly ILogger<WindowService> _logger;
    
    public WindowService(ILogger<WindowService> logger)
    {
        _logger = logger;
    }
    
    public void ShowAboutWindow()
    {
        new AboutWindow().ShowDialog();
        _logger.LogInformation("Окно \"О программе\" успешно открыто");
    }

    public string? ShowDirectorySettingsWindow(string currentDirectory)
    {
        var window = new DirectorySettingsWindow
        {
            SavePath = currentDirectory
        };
        var result = window.ShowDialog() == true ? window.SavePath : null;
        _logger.LogInformation("Окно \"Настройки\" успешно открыто");
        return result;
    }

    public string? ShowDevKeySettingWindow()
    {
        var window = new DevKeySettingWindow();
        var result = window.ShowDialog() == true ? window.DevKey : null;
        _logger.LogInformation("Окно \"Установка DevKey\" успешно открыто");
        return result;
    }
}