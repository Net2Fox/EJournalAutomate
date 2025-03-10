using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomateMVVM.Models;
using EJournalAutomateMVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace EJournalAutomateMVVM.ViewModels
{
    public partial class MainViewModel : ObservableRecipient
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;
        private readonly IDispatcherService _dispatcherService;

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set { SetProperty(ref _isLoading, value); }
        }

        private string _loadingMessage;
        public string LoadingMessage
        {
            get => _loadingMessage;
            set { SetProperty(ref _loadingMessage, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    SetProperty(ref _searchText, value);
                    ApplyFilters();
                }
            }
        }

        private int _selectedStatusIndex;
        public int SelectedStatusIndex
        {
            get => _selectedStatusIndex;
            set
            {
                SetProperty(ref _selectedStatusIndex, value);
                ApplyFilters();
            }
        }

        private int _messageLimit = 20;
        public string MessageLimit
        {
            get => _messageLimit.ToString();
            set
            {
                if (int.TryParse(value, out int limit) && limit > 0 && _messageLimit != limit)
                {
                    SetProperty(ref _messageLimit, limit);
                }
                else
                {
                    SetProperty(ref _messageLimit, 20);
                }
            }
        }

        private bool _isSearchFocused;
        public bool IsSearchFocused
        {
            get => _isSearchFocused;
            set
            {
                SetProperty(ref _isSearchFocused, value);
            }
        }

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

        private ICollectionView _filteredMessages;
        public ICollectionView FilteredMessages => _filteredMessages;

        public MainViewModel(IApiService apiService, INavigationService navigationService, IDispatcherService dispatcherService)
        {
            _apiService = apiService ?? throw new ArgumentException(nameof(apiService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            _dispatcherService = dispatcherService ?? throw new ArgumentException(nameof(dispatcherService));

            _filteredMessages = CollectionViewSource.GetDefaultView(Messages);
            _filteredMessages.Filter = MessageFilter;

            Task.Run(async () => await InitializeAsync());
        }

        private async Task InitializeAsync()
        {
            await LoadMessagesAsync();
        }

        private async Task LoadMessagesAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Загрузка данных, пожалуйста подождите...";

                var messages = await _apiService.GetMessagesAsync(_messageLimit);

                await _dispatcherService.InvokeOnUIThreadAsync(() =>
                {
                    Messages.Clear();
                    foreach (var message in messages)
                    {
                        Messages.Add(message);
                    }
                });

                IsLoading = false;
            }
            catch (Exception ex)
            {
                LoadingMessage = $"Произошла ошибка: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ApplyLimitAsync()
        {
            await LoadMessagesAsync();
        }

        private bool MessageFilter(object item)
        {
            if (!(item is Message message))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string text = SearchText.ToLower();
                bool containsText =
                    message.UserFrom.FirstName.ToLower().Contains(text) ||
                    message.UserFrom.LastName.ToLower().Contains(text) ||
                    (message.UserFrom.MiddleName != null && message.UserFrom.MiddleName.ToLower().Contains(text)) ||
                    message.Subject.ToLower().Contains(text);

                if (!containsText)
                {
                    return false;
                }
            }

            if (SelectedStatusIndex != 0)
            {
                bool filterUnread = (SelectedStatusIndex == 1);
                if (message.Unread != filterUnread)
                {
                    return false;
                }
            }

            return true;
        }

        private void ApplyFilters()
        {
            FilteredMessages.Refresh();
        }

        [RelayCommand]
        private void SelectAllMessages()
        {
            foreach (Message message in Messages)
            {
                message.Selected = true;
            }
        }

        [RelayCommand(CanExecute = nameof(CanDownloadMessages))]
        private async Task DownloadMessagesAsync()
        {
            try
            {
                var messagesToDownload = Messages.Where(m => m.Selected && m.WithFiles).ToList();

                if (messagesToDownload.Count == 0)
                {
                    return;
                }

                IsLoading = true;
                LoadingMessage = $"Скачивание {messagesToDownload.Count()} сообщений...";

                //TODO await _downloadService.DownloadMessageAsync(messagesToDownload);

                foreach (var message in messagesToDownload)
                {
                    message.Unread = false;
                    message.Selected = false;
                }

                ApplyFilters();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                LoadingMessage = $"Ошибка при скачивании: {ex.Message}";
            }
        }

        private bool CanDownloadMessages()
        {
            var messagesToDownload = Messages.Where(m => m.Selected && m.WithFiles).ToList();

            if (messagesToDownload.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
