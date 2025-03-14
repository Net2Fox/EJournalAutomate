using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EJournalAutomateMVVM.Services.Storage
{
    public interface ILocalStorage
    {
        Task<IReadOnlyList<Message>> GetMessagesAsync(int limit = 20);
        Task<MessageInfo> GetMessageInfoAsync(string id);

        Task<IReadOnlyList> GetUsersAsync();
        Task<User> GetUserByIDAsync(string id);

        event EventHandler<DataChangedEventManager> DataChanged;
    }
}
