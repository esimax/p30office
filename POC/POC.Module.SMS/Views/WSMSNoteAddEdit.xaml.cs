using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POL.Lib.Utils;
using POL.DB.P30Office;


namespace POC.Module.SMS.Views
{
    public partial class WSMSNoteAddEdit : DXWindow
    {
        public WSMSNoteAddEdit(DBSMLog call)
        {
            InitializeComponent();
            DynamicDBSMLog = call;
            Loaded += (s, e) =>
            {
                var model = new Models.MSMSNoteAddEdit(this);
                DataContext = model;
                firstFocused.Focus();
                HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                    });
            };
        }

        public DBSMLog DynamicDBSMLog { get; set; }
    }
}
