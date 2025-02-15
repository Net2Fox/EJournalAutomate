using EJournalAutomate.Data;
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

        private AuthRepository _auth;

        private SettingsRepository _settings;

        private List<Message> _messages;

        private List<User> _users;

        internal List<Message> Messages { get { return _messages; } }

        internal delegate void GetMessagesHandler(bool isSuccess, List<Message> messages, string error);
        internal event GetMessagesHandler GetMessagesEvent;

        internal delegate void MessagesLoadingHandler(bool isSuccess, string error);
        internal event MessagesLoadingHandler MessagesLoadingEvent;

        internal delegate void MessageReceiversLoadingHandler(bool isSuccess, string error);
        internal event MessageReceiversLoadingHandler MessageReceiversLoadingEvent;


        private DataRepository(AuthRepository authRepository, SettingsRepository settingsRepository)
        {
            _auth = authRepository;
            _settings = settingsRepository;
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
            SaveCacheData();
        }

        internal async Task GetMessages(FolderType folderType = FolderType.inbox, UnreadOnly unreadOnly = UnreadOnly.no, int limit = 20, int page = 1)
        {
            Result<MessagesResponse> result = await APIv3.GetMessages(folderType, unreadOnly, limit, page, _auth.GetToken);
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
            Result<MessageInfoResponse> result = await APIv3.GetMessageInfo(id, _auth.GetToken);
            if (result.Success)
            {
                result.Data.Message.User_From.GroupName = _users.Find(u => u.Name == result.Data.Message.User_From.Name).Name;
                return result.Data.Message;
            }
            else
            {
                return null;
            }
        }

        internal async Task GetMessageReceivers()
        {
            Result<MessageReceiversResponse> result = await APIv3.GetMessageReceivers(_auth.GetToken);
            if (result.Success)
            {
                var usersWithGroupNames = result.Data.Groups
                    .Where(g => g.Key == "students")
                    .ToList()[0].SubGroups
                    .SelectMany(g => g.Users.Select(u => new User
                    {
                        Name = u.Name,
                        LastName = u.LastName,
                        FirstName = u.FirstName,
                        MiddleName = u.MiddleName,
                        GroupName = g.Name
                    }))
                    .ToList();
                _users = usersWithGroupNames;
            }
            else
            {
                // DataLoadingError???
            }
        }

        internal async void DownloadFile(List<Message> messagesToDownload)
        {
            // TODO BeginDataLoadingEvent?.Invoke($"Скачивание {messagesToDownload.Count} писем...");

            if (!Directory.Exists(_settings.SaveDirectory))
            {
                Directory.CreateDirectory(_settings.SaveDirectory);
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

                    if (!Directory.Exists($"{_settings.SaveDirectory}/{group}"))
                    {
                        Directory.CreateDirectory($"{_settings.SaveDirectory}/{group}");
                    }

                    student = $"{messageInfo.User_From.LastName} {messageInfo.User_From.FirstName}";
                    if (!Directory.Exists($"{_settings.SaveDirectory}/{group}/{student}"))
                    {
                        Directory.CreateDirectory($"{_settings.SaveDirectory}/{group}/{student}");
                    }

                    if (messageInfo.Files.Count > 1)
                    {
                        subDirectory = Regex.Replace(messageInfo.Subject, @"[<>:""|?*]", string.Empty);
                        if (subDirectory.Length > 30)
                        {
                            subDirectory = subDirectory.Remove(30);
                        }
                        if (!Directory.Exists($"{_settings.SaveDirectory}/{group}/{student}/{subDirectory}"))
                        {
                            Directory.CreateDirectory($"{_settings.SaveDirectory}/{group}/{student}/{subDirectory}");
                        }
                    }

                    foreach (Model.API.MessageModel.File file in messageInfo.Files)
                    {
                        if (_settings.SaveDateTime)
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
                            filename = $"{_settings.SaveDirectory}/{group}/{student}/{subDirectory}/{filename}";
                        }
                        else
                        {
                            filename = $"{_settings.SaveDirectory}/{group}/{student}/{filename}";
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

        private void ChangeStatusMessage(Message message)
        {
            message.Unread = false;
            message.Selected = false;
        }

        private void SaveCacheData()
        {
            string json = JsonConvert.SerializeObject(_users);
            System.IO.File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "cache.json"), json);
        }

        private async Task LoadDataFromCache()
        {
            try
            {
                _users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText($"{Environment.CurrentDirectory}/cache.json"));
                if (_users == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                await LoadDataFromAPI();
            }
        }

        internal static void Initialize(AuthRepository authRepository, SettingsRepository settingsRepository)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DataRepository(authRepository, settingsRepository);
                    }
                }
            }
        }

        internal static DataRepository GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("DataRepository не был инициализирован. Вызовите Initialize перед первым использованием.");
            }
            return _instance;
        }
    }
}
