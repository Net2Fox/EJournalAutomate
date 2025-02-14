using EJournalWPF.Model;
using EJournalWPF.Model.API;
using EJournalWPF.Model.API.AuthModel;
using EJournalWPF.Model.API.MessageModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EJournalWPF.Data
{
    internal class DataRepository
    {
        private static DataRepository _instance;
        private static readonly object _lock = new object();

        private string _authToken = string.Empty;

        private List<Message> _messages;

        private List<Model.API.MessageModel.Group> _groups;

        private string _saveDirectory = $"{Environment.CurrentDirectory}\\Письма";

        private bool _saveDateTime = false;

        public string SaveDirectory { get { return _saveDirectory; } }

        public bool SaveDateTime { get { return _saveDateTime; } }

        public List<Message> Messages { get { return _messages; } }

        public List<Model.API.MessageModel.Group> Groups { get { return _groups; } }


        internal delegate void AuthHandler(bool isSuccess, string error);
        internal event AuthHandler AuthEvent;

        internal delegate void GetMessagesHandler(bool isSuccess, List<Message> messages, string error);
        internal event GetMessagesHandler GetMessagesEvent;

        private DataRepository()
        {
            _authToken = TokenStorage.LoadToken();
            LoadSettings();
            Task.Run(async () =>
            {
                if (System.IO.File.Exists($"{Environment.CurrentDirectory}/cache.json"))
                {
                    await LoadDataFromCache();
                    return;
                }
                await LoadDataFromAPI();
            });
        }

        internal async Task LoadDataFromAPI()
        {
            await GetMessageReceivers();
            await SaveCacheData();
        }

        internal async Task<string> SendRequestAsync(string url, CookieContainer cookies)
        {
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = cookies, UseCookies = true })
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36");
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        private void ChangeStatusMessage(Message message)
        {
            message.Unread = false;
            message.Selected = false;
        }

        private async Task SaveCacheData()
        {
            CacheData cacheData = new CacheData(_groups);
            System.IO.File.WriteAllText($"{Environment.CurrentDirectory}/cache.json", JsonConvert.SerializeObject(cacheData));
        }

        private async Task LoadDataFromCache()
        {
            try
            {
                CacheData cache = JsonConvert.DeserializeObject<CacheData>(System.IO.File.ReadAllText($"{Environment.CurrentDirectory}/cache.json"));
                _groups = cache.Groups;
                if (cache.Groups == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                await LoadDataFromAPI();
            }
        }

        internal void SetSaveDirectory(string directory)
        {
            _saveDirectory = directory;
        }

        internal void SaveSettings()
        {
            System.IO.File.WriteAllText($"{Environment.CurrentDirectory}\\settings", $"{_saveDirectory}\n{_saveDateTime}");
        }

        private void LoadSettings()
        {
            if (System.IO.File.Exists($"{Environment.CurrentDirectory}\\settings"))
            {
                string[] settings = System.IO.File.ReadAllText($"{Environment.CurrentDirectory}\\settings").Split('\n');
                if (settings.Length > 0 && settings.Length < 2)
                {
                    _saveDirectory = settings[0];
                }
                else
                {
                    _saveDirectory = settings[0];
                    _saveDateTime = Convert.ToBoolean(settings[1]);
                }
                
            }
        }

        public void SetDateTimeSave()
        {
            _saveDateTime = !_saveDateTime;
        }

        internal async Task Auth(string login, string password)
        {
            Result<AuthResponse> result = await APIv3.Auth(login, password);
            if (result.Success)
            {
                AuthResponse authResult = result.Data;
                _authToken = authResult.Token;
                TokenStorage.SaveToken(_authToken);
                AuthEvent?.Invoke(true, null);
            }
            else
            {
                AuthEvent?.Invoke(false, result.Error);
            }
        }

        internal async Task GetMessages(FolderType folderType = FolderType.inbox, UnreadOnly unreadOnly = UnreadOnly.no, int limit = 20, int page = 1)
        {
            Result<MessagesResponse> result = await APIv3.GetMessages(folderType, unreadOnly, limit, page, _authToken);
            if (result.Success)
            {
                _messages = result.Data.Messages;
                GetMessagesEvent?.Invoke(true, result.Data.Messages, null);
            }
            else
            {
                GetMessagesEvent?.Invoke(false, null, result.Error);
            }
        }

        internal async Task<MessageInfo> GetMessageInfo(string id)
        {
            Result<MessageInfoResponse> result = await APIv3.GetMessageInfo(id, _authToken);
            if (result.Success)
            {
                result.Data.Message.User_From.GroupName = _groups.Find(u => u.Name == result.Data.Message.User_From.Name).Name;
                return result.Data.Message;
            }
            else
            {
                return null;
            }
        }

        internal async Task GetMessageReceivers()
        {
            Result<MessageReceiversResponse> result = await APIv3.GetMessageReceivers(_authToken);
            if (result.Success)
            {
                _groups = result.Data.Groups;
            }
            else
            {
                // DataLoadingError???
            }
        }

        internal async void DownloadFile(List<Message> messagesToDownload)
        {
            // TODO BeginDataLoadingEvent?.Invoke($"Скачивание {messagesToDownload.Count} писем...");

            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }

            using (WebClient client = new WebClient())
            {
                foreach (Message message in messagesToDownload)
                {
                    string group = null;
                    string student = null;
                    string subDirectory = null;
                    string filename = "";

                    group = message.User_From.GroupName;

                    MessageInfo messageInfo = await GetMessageInfo(message.ID);

                    if (!Directory.Exists($"{_saveDirectory}/{group}"))
                    {
                        Directory.CreateDirectory($"{_saveDirectory}/{group}");
                    }

                    student = $"{messageInfo.User_From.LastName} {messageInfo.User_From.FirstName}";
                    if (!Directory.Exists($"{_saveDirectory}/{group}/{student}"))
                    {
                        Directory.CreateDirectory($"{_saveDirectory}/{group}/{student}");
                    }

                    if (messageInfo.Files.Count > 1)
                    {
                        subDirectory = Regex.Replace(messageInfo.Subject, @"[<>:""|?*]", string.Empty);
                        if (subDirectory.Length > 30)
                        {
                            subDirectory = subDirectory.Remove(30);
                        }
                        if (!Directory.Exists($"{_saveDirectory}/{group}/{student}/{subDirectory}"))
                        {
                            Directory.CreateDirectory($"{_saveDirectory}/{group}/{student}/{subDirectory}");
                        }
                    }

                    foreach (Model.API.MessageModel.File file in messageInfo.Files)
                    {
                        if (_saveDateTime)
                        {
                            filename = $"{messageInfo.Date.ToString("dd.MM HH-mm")}";
                        }
                        else
                        {
                            filename = "";
                        }
                        filename = $"{filename} {Regex.Replace(file.Filename, @"[<>:""|?*]", string.Empty)}";
                        if (subDirectory != null)
                        {
                            filename = $"{_saveDirectory}/{group}/{student}/{subDirectory}/{filename}";
                        }
                        else
                        {
                            filename = $"{_saveDirectory}/{group}/{student}/{filename}";
                        }

                        if (System.IO.File.Exists(filename))
                        {
                            break;
                        }

                        byte[] fileBytes = client.DownloadData(file.Link);
                        System.IO.File.WriteAllBytes(filename, fileBytes);
                    }
                    ChangeStatusMessage(message);
                    Thread.Sleep(100);
                }
            }
            // TODO DownloadingFinishEvent?.Invoke();
        }

        internal bool IsAuthorized()
        {
            if (_authToken != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Initialize()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DataRepository();
                    }
                }
            }
        }

        public static DataRepository GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("DataRepository не был инициализирован. Вызовите Initialize перед первым использованием.");
            }
            return _instance;
        }
    }
}
