using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.WPF.Controles.Conv
{
    public class ConvPhoneTypeToImage32 : IValueConverter
    {
        private static object ImagePhoneFax;
        private static object ImageMobile;
        private static object ImageSMS;

        static ConvPhoneTypeToImage32()
        {
            SelfStatic = new ConvPhoneTypeToImage32();
        }

        public static ConvPhoneTypeToImage32 SelfStatic { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ImagePhoneFax == null)
                ImagePhoneFax = HelperImage.GetStandardImage32("_32_Phone2.png");

            if (ImageMobile == null)
                ImageMobile = HelperImage.GetStandardImage32("_32_Mobile.png");

            if (ImageSMS == null)
                ImageSMS = HelperImage.GetStandardImage32("_32_SMS.png");


            if (value is EnumPhoneType)
            {
                var it = (EnumPhoneType) value;
                switch (it)
                {
                    case EnumPhoneType.PhoneFax:
                        return ImagePhoneFax;
                    case EnumPhoneType.Mobile:
                        return ImageMobile;
                }
                return ImageSMS;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
