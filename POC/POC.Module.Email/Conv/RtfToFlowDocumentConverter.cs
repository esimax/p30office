using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace POC.Module.Email.Conv
{
    public class RtfToFlowDocumentConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var document = new FlowDocument();
          var  range = new TextRange(document .ContentStart, document .ContentEnd);
          var stream = GetStream(value); 
          range .Load(stream, DataFormats.Rtf);
          return document ;
        }

        private MemoryStream GetStream(object value)
        {
            var stream = new MemoryStream(Encoding.Unicode.GetBytes(value.ToString()));
            return stream;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
