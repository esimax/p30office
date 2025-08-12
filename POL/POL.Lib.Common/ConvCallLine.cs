using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;

namespace POL.Lib.Common
{
    public class ConvCallLine : IValueConverter
    {
        public ConvCallLine()
        {
            try
            {
                DisplayMode = HelperSettingsClient.CallLineDisplayMode;
            }
            catch
            {
            }
        }

        private POCCore APOCCore { get; set; }

        private EnumCallLineDisplayMode DisplayMode { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            try
            {
                switch (DisplayMode)
                {
                    case EnumCallLineDisplayMode.Code:
                        return value;
                    case EnumCallLineDisplayMode.Name:
                        if (APOCCore == null)
                        {
                            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
                        }
                        if (APOCCore != null)
                        {
                            if (APOCCore.LineNames.ContainsKey((int) value))
                            {
                                return APOCCore.LineNames[(int) value];
                            }
                        }
                        return value;
                    case EnumCallLineDisplayMode.Both:
                        if (APOCCore == null)
                        {
                            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
                        }
                        if (APOCCore != null)
                        {
                            if (APOCCore.LineNames.ContainsKey((int) value))
                            {
                                return string.Format("{0} - {1}", value, APOCCore.LineNames[(int) value]);
                            }
                        }
                        return value;
                }
            }
            catch
            {
                return "err";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
