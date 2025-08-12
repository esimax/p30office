using System.Collections;
using DevExpress.Xpf.Grid;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Models
{
    public class ProfileItemSelector : IChildNodesSelector
    {
        IEnumerable IChildNodesSelector.SelectChildren(object item)
        {
            if (item is CacheItemProfileItem)
                return (item as CacheItemProfileItem).ChildList;

            return null;
        }
    }
}
