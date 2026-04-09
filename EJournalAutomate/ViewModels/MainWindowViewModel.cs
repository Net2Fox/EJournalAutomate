using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EJournalAutomate.Messages;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Services.Storage.Token;
using EJournalAutomate.Views.Pages;
using EJournalAutomate.Views.Windows;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Controls;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.Dialog;
using EJournalAutomate.Services.Window;
using Microsoft.Extensions.Options;

namespace EJournalAutomate.ViewModels
{
    public partial class MainWindowViewModel : ObservableRecipient, IRecipient<NavigationMessage>
    {
        private readonly ISettingsStorage _settingsStorage;
        private readonly IOptions<SettingsModel> _settingsOptions;
        private readonly IWindowService _windowService;
        private readonly IDialogService _dialogService;
        private readonly ILogger<MainWindowViewModel> _logger;

        [ObservableProperty]
        private string _savePath;

        [ObservableProperty]
        private bool _saveDate;

        [ObservableProperty]
        private bool _saveLogs;

        [ObservableProperty]
        private bool _saveFullName;

        [ObservableProperty]
        private bool _isSettingsMenuItemEnabled = true;

        public MainWindowViewModel(
            ISettingsStorage settingsStorage, 
            IOptions<SettingsModel> settingsOptions,
            IWindowService windowService,
            IDialogService dialogService,
            IMessenger messenger, 
            ILogger<MainWindowViewModel> logger)
            : base (messenger)
        {
            _settingsStorage = settingsStorage;
            _settingsOptions = settingsOptions;
            _windowService = windowService;
            _dialogService = dialogService;
            _logger = logger;

            IsActive = true;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                SavePath = _settingsOptions.Value.SavePath;
                SaveDate = _settingsOptions.Value.SaveDate;
                SaveLogs = _settingsOptions.Value.SaveLogs;
                SaveFullName = _settingsOptions.Value.SaveFullName;
                _logger.LogDebug("MainWindowViewModel инициализирована");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
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
                _dialogService.ShowError(ex.Message);
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
                _dialogService.ShowError(ex.Message);
                _logger.LogError(ex, $"Настройка логов не изменена");
            }
        }

        [RelayCommand]
        private async Task ToggleSaveFullName()
        {
            try
            {
                _settingsStorage.SetSaveFullName(SaveFullName);
                await _settingsStorage.SaveSettings();
                _logger.LogInformation($"Настройка логов изменена: {SaveLogs}");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
                _logger.LogError(ex, $"Настройка сохранения отчества не изменена");
            }
        }

        [RelayCommand]
        private void ShowWindowAbout()
        {
            _windowService.ShowAboutWindow();
        }

        [RelayCommand]
        private async Task ShowWindowDirectorySettingsAsync()
        {
            var result = _windowService.ShowDirectorySettingsWindow(SavePath);

            if (result != null)
            {
                SavePath = result;
                try
                {
                    _settingsStorage.SetSavePath(SavePath);
                    await _settingsStorage.SaveSettings();
                    _dialogService.ShowInfo("Путь успешно установлен!");
                    _logger.LogInformation($"Настройка пути скачивания: {SavePath}");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError(ex.Message);
                    _logger.LogError(ex, $"Настройка пути скачивания не изменена");
                }
            }
        }

        public void Receive(NavigationMessage message)
        {
            if (message.NavigatedPageType == typeof(LoginPage))
            {
                IsSettingsMenuItemEnabled = false;
            }
            else
            {
                IsSettingsMenuItemEnabled = true;
            }
        }
    }
}
