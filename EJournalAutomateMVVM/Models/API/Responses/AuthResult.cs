using System.Text.Json.Serialization;

namespace EJournalAutomateMVVM.Models.API.Responses
{
    public class AuthResult
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("expires")]
        public string Expires { get; set; }
    }
}
