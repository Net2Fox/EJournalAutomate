﻿using EJournalAutomate.Exceptions;
using EJournalAutomate.Helpers;
using EJournalAutomate.Models.API.Requests;
using EJournalAutomate.Models.API.Responses;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Services.Storage.Token;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace EJournalAutomate.Services.API
{
    public class APIService : IAPIService
    {
        private const string BaseUrl = "https://api.eljur.ru/api";
        private const string DevKey = "YourDevKey";

        private string _vendor = string.Empty;
        private string _authToken = string.Empty;

        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly ISettingsStorage _settingsStorage;
        private readonly ILogger<APIService> _logger;

        private readonly JsonSerializerOptions _jsonOptions;

        public APIService(ITokenStorage tokenStorage, ISettingsStorage settingsStorage, ILogger<APIService> logger)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _tokenStorage = tokenStorage;
            _settingsStorage = settingsStorage;
            _logger = logger;

            _jsonOptions = new();
            _jsonOptions.Converters.Add(new DateTimeConverter());

            _vendor = _settingsStorage.Vendor;

            _logger.LogInformation("ApiService инициализирован");
        }

        public async Task<bool> LoadTokenFromAsync()
        {
            _authToken = await _tokenStorage.LoadTokenAsync();
            return !string.IsNullOrEmpty(_authToken);
        }

        public async Task AuthenticateAsync(string login, string password, string? vendor = null)
        {
            _logger.LogInformation($"Попытка входа пользователя: {login}");

            if (!string.IsNullOrWhiteSpace(vendor))
            {
                _vendor = vendor;
                _settingsStorage.SetVendor(_vendor);
                await _settingsStorage.SaveSettings();
            }
            else
            {
                var apiException = new APIException("Отсутствует поддомен учебного заведения.");
                _logger.LogError(exception: apiException, "Ошибка при входе");
                throw apiException;
            }

            if (string.IsNullOrWhiteSpace(_vendor))
            {
                var apiException = new APIException("Отсутствует поддомен учебного заведения.");
                _logger.LogError(exception: apiException, "Ошибка при входе");
                throw apiException;
            }

            var url = $"{BaseUrl}/auth?devkey={DevKey}&out_format=json&vendor={_vendor}";
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
                var apiException = new APIException("Ошибка при выполнении запроса на аутентификацию.", ex);
                _logger.LogError(exception: apiException, "Ошибка при входе");
                throw apiException;
            }

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorJson, _jsonOptions);

                    var apiException = new APIException($"Ошибка HTTP: {errorApiResponse.Response.State}, {errorApiResponse.Response.Error}", errorApiResponse.Response.State);
                    _logger.LogError(exception: apiException, "Ошибка при входе");
                    throw apiException;
                }
                catch (JsonException)
                {
                    var apiException = new APIException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
                    _logger.LogError(exception: apiException, "Ошибка при входе");
                    throw apiException;
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResult>>(json, _jsonOptions);

            if (apiResponse == null)
            {
                var apiException = new APIException("Ответ API пустой или не может быть десериализован.");
                _logger.LogError(exception: apiException, "Ошибка при входе");
                throw apiException;
            }

            if (apiResponse.Response.State == 200 && apiResponse.Response.Result != null)
            {
                _logger.LogInformation("Вход выполнен успешно");

                _authToken = apiResponse.Response.Result.Token;
                await _tokenStorage.SaveTokenAsync(_authToken);
            }
            else
            {
                var apiException = new APIException($"Ошибка API: {apiResponse.Response.Error}");
                _logger.LogError(exception: apiException, "Ошибка при входе");
                throw apiException;
            }
        }

        public async Task<List<Message>> GetMessagesAsync(int limit = 20)
        {
            _logger.LogInformation($"Попытка получить список сообщений, лимит: {limit}");

            var url = $"{BaseUrl}/getmessages?folder=inbox&unreadonly=no&limit={limit}&page=1&devkey={DevKey}&auth_token={_authToken}&out_format=json&vendor={_vendor}";
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                var apiException = new APIException("Ошибка при выполнении запроса на получение сообщений.", ex);
                _logger.LogError(exception: apiException, "Ошибка при получении списка сообщений");
                throw apiException;
            }

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorJson, _jsonOptions);

                    var apiException = new APIException($"Ошибка HTTP: {errorApiResponse.Response.State}, {errorApiResponse.Response.Error}", errorApiResponse.Response.State);
                    _logger.LogError(exception: apiException, "Ошибка при получении списка сообщений");
                    throw apiException;
                }
                catch (JsonException)
                {
                    var apiException = new APIException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
                    _logger.LogError(exception: apiException, "Ошибка при получении списка сообщений");
                    throw apiException;
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<MessageListResult>>(json, _jsonOptions);

            if (apiResponse == null)
            {
                var apiException = new APIException("Ответ API пустой или не может быть десериализован.");
                _logger.LogError(exception: apiException, "Ошибка при получении списка сообщений");
                throw apiException;
            }

            if (apiResponse.Response.State == 200 && apiResponse.Response.Result != null)
            {
                _logger.LogInformation("Сообщения успешно получены");

                return apiResponse.Response.Result.Messages;
            }
            else
            {
                var apiException = new APIException($"Ошибка API: {apiResponse?.Response?.Error}");
                _logger.LogError(exception: apiException, "Ошибка при получении списка сообщений");
                throw apiException;
            }
        }

        public async Task<MessageInfo> GetMessageInfoAsync(string id)
        {
            _logger.LogInformation($"Попытка получить информацию о сообщении: {id}");

            var url = $"{BaseUrl}/getmessageinfo?id={id}&devkey={DevKey}&out_format=json&auth_token={_authToken}&vendor={_vendor}";

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                var apiException = new APIException("Ошибка при выполненни запроса на получение информации о сообщении.", ex);
                _logger.LogError(exception: apiException, "Ошибка при получении информации о сообщении");
                throw apiException;
            }

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorJson, _jsonOptions);

                    var apiException = new APIException($"Ошибка HTTP: {errorApiResponse.Response.State}, {errorApiResponse.Response.Error}", errorApiResponse.Response.State);
                    _logger.LogError(exception: apiException, "Ошибка при получении информации о сообщении");
                    throw apiException;
                }
                catch (JsonException)
                {
                    var apiException = new APIException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
                    _logger.LogError(exception: apiException, "Ошибка при получении информации о сообщении");
                    throw apiException;
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<MessageInfoResult>>(json, _jsonOptions);

            if (apiResponse == null)
            {
                var apiException = new APIException("Ответ API пустой или не может быть десериализован.");
                _logger.LogError(exception: apiException, "Ошибка при получении информации о сообщении");
                throw apiException;
            }

            if (apiResponse.Response.State == 200 && apiResponse.Response.Result != null)
            {
                _logger.LogInformation("Информация о сообщении успешно получена");

                return apiResponse.Response.Result.Message;
            }
            else
            {
                var apiException = new APIException($"Ошибка API: {apiResponse?.Response?.Error}");
                _logger.LogError(exception: apiException, "Ошибка при получении информации о сообщении");
                throw apiException;
            }
        }

        public async Task<(List<User>, List<StudentGroup>)> GetMessageReceivers()
        {
            _logger.LogInformation("Попытка получить список пользователей");

            var url = $"{BaseUrl}/getmessagereceivers?devkey={DevKey}&out_format=json&auth_token={_authToken}&vendor={_vendor}";

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                var apiException = new APIException("Ошибка при выполненни запроса на получение информации о сообщении.", ex);
                _logger.LogError(exception: apiException, "Ошибка при получении списка пользователей");
                throw apiException;
            }

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    var errorApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorJson, _jsonOptions);
                    var apiException = new APIException($"Ошибка HTTP: {errorApiResponse.Response.State}, {errorApiResponse.Response.Error}", errorApiResponse.Response.State);
                    _logger.LogError(exception: apiException, "Ошибка при получении списка пользователей");
                    throw apiException;
                }
                catch (JsonException)
                {
                    var apiException = new APIException($"Ошибка HTTP: {(int)response.StatusCode} {response.ReasonPhrase}", (int)response.StatusCode);
                    _logger.LogError(exception: apiException, "Ошибка при получении списка пользователей");
                    throw apiException;
                }
            }

            var json = await response.Content.ReadAsStringAsync();
            (List<User> students, List<StudentGroup> studentGroups) = ExtractStudentsDirectly(json);

            if (students == null)
            {
                var apiException = new APIException("Ответ API пустой или не может быть десериализован.");
                _logger.LogError(exception: apiException, "Ошибка при получении списка пользователей");
                throw apiException;
            }
            _logger.LogInformation("Список пользователей успешно получен");
            return (students, studentGroups);
        }

        /*
         * Ручная обработка Json для метода GetMessageReceivers
         * Нужно, так как автоматическая десериализация очень медленная для обработки этого Json
         */
        private (List<User>, List<StudentGroup>) ExtractStudentsDirectly(string jsonContent)
        {
            var students = new List<User>();
            var studentGroups = new List<StudentGroup>();

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

                                            LastName = lastnameEl.GetString().Trim(),
                                            FirstName = firstnameEl.GetString().Trim(),
                                            MiddleName = userElement.TryGetProperty("middlename", out JsonElement middlenameEl) ? middlenameEl.GetString().Trim() : null,

                                            GroupName = groupName
                                        };
                                        if (studentGroups.FirstOrDefault(g => g.Name == groupName) == null)
                                        {
                                            studentGroups.Add(new StudentGroup { Name = groupName });
                                        }
                                        students.Add(user);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return (students, studentGroups);
        }
    }
}
