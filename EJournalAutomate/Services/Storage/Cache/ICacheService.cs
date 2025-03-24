using EJournalAutomateMVVM.Models.Domain;

namespace EJournalAutomate.Services.Storage.Cache
{
    public interface ICacheService
    {
        bool IsCacheAvailable { get; }

        Task<List<User>> LoadCache();
        void SaveCache(List<User> data);
    }
}
