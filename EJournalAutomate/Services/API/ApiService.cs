using EJournalAutomate.Services.Storage.Token;
using EJournalAutomate.Exceptions;
using EJournalAutomate.Models.API.Requests;
using EJournalAutomate.Models.API.Responses;
using EJournalAutomate.Models.Domain;
using System.Net.Http;
using System.Text;
using System.Text.Json;

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

        public ApiService(ITokenStorage tokenStorage)
        {
            _httpClient = new HttpClient();
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
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

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ApiException("Логин и пароль указан неверно.");
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
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<MessageInfoResult>>(json);

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
                throw new Exception($"Ошибка API: {apiResponse?.Response?.Error}");
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
                throw new ApiException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();
            List<User> students = ExtractStudentsDirectly(json);

            if (students == null)
            {
                throw new ApiException("Ответ API пустой или не может быть десериализован.");
            }

            return students;
        }

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
                            string groupName = string.Empty;
                            if (subgroup.TryGetProperty("name", out JsonElement nameElement))
                            {
                                groupName = nameElement.GetString();
                            }

                            if (subgroup.TryGetProperty("users", out JsonElement users))
                            {
                                foreach (JsonElement userElement in users.EnumerateArray())
                                {
                                    var user = new User
                                    {
                                        GroupName = groupName
                                    };

                                    if (userElement.TryGetProperty("lastname", out JsonElement lastnameEl))
                                        user.LastName = lastnameEl.GetString();

                                    if (userElement.TryGetProperty("firstname", out JsonElement firstnameEl))
                                        user.FirstName = firstnameEl.GetString();

                                    if (userElement.TryGetProperty("middlename", out JsonElement middlenameEl))
                                        user.MiddleName = middlenameEl.GetString();

                                    students.Add(user);
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
