using System;
using System.Windows.Media.Imaging;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.Lib.Common
{
    public class HelperP30office
    {
        public static BitmapSource GetProfileItemImage(EnumProfileItemType pit)
        {
            switch (pit)
            {
                case EnumProfileItemType.Bool:
                    return imgBool ?? (imgBool = HelperImage.GetStandardImage16("_16_UICheckBox.png"));
                case EnumProfileItemType.Double:
                    return imgDouble ?? (imgDouble = HelperImage.GetSpecialImage16("_16_UIDigit.png"));
                case EnumProfileItemType.Country:
                    return imgCountry ?? (imgCountry = HelperImage.GetSpecialImage16("_16_UICountry.png"));
                case EnumProfileItemType.City:
                    return imgCity ?? (imgCity = HelperImage.GetSpecialImage16("_16_UICity.png"));
                case EnumProfileItemType.Location:
                    return imgLocation ?? (imgLocation = HelperImage.GetSpecialImage16("_16_UILocation.png"));
                case EnumProfileItemType.String:
                    return imgString ?? (imgString = HelperImage.GetSpecialImage16("_16_UIString.png"));
                case EnumProfileItemType.Memo:
                    return imgString ?? (imgString = HelperImage.GetSpecialImage16("_16_UIString.png"));
                case EnumProfileItemType.StringCombo:
                    return imgCombo ?? (imgCombo = HelperImage.GetSpecialImage16("_16_UICombo.png"));
                case EnumProfileItemType.StringCheckList:
                    return imgCheckList ?? (imgCheckList = HelperImage.GetSpecialImage16("_16_UICheckList.png"));
                case EnumProfileItemType.Color:
                    return imgColor ?? (imgColor = HelperImage.GetSpecialImage16("_16_UIColor.png"));
                case EnumProfileItemType.File:
                    return imgFile ?? (imgFile = HelperImage.GetSpecialImage16("_16_UIFile.png"));
                case EnumProfileItemType.Image:
                    return imgImage ?? (imgImage = HelperImage.GetStandardImage16("_16_UIImageEdit.png"));
                case EnumProfileItemType.Date:
                    return imgDate ?? (imgDate = HelperImage.GetStandardImage16("_16_UICalendar.png"));
                case EnumProfileItemType.Time:
                    return imgTime ?? (imgTime = HelperImage.GetSpecialImage16("_16_UITime.png"));
                case EnumProfileItemType.DateTime:
                    return imgDate ?? (imgDate = HelperImage.GetSpecialImage16("_16_UIDateTime.png"));
                case EnumProfileItemType.Contact:
                    return imgContact ?? (imgContact = HelperImage.GetSpecialImage16("_16_UIContact.png"));
                case EnumProfileItemType.List:
                    return imgList ?? (imgList = HelperImage.GetSpecialImage16("_16_UIList.png"));
                default:
                    throw new ArgumentOutOfRangeException("pit");
            }
        }

        #region Internal Properties

        internal static BitmapSource imgBool;
        internal static BitmapSource imgDouble;
        internal static BitmapSource imgCountry;
        internal static BitmapSource imgCity;
        internal static BitmapSource imgLocation;
        internal static BitmapSource imgCombo;
        internal static BitmapSource imgString;
        internal static BitmapSource imgCheckList;
        internal static BitmapSource imgColor;
        internal static BitmapSource imgFile;
        internal static BitmapSource imgImage;
        internal static BitmapSource imgDate;
        internal static BitmapSource imgTime;
        internal static BitmapSource imgContact;
        internal static BitmapSource imgList;

        #endregion
    }
}
