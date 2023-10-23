using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestingServerApp
{
    public static class PasswordEncryption
    {
        public static byte[] GenerateSaltAsBytes(byte saltSize = 16)
        {
            byte[] salt = new byte[saltSize]; // 16 bytes is a common salt length
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string GetPasswordHash(string password, byte[] saltAsBytes)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] saltedPassword = new byte[saltAsBytes.Length + Encoding.UTF8.GetByteCount(password)];
                Array.Copy(saltAsBytes, saltedPassword, saltAsBytes.Length);
                Encoding.UTF8.GetBytes(password, 0, password.Length, saltedPassword, saltAsBytes.Length);

                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
