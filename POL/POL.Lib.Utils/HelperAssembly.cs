using System.IO;
using System.Reflection;

namespace POL.Lib.Utils
{
    public class HelperAssembly
    {
        public static string GetApplicationPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
