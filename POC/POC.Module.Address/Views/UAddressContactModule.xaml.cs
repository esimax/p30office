using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using System.Windows;
using System.Windows.Controls;
using POL.Lib.Interfaces;

namespace POC.Module.Address.Views
{
    public partial class UAddressContactModule : UserControl, IModuleRibbon
    {
        public UAddressContactModule()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return addressRibbon;
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
            colTitle.VisibleIndex = 0;
            colCountry.VisibleIndex = 1;
            colState.VisibleIndex = 2;
            colCity.VisibleIndex = 3;
            colArea.VisibleIndex = 4;
            colAddress.VisibleIndex = 5;
            colZipCode.VisibleIndex = 6;
            colPOBOX.VisibleIndex = 7;
            colNote.VisibleIndex = 8;
        }
    }
}
