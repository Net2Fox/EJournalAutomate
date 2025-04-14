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

        public ObservableCollection<StudentGroup> Groups => _userRepository.Groups;

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

            _messenger.Send(new DataLoadMessage("Загрузка данных...", true));

            try
            {
                await Task.WhenAll(
                    _messageRepository.LoadMessagesAsync(),
                    _userRepository.LoadUsersAsync()
                );
                foreach (Message message in _messageRepository.Messages)
                {
                    User? user = _userRepository.Users.FirstOrDefault(u => u.ID == message.UserFrom.ID);
                    if (user != null)
                    {
                        message.UserFrom.GroupName = user.GroupName;
                    }
                }
                _messageRepository.Messages.Select(m => m.UserFrom.GroupName = _userRepository.Users.FirstOrDefault(u => u.ID == m.UserFrom.ID)?.GroupName);
                _logger.LogInformation("Данные успешно загружены");
                _messenger.Send(new DataLoadMessage(false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка загрузки данных");
                _messenger.Send(new ErrorMessage($"Ошибка загрузки данных: {ex.Message}"));
            }
        }

        public async Task RefreshMessagesAsync(int limit = 20)
        {
            _logger.LogInformation("Попытка обновить сообщения");

            _messenger.Send(new DataLoadMessage("Загрузка сообщений...", true));

            try
            {
                await Task.WhenAll(_messageRepository.LoadMessagesAsync(limit));
                foreach (Message message in _messageRepository.Messages)
                {
                    User? user = _userRepository.Users.FirstOrDefault(u => u.ID == message.UserFrom.ID);
                    if (user != null)
                    {
                        message.UserFrom.GroupName = user.GroupName;
                    }
                }
                _logger.LogInformation("Сообщения успешно обновлены");
                _messenger.Send(new DataLoadMessage(false));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка загрузки сообщений");
                _messenger.Send(new ErrorMessage($"Ошибка загрузки сообщений: {ex.Message}"));
            }
        }
    }
}
