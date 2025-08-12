using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using System.Windows;

namespace POC.Module.Call.Views
{
    public partial class UCallContactModule : IModuleRibbon
    {
        public UCallContactModule()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return callRibbon;
        }

        public void UnloadChildRibbons()
        {
        }

        public void LoadChildRibbons()
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
        public GridControl DynamicGrid
        {
            get { return gridCall; }
        }
    }
}
