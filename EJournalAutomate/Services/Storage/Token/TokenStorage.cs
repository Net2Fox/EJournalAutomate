using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EJournalAutomate.Services.Storage.Token
{
    public class TokenStorage : ITokenStorage
    {
        private readonly string _tokenFilePath = Path.Combine(Environment.CurrentDirectory, "token.dat");
        public async Task SaveTokenAsync(string token)
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);

            byte[] encryptedBytes = ProtectedData.Protect(tokenBytes, null, DataProtectionScope.CurrentUser);

            await File.WriteAllBytesAsync(_tokenFilePath, encryptedBytes);

            Array.Clear(tokenBytes, 0, tokenBytes.Length);
        }

        public async Task<string> LoadTokenAsync()
        {
            if (!File.Exists(_tokenFilePath))
            {
                return string.Empty;
            }

            byte[] encryptedBytes = await File.ReadAllBytesAsync(_tokenFilePath);

            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

            string authToken = Encoding.UTF8.GetString(decryptedBytes);

            Array.Clear(decryptedBytes, 0, decryptedBytes.Length);

            return authToken;
        }

        public bool TokenExists() => File.Exists(_tokenFilePath);
    }
}
