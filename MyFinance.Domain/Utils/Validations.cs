using System.Text.RegularExpressions;

namespace MyFinance.Domain.Utils
{
    public static class Validations
    {
        private static readonly Regex NUMBERS = new Regex(@"[^\d]");
        
        public static bool IsMobileNumber(string number, bool canBeNull = false)
        {
            if (!string.IsNullOrEmpty(number))
            {
                if (!number.StartsWith("+"))
                    return false;

                number = number[1..];

                var jusNumber = NUMBERS.Replace(number, string.Empty);

                if (jusNumber != number)
                    return false;

                if (number.Length < 6)
                    return false;

                return true;
            }
            else
                return canBeNull;
        }
    }
}