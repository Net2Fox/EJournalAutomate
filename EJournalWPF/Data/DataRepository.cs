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
using System.Windows.Documents;

namespace EJournalWPF.Data
{
    internal class DataRepository
    {
        private AuthRepository _auth;

        private SettingsRepository _settings;

        private List<Message> _messages;

        private List<User> _users;

        internal List<Message> Messages { get { return _messages; } }

        internal delegate void GetMessagesHandler(bool isSuccess, List<Message> messages, string error);
        internal event GetMessagesHandler GetMessagesEvent;

        internal delegate void DownloadMessagesHandler(bool isSuccess, string error);
        internal event DownloadMessagesHandler DownloadMessagesEvent;

        internal DataRepository(AuthRepository authRepository, SettingsRepository settingsRepository)
        {
            _auth = authRepository;
            _settings = settingsRepository;
        }

        internal async Task<bool> Initialize()
        {
            try
            {
                if (System.IO.File.Exists($"{Environment.CurrentDirectory}/cache.json"))
                {
                    await LoadDataFromCache();
                }
                else
                {
                    await LoadDataFromAPI();
                }
                return true;
            }
            catch
            {
                return false;
            }
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
            if (!Directory.Exists(_settings.GetSaveDirectory))
            {
                Directory.CreateDirectory(_settings.GetSaveDirectory);
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

                    if (!Directory.Exists($"{_settings.GetSaveDirectory}/{group}"))
                    {
                        Directory.CreateDirectory($"{_settings.GetSaveDirectory}/{group}");
                    }

                    student = $"{messageInfo.User_From.LastName} {messageInfo.User_From.FirstName}";
                    if (!Directory.Exists($"{_settings.GetSaveDirectory}/{group}/{student}"))
                    {
                        Directory.CreateDirectory($"{_settings.GetSaveDirectory}/{group}/{student}");
                    }

                    if (messageInfo.Files.Count > 1)
                    {
                        subDirectory = Regex.Replace(messageInfo.Subject, @"[<>:""|?*]", string.Empty);
                        if (subDirectory.Length > 30)
                        {
                            subDirectory = subDirectory.Remove(30);
                        }
                        if (!Directory.Exists($"{_settings.GetSaveDirectory}/{group}/{student}/{subDirectory}"))
                        {
                            Directory.CreateDirectory($"{_settings.GetSaveDirectory}/{group}/{student}/{subDirectory}");
                        }
                    }

                    foreach (Model.API.MessageModel.File file in messageInfo.Files)
                    {
                        if (_settings.GetSaveDateTime)
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
                            filename = $"{_settings.GetSaveDirectory}/{group}/{student}/{subDirectory}/{filename}";
                        }
                        else
                        {
                            filename = $"{_settings.GetSaveDirectory}/{group}/{student}/{filename}";
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
            DownloadMessagesEvent?.Invoke(true, null);
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
            _users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText($"{Environment.CurrentDirectory}/cache.json"));
            if (_users == null)
            {
                throw new Exception();
            }
            await LoadDataFromAPI();
            return;
        }
    }
}
