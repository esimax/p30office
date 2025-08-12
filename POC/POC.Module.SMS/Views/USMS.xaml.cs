using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Module.SMS.Views
{
    public partial class USMS : UserControl, IModuleRibbon
    {
        public USMS()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return smsRibbon;
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
        public GridControl DynamicGrid
        {
            get { return gridSMS; }
        }

        public TextEdit DynamicBody
        {
            get { return teBody; }
        }
    }
}
