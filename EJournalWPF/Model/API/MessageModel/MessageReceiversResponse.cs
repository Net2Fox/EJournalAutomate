using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.MessageModel
{
    internal class MessageReceiversResponse
    {
        [JsonProperty("groups")]
        internal List<Group> Groups;
    }
}
