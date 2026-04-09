using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EJournalAutomate.Services.Storage.Token
{
    public class SecureStorage : ISecureStorage
    {
        private readonly string _filePath;
        private readonly ILogger<SecureStorage> _logger;

        public SecureStorage(string filePath, ILogger<SecureStorage> logger)
        {
            _filePath = filePath;
            _logger = logger;

            _logger.LogInformation("SecureStorage инициализирован");
        }

        public async Task SaveAsync(string value)
        {
            _logger.LogInformation("Попытка сохранить зашифрованные данные");
            try
            {
                byte[] valueBytes = Encoding.UTF8.GetBytes(value);

                byte[] encryptedBytes = ProtectedData.Protect(valueBytes, null, DataProtectionScope.CurrentUser);

                await File.WriteAllBytesAsync(_filePath, encryptedBytes);

                Array.Clear(valueBytes, 0, valueBytes.Length);
            }
            catch (Exception ex)
            {
                var exception = new IOException("Не удалось сохранить зашифрованные данные", ex);
                _logger.LogCritical(exception, "Не удалось сохранить зашифрованные данные");
                throw exception;
            }

        }

        public async Task<string> LoadAsync()
        {
            _logger.LogInformation("Попытка прочитать файл с зашифрованными данными");

            if (!File.Exists(_filePath))
            {
                _logger.LogInformation("Файл с зашифрованными данными отсутствует");
                return string.Empty;
            }

            try
            {
                byte[] encryptedBytes = await File.ReadAllBytesAsync(_filePath);

                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

                string value = Encoding.UTF8.GetString(decryptedBytes);

                Array.Clear(decryptedBytes, 0, decryptedBytes.Length);

                return value;
            }
            catch (Exception ex)
            {
                var exception = new IOException("Не удалось прочитать файл с зашифрованными данными", ex);
                _logger.LogCritical(exception, "Не удалось прочитать файл с зашифрованными данными");
                throw exception;
            }

        }

        public bool Exists() => File.Exists(_filePath);
    }
}
