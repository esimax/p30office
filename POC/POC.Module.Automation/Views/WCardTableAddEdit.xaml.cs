using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Automation.Models;
using POL.DB.P30Office;



namespace POC.Module.Automation.Views
{

    public partial class WCardTableAddEdit : DXWindow
    {
        public DBTMCardTable2 SelectedData { get; set; }
        private MCardTableAddEdit Model { get; set; }

        public WCardTableAddEdit(DBTMCardTable2 selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MCardTableAddEdit(this);
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

        public WCardTableAddEdit(string title, string note, object category, object contact, object sms, object email, object call)
        {
            InitializeComponent();
            SelectedData = null;

            Loaded += (s, e) =>
            {
                Model = new MCardTableAddEdit(this, title, note, category, contact, sms, email, call);
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

        public DBTMCardTable2 DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
        public DBCTContact DynamicContact { get; set; }

        public void MakeReadOnly()
        {
            g1.IsEnabled = false;
            g2.IsEnabled = false;
            g3.IsEnabled = false;
            g4.IsEnabled = false;
        }

        public void IsNew()
        {
            g4.IsEnabled = false;
        }
    }
}
