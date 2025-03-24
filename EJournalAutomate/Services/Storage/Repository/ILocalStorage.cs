using EJournalAutomate.Helpers;
using EJournalAutomate.Models.Domain;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Services.Storage.Repository
{
    public interface ILocalStorage
    {
        ObservableCollection<Message> Messages { get; }
        ObservableCollection<User> Users { get; }

        Task InitializeAsync();
        Task RefreshMessagesAsync(int limit = 20);

        bool IsLoading { get; }
        string LoadingMessage { get; }
        event EventHandler<StatusChangeEventArgs> StatusChanged;
    }
}
