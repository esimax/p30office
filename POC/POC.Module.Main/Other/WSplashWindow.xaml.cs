using System.Windows;
using DevExpress.Xpf.Core;

namespace POC.Module.Main.Other
{
    public partial class WSplashWindow : Window, ISplashScreen
    {
        public WSplashWindow()
        {
            InitializeComponent();

            
        }

        public void CloseSplashScreen()
        {
            Close();
        }

        public void Progress(double value)
        {
            
        }

        public void SetProgressState(bool isIndeterminate)
        {
            
        }
    }
}
