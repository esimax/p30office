using System;
using System.Linq;
using System.Windows.Data;
using DevExpress.Xpf.Grid;
using POL.DB.P30Office;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Models
{
    public class ConvDraggingRowToTypeName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is System.Collections.IList)) return null;
            
            var list = (from n in (value as System.Collections.IList).Cast<TreeListNode>() select n.Content as CacheItemProfileItem).ToList();
            if (list[0].Tag is DBCTProfileItem) return "فیلد";
            if (list[0].Tag is DBCTProfileGroup) return "گروه";
            if (list[0].Tag is DBCTProfileRoot) return "فرم";
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
