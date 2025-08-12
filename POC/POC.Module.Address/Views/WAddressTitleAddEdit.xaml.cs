using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Address.Models;
using POL.DB.P30Office.BT;
using System.Threading.Tasks;


namespace POC.Module.Address.Views
{
    public partial class WAddressTitleAddEdit : DXWindow
    {
        private DBBTAddressTitle2 SelectedData { get; set; }
        private MAddressTitleAddEdit Model { get; set; }

        public WAddressTitleAddEdit(DBBTAddressTitle2 selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new Models.MAddressTitleAddEdit(this);
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

        public DBBTAddressTitle2 DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
