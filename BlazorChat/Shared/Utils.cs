using System;
using System.Security.Cryptography;
using System.Text;

namespace BlazorChat.Shared
{
    public class Utils
    {
        public static string CreateHash()
        {

            using var rng = RandomNumberGenerator.Create();
            byte[] buff = new byte[20];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string PasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            SHA256 sha = SHA256.Create();
            byte[] result = sha.ComputeHash(Encoding.UTF8.GetBytes(saltAndPwd));
            return Convert.ToBase64String(result);
        }
    }
}
