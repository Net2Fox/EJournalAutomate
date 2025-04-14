using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.API.Responses
{
    public class AuthResult
    {
        [JsonPropertyName("token")]
        public required string Token { get; set; }

        [JsonPropertyName("expires")]
        public required string Expires { get; set; }
    }
}
