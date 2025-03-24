using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.Views.Pages;
using System.Text;
using System.Windows;

namespace EJournalAutomate.ViewModels
{
    public partial class LoginViewModel : ObservableRecipient
    {
        private readonly IApiService _apiService = Ioc.Default.GetRequiredService<IApiService>();
        private readonly INavigationService _navigationService = Ioc.Default.GetRequiredService<INavigationService>();

        private string? _login;
        public string? Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private string? _token;
        public string? Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }

        [RelayCommand]
        private async Task AuthenticateAsync()
        {
            StringBuilder errorsString = new StringBuilder();

            if (string.IsNullOrWhiteSpace(Login))
            {
                errorsString.AppendLine("Введите логин!");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                errorsString.AppendLine("Введите пароль!");
            }

            if (errorsString.Length > 0)
            {
                ErrorMessage = errorsString.ToString();
                return;
            }

            try
            {
                await _apiService.AuthenticateAsync(Login, Password);
                _navigationService.NavigateTo(typeof(MainPage));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
