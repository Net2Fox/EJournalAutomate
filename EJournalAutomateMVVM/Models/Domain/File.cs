using System.Text.Json.Serialization;

namespace EJournalAutomateMVVM.Models.Domain
{
    public class File
    {
        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("link")]
        public Uri Link { get; set; }
    }
}
