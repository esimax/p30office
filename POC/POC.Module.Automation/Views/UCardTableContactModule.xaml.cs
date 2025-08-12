using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace POC.Module.Automation.Views
{
    public partial class UCardTableContactModule : UserControl, IModuleRibbon
    {
        public UCardTableContactModule()
        {
            InitializeComponent();
        }

        public object GetRibbon()
        {
            return cardtableRibbon;
        }

        public void LoadChildRibbons()
        {
        }

        public void UnloadChildRibbons()
        {
        }

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
