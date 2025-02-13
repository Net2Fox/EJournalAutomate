using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API
{

    internal class ResponseAPI
    {
        [JsonProperty("response")]
        internal ResponseData Response { get; set; }
    }

    internal class ResponseData
    {
        [JsonProperty("state")]
        internal int State { get; set; }

        [JsonProperty("error")]
        internal string Error {  get; set; }

        [JsonProperty("result")]
        internal JObject Result {  get; set; }
    }
}
