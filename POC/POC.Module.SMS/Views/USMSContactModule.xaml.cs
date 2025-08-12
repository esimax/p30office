using System.Windows;
using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using DevExpress.Xpf.Editors;

namespace POC.Module.SMS.Views
{
    public partial class USMSContactModule : IModuleRibbon
    {
        public USMSContactModule()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return smsRibbon;
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
            get { return gridSMS; }
        }

        public TextEdit DynamicBody
        {
            get { return teBody; }
        }
    }
}
