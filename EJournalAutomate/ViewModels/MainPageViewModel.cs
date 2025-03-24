using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomate.Services.Download;
using EJournalAutomate.Services.Storage.Repository;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.Navigation;
using EJournalAutomate.Services.UI;
using System.ComponentModel;
using System.Windows.Data;

namespace EJournalAutomate.ViewModels
{
    /// <summary>
    /// MainPageViewModel для MainPage
    /// </summary>
    public partial class MainPageViewModel : ObservableRecipient
    {
        private readonly ILocalStorage _localStorage;
        private readonly IDownloadService _downloadService;

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string? _loadingMessage;
        public string? LoadingMessage
        {
            get => _loadingMessage;
            set => SetProperty(ref _loadingMessage, value);
        }

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    if (SetProperty(ref _searchText, value))
                    {
                        ApplyFilters();
                    }
                }
            }
        }

        private int _selectedStatusIndex;
        public int SelectedStatusIndex
        {
            get => _selectedStatusIndex;
            set
            {
                if (SetProperty(ref _selectedStatusIndex, value))
                {
                    ApplyFilters();
                }
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
            set => SetProperty(ref _isSearchFocused, value);
        }

        private bool _isAllSelected;

        public bool IsAllSelected
        {
            get => _isAllSelected;
            set => SetProperty(ref _isAllSelected, value);
        }

        private ICollectionView _filteredMessages;
        public ICollectionView FilteredMessages => _filteredMessages;

        public MainPageViewModel(ILocalStorage localStorage, IDownloadService downloadService)
        {
            _localStorage = localStorage;
            _downloadService = downloadService;

            _localStorage.StatusChanged += LocalStorage_StatusChanged;

            _filteredMessages = CollectionViewSource.GetDefaultView(_localStorage.Messages);
            _filteredMessages.Filter = MessageFilter;

            _localStorage.Messages.CollectionChanged += _messages_CollectionChanged;

            Task.Run(async () => await InitializeAsync());
        }

        private async Task InitializeAsync()
        {
            try
            {
                await _localStorage.InitializeAsync();
            }
            catch(Exception ex)
            {
                IsLoading = true;
                LoadingMessage = $"Ошибка при инициализации данных: {ex.Message}";
            }
        }

        private void LocalStorage_StatusChanged(object? sender, Helpers.StatusChangeEventArgs e)
        {
            IsLoading = e.IsLoading;
            LoadingMessage = e.Message;
        }

        [RelayCommand]
        private async Task ApplyLimitAsync()
        {
            if (int.TryParse(MessageLimit, out int limit) && limit > 0)
            {
                try
                {
                    await _localStorage.RefreshMessagesAsync(limit);
                    ApplyFilters();
                }
                catch (Exception ex)
                {
                    LoadingMessage = $"Ошибка при обновлении сообщений: {ex.Message}";
                }
            }
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
            if (!IsAllSelected)
            {
                foreach (Message message in _localStorage.Messages)
                {
                    message.Selected = true;
                }
                IsAllSelected = true;
            }
            else
            {
                foreach (Message message in _localStorage.Messages)
                {
                    message.Selected = false;
                }
                IsAllSelected = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanDownloadMessages))]
        private async Task DownloadMessagesAsync()
        {
            try
            {
                var messagesToDownload = _localStorage.Messages.Where(m => m.Selected && m.WithFiles).ToList();

                if (messagesToDownload.Count == 0)
                {
                    return;
                }

                IsLoading = true;
                LoadingMessage = $"Скачивание {messagesToDownload.Count()} сообщений...";

                var progress = new Progress<(int current, int total)>(progressInfo =>
                {

                    double percentage = (double)progressInfo.current / progressInfo.total * 100;
                    LoadingMessage = $"Скачивание: {progressInfo.current} из {progressInfo.total} ({percentage:F1}%)";
                });

                await _downloadService.DownloadMessagesAsync(messagesToDownload, progress);

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
            return _localStorage.Messages.Any(m => m.Selected && m.WithFiles);
        }

        private void _messages_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DownloadMessagesCommand.NotifyCanExecuteChanged();

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= MessageItem_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += MessageItem_PropertyChanged;
                }
            }
        }

        private void MessageItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Message.Selected))
            {
                DownloadMessagesCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
