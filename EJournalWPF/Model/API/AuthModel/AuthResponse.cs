using CefSharp.DevTools.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.AuthModel
{
    internal class AuthResponse
    {
        [JsonProperty("token")]
        internal string Token { get; set; }

        [JsonProperty("expires")]
        internal DateTime Expires { get; set; }
    }
}
