using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.API.Requests
{
    public class LoginRequest
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
