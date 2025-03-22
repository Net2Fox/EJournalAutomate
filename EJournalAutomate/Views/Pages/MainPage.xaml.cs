using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.ViewModels;
using System.Windows.Controls;

namespace EJournalAutomateMVVM.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            
            DataContext = Ioc.Default.GetService<MainViewModel>()
                ?? throw new InvalidOperationException("Не удалось получить MainViewModel из DI контейнера.");
        }
    }
}
