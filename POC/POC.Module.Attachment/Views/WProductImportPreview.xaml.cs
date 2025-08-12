using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;
using POL.Lib.Utils;

namespace POC.Module.Attachment.Views
{
    public partial class WProductImportPreview : DXWindow
    {
        public WProductImportPreview()
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
