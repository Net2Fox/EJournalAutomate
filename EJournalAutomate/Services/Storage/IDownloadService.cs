using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Services.Storage
{
    public interface IDownloadService
    {
        Task DownloadMessagesAsync(List<Message> messages, IProgress<(int current, int total)> progress);
    }
}
