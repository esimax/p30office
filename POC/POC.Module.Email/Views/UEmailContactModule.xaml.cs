using DevExpress.Xpf.Grid;
using Microsoft.Practices.ServiceLocation;
using System.Windows;
using System.Windows.Controls;
using POL.Lib.Interfaces;

namespace POC.Module.Email.Views
{
    public partial class UEmailContactModule : UserControl, IModuleRibbon
    {
        public UEmailContactModule()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return emailRibbon;
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
        public WebBrowser DynamicWebBrowserReceive { get { return wbReceive; } }

        public GridControl DynamicReceiveGrid
        {
            get { return receiveGrid; }
        }
        public TableView DynamicReceiveTableView
        {
            get { return receiveTableView; }
        }
    }
}
