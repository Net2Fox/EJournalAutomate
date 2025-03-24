using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.API.Responses
{
    public class AuthResult
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("expires")]
        public string Expires { get; set; }
    }
}
