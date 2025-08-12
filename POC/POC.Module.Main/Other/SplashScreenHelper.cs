using DevExpress.Xpf.Core;

namespace POC.Module.Main.Other
{
    public class SplashScreenHelper
    {
        static SplashScreenHelper _instance = null;
        public static SplashScreenHelper Instance
        {
            get { return _instance ?? (_instance = new SplashScreenHelper()); }
        }

        SplashScreenHelper() { }


        public void ShowSplashScreen()
        {
            if (!DXSplashScreen.IsActive)
                DXSplashScreen.Show<WSplashWindow>();
                
        }

        public void HideSplashScreen()
        {
            if (DXSplashScreen.IsActive)
                DXSplashScreen.Close();
        }
    }
}
