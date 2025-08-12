using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IPOCDashboardUnit
    {
        void Register(DashboardUnitItem item);
        List<DashboardUnitItem> GetList(string tabName);
    }
}
