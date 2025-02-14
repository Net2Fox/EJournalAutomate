using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.MessageModel
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Message
    {
        [JsonProperty("short_text")]
        public string Short_Text { get; set; }

        [JsonProperty("user_from")]
        public User User_From { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("unread")]
        public bool Unread { get; set; }

        [JsonProperty("with_files")]
        public bool With_Files { get; set; }

        [JsonProperty("with_resources")]
        public bool With_Resources { get; set; }

        public bool Selected { get; set; }
    }
}
