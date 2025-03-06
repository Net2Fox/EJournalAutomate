using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.Services;
using EJournalAutomateMVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EJournalAutomateMVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Page
    {
        public LoginView()
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
