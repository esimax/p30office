using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using POC.Module.Call.Models;
using POL.DB.P30Office;


namespace POC.Module.Call.Views
{

    public partial class WCallSync
    {
        public WCallSync(DBCLCall dbCall)
        {
            InitializeComponent();
            DynamicDBCall = dbCall;

            Loaded += (s, e) =>
            {
                DataContext = new MCallSync(this);
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
        public ListBoxEdit DynamicListBoxUser
        {
            get
            {
                return lbeUser;
            }
        }
        public DBCLCall DynamicDBCall { get; set; }
    }
}
