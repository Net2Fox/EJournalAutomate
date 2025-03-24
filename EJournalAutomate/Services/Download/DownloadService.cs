using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomate.Models.Domain;
using EJournalAutomate.Services.API;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using EJournalAutomate.Repositories;

namespace EJournalAutomate.Services.Download
{
    public class DownloadService : IDownloadService
    {
        private readonly ISettingsStorage _settingsStorage;
        private readonly IApiService _apiService;
        private readonly IUserRepository _userRepository;

        public DownloadService(ISettingsStorage settingsStorage, IApiService apiService, IUserRepository userRepository)
        {
            _settingsStorage = settingsStorage;
            _apiService = apiService;
            _userRepository = userRepository;
        }
        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public async Task DownloadMessagesAsync(List<Message> messages, IProgress<(int current, int total)>? progress = null)
        {
            if (messages == null || messages.Count == 0) return;

            EnsureDirectoryExists(_settingsStorage.SavePath);

            using var httpClient = new HttpClient();

            for (int i = 0; i < messages.Count; i++)
            {

                var message = messages[i];
                progress?.Report((i + 1, messages.Count));

                try
                {
                    await DownloadMessageFilesAsync(message, httpClient);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при скачивании файлов сообщения {message.ID}: {ex.Message}");
                }
                await Task.Delay(100);
            }
        }

        private async Task DownloadMessageFilesAsync(Message message, HttpClient httpClient)
        {
            string? group = null;
            string? student = null;
            string? subDirectory = null;
            string? filename = null;

            var user = _userRepository.Users.FirstOrDefault(u => u.ID.Equals(message.UserFrom.ID));

            if (user == null)
            {
                return;
            }

            MessageInfo messageInfo = await _apiService.GetMessageInfoAsync(message.ID);

            student = messageInfo.User_From.FullName;

            messageInfo.User_From.GroupName = user.GroupName;

            group = messageInfo.User_From.GroupName;

            EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group));

            EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group, student));

            if (messageInfo.Files.Count > 1)
            {
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
                    filename = $"{messageInfo.Date.ToString("dd.MM HH-mm")}";
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
