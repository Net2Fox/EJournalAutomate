using EJournalAutomate.Models.Domain;

namespace EJournalAutomate.Services.API
{
    public interface IAPIService
    {
        Task<bool> LoadTokenFromAsync();
        Task AuthenticateAsync(string login, string password, string vendor);
        Task<List<Message>> GetMessagesAsync(int limit = 20);
        Task<MessageInfo> GetMessageInfoAsync(string id);
        Task<(List<User>, List<StudentGroup>)> GetMessageReceivers();
    }
}
