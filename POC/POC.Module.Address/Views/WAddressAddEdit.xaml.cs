using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Address.Models;
using POL.DB.P30Office;


namespace POC.Module.Address.Views
{
    public partial class WAddressAddEdit : DXWindow
    {
        private DBCTAddress SelectedData { get; set; }
        private MAddressAddEdit Model { get; set; }

        public WAddressAddEdit(DBCTAddress selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new Models.MAddressAddEdit(this);
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
            Closing += (s, e) =>
            {
                if (ForceUpdate)
                    DialogResult = true;
            };
        }

        protected bool ForceUpdate { get; set; }

        public DBCTAddress DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }

        public void DynamicRefocus()
        {
            firstFocused.Focus();
            ForceUpdate = true;
        }
    }
}
