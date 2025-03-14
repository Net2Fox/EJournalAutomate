using System.Text.Json.Serialization;

namespace EJournalAutomateMVVM.Models.Domain
{
    public class User
    {

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("middlename")]
        public string MiddleName { get; set; }

        [JsonIgnore]
        public string FullName => $"{LastName} {FirstName} {(!string.IsNullOrWhiteSpace(MiddleName) ? $"{MiddleName[0]}." : string.Empty)}";

        [JsonIgnore]
        public string NameWithInitials => $"{LastName} {FirstName[0]}. {(!string.IsNullOrWhiteSpace(MiddleName) ? $"{MiddleName[0]}." : string.Empty)}";

        [JsonPropertyName("groupname")]
        public string GroupName { get; set; }
    }
}
