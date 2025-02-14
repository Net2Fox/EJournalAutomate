using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model.API.MessageModel
{
    internal class User
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("middlename")]
        public string MiddleName { get; set; }

        public string GroupName { get; set; }

        [JsonIgnore]
        public string FullName
        {
            get
            {
                return $"{LastName} {FirstName[0]}. {MiddleName[0]}.";
            }
        }
    }
}
