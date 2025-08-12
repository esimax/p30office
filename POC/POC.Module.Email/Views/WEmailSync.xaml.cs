using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using POC.Module.Email.Models;
using POL.DB.P30Office;


namespace POC.Module.Email.Views
{
   
    public partial class WEmailSync
    {
        public WEmailSync(DBEMEmailInbox dbEmail)
        {
            InitializeComponent();
            DynamicDBEmail = dbEmail;

            Loaded += (s, e) =>
            {
                DataContext = new MEmailSync(this);
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


        public ListBoxEdit DynamicListBoxCat
        {
            get
            {
                return lbeCat;
            }
        }
        public DBEMEmailInbox DynamicDBEmail { get; set; }
    }
}
