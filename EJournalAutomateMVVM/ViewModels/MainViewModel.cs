using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EJournalAutomateMVVM.Models.Domain;
using EJournalAutomateMVVM.Services.API;
using EJournalAutomateMVVM.Services.Navigation;
using EJournalAutomateMVVM.Services.Storage;
using EJournalAutomateMVVM.Services.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace EJournalAutomateMVVM.ViewModels
{
    public partial class MainViewModel : ObservableRecipient
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ICacheService _cacheService;
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


        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();

        private ICollectionView _filteredMessages;
        public ICollectionView FilteredMessages => _filteredMessages;

        private List<User>? _students;

        public MainViewModel(IApiService apiService,
            INavigationService navigationService,
            IDispatcherService dispatcherService,
            ICacheService cacheService,
            IDownloadService downloadService)
        {
            _apiService = apiService ?? throw new ArgumentException(nameof(apiService));
            _navigationService = navigationService ?? throw new ArgumentException(nameof(navigationService));
            _dispatcherService = dispatcherService ?? throw new ArgumentException(nameof(dispatcherService));
            _cacheService = cacheService ?? throw new ArgumentException(nameof(cacheService));
            _downloadService = downloadService ?? throw new ArgumentException(nameof(downloadService));

            _filteredMessages = CollectionViewSource.GetDefaultView(_messages);
            _filteredMessages.Filter = MessageFilter;

            _messages.CollectionChanged += _messages_CollectionChanged;

            Task.Run(async () => await InitializeAsync());
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

        private async Task InitializeAsync()
        {
            await LoadMessagesAsync();
            await LoadStudentsAsync();
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
                    _messages.Clear();
                    foreach (var message in messages)
                    {
                        _messages.Add(message);
                    }
                });

                IsLoading = false;
            }
            catch (Exception ex)
            {
                LoadingMessage = $"Произошла ошибка: {ex.Message}";
            }
        }

        private async Task LoadStudentsAsync()
        {
            IsLoading = true;
            LoadingMessage = "Загрузка данных, пожалуйста подождите...";
            if (_cacheService.IsCacheAvailable)
            {
                _students = await _cacheService.LoadCache();
            }
            else
            {
                _students = await _apiService.GetMessageReceivers();
                _cacheService.SaveCache(_students);
            }
            IsLoading = false;
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
            if (!IsAllSelected)
            {
                foreach (Message message in _messages)
                {
                    message.Selected = true;
                }
                IsAllSelected = true;
            }
            else
            {
                foreach (Message message in _messages)
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
                var messagesToDownload = _messages.Where(m => m.Selected && m.WithFiles).ToList();

                if (messagesToDownload.Count == 0)
                {
                    return;
                }

                IsLoading = true;
                //TotalItems = messagesToDownload.Count();
                //CurrentProgress = 0;
                LoadingMessage = $"Скачивание {messagesToDownload.Count()} сообщений...";
                var progress = new Progress<(int current, int total, string status)>(progressInfo =>
                {
                    //CurrentProgress = progressInfo.current;
                    //TotalItems = progressInfo.total;
                    //ProgressStatus = progressInfo.status;

                    double percentage = (double)progressInfo.current / progressInfo.total * 100;
                    LoadingMessage = $"Скачивание: {progressInfo.current} из {progressInfo.total} ({percentage:F1}%)\n{progressInfo.status}";
                });


                if (_students != null)
                {
                    await _downloadService.DownloadMessagesAsync(messagesToDownload, _students, progress);
                }

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
            return _messages.Any(m => m.Selected && m.WithFiles);
        }
    }
}
