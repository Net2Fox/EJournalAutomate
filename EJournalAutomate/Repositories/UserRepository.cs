using EJournalAutomate.Exceptions;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Cache;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.IO;

namespace EJournalAutomate.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IAPIService _apiService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserRepository> _logger;

        private readonly ObservableCollection<User> _users = new();

        private readonly ObservableCollection<StudentGroup> _groups = new() { new StudentGroup { Name = "Все" } };

        public ObservableCollection<User> Users => _users;

        public ObservableCollection<StudentGroup> Groups => _groups;

        public UserRepository(IAPIService apiService, ICacheService cacheService, ILogger<UserRepository> logger)
        {
            _apiService = apiService;
            _cacheService = cacheService;
            _logger = logger;

            _logger.LogInformation("UserRepository инициализирован");
        }

        public async Task LoadUsersAsync()
        {
            _logger.LogInformation("Попытка загрузки пользователей");

            List<User> users;
            List<StudentGroup> studentGroups;

            if (_cacheService.IsCacheAvailable)
            {
                (users, studentGroups) = await LoadUsersFromCache();
            }
            else
            {
                (users, studentGroups) = await LoadUsersFromAPI();
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

        private async Task<(List<User>, List<StudentGroup>)> LoadUsersFromCache()
        {
            _logger.LogInformation("Попытка загрузки пользователей из кэша");
            try
            {
                return await _cacheService.LoadCache();
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is FileFormatException || ex is EmptyFileException)
            {
                _logger.LogError($"Проблема с кэшем: {ex.Message}");
                return await LoadUsersFromAPI();
            }

        }

        private async Task<(List<User>, List<StudentGroup>)> LoadUsersFromAPI()
        {
            _logger.LogInformation("Попытка загрузки пользователей из API");
            try
            {
                List<User> users;
                List<StudentGroup> studentGroups;
                (users, studentGroups) = await _apiService.GetMessageReceivers();
                _cacheService.SaveCache(users, studentGroups);
                return await _apiService.GetMessageReceivers();
            }
            catch (Exception ex)
            {
                var exception = new Exception($"Ошибка загрузки пользователей: {ex.Message}");
                _logger.LogCritical(exception, "Ошибка загрузки пользователей");
                throw exception;
            }
        }
    }
}
