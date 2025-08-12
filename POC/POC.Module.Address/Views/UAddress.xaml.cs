using DevExpress.Xpf.Grid;
using POL.Lib.Interfaces;
using System.Windows.Controls;

namespace POC.Module.Address.Views
{
    public partial class UAddress : UserControl, IModuleRibbon
    {
        public UAddress()
        {
            InitializeComponent();
        }

        #region Implementation of IModuleRibbon
        public object GetRibbon()
        {
            return addressRibbon;
        }
        public void UnloadChildRibbons()
        {
        }
        public void LoadChildRibbons()
        {
        }
        #endregion

        public GridControl DynamicGridControl
        {
            get { return gcMain; }
        }
    }
}
