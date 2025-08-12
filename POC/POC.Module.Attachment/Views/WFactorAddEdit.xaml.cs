using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POC.Module.Attachment.Models;
using POL.DB.P30Office;
using POL.DB.P30Office.BT;
using System.Threading.Tasks;


namespace POC.Module.Attachment.Views
{
    public partial class WFactorAddEdit : DXWindow
    {
        private DBACFactor SelectedData { get; set; }
        private DBCTContact SelectedContact { get; set; }
        private MFactorAddEdit Model { get; set; }

        public WFactorAddEdit(DBACFactor selectedData, DBCTContact selectedContact)
        {
            InitializeComponent();
            SelectedData = selectedData;
            SelectedContact = selectedContact;

            Loaded += (s, e) =>
            {
                Model = new Models.MFactorAddEdit(this);
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

        public DBACFactor DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
        public TableView DynamicTableView{get { return tvMain; }}
        public DBCTContact DynamicSelectedContact { get { return SelectedContact; } }
    }
}
