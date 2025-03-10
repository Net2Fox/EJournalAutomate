using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomateMVVM.Services;
using EJournalAutomateMVVM.Views;

namespace EJournalAutomateMVVM.ViewModels
{
    public class LoginViewModel : ObservableRecipient
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;

        private string _login;
        public string Login
        {
            get => _login;
            set { SetProperty(ref _login, value); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { SetProperty(ref _password, value); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { SetProperty(ref _errorMessage, value); }
        }

        private string _token;
        public string Token
        {
            get => _token;
            set { SetProperty(ref _token, value); }
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
