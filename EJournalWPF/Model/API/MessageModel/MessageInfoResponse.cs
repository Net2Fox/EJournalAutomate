using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.MessageModel
{
    internal class MessageInfoResponse
    {
        [JsonProperty("message")]
        internal MessageInfo Message {  get; set; }
    }
}
