using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.Domain
{
    public class User
    {
        [JsonPropertyName("name")]
        public required string ID { get; set; }

        [JsonPropertyName("lastname")]
        public required string LastName { get; set; }

        [JsonPropertyName("firstname")]
        public required string FirstName { get; set; }

        [JsonPropertyName("middlename")]
        public string? MiddleName { get; set; }

        [JsonIgnore]
        public string FullNamePatronymic => $"{LastName.Replace(" ", string.Empty)} {FirstName.Replace(" ", string.Empty)}{(!string.IsNullOrWhiteSpace(MiddleName) ? $" {MiddleName.Replace(" ", string.Empty)}" : string.Empty)}";

        [JsonIgnore]
        public string FullNameWithInitials => $"{LastName.Replace(" ", string.Empty)} {FirstName[0]}. {(!string.IsNullOrWhiteSpace(MiddleName) ? $"{MiddleName[0]}." : string.Empty)}";

        [JsonIgnore]
        public string FullName => $"{LastName.Replace(" ", string.Empty)} {FirstName.Replace(" ", string.Empty)}";

        [JsonPropertyName("groupname")]
        public string? GroupName { get; set; }
    }
}
