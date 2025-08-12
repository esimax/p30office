using System.Windows;
using DevExpress.Xpf.Core;
using POC.Module.Phone.Models;
using POL.Lib.Utils;
using DevExpress.Xpf.Grid;
using POL.DB.P30Office.BT;


namespace POC.Module.Phone.Views
{
    public partial class WPhoneTitleManage : DXWindow
    {
        private MPhoneTitleManage Model { get; set; }
        public WPhoneTitleManage(bool isSelectMode)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;

            Loaded += (s, e) =>
            {
                Model = new MPhoneTitleManage(this);
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

        public DBBTPhoneTitle2 SelectedData
        {
            get { return Model.SelectedData; }
        }
    }
}
