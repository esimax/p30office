using System.Collections.Generic;
using System.Linq;
using System.Windows;
using POL.Lib.Interfaces;

namespace POC.Shell.Adapters
{
    internal class AdapterPOCSettings : IPOCSettings
    {
        #region CTOR
        public AdapterPOCSettings()
        {
            Holder = new Dictionary<string, POCSettingItem>();
        }
        #endregion

        private Dictionary<string, POCSettingItem> Holder { get; set; }

        #region IPOCSettings
        public void RegisterUIElement(string key, POCSettingItem item)
        {
            if (Holder.ContainsKey(key)) return;
            Holder.Add(key, item);
        }
        public List<UIElement> GetList()
        {
            var q = from n in Holder orderby n.Value.Order select n.Value.Element;
            return q.Any() ? q.ToList() : null;
        }
        #endregion
    }
}
