using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.Domain
{
    public class File
    {
        [JsonPropertyName("filename")]
        public required string Filename { get; set; }

        [JsonPropertyName("link")]
        public required Uri Link { get; set; }
    }
}
