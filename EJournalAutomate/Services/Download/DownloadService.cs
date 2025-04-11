using EJournalAutomate.Models.Domain;
using EJournalAutomate.Repositories;
using EJournalAutomate.Services.API;
using EJournalAutomate.Services.Storage.Settings;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace EJournalAutomate.Services.Download
{
    public class DownloadService : IDownloadService
    {
        private readonly ISettingsStorage _settingsStorage;
        private readonly IAPIService _apiService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DownloadService> _logger;

        public DownloadService(ISettingsStorage settingsStorage, IAPIService apiService, IUserRepository userRepository, ILogger<DownloadService> logger)
        {
            _settingsStorage = settingsStorage;
            _apiService = apiService;
            _userRepository = userRepository;

            _logger = logger;
            _logger.LogInformation("DownloadService инициализирован");
        }

        public async Task DownloadMessagesAsync(List<Message> messages, IProgress<(int current, int total)>? progress = null)
        {
            _logger.LogInformation($"Скачивание сообщений: {messages.Count()}");

            if (messages == null || messages.Count == 0) return;

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

            string? directory = null;

            var user = _userRepository.Users.FirstOrDefault(u => u.ID.Equals(message.UserFrom.ID));

            if (user == null) return;

            MessageInfo messageInfo = await _apiService.GetMessageInfoAsync(message.ID);

            string studentFullName = messageInfo.User_From.FullName;

            string? studentGroup = user.GroupName;

            if (studentGroup == null)
            {
                directory = Path.Combine(_settingsStorage.SavePath, studentFullName);
            }
            else
            {
                directory = Path.Combine(_settingsStorage.SavePath, studentGroup, studentFullName);
            }

            if (messageInfo.Files.Count > 1 || !IsOnlySignature(messageInfo.Text))
            {
                string subDirectory = Regex.Replace(messageInfo.Subject, @"[<>:""|?*]", string.Empty);

                directory = Path.Combine(directory, subDirectory);

                EnsureDirectoryExists(directory);

                await System.IO.File.WriteAllTextAsync(Path.Combine(directory, "Сообщение.txt"), CleanAllHtmlTags(messageInfo.Text));
            }
            else
            {
                EnsureDirectoryExists(directory);
            }

            foreach (Models.Domain.File file in messageInfo.Files)
            {
                string filename = Regex.Replace(file.Filename, @"[<>:""|?*]", string.Empty);
                if (_settingsStorage.SaveDate)
                {
                    filename = $"{messageInfo.Date.ToString("dd.MM HH-mm")} {filename}";
                }

                string fullPath = Path.Combine(directory, filename);

                if (System.IO.File.Exists(fullPath)) continue;

                byte[] fileBytes = await httpClient.GetByteArrayAsync(file.Link);
                await System.IO.File.WriteAllBytesAsync(fullPath, fileBytes);
                _logger.LogInformation($"Сообщение успешно скачано: {message.ID}");
            }
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

        private string CleanAllHtmlTags(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return htmlText;
            }

            return Regex.Replace(htmlText, @"<.*?>", string.Empty);
        }

        private bool IsOnlySignature(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            string cleanText = CleanAllHtmlTags(text);

            cleanText = Regex.Replace(cleanText, @"\s+", " ").Trim();

            return Regex.IsMatch(cleanText, @"^(-+\s*)?С уважением,\s+[\p{L}\s\-\.]+$", RegexOptions.IgnoreCase);
        }
    }
}
