using System.Globalization;
using System.Linq;
using System.Net.Mail;

namespace POL.Lib.Utils
{
    public static class ExtensionString
    {
        public static bool IsDigital(this string theValue)
        {
            if (theValue.Any(c => !char.IsDigit(c)))
            {
                return false;
            }
            return theValue.Length > 0;
        }

        public static bool IsValidEmailAddress(this string s)
        {
            try
            {
                new MailAddress(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsAny(this string theString, char[] characters)
        {
            return characters.Any(character => theString.Contains(character.ToString(CultureInfo.InvariantCulture)));
        }

        public static bool IsNumeric(this string theValue)
        {
            long retNum;
            return long.TryParse(theValue, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out retNum);
        }

        public static bool ContainsUnicodeCharacter(this string input)
        {
            const int maxAnsiCode = 255;
            if (string.IsNullOrEmpty(input)) return false;
            return input.ToCharArray().Any(c => c > maxAnsiCode);
        }
    }

}
