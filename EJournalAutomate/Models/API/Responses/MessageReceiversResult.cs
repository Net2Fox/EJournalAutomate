using EJournalAutomate.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJournalAutomate.Models.API.Responses
{
    public class MessageReceiversResult
    {
        [JsonPropertyName("groups")]
        public required List<Group> Groups { get; set; }
    }
}
