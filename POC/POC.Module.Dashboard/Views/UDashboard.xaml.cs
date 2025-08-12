using System;
using System.Windows;
using System.Windows.Controls;
using POL.Lib.Interfaces;

namespace POC.Module.Dashboard.Views
{
    public partial class UDashboard : UserControl, IModuleRibbon
    {
        public UDashboard()
        {
            InitializeComponent();
        }

        public object GetRibbon()
        {
            return null;
        }

        public void LoadChildRibbons()
        {
        }
        public void UnloadChildRibbons()
        {
        }

        public void AddUnit(string tabName, DashboardUnitItem item)
        {
            switch (tabName)
            {
                case POL.Lib.XOffice.ConstantDashboardTabs.NameContact:
                    {
                        var u = Activator.CreateInstance(item.ContentType) as UIElement;
                        if (u != null)
                            spContact.Children.Add(u);
                    }
                    break;
                case POL.Lib.XOffice.ConstantDashboardTabs.NameCall:
                    {
                        var u = Activator.CreateInstance(item.ContentType) as UIElement;
                        if (u != null)
                            spCall.Children.Add(u);
                    }
                    break;
            }
        }
    }
}
