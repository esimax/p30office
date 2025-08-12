using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Phone.Models;
using POL.DB.P30Office.BT;
using System.Threading.Tasks;


namespace POC.Module.Phone.Views
{
    public partial class WPhoneTitleAddEdit : DXWindow
    {
        private DBBTPhoneTitle2 SelectedData { get; set; }
        private MPhoneTitleAddEdit Model { get; set; }

        public WPhoneTitleAddEdit(DBBTPhoneTitle2 selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new Models.MPhoneTitleAddEdit(this);
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

        public DBBTPhoneTitle2 DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
