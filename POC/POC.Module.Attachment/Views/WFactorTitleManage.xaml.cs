using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POC.Module.Attachment.Models;
using POL.DB.P30Office.BT;
using POL.Lib.Utils;

namespace POC.Module.Attachment.Views
{
    public partial class WFactorTitleManage : DXWindow
    {
        private MFactorTitleManage Model { get; set; }
        public WFactorTitleManage(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MFactorTitleManage(this);
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

        public DBBTFactorTitle2 SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
