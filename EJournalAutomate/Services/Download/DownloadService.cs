using CommunityToolkit.Mvvm.DependencyInjection;
using EJournalAutomate.Services.Storage.Repository;
using EJournalAutomate.Services.Storage.Settings;
using EJournalAutomateMVVM.Models.Domain;
using EJournalAutomateMVVM.Services.API;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace EJournalAutomate.Services.Download
{
    public class DownloadService : IDownloadService
    {
        private readonly ISettingsStorage _settingsStorage = Ioc.Default.GetRequiredService<ISettingsStorage>();
        private readonly IApiService _apiService = Ioc.Default.GetRequiredService<IApiService>();
        private readonly IUserRepository _userRepository = Ioc.Default.GetRequiredService<IUserRepository>();

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
                    // TODO Logger.LogError($"Ошибка при скачивании файлов сообщения {message.ID}: {ex.Message}");
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

            messageInfo.User_From.GroupName = _userRepository.Users.ToList()
                .Find(u => u.LastName == messageInfo.User_From.LastName).GroupName;

            group = messageInfo.User_From.GroupName;

            EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group));

            EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group, student));

            if (messageInfo.Files.Count > 1)
            {
                // TODO Использовать Path.GetInvalidPathChars?
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
                    filename = $"{messageInfo.Date}";// TODO .ToString("dd.MM HH-mm")}";
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
