using System;
using System.Globalization;
using System.Windows.Media;

namespace POL.Lib.Utils
{
    public static class HelperConvert
    {
        public const char WrongK = 'ک';
        public const char CorrectK = 'ك';

        public const char WrongY = 'ي';
        public const char CorrectY = 'ی';


        public static void CorrectPersianBug(ref string str)
        {
            if (str == null) return;
            str = str.Replace(WrongY, CorrectY);
            str = str.Replace(WrongK, CorrectK);
        }

        public static string CorrectPersianBug(string str)
        {
            var rv = str;
            CorrectPersianBug(ref rv);
            return rv;
        }

        public static void UnCorrectPersianBug(ref string str)
        {
            if (str == null) return;
            str = str.Replace(CorrectY, WrongY);
            str = str.Replace(CorrectK, WrongK);
        }

        public static string UnCorrectPersianBug(string str)
        {
            var rv = str;
            UnCorrectPersianBug(ref rv);
            return rv;
        }

        public static void ConvertToPersianDigit(ref string str)
        {
            if (str == null) return;
            str = str.Replace("0", "\u0660");
            str = str.Replace("1", "\u0661");
            str = str.Replace("2", "\u0662");
            str = str.Replace("3", "\u0663");
            str = str.Replace("4", "\u0664");
            str = str.Replace("5", "\u0665");
            str = str.Replace("6", "\u0666");
            str = str.Replace("7", "\u0667");
            str = str.Replace("8", "\u0668");
            str = str.Replace("9", "\u0669");
        }

        public static string ConvertToPersianDigit(string str)
        {
            var rv = str;
            ConvertToPersianDigit(ref rv);
            return rv;
        }

        public static void ConvertToEnglishDigit(ref string str)
        {
            if (str == null) return;
            str = str.Replace("\u0660", "0");
            str = str.Replace("\u0661", "1");
            str = str.Replace("\u0662", "2");
            str = str.Replace("\u0663", "3");
            str = str.Replace("\u0664", "4");
            str = str.Replace("\u0665", "5");
            str = str.Replace("\u0666", "6");
            str = str.Replace("\u0667", "7");
            str = str.Replace("\u0668", "8");
            str = str.Replace("\u0669", "9");
        }

        public static string ConvertToEnglishDigit(string str)
        {
            var rv = str;
            ConvertToEnglishDigit(ref rv);
            return rv;
        }

        public static string ConvertToFileSizeFormat(decimal d)
        {
            if (d < 1024)
                return string.Format("{0} bytes", d);
            if (d < 1024*1024)
            {
                var dbl = Convert.ToDouble(d)/1024;
                dbl = dbl*100;
                dbl = Math.Ceiling(dbl);
                dbl = dbl/100;

                var s = dbl.ToString(CultureInfo.InvariantCulture);
                var i = s.IndexOf('.');
                var l = s.Length;
                l = l - i;
                if (i < 0)
                    s += ".00";
                else if (l == 2)
                    s += "0";

                return string.Format("{0} KB", s);
            }
            if (d < 1024*1024*1024)
            {
                var dbl = Convert.ToDouble(d)/(1024*1024);
                dbl = dbl*100;
                dbl = Math.Ceiling(dbl);
                dbl = dbl/100;

                var s = dbl.ToString(CultureInfo.InvariantCulture);
                var i = s.IndexOf('.');
                var l = s.Length;
                l = l - i;
                if (i < 0)
                    s += ".00";
                else if (l == 2)
                    s += "0";

                return string.Format("{0} MB", s);
            }
            {
                var dbl = Convert.ToDouble(d)/(1024*1024*1024);
                dbl = dbl*100;
                dbl = Math.Ceiling(dbl);
                dbl = dbl/100;

                var s = dbl.ToString(CultureInfo.InvariantCulture);
                var i = s.IndexOf('.');
                var l = s.Length;
                l = l - i;
                if (i < 0)
                    s += ".00";
                else if (l == 2)
                    s += "0";

                return string.Format("{0} GB", s);
            }
        }

        public static string SeperateBy(string val, char ch, int count)
        {
            var rv = val;
            var l = val.Length;
            while (l > count)
            {
                l = l - count;
                rv = rv.Insert(l, ch.ToString(CultureInfo.InvariantCulture));
            }
            rv = rv.Replace("-,", "- ");
            return rv;
        }


        public static Color DoubleToColor(double d)
        {
            return Int64ToColor(Convert.ToInt64(d));
        }

        public static double ColorToDouble(Color c)
        {
            return Convert.ToDouble(ColorToInt64(c));
        }

        public static Color Int64ToColor(long val)
        {
            return Color.FromArgb((byte) (val >> 24), (byte) ((val << 8) >> 24), (byte) ((val << 16) >> 24),
                (byte) ((val << 24) >> 24));
        }

        public static long ColorToInt64(Color c)
        {
            return (long) c.A*256*256*256 + (long) c.R*256*256 + (long) c.G*256 + c.B;
        }
    }
}
