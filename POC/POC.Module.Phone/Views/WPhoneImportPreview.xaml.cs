using System.Windows;
using DevExpress.Xpf.Core;
using System.Threading.Tasks;
using System.Threading;
using POL.Lib.Utils;


namespace POC.Module.Phone.Views
{
    public partial class WPhoneImportPreview : DXWindow
    {
        public WPhoneImportPreview()
        {
            InitializeComponent();
            Loaded +=
                (s, e) => Task.Factory.StartNew(
                    () =>
                    {
                        Thread.Sleep(1000);
                        HelperUtils.DoDispatcher(() => tvMain.BestFitColumns());
                    });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
