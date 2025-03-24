using EJournalAutomate.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJournalAutomate.Models.API.Responses
{
    class MessageInfoResult
    {
        [JsonPropertyName("message")]
        public required MessageInfo Message { get; set; }
    }
}
