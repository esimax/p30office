using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Attachment.Models;
using POL.DB.P30Office.BT;
using System.Threading.Tasks;


namespace POC.Module.Attachment.Views
{
    public partial class WUnitAddEdit : DXWindow
    {
        private DBBTUnit SelectedData { get; set; }
        private MUnitAddEdit Model { get; set; }

        public WUnitAddEdit(DBBTUnit selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new Models.MUnitAddEdit(this);
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

        public DBBTUnit DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
