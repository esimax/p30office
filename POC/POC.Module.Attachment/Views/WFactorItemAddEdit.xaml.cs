using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Attachment.Models;
using POL.DB.P30Office.BT;
using System.Threading.Tasks;


namespace POC.Module.Attachment.Views
{
    public partial class WFactorItemAddEdit : DXWindow
    {
        private DBACFactorItem SelectedData { get; set; }
        private MFactorItemAddEdit Model { get; set; }

        public WFactorItemAddEdit(DBACFactor factor, DBACFactorItem selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;
            DynamicFactor = factor;
            Loaded += (s, e) =>
            {
                Model = new Models.MFactorItemAddEdit(this);
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

        public DBACFactor DynamicFactor { get; set; }
        public DBACFactorItem DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
