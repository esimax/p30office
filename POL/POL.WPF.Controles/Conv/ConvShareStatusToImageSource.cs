using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.WPF.Controles.Conv
{
    public class ConvShareStatusToImageSource : IValueConverter
    {
        private static object ImageSpace;
        private static object ImageShare;
        private static object ImageUser;

        static ConvShareStatusToImageSource()
        {
            SelfStatic = new ConvShareStatusToImageSource();
        }

        public static ConvShareStatusToImageSource SelfStatic { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ImageShare == null)
                ImageShare = HelperImage.GetStandardImage16("_16_Share.png");

            if (ImageSpace == null)
                ImageSpace = HelperImage.GetStandardImage16("_16_Space.png");

            if (ImageUser == null)
                ImageUser = HelperImage.GetStandardImage16("_16_User.png");


            if (value is EnumShareStatus)
            {
                var it = (EnumShareStatus) value;
                switch (it)
                {

                    case EnumShareStatus.SharedForOthers:
                        return ImageShare;
                    case EnumShareStatus.OthersSharedThis:
                        return ImageUser;
                }
                return ImageSpace;
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
