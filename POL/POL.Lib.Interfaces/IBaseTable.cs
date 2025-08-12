using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IBaseTable
    {
        void RegisterBaseTable(BaseTableItem item);
        List<BaseTableItem> GetList();
    }
}
