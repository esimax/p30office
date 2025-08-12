using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.DB.P30Office;

namespace POC.Module.Profile.Views
{
    public partial class WListAddEdit : DXWindow
    {
        private DBCTList SelectedData { get; set; }
        private MListAddEdit Model { get; set; }

        public WListAddEdit(DBCTList selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;
            Loaded += (s, e) =>
            {
                Model = new MListAddEdit(this);
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

        public DBCTList DynamicSelectedData { get { return SelectedData; } }
    }
}
