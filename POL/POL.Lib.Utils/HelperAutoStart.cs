using Microsoft.Win32;

namespace POL.Lib.Utils
{
    public class HelperAutoStart
    {
        public static void AutoStartApplication(bool doStart, string appName, string exepath)
        {
            try
            {
                var myKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (doStart)
                {
                    if (myKey != null) myKey.SetValue(appName, string.Format("\"{0}\"", exepath));
                }
                else
                {
                    if (myKey != null) myKey.DeleteValue(appName);
                }
            }
            catch
            {
            }
        }

        public static bool AutoStartStatus(string AppName)
        {
            try
            {
                var myKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (myKey != null)
                {
                    var ss = myKey.GetValue(AppName, "").ToString();
                    if (string.IsNullOrEmpty(ss))
                        return false;
                }
                return true;
            }
            catch
            {
            }
            return false;
        }
    }
}
