using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using DevExpress.Xpf.Grid;

namespace POC.Module.Email.Views
{
    public partial class UEmail : UserControl, IModuleRibbon
    {
        public UEmail()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
            };
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return mainRibbon;
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
            get { return ServiceLocator.Current.GetInstance<IPOCMainWindow>().GetWindow(); }
        }
        public GridControl DynamicGridControl { get { return grid; } }
        public WebBrowser DynamicWebBrowser { get { return wb; } }
    }
}
