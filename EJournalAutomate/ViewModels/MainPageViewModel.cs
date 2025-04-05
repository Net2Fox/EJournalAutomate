using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EJournalAutomate.Messages;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Repositories;
using EJournalAutomate.Services.Download;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace EJournalAutomate.ViewModels
{
    /// <summary>
    /// MainPageViewModel для MainPage
    /// </summary>
    public partial class MainPageViewModel : ObservableRecipient, IRecipient<StatusMessage>
    {
        private readonly ILocalStorage _localStorage;
        private readonly IDownloadService _downloadService;
        private readonly ILogger<MainPageViewModel> _logger;

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

        private int _selectedGroupIndex = 0;
        public int SelectedGroupIndex
        {
            get => _selectedGroupIndex;
            set
            {
                if (SetProperty(ref _selectedGroupIndex, value))
                {
                    ApplyFilters();
                }
            }
        }

        private bool _isDateFilterEnabled = false;

        public bool IsDateFilterEnabled
        {
            get => _isDateFilterEnabled;
            set
            {
                if (SetProperty(ref _isDateFilterEnabled, value))
                {
                    ApplyFilters();
                }
            }
        }

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
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

        public ObservableCollection<StudentGroup> StudentGroups => _localStorage.Groups;

        private ICollectionView _filteredMessages;
        public ICollectionView FilteredMessages => _filteredMessages;

        public MainPageViewModel(ILocalStorage localStorage, IDownloadService downloadService, IMessenger messenger, ILogger<MainPageViewModel> logger)
            : base(messenger)
        {
            _localStorage = localStorage;
            _downloadService = downloadService;
            _logger = logger;

            IsActive = true;

            _filteredMessages = CollectionViewSource.GetDefaultView(_localStorage.Messages);
            _filteredMessages.Filter = MessageFilter;

            _localStorage.Messages.CollectionChanged += _messages_CollectionChanged;

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await _localStorage.InitializeAsync();
            _logger.LogDebug("MainPageViewModel инициализирована");
        }

        [RelayCommand]
        private async Task ApplyLimitAsync()
        {
            if (int.TryParse(MessageLimit, out int limit) && limit > 0)
            {
                _logger.LogInformation($"Обновление сообщений по новому лимиту: {limit}");
                try
                {
                    await _localStorage.RefreshMessagesAsync(limit);
                    ApplyFilters();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при обновлении сообщений");
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

            bool passesSearchFilter = true;
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string text = SearchText.ToLower();
                passesSearchFilter =
                    message.UserFrom.FirstName.ToLower().Contains(text) ||
                    message.UserFrom.LastName.ToLower().Contains(text) ||
                    (message.UserFrom.MiddleName != null && message.UserFrom.MiddleName.ToLower().Contains(text)) ||
                    message.Subject.ToLower().Contains(text);
            }

            bool passesStatusFilter = true;
            if (SelectedStatusIndex != 0)
            {
                bool filterUnread = (SelectedStatusIndex == 1);
                passesStatusFilter = (message.Unread == filterUnread);
            }

            bool passesGroupFilter = true;
            if (SelectedGroupIndex != 0)
            {
                passesGroupFilter = (message.UserFrom.GroupName == StudentGroups[SelectedGroupIndex].Name);
            }

            bool passesDateFilter = true;
            if (IsDateFilterEnabled)
            {
                passesDateFilter = (message.Date.Year == SelectedDate.Year &&
                                  message.Date.Month == SelectedDate.Month &&
                                  message.Date.Day == SelectedDate.Day);
            }

            return passesSearchFilter && passesStatusFilter && passesGroupFilter && passesDateFilter;
        }

        private void ApplyFilters()
        {
            FilteredMessages.Refresh();
            _logger.LogDebug($"""
                Применены фильтры:
                \n\tПоиск='{SearchText}',
                \n\tСтатус={SelectedStatusIndex}
                \n\tГруппа={SelectedGroupIndex}
                \n\tДата={SelectedDate}
                """);
        }

        [RelayCommand]
        private void SelectAllMessages()
        {
            if (!IsAllSelected)
            {
                foreach (Message message in _filteredMessages)
                {
                    message.Selected = true;
                }
                IsAllSelected = true;
            }
            else
            {
                foreach (Message message in _filteredMessages)
                {
                    message.Selected = false;
                }
                IsAllSelected = false;
            }
            _logger.LogInformation($"Пользователь {(IsAllSelected ? "выбрал" : "снял выбор со")} всех сообщений. Затронуто: {_localStorage.Messages.Count}");
        }

        [RelayCommand(CanExecute = nameof(CanDownloadMessages))]
        private async Task DownloadMessagesAsync()
        {
            try
            {
                _logger.LogInformation("Попытка скачать выбранные сообщения");
                var messagesToDownload = _localStorage.Messages.Where(m => m.Selected && m.WithFiles).ToList();

                if (messagesToDownload.Count == 0)
                {
                    _logger.LogInformation("Нет выбранных сообщений");
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
                _logger.LogInformation("Выбранные сообщения успешно скачаны");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при скачивании");
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

        public void Receive(StatusMessage message)
        {
            LoadingMessage = message.Message;
            IsLoading = message.IsLoading;
        }
    }
}
