using EJournalAutomateMVVM.Helpers;
using EJournalAutomateMVVM.Models.Domain;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Services.Storage.Repository
{
    public interface IUserRepository
    {
        ObservableCollection<User> Users { get; }

        Task LoadUsersAsync();

        bool IsLoading { get; }
        string LoadingMessage { get; }
        event EventHandler<StatusChangeEventArgs> StatusChanged;
    }
}
