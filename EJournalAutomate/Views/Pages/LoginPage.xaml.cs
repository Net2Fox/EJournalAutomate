using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.ViewModels;
using System.Windows.Controls;

namespace EJournalAutomate.Views.Pages
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

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }
}
