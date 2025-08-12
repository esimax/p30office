using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Attachment.Models;
using System.Threading.Tasks;
using POL.DB.P30Office.AC;


namespace POC.Module.Attachment.Views
{
    public partial class WProductAddEdit : DXWindow
    {
        private DBACProduct SelectedData { get; set; }
        private MProductAddEdit Model { get; set; }

        public WProductAddEdit(DBACProduct selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new Models.MProductAddEdit(this);
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

        public DBACProduct DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
