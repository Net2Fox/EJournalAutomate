using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Services
{
    public interface ITokenStorage
    {
        Task SaveTokenAsync(string token);
        Task<string> LoadTokenAsync();
        bool TokenExists();
    }
}
