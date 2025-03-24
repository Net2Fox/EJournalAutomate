using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.Domain
{
    public class MessageInfo
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("short_text")]
        public string Short_Text { get; set; }

        [JsonPropertyName("user_from")]
        public User User_From { get; set; }

        [JsonPropertyName("user_to")]
        public List<User> User_To { get; set; }

        [JsonPropertyName("files")]
        public List<File> Files { get; set; }

        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }
}
