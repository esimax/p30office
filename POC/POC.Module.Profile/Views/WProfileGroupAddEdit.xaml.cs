using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.DB.P30Office;

namespace POC.Module.Profile.Views
{
    public partial class WProfileGroupAddEdit : DXWindow
    {
        private MProfileGroupAddEdit Model { get; set; }

        public WProfileGroupAddEdit(DBCTProfileRoot profileRoot,DBCTProfileGroup selectedData)
        {
            InitializeComponent();
            DynamicSelectedData = selectedData;
            DynamicProfileRoot = profileRoot;
            Loaded += (s, e) =>
            {
                Model = new MProfileGroupAddEdit(this);
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

        public DBCTProfileRoot DynamicProfileRoot { get; set; }

        public DBCTProfileGroup DynamicSelectedData { get; set; }
    }
}
