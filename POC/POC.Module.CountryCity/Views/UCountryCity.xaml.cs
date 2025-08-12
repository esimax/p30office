using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Ribbon;
using POL.Lib.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace POC.Module.CountryCity.Views
{
    public partial class UCountryCity : IModuleRibbon
    {
        public UCountryCity()
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
        public RibbonControl DynamicRibbonControl
        {
            get { return GetRibbon() as RibbonControl; }
        }
        public GridControl DynamicDynamicGrid
        {
            get { return gcMain; }
        }
    }
}
