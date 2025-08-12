using System.Windows;
using DevExpress.Xpf.Core;
using POC.Module.Address.Models;
using POL.Lib.Utils;
using DevExpress.Xpf.Grid;
using POL.DB.P30Office.BT;


namespace POC.Module.Address.Views
{
    public partial class WAddressTitleManage : DXWindow
    {
        private MAddressTitleManage Model { get; set; }
        public WAddressTitleManage(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MAddressTitleManage(this);
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

        public DBBTAddressTitle2 SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
