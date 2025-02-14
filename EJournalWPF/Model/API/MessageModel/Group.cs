using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.MessageModel
{
    internal class Group
    {
        [JsonProperty("users")]
        internal List<User> Users { get; set; }

        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("key")]
        internal string Key { get; set; }

        [JsonProperty("subgroups")]
        internal List<Group> SubGroups { get; set; }
    }
}
