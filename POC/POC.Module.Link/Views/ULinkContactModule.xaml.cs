using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Module.Link.Views
{
    public partial class ULinkContactModule : UserControl, IModuleRibbon
    {
        public ULinkContactModule()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return linkRibbon;
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
    }
}
