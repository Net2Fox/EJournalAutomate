using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace EJournalAutomate.Models.Domain
{
    public class Message : ObservableObject
    {
        [JsonPropertyName("short_text")]
        public string ShortText { get; set; }

        [JsonPropertyName("user_from")]
        public User UserFrom { get; set; }

        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        private bool _unread;

        [JsonPropertyName("unread")]
        public bool Unread
        {
            get => _unread;
            set { SetProperty(ref _unread, value); }
        }

        [JsonPropertyName("with_files")]
        public bool WithFiles { get; set; }

        [JsonPropertyName("with_resources")]
        public bool WithResources { get; set; }

        private bool _selected;

        [JsonIgnore]
        public bool Selected
        {
            get => _selected;
            set { SetProperty(ref _selected, value); }
        }
    }
}
