using System.Text;
using System.Security.Cryptography;
using System;

namespace PresupuestoMVC.Security
{
    public class CryptoHelper
    {
        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            var bytes = Convert.FromBase64String(encryptedText);

            var decryptedBytes = ProtectedData.Unprotect(
                bytes,
                null,
                DataProtectionScope.LocalMachine
            );

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
