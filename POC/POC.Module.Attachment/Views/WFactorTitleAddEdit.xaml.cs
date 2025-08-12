using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Attachment.Models;
using POL.DB.P30Office.BT;

namespace POC.Module.Attachment.Views
{
    public partial class WFactorTitleAddEdit : DXWindow
    {
        private DBBTFactorTitle2 SelectedData { get; set; }
        private MFactorTitleAddEdit Model { get; set; }

        public WFactorTitleAddEdit(DBBTFactorTitle2 selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MFactorTitleAddEdit(this);
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

        public DBBTFactorTitle2 DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
