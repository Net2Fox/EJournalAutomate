﻿using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Services.Storage.Cache;
using EJournalAutomate.Helpers;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.API;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Services.Storage.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IApiService _apiService = Ioc.Default.GetRequiredService<IApiService>();
        private readonly ICacheService _cacheService = Ioc.Default.GetRequiredService<ICacheService>();
        private readonly ObservableCollection<User> _users = new();
        private bool _isLoading;
        private string _loadingMessage = string.Empty;

        public ObservableCollection<User> Users => _users;

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

        public async Task LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Загрузка данных пользователей...";

                List<User> users;

                if (_cacheService.IsCacheAvailable)
                {
                    LoadingMessage = "Загрузка данных пользователей из кэша...";
                    users = await _cacheService.LoadCache();
                }
                else
                {
                    LoadingMessage = "Загрузка данных пользователей из API...";
                    users = await _apiService.GetMessageReceivers();
                    _cacheService.SaveCache(users);
                }

                _users.Clear();

                foreach (var user in users)
                {
                    _users.Add(user);
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                LoadingMessage = $"Ошибка загрузки пользователей: {ex.Message}";
                throw;
            }
        }

        private void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, new StatusChangeEventArgs(_loadingMessage, _isLoading));
        }
    }
}
