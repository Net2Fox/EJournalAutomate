using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.UI;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IApiService _apiService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ILogger<MessageRepository> _logger;

        private readonly ObservableCollection<Message> _messages = new();

        public ObservableCollection<Message> Messages => _messages;

        public MessageRepository(IApiService apiService, IDispatcherService dispatcherService, ILogger<MessageRepository> logger)
        {
            _apiService = apiService;
            _dispatcherService = dispatcherService;
            _logger = logger;

            _logger.LogInformation("MessageRepository инициализирован");
        }

        public async Task LoadMessagesAsync(int limit = 20)
        {
            _logger.LogInformation($"Попытка загрузки сообщений: {limit}");

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

                _logger.LogInformation("Сообщения успешно загружены");
            }
            catch (Exception ex)
            {
                var exception = new Exception($"Ошибка загрузки сообщений: {ex.Message}");
                _logger.LogCritical(exception, "Ошибка загрузки сообщений");
                throw exception;
            }
        }
    }
}
