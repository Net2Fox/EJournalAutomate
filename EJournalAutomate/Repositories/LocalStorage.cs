using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using EJournalAutomate.Helpers;
using EJournalAutomate.Messages;
using EJournalAutomate.Models.Domain;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Repositories
{
    public class LocalStorage : ILocalStorage
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessenger _messenger;

        public ObservableCollection<Message> Messages => _messageRepository.Messages;

        public ObservableCollection<User> Users => _userRepository.Users;

        public LocalStorage(IMessageRepository messageRepository, IUserRepository userRepository, IMessenger messenger)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _messenger = messenger;
        }

        public async Task InitializeAsync()
        {
            _messenger.Send(new StatusMessage("Загрузка данных...", true));

            try
            {
                await Task.WhenAll(
                    _messageRepository.LoadMessagesAsync(),
                    _userRepository.LoadUsersAsync()
                );
                _messenger.Send(new StatusMessage(string.Empty, false));
            }
            catch (Exception ex)
            {
                _messenger.Send(new StatusMessage($"Ошибка инициализации: {ex.Message}", true));
            }
        }

        public async Task RefreshMessagesAsync(int limit = 20)
        {
            _messenger.Send(new StatusMessage("Загрузка сообщений...", true));

            try
            {
                await _messageRepository.LoadMessagesAsync(limit);

                _messenger.Send(new StatusMessage(string.Empty, false));
            }
            catch (Exception ex)
            {
                _messenger.Send(new StatusMessage($"Ошибка загрузки сообщений: {ex.Message}", true));
            }
        }
    }
}
