using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API
{
    internal class Result<T>
    {
        internal bool Success { get; set; }

        internal T Data { get; set; }

        internal string Error { get; set; }

        internal Result(bool success, T data, string error)
        {
            Success = success;
            Data = data;
            Error = error;
        }
    }
}
