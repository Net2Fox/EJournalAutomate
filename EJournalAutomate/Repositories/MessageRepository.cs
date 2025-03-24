using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using EJournalAutomate.Helpers;
using EJournalAutomate.Messages;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.UI;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IApiService _apiService;
        private readonly IDispatcherService _dispatcherService;

        private readonly ObservableCollection<Message> _messages = new();

        public ObservableCollection<Message> Messages => _messages;

        public MessageRepository(IApiService apiService, IDispatcherService dispatcherService)
        {
            _apiService = apiService;
            _dispatcherService = dispatcherService;
        }

        public async Task LoadMessagesAsync(int limit = 20)
        {
            try
            {
                var messages = await _apiService.GetMessagesAsync(limit);

                await _dispatcherService.InvokeOnUIThreadAsync(() => {
                    _messages.Clear();
                    foreach (var message in messages)
                    {
                        _messages.Add(message);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки сообщений: {ex.Message}");
            }
        }
    }
}
