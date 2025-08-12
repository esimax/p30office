using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IPOCEventUnit
    {
        void Register(POCEventItem item);
        List<POCEventItem> GetList();

        void InvokeByKey(string key,params object[] args);
    }
}
