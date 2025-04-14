using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EJournalAutomate.Services.Storage.Token
{
    public class TokenStorage : ITokenStorage
    {
        private readonly string _tokenFilePath = Path.Combine(Environment.CurrentDirectory, "token.dat");
        private readonly ILogger<TokenStorage> _logger;

        public TokenStorage(ILogger<TokenStorage> logger)
        {
            _logger = logger;

            _logger.LogInformation("TokenStorage инициализирован");
        }

        public async Task SaveTokenAsync(string token)
        {
            _logger.LogInformation("Попытка сохранить токен");
            try
            {
                byte[] tokenBytes = Encoding.UTF8.GetBytes(token);

                byte[] encryptedBytes = ProtectedData.Protect(tokenBytes, null, DataProtectionScope.CurrentUser);

                await File.WriteAllBytesAsync(_tokenFilePath, encryptedBytes);

                Array.Clear(tokenBytes, 0, tokenBytes.Length);
            }
            catch (Exception ex)
            {
                var exception = new IOException("Не удалось сохранить токен", ex);
                _logger.LogCritical(exception, "Не удалось сохранить токен");
                throw exception;
            }

        }

        public async Task<string> LoadTokenAsync()
        {
            _logger.LogInformation("Попытка прочитать файл с токеном");

            if (!File.Exists(_tokenFilePath))
            {
                _logger.LogInformation("Файл с токеном отсутствует");
                return string.Empty;
            }

            try
            {
                byte[] encryptedBytes = await File.ReadAllBytesAsync(_tokenFilePath);

                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

                string authToken = Encoding.UTF8.GetString(decryptedBytes);

                Array.Clear(decryptedBytes, 0, decryptedBytes.Length);

                return authToken;
            }
            catch (Exception ex)
            {
                var exception = new IOException("Не удалось прочитать файл с токеном", ex);
                _logger.LogCritical(exception, "Не удалось прочитать файл с токеном");
                throw exception;
            }

        }

        public bool TokenExists() => File.Exists(_tokenFilePath);
    }
}
