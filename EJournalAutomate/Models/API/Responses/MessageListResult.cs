using EJournalAutomate.Models.Domain;
using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.API.Responses
{
    class MessageListResult
    {
        [JsonPropertyName("total")]
        public string Total { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
    }
}
