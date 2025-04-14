using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.API.Requests
{
    public class LoginRequest
    {
        [JsonPropertyName("login")]
        public required string Login { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
