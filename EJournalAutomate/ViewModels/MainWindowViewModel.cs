using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Views.Windows;

namespace EJournalAutomate.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ISettingsStorage _settingsStorage;

        [ObservableProperty]
        private bool _saveDate;

        public MainWindowViewModel(ISettingsStorage settingsStorage)
        {
            _settingsStorage = settingsStorage;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await _settingsStorage.LoadSettings();
            SaveDate = _settingsStorage.SaveDate;
        }

        [RelayCommand]
        private async Task ToggleSaveDate()
        {
            _settingsStorage.SetSaveDate(SaveDate);
            await _settingsStorage.SaveSettings();
        }

        [RelayCommand]
        private void ShowWindowAbout()
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}
