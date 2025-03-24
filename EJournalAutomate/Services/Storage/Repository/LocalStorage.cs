using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomateMVVM.Helpers;
using EJournalAutomateMVVM.Models.Domain;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Services.Storage.Repository
{
    public class LocalStorage : ILocalStorage
    {
        private readonly IMessageRepository _messageRepository = Ioc.Default.GetRequiredService<IMessageRepository>();
        private readonly IUserRepository _userRepository = Ioc.Default.GetRequiredService<IUserRepository>();
        private bool _isLoading;
        private string _loadingMessage = string.Empty;

        public ObservableCollection<Message> Messages => _messageRepository.Messages;

        public ObservableCollection<User> Users => _userRepository.Users;

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

        public LocalStorage()
        {
            _messageRepository.StatusChanged += Repository_StatusChanged;
            _userRepository.StatusChanged += Repository_StatusChanged;
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            LoadingMessage = "Инициализация данных...";

            try
            {
                await Task.WhenAll(
                    _messageRepository.LoadMessagesAsync(),
                    _userRepository.LoadUsersAsync()
                );

                IsLoading = false;
            }
            catch (Exception ex)
            {
                LoadingMessage = $"Ошибка инициализации: {ex.Message}";
            }
        }

        public Task RefreshMessagesAsync(int limit = 20)
        {
            return _messageRepository.LoadMessagesAsync(limit);
        }

        private void Repository_StatusChanged(object? sender, StatusChangeEventArgs e)
        {
            if (e.IsLoading)
            {
                IsLoading = true;
                LoadingMessage = e.Message;
            }
            else if (_messageRepository.IsLoading || _userRepository.IsLoading)
            {
                IsLoading = true;
            }
            else
            {
                IsLoading = false;
                LoadingMessage = string.Empty;
            }
        }

        private void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, new StatusChangeEventArgs(_loadingMessage, _isLoading));
        }
    }
}
