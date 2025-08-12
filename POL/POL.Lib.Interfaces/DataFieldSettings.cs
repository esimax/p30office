namespace POL.Lib.Interfaces
{
    public class DataFieldSettings
    {
        public DataFieldSettings()
        {
            BoolSettings = new DataFieldBoolSettings();
            CitySettings = new DataFieldCitySettings();
            ColorSettings = new DataFieldColorSettings();
            ContactSettings = new DataFieldContactSettings();
            CountrySettings = new DataFieldCountrySettings();
            DateSettings = new DataFieldDateSettings();
            DateTimeSettings = new DataFieldDateTimeSettings();
            DoubleSettings = new DataFieldDoubleSettings();
            FileSettings = new DataFieldFileSettings();
            ImageSettings = new DataFieldImageSettings();
            LocationSettings = new DataFieldLocationSettings();
            MemoSettings = new DataFieldMemoSettings();
            StringSettings = new DataFieldStringSettings();
            CheckListSettings = new DataFieldCheckListSettings();
            ComboSettings = new DataFieldComboSettings();
            TimeSettings = new DataFieldTimeSettings();
        }

        public DataFieldBoolSettings BoolSettings { get; set; }
        public DataFieldCitySettings CitySettings { get; set; }
        public DataFieldColorSettings ColorSettings { get; set; }
        public DataFieldContactSettings ContactSettings { get; set; }
        public DataFieldCountrySettings CountrySettings { get; set; }

        public DataFieldDateSettings DateSettings { get; set; }
        public DataFieldDateTimeSettings DateTimeSettings { get; set; }
        public DataFieldDoubleSettings DoubleSettings { get; set; }
        public DataFieldFileSettings FileSettings { get; set; }
        public DataFieldImageSettings ImageSettings { get; set; }
        public DataFieldLocationSettings LocationSettings { get; set; }
        public DataFieldMemoSettings MemoSettings { get; set; }
        public DataFieldStringSettings StringSettings { get; set; }
        public DataFieldCheckListSettings CheckListSettings { get; set; }
        public DataFieldComboSettings ComboSettings { get; set; }
        public DataFieldTimeSettings TimeSettings { get; set; }
    }
}
