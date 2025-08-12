using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace POC.Module.Profile.Views
{
    public partial class UProfileReport : UserControl, IModuleRibbon
    {
        public UProfileReport()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return theRibbon;
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
            get { return ServiceLocator.Current.GetInstance<IPOCMainWindow>().GetWindow(); }
        }
        public GridControl DynamicGrid
        {
            get { return gridReport; }
        }
    }
}
