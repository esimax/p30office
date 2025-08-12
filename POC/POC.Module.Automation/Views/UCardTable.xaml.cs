using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using POL.Lib.Interfaces;

namespace POC.Module.Automation.Views
{
    public partial class UCardTable : UserControl, IModuleRibbon
    {
        public UCardTable()
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


        public void UpdateCellTemplate()
        {
            var temp = colStartingDate.CellTemplate;
            colStartingDate.CellTemplate = null;
            colStartingDate.CellTemplate = temp;

            var temp2 = colEndingDate.CellTemplate;
            colEndingDate.CellTemplate = null;
            colEndingDate.CellTemplate = temp2;
        }
    }
}
