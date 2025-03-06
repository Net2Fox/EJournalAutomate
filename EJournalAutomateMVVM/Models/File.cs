using System.Text.Json.Serialization;

namespace EJournalAutomateMVVM.Models
{
    public class File
    {
        [JsonPropertyName("filename")]
        internal string Filename { get; set; }

        [JsonPropertyName("link")]
        internal Uri Link { get; set; }
    }
}
