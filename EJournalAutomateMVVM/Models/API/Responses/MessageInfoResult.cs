using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Models.API.Responses
{
    class MessageInfoResult
    {
        [JsonPropertyName("message")]
        public MessageInfo Message { get; set; }
    }
}
