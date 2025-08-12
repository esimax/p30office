using System.Windows;
using DevExpress.Xpf.Core;
using System.Threading.Tasks;
using System.Threading;
using POL.Lib.Utils;


namespace POC.Module.Address.Views
{
    public partial class WAddressImportPreview : DXWindow
    {
        public WAddressImportPreview()
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
