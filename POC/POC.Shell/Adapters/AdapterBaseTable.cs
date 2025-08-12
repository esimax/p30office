using System.Collections.Generic;
using System.Linq;
using POL.Lib.Interfaces;

namespace POC.Shell.Adapters
{
    internal class AdapterBaseTable : IBaseTable
    {
        #region CTOR
        public AdapterBaseTable()
        {
            Holder = new Dictionary<string, BaseTableItem>();
        }
        #endregion

        private Dictionary<string, BaseTableItem> Holder { get; set; }

        #region IPOCRootTools
        public List<BaseTableItem> GetList()
        {
            var q = from n in Holder orderby n.Value.Order select n.Value;
            return q.Any() ? q.ToList() : null;
        }

        public void RegisterBaseTable(BaseTableItem item)
        {
            if(item == null)return;
            if (Holder.ContainsKey(item.Key)) return;
            Holder.Add(item.Key, item);
        }
        #endregion
    }
}
