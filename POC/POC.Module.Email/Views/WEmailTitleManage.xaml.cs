using System.Windows;
using POC.Module.Email.Models;
using POL.Lib.Utils;
using DevExpress.Xpf.Grid;
using POL.DB.P30Office.BT;


namespace POC.Module.Email.Views
{
    public partial class WEmailTitleManage
    {
        private MEmailTitleManage Model { get; set; }
        public WEmailTitleManage(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MEmailTitleManage(this);
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

        public DBBTEmailTitle2 SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
