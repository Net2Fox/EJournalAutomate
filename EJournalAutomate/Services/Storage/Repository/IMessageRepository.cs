﻿using EJournalAutomateMVVM.Helpers;
using EJournalAutomateMVVM.Models.Domain;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Services.Storage.Repository
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
