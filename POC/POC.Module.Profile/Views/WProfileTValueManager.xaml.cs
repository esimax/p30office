using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using POC.Module.Profile.Models;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Module.Profile.Views
{
    public partial class WProfileTValueManager : DXWindow
    {
        private MProfileTValueManage Model { get; set; }
        public WProfileTValueManager(bool isSelectMode,DBCTProfileTable table)
        {
            InitializeComponent();
            DynamicIsSelectionMode = isSelectMode;
            DynamicTable = table;

            Loaded += (s, e) =>
            {
                Model = new MProfileTValueManage(this);
                DataContext = Model;
                HelperLocalize.SetLanguageToDefault();
            };

        }



        public Window DynamicOwner
        {
            get { return this; }
        }
        public TreeListControl DynamicTreeListControl
        {
            get { return tlcMain; }
        }

        public bool DynamicIsSelectionMode { get; set; }

        public CacheItemProfileTValue SelectedData
        {
            get { return  Model.SelectedData; }
        }

        public DBCTProfileTable DynamicTable { get; set; }
    }
}
