using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.Lib.Utils;
using POL.DB.P30Office;


namespace POC.Module.Call.Views
{
    public partial class WCallReportAddEdit : DXWindow
    {
        public WCallReportAddEdit(DBCLCallReport report)
        {
            InitializeComponent();
            DynamicDBCallReport = report;
            Loaded += (s, e) =>
            {
                var model = new Models.MCallReportAddEdit(this);
                DataContext = model;
                firstFocused.Focus();
                HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                    });
            };
        }

        public DBCLCallReport DynamicDBCallReport { get; set; }

    }
}
