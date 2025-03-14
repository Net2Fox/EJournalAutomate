using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Models.Domain
{
    public class Group
    {
        [JsonPropertyName("users")]
        public List<User> Users { get; set; }

        [JsonPropertyName("subgroups")]
        public List<Group> SubGroups { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
