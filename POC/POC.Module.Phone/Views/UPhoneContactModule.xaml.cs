using System.Windows;
using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Module.Phone.Views
{
    public partial class UPhoneContactModule : IModuleRibbon
    {
        public UPhoneContactModule()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return phoneRibbon;
        }
        public void LoadChildRibbons()
        {
        }
        public void UnloadChildRibbons()
        {
        }
        #endregion


        public Window DynamicOwner
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPOCMainWindow>().GetWindow();
            }
        }
        public GridControl DynamicGridControl
        {
            get { return gcMain; }
        }
        public TableView DynamicTableView
        {
            get { return tvMain; }
        }
        public void DynamicBestFitColumn()
        {
            tvMain.BestFitColumns();
        }

        public void DynamicReorderColumns()
        {
            colPhoneType.VisibleIndex = 0;
            colTitle.VisibleIndex = 1;

            colPhoneNumber.VisibleIndex = 2;
            colCodeString.VisibleIndex = 3;
            colNote.VisibleIndex = 4;

            colCallOutCount.VisibleIndex = 5;
            colCallInCount.VisibleIndex = 6;
            colCallTotalCount.VisibleIndex = 7;
            colDurationOut.VisibleIndex = 8;
            colDurationIn.VisibleIndex = 9;
            colDurationTotal.VisibleIndex = 10;


        }
    }
}
