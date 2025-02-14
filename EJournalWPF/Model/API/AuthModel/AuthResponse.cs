using Newtonsoft.Json;
using System;

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
