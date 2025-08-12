using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using POC.Module.Attachment.Models;
using POL.DB.P30Office.AC;

namespace POC.Module.Attachment.Views
{
    public partial class WFactorReportTemplateAddEdit : DXWindow
    {
        private DBACFactorReportTemplate SelectedData { get; set; }
        private MFactorReportTemplateAddEdit Model { get; set; }

        public WFactorReportTemplateAddEdit(DBACFactorReportTemplate selectedData)
        {
            InitializeComponent();
            SelectedData = selectedData;

            Loaded += (s, e) =>
            {
                Model = new MFactorReportTemplateAddEdit(this);
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

        public DBACFactorReportTemplate DynamicSelectedData { get { return SelectedData; } }
        public Window DynamicOwner { get { return this; } }
    }
}
