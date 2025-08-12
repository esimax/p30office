using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POC.Module.Attachment.Models;
using POL.DB.P30Office.AC;
using POL.Lib.Utils;

namespace POC.Module.Attachment.Views
{
    public partial class WFactorReportTemplateManage : DXWindow
    {
        private MFactorReportTemplateManage Model { get; set; }
        public WFactorReportTemplateManage(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MFactorReportTemplateManage(this);
                DataContext = Model;
                HelperLocalize.SetLanguageToDefault();
            };

        }

        public Window DynamicOwner
        {
            get { return this; }
        }
        public GridControl DynamicGridControl
        {
            get { return gcMain; }
        }
        public TableView DynamicTableView
        {
            get { return tvMain; }
        }
        public bool DynamicIsSelectionMode { get; set; }

        public DBACFactorReportTemplate SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
