using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Link.Models;
using POL.DB.P30Office;


namespace POC.Module.Link.Views
{
    public partial class WLinkRelSubAddEdit : DXWindow
    {
        private DBCTContactRelSub SelectedData { get; set; }
        private MLinkRelSubAddEdit Model { get; set; }

        public WLinkRelSubAddEdit(DBCTContactRelMain relMain,DBCTContactRelSub selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;
            DynamicRelMain = relMain;
            Loaded += (s, e) =>
            {
                Model = new MLinkRelSubAddEdit(this);
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

        public DBCTContactRelMain DynamicRelMain { get; set; }

        public DBCTContactRelSub DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
