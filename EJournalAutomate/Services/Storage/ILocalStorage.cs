﻿using EJournalAutomateMVVM.Helpers;
using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Services.Storage
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
