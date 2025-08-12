using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Shell.Adapters
{
    internal class AdapterPOCFastContactUnit : IPOCFastContactUnit
    {
        #region CTOR
        public AdapterPOCFastContactUnit()
        {
            Holder = new Dictionary<string, FastContactUnitItem>();
        }
        #endregion

        private Dictionary<string, FastContactUnitItem> Holder { get; set; }

        #region IPOCFastContactUnit
        public List<FastContactUnitItem> GetList()
        {
            var aMembership = ServiceLocator.Current.GetInstance<IMembership>();
            var q = from n in Holder where aMembership.HasPermission(n.Value.Permission) orderby n.Value.Order select n.Value;
            return q.Any() ? q.ToList() : new List<FastContactUnitItem>();
        }

        public void Register(FastContactUnitItem item)
        {
            if (item == null) return;
            if (Holder.ContainsKey(item.Key)) return;
            Holder.Add(item.Key, item);
        }
        #endregion
    }
}
