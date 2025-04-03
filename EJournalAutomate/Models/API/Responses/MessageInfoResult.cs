using EJournalAutomate.Models.Domain;
using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.API.Responses
{
    class MessageInfoResult
    {
        [JsonPropertyName("message")]
        public required MessageInfo Message { get; set; }
    }
}
