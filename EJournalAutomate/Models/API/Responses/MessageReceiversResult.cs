using EJournalAutomate.Models.Domain;
using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.API.Responses
{
    public class MessageReceiversResult
    {
        [JsonPropertyName("groups")]
        public required List<Group> Groups { get; set; }
    }
}
