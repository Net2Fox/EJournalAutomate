namespace EJournalAutomate.Services.Storage.Token
{
    public interface ITokenStorage
    {
        Task SaveTokenAsync(string token);
        Task<string> LoadTokenAsync();
        bool TokenExists();
    }
}
