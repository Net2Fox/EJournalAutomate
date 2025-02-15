using EJournalAutomate.Data;
using EJournalWPF.Data;
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

namespace EJournalWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private AuthRepository _authRepository;
        public AuthPage()
        {
            InitializeComponent();
            _authRepository = (App.Current as App).GetAuthRepository;
            _authRepository.AuthEvent += AuthEvent;
        }

        private void AuthEvent(bool isSuccess, string error)
        {
            if (isSuccess)
            {
                MessageBox.Show("Авторизация успешна!");
                NavigationService.Navigate(new MainPage());
            }
            else
            {
                MessageBox.Show(error);
            }
        }

        private async void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            await _authRepository.Auth(LoginTextBox.Text, PasswordTextBox.Password);
        }
    }
}
