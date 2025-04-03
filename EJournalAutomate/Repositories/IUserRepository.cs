using EJournalAutomate.Models.Domain;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Repositories
{
    public interface IUserRepository
    {
        ObservableCollection<User> Users { get; }

        Task LoadUsersAsync();
    }
}
