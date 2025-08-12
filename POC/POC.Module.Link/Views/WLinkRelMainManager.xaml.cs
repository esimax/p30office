using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POC.Module.Link.Models;
using POL.DB.P30Office;
using POL.Lib.Utils;


namespace POC.Module.Link.Views
{
    public partial class WLinkRelMainManager : DXWindow
    {
        private MLinkRelMainManage Model { get; set; }
        public WLinkRelMainManager(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MLinkRelMainManage(this);
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

        public DBCTContactRelMain SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
