using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.ViewModels;
using EJournalAutomate.Views.Pages;
using System.Windows;
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
        
        public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService, IAPIService apiService)
        {
            InitializeComponent();
            DataContext = viewModel;
            _navigationService = navigationService;
            _apiService = apiService;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _navigationService.SetFrame(MainFrame);

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