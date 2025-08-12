using System.Collections;
using DevExpress.Xpf.Grid;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Models
{
    public class ProfileTValueChildSelector : IChildNodesSelector
    {
        IEnumerable IChildNodesSelector.SelectChildren(object item)
        {
            var v = item as CacheItemProfileTValue;
            return v != null ? v.ChildList : null;
        }
    }
}
