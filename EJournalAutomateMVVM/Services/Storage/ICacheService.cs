using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Services.Storage
{
    public interface ICacheService
    {
        bool IsCacheAvailable { get; }

        Task<List<User>> LoadCache();
        void SaveCache(List<User> data);
    }
}
