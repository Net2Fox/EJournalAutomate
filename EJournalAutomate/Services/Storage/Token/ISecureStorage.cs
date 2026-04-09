namespace EJournalAutomate.Services.Storage.Token
{
    public interface ISecureStorage
    {
        Task SaveAsync(string value);
        Task<string> LoadAsync();
        bool Exists();
    }
}
