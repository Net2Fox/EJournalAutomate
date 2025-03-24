using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.Helpers;
using EJournalAutomateMVVM.Models.Domain;
using EJournalAutomateMVVM.Services.API;
using EJournalAutomateMVVM.Services.UI;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Services.Storage.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IApiService _apiService = Ioc.Default.GetRequiredService<IApiService>();
        private readonly IDispatcherService _dispatcherService = Ioc.Default.GetRequiredService<IDispatcherService>();
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
