using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Models
{
    public class User
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("middlename")]
        public string MiddleName { get; set; }

        [JsonIgnore]
        public string FullName => $"{LastName} {FirstName} {MiddleName}";

        [JsonIgnore]
        public string NameWithInitials => $"{LastName} {FirstName[0]}. {(!string.IsNullOrWhiteSpace(MiddleName) ? $"{MiddleName[0]}." : string.Empty)}";
    }
}
