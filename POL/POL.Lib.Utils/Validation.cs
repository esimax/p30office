namespace POL.Lib.Utils
{
    public class Validation
    {
        public static bool CheckString(string s, bool canNotBeNull, bool canNotBeEmpty, bool canNotBeSpace,
            int minLength, int maxLength)
        {
            if (canNotBeNull)
                if (s == null)
                    return false;
            if (canNotBeEmpty)
                if (s == string.Empty)
                    return false;
            if (canNotBeSpace)
                if (s != null)
                    if (s.Trim() == string.Empty)
                        return false;
            if (minLength > 0)
                if (s != null)
                    if (s.Length < minLength)
                        return false;
            if (maxLength > 0)
                if (s != null)
                    if (s.Length > maxLength)
                        return false;
            return true;
        }

        public static bool CheckString(string s, bool canNotBeNull = true, bool canNotBeEmpty = true,
            bool canNotBeSpace = true)
        {
            return CheckString(s, canNotBeNull, canNotBeEmpty, canNotBeSpace, 0, 0);
        }

        public static bool CheckString(string s, int minLength, int maxLength)
        {
            return CheckString(s, true, true, true, minLength, maxLength);
        }

        public static bool IsNull(object o)
        {
            return o == null;
        }
    }
}
