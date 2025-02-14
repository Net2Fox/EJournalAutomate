using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.MessageModel
{
    internal class MessagesResponse
    {
        [JsonProperty("total")]
        internal int Total {  get; set; }

        [JsonProperty("")]
        internal int count {  get; set; }

        [JsonProperty("messages")]
        internal List<Message> Messages { get; set; }
    }
}
