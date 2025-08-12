using System;
using System.Globalization;
using DevExpress.Xpf.Editors;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class MyDateEditStrategy : DateEditStrategy
    {
        public MyDateEditStrategy(DateEdit editor)
            : base(editor)
        {
        }


        protected static Calendar CurrentCalendar
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.Calendar; }
        }


        public override object CoerceEditValue(object value)
        {
            if (value is string)
            {
                try
                {
                    var s = value as string;
                    value = DateTime.Parse(s, CultureInfo.InvariantCulture);
                }
                catch
                {
                }
            }
            return base.CoerceEditValue(value);
        }
    }
}
