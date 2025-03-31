using EJournalAutomate.Helpers;
using EJournalAutomate.Models.Domain;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Repositories
{
    public interface IMessageRepository
    {
        ObservableCollection<Models.Domain.Message> Messages { get; }

        Task LoadMessagesAsync(int limit = 20);
     }
}
