using System.Security.Cryptography;
using System.Text;

namespace IIS_Manager.Utility
{
    public class PasswordEncrypter
    {
        public PasswordEncrypter()
        {

        }

        public string Encrypt(string password, string id)
        {
            byte[] encryptedBytes;
            byte[] ivBytes;

            byte[] keyBytes;
            using (var deriveBytes = new Rfc2898DeriveBytes(id, Encoding.UTF8.GetBytes(id), 1000))
            {
                keyBytes = deriveBytes.GetBytes(32);
            }

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV();
                ivBytes = aes.IV;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        var plainTextBytes = Encoding.UTF8.GetBytes(password);
                        cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            var combinedBytes = new byte[encryptedBytes.Length + ivBytes.Length];
            Array.Copy(encryptedBytes, 0, combinedBytes, 0, encryptedBytes.Length);
            Array.Copy(ivBytes, 0, combinedBytes, encryptedBytes.Length, ivBytes.Length);

            return Convert.ToBase64String(combinedBytes);
        }

        public string Decrypt(string passwordHash, string id)
        {
            var combinedBytes = Convert.FromBase64String(passwordHash);
            var encryptedBytes = new byte[combinedBytes.Length - 16];
            var ivBytes = new byte[16];
            Array.Copy(combinedBytes, 0, encryptedBytes, 0, encryptedBytes.Length);
            Array.Copy(combinedBytes, encryptedBytes.Length, ivBytes, 0, ivBytes.Length);

            // Generate the same key using PBKDF2
            byte[] keyBytes;
            using (var deriveBytes = new Rfc2898DeriveBytes(id, Encoding.UTF8.GetBytes(id), 1000))
            {
                keyBytes = deriveBytes.GetBytes(32);
            }

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(encryptedBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}
