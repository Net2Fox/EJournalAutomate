using CefSharp.DevTools.Network;
using EJournalWPF.Model;
using EJournalWPF.Model.API;
using EJournalWPF.Model.API.AuthModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Response = EJournalWPF.Model.API.ResponseAPI;

namespace EJournalWPF.Data
{
    internal static class APIv3
    {
        private const string DevKey = "HERE_YOU_NEED_DEVKEY";

        private static HttpClient _client = new HttpClient();

        internal static async Task<(bool IsSuccess, AuthResult Data, string Error)> Auth(string username, string password) 
        {
            AuthModel authBody = new AuthModel(username, password);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://kip.eljur.ru/apiv3/auth?devkey={DevKey}&out_format=json&auth_token&vendor=kip");
            StringContent content = new StringContent(JsonConvert.SerializeObject(authBody), null, "application/json");
            request.Content = content;
            HttpResponseMessage response = await _client.SendAsync(request);
            Response jsonResponse = JsonConvert.DeserializeObject<ResponseAPI>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                return (true, jsonResponse.Response.Result.ToObject<AuthResult>(), null);

            }
            else
            {
                return (false, null, jsonResponse.Response.Error);
            }
            
        }
    }
}
