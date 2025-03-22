using System.Text.Json.Serialization;

namespace EJournalAutomateMVVM.Models.API.Responses
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
