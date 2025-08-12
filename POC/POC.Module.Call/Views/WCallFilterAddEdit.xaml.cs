using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.Lib.Utils;
using POL.DB.P30Office;


namespace POC.Module.Call.Views
{
    public partial class WCallFilterAddEdit : DXWindow
    {
        public WCallFilterAddEdit(DBCLCallFilter filter )
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var model = new Models.MCallFilterAddEdit(this);
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

        public DBCLCallFilter DynamicDBCallFilter { get; set; }

    }
}
