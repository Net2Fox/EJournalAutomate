using EJournalAutomate.Models.Domain;
using EJournalAutomate.Repositories;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Storage.Settings;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace EJournalAutomate.Services.Download
{
    public class DownloadService : IDownloadService
    {
        private readonly ISettingsStorage _settingsStorage;
        private readonly IApiService _apiService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DownloadService> _logger;

        public DownloadService(ISettingsStorage settingsStorage, IApiService apiService, IUserRepository userRepository, ILogger<DownloadService> logger)
        {
            _settingsStorage = settingsStorage;
            _apiService = apiService;
            _userRepository = userRepository;

            _logger = logger;
            _logger.LogInformation("DownloadService инициализирован");
        }
        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                _logger.LogDebug($"Создание директории: {directory}");
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    var exception = new Exception($"Ошибка при создании директории: {directory}", ex);
                    _logger.LogError(exception, $"Ошибка при создании директории: {directory}");
                    throw exception;
                }
            }
        }

        public async Task DownloadMessagesAsync(List<Message> messages, IProgress<(int current, int total)>? progress = null)
        {
            _logger.LogInformation($"Скачивание сообщений: {messages.Count()}");

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
                    var exception = new Exception($"Ошибка при скачивании файлов сообщения {message.ID}: {ex.Message}");
                    _logger.LogError(exception, "Ошибка при скачивании сообщений");
                    throw exception;
                }
                await Task.Delay(100);
            }
        }

        private async Task DownloadMessageFilesAsync(Message message, HttpClient httpClient)
        {
            _logger.LogInformation($"Попытка скачать сообщение: {message.ID}");

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

            group = user.GroupName;

            EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group, student));

            if (messageInfo.Files.Count > 1 || !IsOnlySignature(messageInfo.Text))
            {
                subDirectory = Regex.Replace(messageInfo.Subject, @"[<>:""|?*]", string.Empty);
                if (subDirectory.Length > 30)
                {
                    subDirectory = subDirectory.Remove(30);
                }
                EnsureDirectoryExists(Path.Combine(_settingsStorage.SavePath, group, student, subDirectory));

                await SaveMessageText(Path.Combine(_settingsStorage.SavePath, group, student, subDirectory), messageInfo.Text);
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
                _logger.LogInformation($"Сообщение успешно скачано: {message.ID}");
            }
        }

        private async Task SaveMessageText(string directory, string text)
        {
            await System.IO.File.WriteAllTextAsync(Path.Combine(directory, "Сообщение.txt"), CleanAllHtmlTags(text));
        }

        private string CleanAllHtmlTags(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
                return htmlText;

            return Regex.Replace(htmlText, @"<.*?>", string.Empty);
        }

        private bool IsOnlySignature(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string cleanText = CleanAllHtmlTags(text);

            cleanText = Regex.Replace(cleanText, @"\s+", " ").Trim();

            return Regex.IsMatch(cleanText, @"^(-+\s*)?С уважением,\s+[\p{L}\s\-\.]+$", RegexOptions.IgnoreCase);
        }
    }
}
