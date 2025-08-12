using System.Windows;
using DevExpress.Xpf.Core;
using POC.Module.Attachment.Models;
using POL.Lib.Utils;
using DevExpress.Xpf.Grid;
using POL.DB.P30Office.BT;


namespace POC.Module.Attachment.Views
{
    public partial class WUnitManage : DXWindow
    {
        private MUnitManage Model { get; set; }
        public WUnitManage(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MUnitManage(this);
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

        public DBBTUnit SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
