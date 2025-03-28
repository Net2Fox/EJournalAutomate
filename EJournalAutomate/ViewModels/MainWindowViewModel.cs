using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Views.Windows;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace EJournalAutomate.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ISettingsStorage _settingsStorage;
        private readonly ILogger<MainWindowViewModel> _logger;

        [ObservableProperty]
        private bool _saveDate;

        public MainWindowViewModel(ISettingsStorage settingsStorage, ILogger<MainWindowViewModel> logger)
        {
            _settingsStorage = settingsStorage;
            _logger = logger;

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await _settingsStorage.LoadSettings();
                SaveDate = _settingsStorage.SaveDate;
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
        private void ShowWindowAbout()
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
            _logger.LogInformation("Окно \"О программе\" успешно открыто");
        }
    }
}
