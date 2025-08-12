using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POC.Module.Contact.Models;
using POL.DB.P30Office;
using POL.Lib.Utils;


namespace POC.Module.Contact.Views
{
    public partial class WContactCatManage : DXWindow
    {
        private MContactCatManage Model { get; set; }
        public WContactCatManage(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MContactCatManage(this);
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

        public DBCTContactCat SelectedData
        {
            get { return Model.SelectedData == null ? null : Model.SelectedData.Tag as DBCTContactCat; }
        }
    }
}
