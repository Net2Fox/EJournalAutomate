using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.Services.API;
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
            var navigationService = Ioc.Default.GetService<NavigationService>();
            if (navigationService != null)
            {
                navigationService.SetFrame(MainFrame);
            }

            var apiService = Ioc.Default.GetService<IApiService>();
            bool tokenExists = await apiService.LoadTokenFromAsync();
            if (tokenExists)
            {
                MainFrame.Navigate(new MainPage());
            }
            else
            {
                MainFrame.Navigate(new LoginPage());
            }
        }
    }
}