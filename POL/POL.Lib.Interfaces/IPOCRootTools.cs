using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IPOCRootTools
    {
        void RegisterRootTool(POCRootToolItem item);
        List<POCRootToolItem> GetList();
    }
}
