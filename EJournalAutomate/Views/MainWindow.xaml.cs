using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.ViewModels;
using EJournalAutomate.Views.Pages;
using System.Windows;
using EJournalAutomate.Services.Dialog;
using EJournalAutomate.Services.Storage.Token;
using EJournalAutomate.Services.Window;
using Microsoft.Extensions.DependencyInjection;

namespace EJournalAutomate.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;
        private readonly IAPIService _apiService;
        private readonly IWindowService _windowService;
        private readonly ISecureStorage _devKeyStorage;
        private readonly IDialogService _dialogService;
        
        public MainWindow(
            MainWindowViewModel viewModel, 
            INavigationService navigationService, 
            IAPIService apiService, 
            IWindowService windowService,
            [FromKeyedServices("devkey")]ISecureStorage devKeyStorage,
            IDialogService dialogService)
        {
            InitializeComponent();
            DataContext = viewModel;
            _navigationService = navigationService;
            _apiService = apiService;
            _windowService = windowService;
            _devKeyStorage = devKeyStorage;
            _dialogService = dialogService;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _navigationService.SetFrame(MainFrame);

            bool devKeyExists = await _apiService.LoadDevKeyFromAsync();
            if (!devKeyExists)
            {
                var result = _windowService.ShowDevKeySettingWindow();
                if (result == null)
                {
                    _dialogService.ShowError("Для работы приложения необходимо задать DevKey. Откройте приложение заново и введите DevKey.");
                    Application.Current.Shutdown();
                    return;
                }
                await _devKeyStorage.SaveAsync(result);
                await _apiService.LoadDevKeyFromAsync();
            }
            
            bool tokenExists = await _apiService.LoadTokenFromAsync();
            if (tokenExists)
            {
                _navigationService.NavigateTo(typeof(MainPage));
            }
            else
            {
                _navigationService.NavigateTo(typeof(LoginPage));
            }
        }
    }
}