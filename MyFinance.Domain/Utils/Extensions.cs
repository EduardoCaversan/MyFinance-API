using System;
using System.Security.Cryptography;
using System.Text;

namespace MyFinance.Domain.Utils
{
    public static class Extensions
    {
        public static string SHA256Encrypt(this string str, bool useBase64 = true)
        {
            var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.Default.GetBytes(str));
            if (useBase64)
                return Convert.ToBase64String(bytes);
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
