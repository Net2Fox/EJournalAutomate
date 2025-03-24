using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.Domain
{
    public class MessageInfo
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        [JsonPropertyName("short_text")]
        public required string Short_Text { get; set; }

        [JsonPropertyName("user_from")]
        public required User User_From { get; set; }

        [JsonPropertyName("user_to")]
        public required List<User> User_To { get; set; }

        [JsonPropertyName("files")]
        public required List<File> Files { get; set; }

        [JsonPropertyName("id")]
        public required string ID { get; set; }

        [JsonPropertyName("subject")]
        public required string Subject { get; set; }

        [JsonPropertyName("date")]
        public required string Date { get; set; }
    }
}
