using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJournalAutomate.Models.Domain
{
    public class Group
    {
        [JsonPropertyName("users")]
        public required List<User> Users { get; set; }

        [JsonPropertyName("subgroups")]
        public List<Group>? SubGroups { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("key")]
        public required string Key { get; set; }
    }
}
