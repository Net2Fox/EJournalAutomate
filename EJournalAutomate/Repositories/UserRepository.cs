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

        private readonly ObservableCollection<StudentGroup> _groups = new() { new StudentGroup { Name = "Все" } };

        public ObservableCollection<User> Users => _users;

        public ObservableCollection<StudentGroup> Groups => _groups;

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
                List<StudentGroup> studentGroups;

                if (_cacheService.IsCacheAvailable)
                {
                    (users, studentGroups) = await _cacheService.LoadCache();
                }
                else
                {
                    (users, studentGroups) = await _apiService.GetMessageReceivers();
                    _cacheService.SaveCache(users, studentGroups);
                }

                Users.Clear();

                foreach (var user in users)
                {
                    Users.Add(user);
                }

                foreach (var group in studentGroups)
                {
                    Groups.Add(group);
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
