using EJournalAutomateMVVM.Helpers;
using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Services.Storage
{
    public interface IMessageRepository
    {
        ObservableCollection<Message> Messages { get; }

        Task LoadMessagesAsync(int limit = 20);

        bool IsLoading { get; }
        string LoadingMessage { get; }
        event EventHandler<StatusChangeEventArgs> StatusChanged;
     }
}
