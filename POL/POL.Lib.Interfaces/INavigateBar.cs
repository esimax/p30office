using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface INavigationBar
    {
        List<NavigateBarItem> Items { get; }
        void RegisterNavigateBar(NavigateBarItem item);
    }
}
