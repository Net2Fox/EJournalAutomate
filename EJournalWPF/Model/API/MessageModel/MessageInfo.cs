using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.MessageModel
{
    internal class MessageInfo
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("short_text")]
        public string Short_Text { get; set; }

        [JsonProperty("user_from")]
        public User User_From { get; set; }

        [JsonProperty("user_to")]
        public List<User> User_To { get; set; }

        [JsonProperty("files")]
        public List<File> Files { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}
