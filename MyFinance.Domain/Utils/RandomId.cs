using System.Linq;

namespace MyFinance.Domain.Utils
{
    public static class RandomId
    {
        const string NUMBERS = "0123456789";
        const string LOWERCASE_LETTERS = "abcdefghijklmnopqrstuvxwyz";
        const string UPPERCASE_LETTERS = "ABCDEFGHIJKLMNOPQRSTUVXWYZ";

        public static string New(int length = 8, string chars = NUMBERS + LOWERCASE_LETTERS + UPPERCASE_LETTERS)
        {
            return GetRandomString(length, chars);
        }

        public static string NewRandomPassword(int length = 10, string chars = NUMBERS + LOWERCASE_LETTERS + UPPERCASE_LETTERS)
        {
            return GetRandomString(length, chars);
        }

        public static string NewRandomConfirmationNumber(int lenght = 6, string chars = NUMBERS)
        {
            return GetRandomString(lenght, chars);
        }

        static string GetRandomString(int length = 8, string chars = null)
        {
            if (string.IsNullOrEmpty(chars))
                chars = NUMBERS + UPPERCASE_LETTERS + LOWERCASE_LETTERS;
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[StaticRandom.Next(0, s.Length)])
                .ToArray());
        }
    }
}