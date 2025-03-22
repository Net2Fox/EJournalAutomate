using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.Services.API;
using EJournalAutomateMVVM.Services.Navigation;
using EJournalAutomateMVVM.Views.Pages;
using System.Windows;
using NavigationService = EJournalAutomateMVVM.Services.Navigation.NavigationService;

namespace EJournalAutomateMVVM.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var navigationService = Ioc.Default.GetService<INavigationService>();

            navigationService.SetFrame(MainFrame);

            var apiService = Ioc.Default.GetService<IApiService>();
            bool tokenExists = await apiService.LoadTokenFromAsync();
            if (tokenExists)
            {
                navigationService.NavigateTo(typeof(MainPage));
            }
            else
            {
                navigationService.NavigateTo(typeof(LoginPage));
            }
        }
    }
}