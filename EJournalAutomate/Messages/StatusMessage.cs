using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomate.Messages
{
    public class StatusMessage
    {
        public string Message { get; }
        public bool IsLoading { get; }

        public StatusMessage(string message, bool isLoading)
        {
            Message = message;
            IsLoading = isLoading;
        }
    }
}
