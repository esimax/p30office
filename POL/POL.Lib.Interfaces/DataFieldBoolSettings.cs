namespace POL.Lib.Interfaces
{
    public class DataFieldBoolSettings
    {
        public DataFieldBoolSettings()
        {
            ReturnForFalse = "0";
            ReturnForNull = "-";
            ReturnForTrue = "1";
        }

        public string ReturnForTrue { get; set; }
        public string ReturnForFalse { get; set; }
        public string ReturnForNull { get; set; }
    }

    public class DataFieldCitySettings
    {
        public DataFieldCitySettings()
        {
            Format = "{0} -> {1}";
        }

        public string Format { get; set; }
    }

    public class DataFieldColorSettings
    {
        public DataFieldColorSettings()
        {
            Format = "#{0:x2}{1:x2}{2:x2}{3:x2}";
        }

        public string Format { get; set; }
    }

    public class DataFieldContactSettings
    {
        public DataFieldContactSettings()
        {
            Format = "{1}";
        }

        public string Format { get; set; }
    }

    public class DataFieldCountrySettings
    {
        public DataFieldCountrySettings()
        {
            Format = "{1}";
        }

        public string Format { get; set; }
    }

    public class DataFieldDateSettings
    {
        public DataFieldDateSettings()
        {
            CalenadrType = EnumCalendarType.ApplicationSettings;
            NullValue = "??";
            Format = "dd MMMM yy";
        }

        public EnumCalendarType CalenadrType { get; set; }

        public string Format { get; set; }

        public string NullValue { get; set; }
    }

    public class DataFieldDateTimeSettings
    {
        public DataFieldDateTimeSettings()
        {
            CalenadrType = EnumCalendarType.ApplicationSettings;
            NullValue = "??";
            Format = "dd MMMM yy - HH:mm";
        }

        public EnumCalendarType CalenadrType { get; set; }

        public string Format { get; set; }

        public string NullValue { get; set; }
    }

    public class DataFieldDoubleSettings
    {
    }

    public class DataFieldFileSettings
    {
        public DataFieldFileSettings()
        {
            Format = "{0}";
        }

        public string Format { get; set; }
    }

    public class DataFieldImageSettings
    {
        public DataFieldImageSettings()
        {
            Format = "{2}";
        }

        public string Format { get; set; }
    }

    public class DataFieldLocationSettings
    {
        public DataFieldLocationSettings()
        {
            Format = "{0} , {1}";
        }

        public string Format { get; set; }
    }

    public class DataFieldMemoSettings
    {
        public DataFieldMemoSettings()
        {
            DirDefValue = string.Empty;
            DirRTLValue = string.Empty;
            DirLTRValue = string.Empty;
            Format = "{0}";
        }

        public string Format { get; set; }

        public string DirDefValue { get; set; }
        public string DirRTLValue { get; set; }
        public string DirLTRValue { get; set; }
    }

    public class DataFieldStringSettings
    {
        public DataFieldStringSettings()
        {
            DirDefValue = string.Empty;
            DirRTLValue = string.Empty;
            DirLTRValue = string.Empty;
            Format = "{0}";
        }

        public string Format { get; set; }

        public string DirDefValue { get; set; }
        public string DirRTLValue { get; set; }
        public string DirLTRValue { get; set; }
    }

    public class DataFieldCheckListSettings
    {
        public DataFieldCheckListSettings()
        {
            Prefix = string.Empty;
            Postfix = string.Empty;
            Seprator = " , ";
        }

        public string Seprator { get; set; }
        public string Prefix { get; set; }
        public string Postfix { get; set; }
    }

    public class DataFieldComboSettings
    {
        public DataFieldComboSettings()
        {
            Format = "{0} , {1}";
        }

        public string Format { get; set; }
    }

    public class DataFieldTimeSettings
    {
        public DataFieldTimeSettings()
        {
            Format = "{0}:{1}";
        }

        public string Format { get; set; }
    }
}
