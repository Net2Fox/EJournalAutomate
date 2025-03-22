using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.ViewModels;
using System.Windows.Controls;

namespace EJournalAutomateMVVM.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

            DataContext = Ioc.Default.GetService<LoginViewModel>()
                ?? throw new InvalidOperationException("Не удалось получить LoginViewModel из DI контейнера.");
        }

        private void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }
}
