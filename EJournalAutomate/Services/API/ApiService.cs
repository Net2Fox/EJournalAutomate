using EJournalAutomate.Services.Storage.Token;
using EJournalAutomate.Exceptions;
using EJournalAutomate.Models.API.Requests;
using EJournalAutomate.Models.API.Responses;
using EJournalAutomate.Models.Domain;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using EJournalAutomate.Helpers;

namespace EJournalAutomate.Services.API
{
    public class ApiService : IApiService
    {
        private const string BaseUrl = "https://kip.eljur.ru/apiv3";
        private const string DevKey = "YourDevKey";
        private const string Vendor = "kip";

        private string _authToken = string.Empty;

        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;

        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(ITokenStorage tokenStorage)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _tokenStorage = tokenStorage;

            _jsonOptions = new();
            _jsonOptions.Converters.Add(new DateTimeConverter());
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

            //if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            //{
            //    throw new ApiException("Логин и пароль указан неверно.");
            //}

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorJson, _jsonOptions);
                    throw new ApiException($"Ошибка HTTP: {errorApiResponse.Response.State}, {errorApiResponse.Response.Error}", errorApiResponse.Response.State);
                }
                catch (JsonException)
                {
                    throw new ApiException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResult>>(json, _jsonOptions);

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
                try
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorJson, _jsonOptions);
                    throw new ApiException($"Ошибка HTTP: {errorApiResponse.Response.State}, {errorApiResponse.Response.Error}", errorApiResponse.Response.State);
                }
                catch (JsonException)
                {
                    throw new ApiException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<MessageListResult>>(json, _jsonOptions);

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
                throw new ApiException($"Ошибка API: {apiResponse?.Response?.Error}");
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
                try
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorJson, _jsonOptions);
                    throw new ApiException($"Ошибка HTTP: {errorApiResponse.Response.State}, {errorApiResponse.Response.Error}", errorApiResponse.Response.State);
                }
                catch (JsonException)
                {
                    throw new ApiException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<MessageInfoResult>>(json, _jsonOptions);

            if (apiResponse == null)
            {
                throw new ApiException("Ответ API пустой или не может быть десериализован.");
            }

            if (apiResponse.Response.State == 200 && apiResponse.Response.Result != null)
            {
                return apiResponse.Response.Result.Message;
            }
            else
            {
                throw new ApiException($"Ошибка API: {apiResponse?.Response?.Error}");
            }
        }

        public async Task<List<User>> GetMessageReceivers()
        {
            var url = $"{BaseUrl}/getmessagereceivers?devkey={DevKey}&out_format=json&auth_token={_authToken}&vendor={Vendor}";

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
                try
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorJson, _jsonOptions);
                    throw new ApiException($"Ошибка HTTP: {errorApiResponse.Response.State}, {errorApiResponse.Response.Error}", errorApiResponse.Response.State);
                }
                catch (JsonException)
                {
                    throw new ApiException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            List<User> students = ExtractStudentsDirectly(json);

            if (students == null)
            {
                throw new ApiException("Ответ API пустой или не может быть десериализован.");
            }

            return students;
        }

        /*
         * Ручная обработка Json для метода GetMessageReceivers
         * Нужно, так как автоматическая десериализация очень медленная для обработки этого Json
         */
        private List<User> ExtractStudentsDirectly(string jsonContent)
        {
            var students = new List<User>();

            using JsonDocument document = JsonDocument.Parse(jsonContent);

            JsonElement root = document.RootElement;

            if (root.TryGetProperty("response", out JsonElement response) &&
                response.TryGetProperty("result", out JsonElement result) &&
                result.TryGetProperty("groups", out JsonElement groups))
            {
                foreach (JsonElement group in groups.EnumerateArray())
                {
                    if (group.TryGetProperty("key", out JsonElement keyElement) &&
                        keyElement.GetString() == "students" &&
                        group.TryGetProperty("subgroups", out JsonElement subgroups))
                    {
                        foreach (JsonElement subgroup in subgroups.EnumerateArray())
                        {
                            string? groupName = null;
                            if (subgroup.TryGetProperty("name", out JsonElement nameElement))
                            {
                                groupName = nameElement.GetString();
                            }

                            if (subgroup.TryGetProperty("users", out JsonElement users))
                            {
                                foreach (JsonElement userElement in users.EnumerateArray())
                                {

                                    if (userElement.TryGetProperty("lastname", out JsonElement lastnameEl)
                                        && userElement.TryGetProperty("firstname", out JsonElement firstnameEl)
                                        && userElement.TryGetProperty("name", out JsonElement id))
                                    {
                                        var user = new User
                                        {
                                            ID = id.GetString(),

                                            LastName = lastnameEl.GetString(),
                                            FirstName = firstnameEl.GetString(),
                                            MiddleName = userElement.TryGetProperty("middlename", out JsonElement middlenameEl) ? middlenameEl.GetString() : null,

                                            GroupName = groupName
                                        };
                                        students.Add(user);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return students;
        }
    }
}
