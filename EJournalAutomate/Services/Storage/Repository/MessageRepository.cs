using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Helpers;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.UI;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Services.Storage.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IApiService _apiService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ObservableCollection<Message> _messages = new();
        private bool _isLoading;
        private string _loadingMessage = string.Empty;

        public ObservableCollection<Message> Messages => _messages;

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                OnStatusChanged();
            }
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            private set
            {
                _loadingMessage = value;
                OnStatusChanged();
            }
        }


        public event EventHandler<StatusChangeEventArgs> StatusChanged;

        public MessageRepository(IApiService apiService, IDispatcherService dispatcherService)
        {
            _apiService = apiService;
            _dispatcherService = dispatcherService;
        }

        public async Task LoadMessagesAsync(int limit = 20)
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Загрузка сообщений...";

                var messages = await _apiService.GetMessagesAsync(limit);

                await _dispatcherService.InvokeOnUIThreadAsync(() => {
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
                LoadingMessage = $"Ошибка загрузки сообщений: {ex.Message}";
                throw;
            }
        }

        private void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, new StatusChangeEventArgs(_loadingMessage, _isLoading));
        }
    }
}
