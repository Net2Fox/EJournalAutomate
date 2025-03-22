namespace EJournalAutomateMVVM.Services.Storage
{
    public interface ITokenStorage
    {
        Task SaveTokenAsync(string token);
        Task<string> LoadTokenAsync();
        bool TokenExists();
    }
}
