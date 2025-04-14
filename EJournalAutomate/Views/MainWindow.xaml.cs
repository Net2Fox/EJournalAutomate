using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.ViewModels;
using EJournalAutomate.Views.Pages;
using System.Windows;

namespace EJournalAutomate.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = Ioc.Default.GetService<MainWindowViewModel>()
                ?? throw new InvalidOperationException("Не удалось получить MainWindowViewModel из DI контейнера.");
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var navigationService = Ioc.Default.GetService<INavigationService>();

            navigationService.SetFrame(MainFrame);

            var apiService = Ioc.Default.GetService<IAPIService>();
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