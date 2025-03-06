using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomateMVVM.Services;
using EJournalAutomateMVVM.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EJournalAutomateMVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;

        private string _login;
        public string Login
        {
            get { return _login; }
            set { _login = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private string _token;
        public string Token
        {
            get => _token;
            set { _token = value; OnPropertyChanged(); }
        }

        public IAsyncRelayCommand AuthenticateCommand { get; }

        public LoginViewModel(IApiService apiService, INavigationService navigationService)
        {
            _apiService = apiService ?? throw new ArgumentException(nameof(apiService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            AuthenticateCommand = new AsyncRelayCommand(AuthenticateAsync);
        }

        private async Task AuthenticateAsync()
        {
            try
            {
                await _apiService.AuthenticateAsync(Login, Password);
                _navigationService.NavigateTo(typeof(MainView));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
