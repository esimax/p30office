using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.WPF.Controles.Conv
{
    public class ConvPhoneTypeToImage : IValueConverter
    {
        private static object ImagePhoneFax;
        private static object ImageMobile;
        private static object ImageSMS;

        static ConvPhoneTypeToImage()
        {
            SelfStatic = new ConvPhoneTypeToImage();
        }

        public static ConvPhoneTypeToImage SelfStatic { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ImagePhoneFax == null)
                ImagePhoneFax = HelperImage.GetStandardImage16("_16_Phone2.png");

            if (ImageMobile == null)
                ImageMobile = HelperImage.GetStandardImage16("_16_Mobile.png");

            if (ImageSMS == null)
                ImageSMS = HelperImage.GetStandardImage16("_16_SMS.png");


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
