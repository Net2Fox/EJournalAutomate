using EJournalAutomateMVVM.Models.Domain;

namespace EJournalAutomate.Services.Download
{
    public interface IDownloadService
    {
        Task DownloadMessagesAsync(List<Message> messages, IProgress<(int current, int total)> progress);
    }
}
