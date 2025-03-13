using EJournalAutomateMVVM.Exceptions;
using EJournalAutomateMVVM.Models.API.Requests;
using EJournalAutomateMVVM.Models.API.Responses;
using EJournalAutomateMVVM.Models.Domain;
using EJournalAutomateMVVM.Services.Storage;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace EJournalAutomateMVVM.Services.API
{
    public class ApiService : IApiService
    {
        private const string BaseUrl = "https://kip.eljur.ru/apiv3";
        private const string DevKey = "YourDevKey";
        private const string Vendor = "kip";

        private string _authToken = "";

        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;

        public ApiService(ITokenStorage tokenStorage)
        {
            _httpClient = new HttpClient();
            _tokenStorage = tokenStorage;
        }

        public async Task<bool> LoadTokenFromAsync()
        {
            _authToken = await _tokenStorage.LoadTokenAsync();
            return !string.IsNullOrEmpty(_authToken);
        }

        public async Task AuthenticateAsync(string login, string password)
        {
            var url = $"{BaseUrl}/auth?devkey={DevKey}&out_format=json&vendor={Vendor}";
            var loginRequest = new LoginRequest { Login = login, Password = password };
            var jsonContent = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                throw new ApiException("Ошибка при выполнении запроса на аутентификацию.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResult>>(json);

            if (apiResponse == null)
            {
                throw new ApiException("Ответ API пустой или не может быть десериализован.");
            }

            if (apiResponse.Response.State == 200 && apiResponse.Response.Result != null)
            {
                _authToken = apiResponse.Response.Result.Token;
                await _tokenStorage.SaveTokenAsync(_authToken);
            }
            else
            {
                throw new ApiException($"Ошибка API: {apiResponse.Response.Error}");
            }
        }

        public async Task<List<Message>> GetMessagesAsync(int limit = 20)
        {
            var url = $"{BaseUrl}/getmessages?folder=inbox&unreadonly=no&limit={limit}&page=1&devkey={DevKey}&auth_token={_authToken}&out_format=json&vendor={Vendor}";
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                throw new ApiException("Ошибка при выполнении запроса на получение сообщений.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<MessageListResult>>(json);

            if (apiResponse == null)
            {
                throw new ApiException("Ответ API пустой или не может быть десериализован.");
            }

            if (apiResponse.Response.State == 200 && apiResponse.Response.Result != null)
            {
                return apiResponse.Response.Result.Messages;
            }
            else
            {
                throw new Exception($"Ошибка API: {apiResponse?.Response?.Error}");
            }
        }

        public async Task<MessageInfo> GetMessageInfoAsync(string id)
        {
            var url = $"{BaseUrl}/getmessageinfo?id={id}&devkey={DevKey}&out_format=json&auth_token={_authToken}&vendor={Vendor}";

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                throw new ApiException("Ошибка при выполненни запроса на получение информации о сообщении.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<MessageInfo>>(json);

            if (apiResponse == null)
            {
                throw new ApiException("Ответ API пустой или не может быть десериализован.");
            }

            if (apiResponse.Response.State == 200 && apiResponse.Response.Result != null)
            {
                return apiResponse.Response.Result;
            }
            else
            {
                throw new Exception($"Ошибка API: {apiResponse?.Response?.Error}");
            }
        }
    }
}
