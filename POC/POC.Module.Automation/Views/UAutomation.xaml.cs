using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using POL.Lib.Interfaces;

namespace POC.Module.Automation.Views
{
    public partial class UAutomation : UserControl, IModuleRibbon
    {
        public UAutomation()
        {
            InitializeComponent();
        }

        #region Implementation of IModuleRibbon
        public object GetRibbon()
        {
            return phoneRibbon;
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
