using System.Windows;
using DevExpress.Xpf.Core;
using POC.Module.Attachment.Models;
using POL.Lib.Utils;
using DevExpress.Xpf.Grid;
using POL.DB.P30Office.AC;


namespace POC.Module.Attachment.Views
{
    public partial class WProductManage : DXWindow
    {
        private MProductManage Model { get; set; }
        public WProductManage(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MProductManage(this);
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

        public DBACProduct SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
