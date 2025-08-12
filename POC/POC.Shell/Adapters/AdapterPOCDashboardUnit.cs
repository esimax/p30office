using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Shell.Adapters
{
    internal class AdapterPOCDashboardUnit : IPOCDashboardUnit
    {
        #region CTOR
        public AdapterPOCDashboardUnit()
        {
            Holder = new Dictionary<string, DashboardUnitItem>();
        }
        #endregion

        private Dictionary<string, DashboardUnitItem> Holder { get; set; }

        #region IPOCDashboardUnit
        public List<DashboardUnitItem> GetList(string tabName)
        {
            var aMembership = ServiceLocator.Current.GetInstance<IMembership>();
            var q = from n in Holder where aMembership.HasPermission(n.Value.Permission) orderby n.Value.Order where n.Value.TabName == tabName select n.Value;
            return q.Any() ? q.ToList() : null;
        }

        public void Register(DashboardUnitItem item)
        {
            if (item == null) return;
            if (Holder.ContainsKey(item.Key)) return;
            Holder.Add(item.Key, item);
        }
        #endregion
    }
}
