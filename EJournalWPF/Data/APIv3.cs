using CefSharp.DevTools.Network;
using EJournalWPF.Model;
using EJournalWPF.Model.API;
using EJournalWPF.Model.API.AuthModel;
using EJournalWPF.Model.API.MessageModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EJournalWPF.Data
{
    internal static class APIv3
    {
        private const string DevKey = "HERE_YOU_NEED_DEVKEY";

        private static HttpClient _client = new HttpClient();

        internal static async Task<Result<AuthResponse>> Auth(string username, string password) 
        {
            AuthRequest authBody = new AuthRequest(username, password);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://kip.eljur.ru/apiv3/auth?devkey={DevKey}&out_format=json&auth_token&vendor=kip");
            StringContent content = new StringContent(JsonConvert.SerializeObject(authBody), null, "application/json");
            request.Content = content;
            HttpResponseMessage response = await _client.SendAsync(request);
            ResponseAPI jsonResponse = JsonConvert.DeserializeObject<ResponseAPI>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                return new Result<AuthResponse>(true, jsonResponse.Response.Result.ToObject<AuthResponse>(), null);

            }
            else
            {
                return new Result<AuthResponse>(false, null, jsonResponse.Response.Error);
            }
        }

        internal static async Task<Result<MessagesResponse>> GetMessages(FolderType folderType, UnreadOnly readType, int limit, int page, string authToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://kip.eljur.ru/apiv3/getmessages?folder={folderType}&unreadonly={readType}&limit={limit}&page={page}&filter=&devkey={DevKey}&out_format=json&auth_token={authToken}&vendor=kip");
            HttpResponseMessage response = await _client.SendAsync(request);
            ResponseAPI jsonResponse = JsonConvert.DeserializeObject<ResponseAPI>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                return new Result<MessagesResponse>(true, jsonResponse.Response.Result.ToObject<MessagesResponse>(), null);
            }
            else
            {
                return new Result<MessagesResponse>(false, null, jsonResponse.Response.Error);
            }
        }

        internal static async Task<Result<MessageInfoResponse>> GetMessageInfo(string id, string authToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://kip.eljur.ru/apiv3/getmessageinfo?id={id}&devkey={DevKey}&out_format=json&auth_token={authToken}&vendor=kip");
            HttpResponseMessage response = await _client.SendAsync(request);
            ResponseAPI jsonResponse = JsonConvert.DeserializeObject<ResponseAPI>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                return new Result<MessageInfoResponse>(true, jsonResponse.Response.Result.ToObject<MessageInfoResponse>(), null);
            }
            else
            {
                return new Result<MessageInfoResponse>(false, null, jsonResponse.Response.Error);
            }
        }

        internal static async Task<Result<MessageReceiversResponse>> GetMessageReceivers(string authToken)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://kip.eljur.ru/apiv3/getmessagereceivers?&devkey={DevKey}&out_format=json&auth_token={authToken}&vendor=kip");
            HttpResponseMessage response = await _client.SendAsync(request);
            ResponseAPI jsonResponse = JsonConvert.DeserializeObject<ResponseAPI>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                return new Result<MessageReceiversResponse>(true, jsonResponse.Response.Result.ToObject<MessageReceiversResponse>(), null);
            }
            else
            {
                return new Result<MessageReceiversResponse>(false, null, jsonResponse.Response.Error);
            }
        }
    }
}
