using EJournalAutomate.Models.Domain;
using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.API.Responses
{
    class MessageListResult
    {
        [JsonPropertyName("total")]
        public required string Total { get; set; }

        [JsonPropertyName("count")]
        public required int Count { get; set; }

        [JsonPropertyName("messages")]
        public required List<Message> Messages { get; set; }
    }
}
