using EJournalAutomate.Models.Domain;

namespace EJournalAutomate.Services.Download
{
    public interface IDownloadService
    {
        Task DownloadMessagesAsync(List<Models.Domain.Message> messages, IProgress<(int current, int total)> progress);
    }
}
