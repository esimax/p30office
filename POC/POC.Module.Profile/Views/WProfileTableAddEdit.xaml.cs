using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.DB.P30Office;

namespace POC.Module.Profile.Views
{
    public partial class WProfileTableAddEdit : DXWindow
    {
        
        private MProfileTableAddEdit Model { get; set; }

        public WProfileTableAddEdit(DBCTProfileTable selectedData)
        {
            InitializeComponent();
            DynamicSelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MProfileTableAddEdit(this);
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

        public DBCTProfileTable DynamicSelectedData { get; set; }
        public Window DynamicOwner { get { return this; } }
    }
}
