using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Exceptions
{
    public class ApiException : Exception
    {
        public int? StatusCode { get; }

        public ApiException(string message, int? statusCode = null) 
            : base(message)
        { 
            StatusCode = statusCode; 
        }

        public ApiException(string message, Exception innerException, int? statusCode = null) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
