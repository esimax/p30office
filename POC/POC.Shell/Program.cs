using System;
using System.Windows;
using DevExpress.Utils;
using POL.Lib.Utils;

namespace POC.Shell
{
    public class Program
    {
        [STAThread]
        static void Main()
        {

            var screen = new SplashScreen("SplashScreen.png");
            screen.Show(true, false);
#if DEBUG
            var app = AppStartup.CreateApp(null);
            app.Run();
#else
            try
            {
                var app = AppStartup.CreateApp(null);
                app.Run();
            }
            catch (Exception ex)
            {
                var fileName = string.Format("{0}\\xoffice-{1}.exception",
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    HelperPersianCalendar.ToString(DateTime.Now, "yyMMdd-HHmmss"));
                using (var f = System.IO.File.CreateText(fileName))
                {
                    f.Write(ex);
                }
            }
#endif
        }
    }
    public static class AppStartup
    {
        public static Application CreateApp(Application app)
        {
            if (app == null)
            {
                app = new Application();
                app.Startup += (s, e) =>
                {
                    var bootstrapper = new Bootstrapper();
                    bootstrapper.Run();
                };
            }
            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = AssemblyHelper.GetResourceUri(typeof(AppStartup).Assembly, "Themes/Common.xaml") });
            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = AssemblyHelper.GetResourceUri(typeof(AppStartup).Assembly, "Themes/BlendScroll.xaml") });
            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = AssemblyHelper.GetResourceUri(typeof(AppStartup).Assembly, "Themes/Images.xaml") });

            app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = AssemblyHelper.GetResourceUri(typeof(POL.WPF.DXControls.POLHeaderedSeparator.POLHeaderedSeparator).Assembly, "Themes/Generic.xaml") });
            return app;
        }
    }
}
