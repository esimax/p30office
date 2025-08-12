using DevExpress.Xpf.Core;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace POC.Module.Profile.Views
{
    public partial class UProfileContactModule : UserControl,IModuleRibbon
    {
        public UProfileContactModule()
        {
            InitializeComponent();
        }

        #region IModuleRibbon
        public object GetRibbon()
        {
            return profileRibbon;
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
        public DXTabControl DynamicTabControl
        {
            get { return tcMain; }
        }
        
    }
}
