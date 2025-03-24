﻿using EJournalAutomate.Models.Domain;

namespace EJournalAutomate.Services.API
{
    public interface IApiService
    {
        Task<bool> LoadTokenFromAsync();
        Task AuthenticateAsync(string login, string password);
        Task<List<Message>> GetMessagesAsync(int limit = 20);
        Task<MessageInfo> GetMessageInfoAsync(string id);
        Task<List<User>> GetMessageReceivers();
    }
}
