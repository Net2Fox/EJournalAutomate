using EJournalAutomateMVVM.Helpers;
using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
