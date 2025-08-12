using System;
using System.Globalization;
using DevExpress.Data.Mask;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class MyDateTimeYearElementEditor : DateTimeNumericRangeElementEditor
    {
        private readonly DateTimeFormatInfo _DateTimeFormatInfo;
        private readonly int _MaskLength;


        public MyDateTimeYearElementEditor(int initialYear, int maskLength, DateTimeFormatInfo dateTimeFormatInfo)
            : base(
                maskLength <= 4 ? 0 : DateTime.MinValue.Year, maskLength < 4 ? 0x63 : DateTime.MaxValue.Year,
                maskLength == 2 ? 2 : 1, maskLength > 3 ? 4 : 2)
        {
            _MaskLength = maskLength;
            _DateTimeFormatInfo = dateTimeFormatInfo;
            var num = initialYear + GetYearShift(dateTimeFormatInfo.Calendar);
            num = Math.Max(Math.Min(num, 0x270f), 1);
            num = maskLength < 4 ? num%100 : num;
            SetUntouchedValue(num);
        }


        public override int GetResult()
        {
            var result = base.GetResult();
            if ((result < DateTime.MinValue.Year) || ((_MaskLength < 4) && (result <= 0x63)) ||
                ((_MaskLength == 4) && (result <= 0x63) && (digitsEntered <= 2)))
            {
                try
                {
                    result = _DateTimeFormatInfo.Calendar.ToFourDigitYear(result);
                }
                catch
                {
                }
            }
            result -= GetYearShift(_DateTimeFormatInfo.Calendar);
            return Math.Max(Math.Min(result, _DateTimeFormatInfo.Calendar.MaxSupportedDateTime.Year),
                _DateTimeFormatInfo.Calendar.MinSupportedDateTime.Year);
        }

        private static int GetYearShift(Calendar calendar)
        {
            return 0; 
        }
    }
}
