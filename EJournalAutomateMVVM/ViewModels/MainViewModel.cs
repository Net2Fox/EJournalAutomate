using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomateMVVM.Models;
using EJournalAutomateMVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EJournalAutomateMVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

        public IAsyncRelayCommand LoadMessagesCommand { get; }

        public MainViewModel(IApiService apiService, INavigationService navigationService)
        {
            _apiService = apiService ?? throw new ArgumentException(nameof(apiService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            LoadMessagesCommand = new AsyncRelayCommand(LoadMessagesAsync);
        }

        private async Task LoadMessagesAsync()
        {
            var messages = await _apiService.GetMessagesAsync();
            Messages.Clear();
            foreach (var message in messages)
            {
                Messages.Add(message);
            }
        }
    }
}
