using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.DB.P30Office;

namespace POC.Module.Profile.Views
{
    public partial class WProfileReportAddEdit : DXWindow
    {
        private MProfileReportAddEdit Model { get; set; }

        public WProfileReportAddEdit(DBCTProfileReport selectedData)
        {
            InitializeComponent();
            DynamicSelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MProfileReportAddEdit(this);
                DataContext = Model;
                firstFocused.Focus();
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                    });
            };
        }

        public DBCTProfileReport DynamicSelectedData { get; set; }
    }
}
