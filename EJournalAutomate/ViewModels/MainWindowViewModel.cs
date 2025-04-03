using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomate.Services.Logger;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Views.Windows;
using Microsoft.Extensions.Logging;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace EJournalAutomate.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ISettingsStorage _settingsStorage;
        private readonly ILogger<MainWindowViewModel> _logger;

        [ObservableProperty]
        private string _savePath;

        [ObservableProperty]
        private bool _saveDate;

        [ObservableProperty]
        private bool _saveLogs;

        public MainWindowViewModel(ISettingsStorage settingsStorage, ILogger<MainWindowViewModel> logger)
        {
            _settingsStorage = settingsStorage;
            _logger = logger;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                SavePath = _settingsStorage.SavePath;
                SaveDate = _settingsStorage.SaveDate;
                SaveLogs = _settingsStorage.SaveLogs;
                _logger.LogDebug("MainWindowViewModel инициализирована");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogCritical(ex, "MainWindowViewModel не инициализирована");
            }

        }

        [RelayCommand]
        private async Task ToggleSaveDate()
        {
            try
            {
                _settingsStorage.SetSaveDate(SaveDate);
                await _settingsStorage.SaveSettings();
                _logger.LogInformation($"Настройка даты изменена: {SaveDate}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError(ex, $"Настройка даты не изменена");
            }

        }

        [RelayCommand]
        private async Task ToggleSaveLogs()
        {
            try
            {
                _settingsStorage.SetSaveLogs(SaveLogs);
                await _settingsStorage.SaveSettings();
                _logger.LogInformation($"Настройка логов изменена: {SaveLogs}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.LogError(ex, $"Настройка логов не изменена");
            }
        }

        [RelayCommand]
        private void ShowWindowAbout()
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
            _logger.LogInformation("Окно \"О программе\" успешно открыто");
        }

        [RelayCommand]
        private async Task ShowWindowDirectorySettingsAsync()
        {
            DirectorySettingsWindow settingsWindow = new DirectorySettingsWindow();
            _logger.LogInformation("Окно \"Настройки\" успешно открыто");
            settingsWindow.SavePath = SavePath;
            if (settingsWindow.ShowDialog() == true)
            {
                SavePath = settingsWindow.SavePath;
                try
                {
                    _settingsStorage.SetSavePath(SavePath);
                    await _settingsStorage.SaveSettings();
                    System.Windows.MessageBox.Show("Путь успешно установлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _logger.LogInformation($"Настройка пути скачивания: {SavePath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.LogError(ex, $"Настройка пути скачивания не изменена");
                }
            }
        }
    }
}
