using EJournalAutomateMVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Services
{
    public interface IApiService
    {
        Task<bool> LoadTokenFromAsync();
        Task AuthenticateAsync(string login, string password);
        Task<List<Message>> GetMessagesAsync(int limit = 20);
        Task<MessageInfo> GetMessageInfoAsync(string id);
    }
}
