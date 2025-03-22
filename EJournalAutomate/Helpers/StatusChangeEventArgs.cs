using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Helpers
{
    public class StatusChangeEventArgs : EventArgs
    {
        public string Message { get; }
        public bool IsLoading { get; }

        public StatusChangeEventArgs(string message, bool isLoading)
        {
            Message = message;
            IsLoading = isLoading;
        }
    }
}
