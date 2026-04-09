using EJournalAutomate.Models.Domain;

namespace EJournalAutomate.Services.Cache
{
    public interface ICacheService
    {
        bool IsCacheAvailable { get; }

        bool IsCacheValid(string json);

        Task<(List<User>, List<StudentGroup>)> LoadCache();
        Task SaveCache(List<User> users, List<StudentGroup> groups);
    }
}
