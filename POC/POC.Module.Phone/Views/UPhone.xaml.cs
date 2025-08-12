using DevExpress.Xpf.Grid;
using POL.Lib.Interfaces;
using System.Windows.Controls;

namespace POC.Module.Phone.Views
{
    public partial class UPhone : UserControl, IModuleRibbon
    {
        public UPhone()
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
