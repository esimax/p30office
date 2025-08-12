using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IPOCFastContactUnit
    {
        void Register(FastContactUnitItem item);
        List<FastContactUnitItem> GetList();
    }
}
