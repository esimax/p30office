using System;
using System.Windows;
using DevExpress.Xpf.Core;
using POL.DB.P30Office;
using POL.Lib.Utils;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace POC.Module.Call.Views
{
    public partial class WCallCorrection : DXWindow
    {
        public WCallCorrection(DBCLCall dbcall)
        {
            InitializeComponent();

            DynamicDBCall = dbcall;

            Loaded += (s, e) =>
            {
                var model = new Models.MCallCorrection(this);
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

        public Window DynamicOwner { get { return this; } }
        public DBCLCall DynamicDBCall { get; set; }
    }
}
