using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomate.Services.Download
{
    public interface IDownloadService
    {
        Task DownloadMessagesAsync(List<Message> messages, IProgress<(int current, int total)> progress);
    }
}
