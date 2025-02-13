using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace EJournalWPF.Data
{
    internal static class TokenStorage
    {
        private static readonly string _tokenFilePath = Path.Combine(Environment.CurrentDirectory, "token.dat");

        internal static void SaveToken(string authToken)
        {
            byte[] tokenBytes = UnicodeEncoding.UTF8.GetBytes(authToken);

            byte[] encryptedBytes = ProtectedData.Protect(tokenBytes, null, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(_tokenFilePath, encryptedBytes);

            Array.Clear(tokenBytes, 0, tokenBytes.Length);
        }

        internal static string LoadToken()
        {
            if (!File.Exists(_tokenFilePath))
            {
                return null;
            }

            byte[] encryptedBytes = File.ReadAllBytes(_tokenFilePath);

            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

            string authToken = UnicodeEncoding.UTF8.GetString(decryptedBytes);

            Array.Clear(decryptedBytes, 0, decryptedBytes.Length);

            return authToken;
        }
    }
}
