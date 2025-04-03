using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.Views.Pages;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EJournalAutomate.ViewModels
{
    public partial class LoginViewModel : ObservableRecipient
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;
        private readonly ILogger<LoginViewModel> _logger;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AuthenticateCommand))]
        private string _vendor;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AuthenticateCommand))]
        private string? _login;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AuthenticateCommand))]
        private string? _password;

        [ObservableProperty]
        private string? _errorMessage;

        public LoginViewModel(IApiService apiService, INavigationService navigationService, ILogger<LoginViewModel> logger)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            _logger = logger;

            _logger.LogDebug("LoginViewModel создана");
        }

        [RelayCommand(CanExecute = nameof(CanAuthenticate))]
        private async Task AuthenticateAsync()
        {
            _logger.LogInformation("Начата авторизация");

            try
            {
                await _apiService.AuthenticateAsync(Login, Password, Vendor);

                _logger.LogInformation("Успешная авторизация");

                _navigationService.NavigateTo(typeof(MainPage));
            }
            catch (Exception ex)
            {
                _logger.LogError(exception:ex, "Ошибка авторизации");
                ErrorMessage = ex.Message;
            }
        }

        private bool CanAuthenticate()
        {
            return !string.IsNullOrWhiteSpace(Vendor) && !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
