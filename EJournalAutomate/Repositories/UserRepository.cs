using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Services.Storage.Cache;
using EJournalAutomate.Helpers;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.API;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using EJournalAutomate.Messages;

namespace EJournalAutomate.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IApiService _apiService;
        private readonly ICacheService _cacheService;

        private readonly ObservableCollection<User> _users = new();
        public ObservableCollection<User> Users => _users;

        public UserRepository(IApiService apiService, ICacheService cacheService)
        {
            _apiService = apiService;
            _cacheService = cacheService;
        }

        public async Task LoadUsersAsync()
        {
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки пользователей: {ex.Message}");
            }
        }
    }
}
