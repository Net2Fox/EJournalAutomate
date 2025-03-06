﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("response")]
        public ResponseData<T> Response { get; set; }
    }

    public class ResponseData<T>
    {
        [JsonPropertyName("state")]
        public int State { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("result")]
        public T Result { get; set; }
    }
}
