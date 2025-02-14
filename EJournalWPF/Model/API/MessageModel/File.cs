using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.MessageModel
{
    internal class File
    {
        [JsonProperty("filename")]
        internal string FileName { get; set; }

        [JsonProperty("link")]
        internal Uri Link { get; set; }
    }
}
