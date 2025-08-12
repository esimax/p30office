using System.Collections.Generic;
using System.Linq;
using POL.Lib.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace POC.Shell.Adapters
{
    internal class AdapterPOCRootTools : IPOCRootTools
    {
        #region CTOR
        public AdapterPOCRootTools()
        {
            Holder = new Dictionary<string, POCRootToolItem>();
        }
        #endregion

        private Dictionary<string, POCRootToolItem> Holder { get; set; }

        #region IPOCRootTools
        public List<POCRootToolItem> GetList()
        {
            var aMembership = ServiceLocator.Current.GetInstance<IMembership>();
            var q = from n in Holder where n.Value!=null && aMembership.HasPermission(n.Value.Permission) orderby n.Value.Order select n.Value;
            return q.Any() ? q.ToList() : new List<POCRootToolItem>();
        }

        public void RegisterRootTool( POCRootToolItem item)
        {
            if (item == null) return;
            if (Holder.ContainsKey(item.Key)) return;
            Holder.Add(item.Key, item);
        }
        #endregion
    }
}
