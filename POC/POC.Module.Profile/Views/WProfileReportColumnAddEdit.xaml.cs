using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Profile.Models;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Views
{
    public partial class WProfileReportColumnAddEdit : DXWindow
    {
        private MProfileReportColumnAddEdit Model { get; set; }

        public WProfileReportColumnAddEdit(MetaDataProfileReportItem selectedData)
        {
            InitializeComponent();
            DynamicSelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MProfileReportColumnAddEdit(this);
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

        public MetaDataProfileReportItem DynamicSelectedData { get; set; }
    }
}
