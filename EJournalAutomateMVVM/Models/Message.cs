using System.Text.Json.Serialization;

namespace EJournalAutomateMVVM.Models
{
    public class Message
    {
        [JsonPropertyName("short_text")]
        public string ShortText { get; set; }

        [JsonPropertyName("user_from")]
        public User UserFrom { get; set; }

        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("unread")]
        public bool Unread { get; set; }

        [JsonPropertyName("with_files")]
        public bool WithFiles { get; set; }

        [JsonPropertyName("with_resources")]
        public bool WithResources { get; set; }

        [JsonIgnore]
        public bool Selected { get; set; }
    }
}
