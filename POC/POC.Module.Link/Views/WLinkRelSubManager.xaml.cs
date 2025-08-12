using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POC.Module.Link.Models;
using POL.DB.P30Office;
using POL.Lib.Utils;


namespace POC.Module.Link.Views
{
    public partial class WLinkRelSubManager : DXWindow
    {
        private MLinkRelSubManage Model { get; set; }
        public WLinkRelSubManager(bool isSelectMode,DBCTContactRelMain relMain)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;
            DynamicRelMain = relMain;

            Loaded += (s, e) =>
            {
                Model = new MLinkRelSubManage(this);
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

        public DBCTContactRelSub SelectedData
        {
            get { return Model.SelectedData; }
        }

        public DBCTContactRelMain DynamicRelMain { get; set; }
    }
}
