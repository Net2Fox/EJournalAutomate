using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.AuthModel
{
    internal class AuthModel
    {
        [JsonProperty("login")]
        internal string Login { get; set; }

        [JsonProperty("password")]
        internal string Password { get; set; }

        internal AuthModel(string login, string password)
        {
            Login = login;
            Password = password;
        }   
    }
}
