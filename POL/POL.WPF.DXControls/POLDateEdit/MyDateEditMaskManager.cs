using System.Globalization;
using DevExpress.Data.Mask;

namespace POL.WPF.DXControls.POLDateEdit
{
    internal class MyDateEditMaskManager : DateTimeMaskManager
    {
        public MyDateEditMaskManager(string mask, bool isOperatorMask, CultureInfo culture)
            : this(mask, isOperatorMask, culture, true)
        {
        }

        public MyDateEditMaskManager(string mask, bool isOperatorMask, CultureInfo culture, bool allowNull)
            : base(mask, isOperatorMask, culture, allowNull)
        {
        }

        public new static DateTimeFormatInfo GetGoodCalendarDateTimeFormatInfo(CultureInfo inputCulture)
        {
            if (IsGoodCalendar(inputCulture.DateTimeFormat.Calendar))
            {
                return inputCulture.DateTimeFormat;
            }
            var info = (DateTimeFormatInfo) inputCulture.DateTimeFormat.Clone();
            foreach (var calendar in inputCulture.OptionalCalendars)
            {
                if (!IsGoodCalendar(calendar)) continue;
                info.Calendar = calendar;
                return info;
            }
            return DateTimeFormatInfo.InvariantInfo;
        }

        private static bool IsGoodCalendar(Calendar calendar)
        {
            return calendar is GregorianCalendar || calendar is KoreanCalendar || calendar is TaiwanCalendar ||
                   calendar is ThaiBuddhistCalendar || calendar is PersianCalendar || calendar is HijriCalendar;
        }
    }
}
