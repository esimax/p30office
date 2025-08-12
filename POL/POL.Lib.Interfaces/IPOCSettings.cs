using System.Collections.Generic;
using System.Windows;

namespace POL.Lib.Interfaces
{
    public interface IPOCSettings
    {
        void RegisterUIElement(string key, POCSettingItem item);
        List<UIElement> GetList();
    }
}
