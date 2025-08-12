using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using POL.Lib.Interfaces;

namespace POC.Module.Attachment.Views
{
    public partial class UFactor : UserControl, IModuleRibbon
    {
        public UFactor()
        {
            InitializeComponent();
        }

        #region Implementation of IModuleRibbon
        public object GetRibbon()
        {
            return FactorRibbon;
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
