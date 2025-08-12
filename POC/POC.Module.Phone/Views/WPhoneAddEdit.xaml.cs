using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.DB.P30Office;
using POC.Module.Phone.Models;


namespace POC.Module.Phone.Views
{
    
    public partial class WPhoneAddEdit : DXWindow
    {
        private DBCTPhoneBook SelectedData { get; set; }
        private MPhoneAddEdit Model { get; set; }

        public WPhoneAddEdit(DBCTPhoneBook selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new Models.MPhoneAddEdit(this);
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

        public DBCTPhoneBook DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }

        public void DynamicRefocus()
        {
            firstFocused.Focus();
            ForceUpdate = true;
        }
    }
}
