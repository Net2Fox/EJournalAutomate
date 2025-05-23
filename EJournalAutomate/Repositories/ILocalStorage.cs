﻿using EJournalAutomate.Models.Domain;
using System.Collections.ObjectModel;

namespace EJournalAutomate.Repositories
{
    public interface ILocalStorage
    {
        ObservableCollection<Message> Messages { get; }
        ObservableCollection<User> Users { get; }
        ObservableCollection<StudentGroup> Groups { get; }

        Task InitializeAsync();
        Task RefreshMessagesAsync(int limit = 20);
    }
}
