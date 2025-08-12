using System;
using System.Text.RegularExpressions;
using DevExpress.Xpo;
using POL.Lib.Utils;

namespace POL.DB.Root
{
    public class HelperValidation
    {
        public static void CheckSession(Session session)
        {
            if (Validation.IsNull(session))
                throw new ArgumentNullException("session");
        }

        public static void CheckNullEmptySpace(string s, string paramname)
        {
            if (!Validation.CheckString(s, true, true, true))
                throw new ArgumentException("Parameter can not be Null,Empty or Space.", paramname);
        }

        public static void CheckNullEmptySpaceMax(string s, string paramname, int maxsize)
        {
            if (!Validation.CheckString(s, 0, maxsize))
                throw new ArgumentException(
                    string.Format("Parameter can not be Null,Empty or Space and length can not exceed {0}", maxsize),
                    paramname);
        }

        public static void CheckEmail(string email, string paramname, int maxsize)
        {
            CheckNullEmptySpaceMax(email, paramname, maxsize);
            var re = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
            if (!re.IsMatch(email))
                throw new ArgumentException("Parameter is not a correct email.", paramname);
        }

        public static void CheckNull(object obj, string paramname)
        {
            if (Validation.IsNull(obj))
                throw new ArgumentNullException(paramname);
        }


        public static void CheckStartingSpace(string s, string paramname)
        {
            if (s.StartsWith(" "))
                throw new ArgumentException("Can not start with space.", paramname);
        }

        public static void CheckEndingSpace(string s, string paramname)
        {
            if (s.EndsWith(" "))
                throw new ArgumentException("Can not end with space.", paramname);
        }
    }
}
