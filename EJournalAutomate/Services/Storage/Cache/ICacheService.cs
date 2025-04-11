using EJournalAutomate.Models.Domain;

namespace EJournalAutomate.Services.Storage.Cache
{
    public interface ICacheService
    {
        bool IsCacheAvailable { get; }

        bool IsCacheValid(string json);

        Task<(List<User>, List<StudentGroup>)> LoadCache();
        void SaveCache(List<User> users, List<StudentGroup> groups);
    }
}
