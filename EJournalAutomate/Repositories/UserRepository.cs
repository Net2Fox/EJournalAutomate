using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Storage.Cache;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IApiService _apiService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserRepository> _logger;

        private readonly ObservableCollection<User> _users = new();
        public ObservableCollection<User> Users => _users;

        public UserRepository(IApiService apiService, ICacheService cacheService, ILogger<UserRepository> logger)
        {
            _apiService = apiService;
            _cacheService = cacheService;
            _logger = logger;

            _logger.LogInformation("UserRepository инициализирован");
        }

        public async Task LoadUsersAsync()
        {
            _logger.LogInformation("Попытка загрузки пользователей");

            try
            {
                List<User> users;

                if (_cacheService.IsCacheAvailable)
                {
                    users = await _cacheService.LoadCache();
                }
                else
                {
                    users = await _apiService.GetMessageReceivers();
                    _cacheService.SaveCache(users);
                }

                _users.Clear();

                foreach (var user in users)
                {
                    _users.Add(user);
                }

                _logger.LogInformation("Пользователи успешно загружены");
            }
            catch (Exception ex)
            {
                var exception = new Exception($"Ошибка загрузки пользователей: {ex.Message}");
                _logger.LogError(exception, "Ошибка загрузки пользователей");
                throw exception;
            }
        }
    }
}
