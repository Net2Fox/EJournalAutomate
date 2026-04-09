using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.ViewModels;
using System.Windows.Controls;

namespace EJournalAutomate.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
