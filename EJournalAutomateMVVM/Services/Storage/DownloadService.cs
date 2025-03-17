using EJournalAutomateMVVM.Models.Domain;
using EJournalAutomateMVVM.Services.API;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace EJournalAutomateMVVM.Services.Storage
{
    public class DownloadService : IDownloadService
    {
        private readonly ISettingsStorage _settingsStorage;
        private readonly IApiService _apiService;
        private readonly ICacheService _cacheService;

        private List<User>? _users;

        public DownloadService(ISettingsStorage settingsStorage, IApiService apiService, ICacheService cacheService)
        {
            _settingsStorage = settingsStorage ?? throw new ArgumentException(nameof(settingsStorage));
            _apiService = apiService ?? throw new ArgumentException(nameof(apiService));
            _cacheService = cacheService ?? throw new ArgumentException(nameof(cacheService));
        }

        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public async Task DownloadMessagesAsync(List<Message> messages, List<User> users,
                IProgress<(int current, int total, string status)>? progress = null)
        {
            if (messages == null || messages.Count == 0) return;

            if(_users == null)
            {
                _users = users;
            }

            EnsureDirectoryExists(_settingsStorage.SavePath);

            using var httpClient = new HttpClient();

            for (int i = 0; i < messages.Count; i++)
            {

                var message = messages[i];
                progress?.Report((i + 1, messages.Count, $"Обработка сообщения: {message.Subject}"));

                try
                {
                    await DownloadMessageFilesAsync(message, httpClient);
                }
                catch (Exception ex)
                {
                    // Логирование ошибки, но продолжение обработки других сообщений
                    // Logger.LogError($"Ошибка при скачивании файлов сообщения {message.ID}: {ex.Message}");
                }
                await Task.Delay(100);
            }
        }

        private async Task DownloadMessageFilesAsync(Message message, HttpClient httpClient)
        {
            string group = null;
            string student = null;
            string subDirectory = null;
            string filename = "";



            MessageInfo messageInfo = await _apiService.GetMessageInfoAsync(message.ID);

            student = messageInfo.User_From.FullName;

            messageInfo.User_From.GroupName = _users.Find(u => u.LastName == messageInfo.User_From.LastName).GroupName;

            group = messageInfo.User_From.GroupName;

            EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group));

            EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group, student));

            if (messageInfo.Files.Count > 1)
            {
                //Path.GetInvalidPathChars
                subDirectory = Regex.Replace(messageInfo.Subject, @"[<>:""|?*]", string.Empty);
                if (subDirectory.Length > 30)
                {
                    subDirectory = subDirectory.Remove(30);
                }

                EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group, student, subDirectory));
            }

            foreach (Models.Domain.File file in messageInfo.Files)
            {
                if (_settingsStorage.SaveDate)
                {
                    filename = $"{messageInfo.Date}";//.ToString("dd.MM HH-mm")}";
                }
                else
                {
                    filename = "";
                }

                filename = $"{filename} {Regex.Replace(file.Filename, @"[<>:""|?*]", string.Empty)}";

                if (subDirectory != null)
                {
                    filename = Path.Combine(_settingsStorage.SavePath, group, student, subDirectory, filename);
                }
                else
                {
                    filename = Path.Combine(_settingsStorage.SavePath, group, student, filename);
                }

                if (System.IO.File.Exists(filename))
                {
                    continue;
                }

                byte[] fileBytes = await httpClient.GetByteArrayAsync(file.Link);
                await System.IO.File.WriteAllBytesAsync(filename, fileBytes);
            }
        }
    }
}
