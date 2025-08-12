using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POC.Module.Profile.Models;
using POL.DB.P30Office;
using POL.Lib.Utils;

namespace POC.Module.Profile.Views
{
    public partial class WListManager : DXWindow
    {
        private MListManage Model { get; set; }
        public WListManager(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;
            Loaded += (s, e) =>
            {
                Model = new MListManage(this);
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

        public DBCTList SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
