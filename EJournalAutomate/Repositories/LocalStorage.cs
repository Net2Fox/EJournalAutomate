using CommunityToolkit.Mvvm.Messaging;
using EJournalAutomate.Messages;
using EJournalAutomate.Models.Domain;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Repositories
{
    public class LocalStorage : ILocalStorage
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessenger _messenger;
        private readonly ILogger<LocalStorage> _logger;

        public ObservableCollection<Message> Messages => _messageRepository.Messages;

        public ObservableCollection<User> Users => _userRepository.Users;

        public LocalStorage(IMessageRepository messageRepository, IUserRepository userRepository, IMessenger messenger, ILogger<LocalStorage> logger)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _messenger = messenger;
            _logger = logger;

            _logger.LogInformation("LocalStorage инициализирован");
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Попытка загрузка данных");

            _messenger.Send(new StatusMessage("Загрузка данных...", true));

            try
            {
                await Task.WhenAll(
                    _messageRepository.LoadMessagesAsync(),
                    _userRepository.LoadUsersAsync()
                );
                _logger.LogInformation("Данные успешно загружены");
                _messenger.Send(new StatusMessage(string.Empty, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка загрузки данных");
                _messenger.Send(new StatusMessage($"Ошибка загрузки данных: {ex.Message}", true));
            }
        }

        public async Task RefreshMessagesAsync(int limit = 20)
        {
            _logger.LogInformation("Попытка обновить сообщения");

            _messenger.Send(new StatusMessage("Загрузка сообщений...", true));

            try
            {
                await _messageRepository.LoadMessagesAsync(limit);
                _logger.LogInformation("Сообщения успешно обновлены");
                _messenger.Send(new StatusMessage(string.Empty, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка загрузки сообщений");
                _messenger.Send(new StatusMessage($"Ошибка загрузки сообщений: {ex.Message}", true));
            }
        }
    }
}
