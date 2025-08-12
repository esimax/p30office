using System.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Dashboard.Models
{
    public class MDashboard : NotifyObjectBase
    {
        private dynamic View { get; set; }
        private IPOCDashboardUnit APOCDashboardUnit { get; set; }

        public MDashboard(object mainView)
        {
            APOCDashboardUnit = ServiceLocator.Current.GetInstance<IPOCDashboardUnit>();
            View = mainView as UserControl;
            PopulateDashboardUnits();
        }

        private void PopulateDashboardUnits()
        {
            var q = APOCDashboardUnit.GetList(ConstantDashboardTabs.NameContact);
            if (q != null) q.ForEach(n => View.AddUnit(ConstantDashboardTabs.NameContact, n));

            q = APOCDashboardUnit.GetList(ConstantDashboardTabs.NameCall);
            if (q != null)
                q.ForEach(n =>
                {
                    try
                    {
                        View.AddUnit(ConstantDashboardTabs.NameCall, n);
                    }
                    catch
                    {
                        
                    }

                });
        }
    }
}
