using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.P30Office.GL;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Interfaces.PObjects;

namespace POL.DB.P30Office
{
    public class DBCTPhoneBook : XPGUIDLogableObject 
    {
        #region CTOR

        public DBCTPhoneBook(Session session) : base(session)
        {
        }
        #endregion

        #region Override

        protected override void OnSaving()
        {
            CityCodeString = CityCode;
            CountryCodeString = CountryCode;
            TeleCode2 = CalculateTelecode2();
            base.OnSaving();
        }

        #endregion

        #region PhoneNumber

        private string _PhoneNumber;

        [Size(16)]
        [DisplayName("شماره تماس")]
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { SetPropertyValue("PhoneNumber", ref _PhoneNumber, value); }
        }

        #endregion

        #region PhoneType

        private EnumPhoneType _PhoneType;

        [DisplayName("نوع شماره")]
        public EnumPhoneType PhoneType
        {
            get { return _PhoneType; }
            set { SetPropertyValue("PhoneType", ref _PhoneType, value); }
        }

        #endregion PhoneType

        #region Country

        private DBGLCountry _Country;

        [DisplayName("كشور")]
        public DBGLCountry Country
        {
            get { return _Country; }
            set
            {
                SetPropertyValue("Country", ref _Country, value);
                if (value != null)
                    City = null;
            }
        }

        #endregion

        #region City

        private DBGLCity _City;

        [DisplayName("شهر")]
        public DBGLCity City
        {
            get { return _City; }
            set
            {
                SetPropertyValue("City", ref _City, value);
                if (value != null)
                    Country = null;
            }
        }

        #endregion

        #region Title

        private string _Title;

        [Size(32)]
        [DisplayName("عنوان شماره")]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Note

        private string _Note;

        [Size(256), PersianString]
        [DisplayName("نكته")]
        public string Note
        {
            get { return _Note; }
            set { SetPropertyValue("Note", ref _Note, value); }
        }

        #endregion

        #region CallOutCount

        private int _CallOutCount;

        [DisplayName("تعداد تماسهای ارسالی")]
        public int CallOutCount
        {
            get { return _CallOutCount; }
            set { SetPropertyValue("CallOutCount", ref _CallOutCount, value); }
        }

        #endregion

        #region CallInCount

        private int _CallInCount;

        [DisplayName("تعداد تماسهای دریافتی")]
        public int CallInCount
        {
            get { return _CallInCount; }
            set { SetPropertyValue("CallInCount", ref _CallInCount, value); }
        }

        #endregion

        #region CallTotalCount

        private int _CallTotalCount;

        [DisplayName("تعداد كل تماس ها")]
        public int CallTotalCount
        {
            get { return _CallTotalCount; }
            set { SetPropertyValue("CallTotalCount", ref _CallTotalCount, value); }
        }

        #endregion

        #region DurationOut

        private int _DurationOut;

        [DisplayName("مدت مكالمه ارسالی (ثانیه)")]
        public int DurationOut
        {
            get { return _DurationOut; }
            set { SetPropertyValue("DurationOut", ref _DurationOut, value); }
        }

        #endregion

        #region DurationIn

        private int _DurationIn;

        [DisplayName("مدت مكالمه دریافتی (ثانیه)")]
        public int DurationIn
        {
            get { return _DurationIn; }
            set { SetPropertyValue("DurationIn", ref _DurationIn, value); }
        }

        #endregion

        #region DurationTotal

        private int _DurationTotal;

        [DisplayName("مدت مكالمه (ثانیه)")]
        public int DurationTotal
        {
            get { return _DurationTotal; }
            set { SetPropertyValue("DurationTotal", ref _DurationTotal, value); }
        }

        #endregion

        #region DupCount

        private int _DupCount;

        [DisplayName("تكراری میباشد؟")]
        public int DupCount
        {
            get { return _DupCount; }
            set { SetPropertyValue("DupCount", ref _DupCount, value); }
        }

        #endregion

        #region LastCallInDate

        private DateTime? _LastCallInDate;

        [DisplayName("تاریخ آخرین تماس دریافتی")]
        public DateTime? LastCallInDate
        {
            get { return _LastCallInDate; }
            set { SetPropertyValue("LastCallInDate", ref _LastCallInDate, value); }
        }

        #endregion

        #region LastCallOutDate

        private DateTime? _LastCallOutDate;

        [DisplayName("تاریخ آخرین تماس ارسالی")]
        public DateTime? LastCallOutDate
        {
            get { return _LastCallOutDate; }
            set { SetPropertyValue("LastCallOutDate", ref _LastCallOutDate, value); }
        }

        #endregion

        #region Contact

        private DBCTContact _Contact;

        [Association("CTContact_CTPhoneBooks")]
        [DisplayName("پرونده مربوطه")]
        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
        }

        #endregion

        #region CityCodeString

        private string _CityCodeString;

        [Size(32)]
        [DisplayName("كد مخابراتی")]
        public string CityCodeString
        {
            get { return _CityCodeString; }
            set { SetPropertyValue("CityCodeString", ref _CityCodeString, value); }
        }

        #endregion

        #region CountryCodeString

        private string _CountryCodeString;

        [Size(32)]
        [DisplayName("كد مخابراتی كشور")]
        public string CountryCodeString
        {
            get { return _CountryCodeString; }
            set { SetPropertyValue("CountryCodeString", ref _CountryCodeString, value); }
        }

        #endregion

        #region TeleCode2

        private string _TeleCode2;

        [Size(36)]
        [DisplayName("كد مخابراتی")]
        public string TeleCode2
        {
            get { return _TeleCode2; }
            set { SetPropertyValue("TeleCode2", ref _TeleCode2, value); }
        }

        #endregion

        #region NonPersistent

        [NonPersistent]
        [DisplayName("كد مخابراتی")]
        public string CodeString
        {
            get
            {
                try
                {
                    if (City != null)
                        return City.TeleCodeString;
                    return Country != null ? Country.TeleCodeString : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        [DisplayName("كد مخابراتی كشور")]
        public string CountryCode
        {
            get
            {
                try
                {
                    if (City != null)
                        return City.Country != null ? City.Country.TeleCodeString : string.Empty;
                    return Country != null ? Country.TeleCodeString : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        [DisplayName("كد مخابراتی شهر")]
        public string CityCode
        {
            get
            {
                try
                {
                    return City != null ? City.PhoneCode.ToString() : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        [DisplayName("عنوان كشور/شهر")]
        public string CountryCityTitle
        {
            get
            {
                try
                {
                    return City != null
                        ? string.Format("{0} - {1}", City.TitleXX, City.Country.TitleXX)
                        : (Country == null ? string.Empty : Country.TitleXX);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        #endregion

        #region Methods

        public static DBCTPhoneBook FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTPhoneBook>(new BinaryOperator("Oid", oid));
        }

        public static DBCTPhoneBook FindByPhoneAndCodeExcept(Session session, int countrycode1, int countrycode2,
            int citycode, string phone, DBCTPhoneBook exception)
        {
            return FindByPhoneAndCodeExcept(session, countrycode1, countrycode2, citycode, phone,
                exception == null ? Guid.Empty : exception.Oid);
        }

        public static DBCTPhoneBook FindByPhoneAndCodeExcept(Session session, int countrycode1, int countrycode2,
            int citycode, string phone, Guid oid)
        {
            var go = new GroupOperator();

            if (citycode <= 0 && countrycode1 < 0 && countrycode2 < 0)
            {
                go.Operands.Add(new NullOperator("City"));
                go.Operands.Add(new NullOperator("Country"));
            }
            else if (citycode <= 0)
            {
                go.Operands.Add(new BinaryOperator("Country.TeleCode1", countrycode1));
                go.Operands.Add(new BinaryOperator("Country.TeleCode2", countrycode2));
            }
            else
            {
                go.Operands.Add(new BinaryOperator("City.PhoneCode", citycode));
                go.Operands.Add(new BinaryOperator("City.Country.TeleCode1", countrycode1));
                go.Operands.Add(new BinaryOperator("City.Country.TeleCode2", countrycode2));
            }
            go.Operands.Add(new BinaryOperator("PhoneNumber", phone));

            if (oid != Guid.Empty)
                go.Operands.Add(new BinaryOperator("Oid", oid, BinaryOperatorType.NotEqual));

            return session.FindObject<DBCTPhoneBook>(go);
        }

        public static DBCTPhoneBook FindByPhoneAndCode(Session session, int countrycode, int citycode, string phone)
        {
            var go = new GroupOperator();
            if (citycode <= 0 && countrycode <= 0)
            {
                go.Operands.Add(new NullOperator("City"));
                go.Operands.Add(new NullOperator("Country"));
            }
            else if (citycode <= 0)
            {
                go.Operands.Add(new BinaryOperator("Country.TeleCode", countrycode));
                go.Operands.Add(new NullOperator("City"));
            }
            else
            {
                go.Operands.Add(new BinaryOperator("City.PhoneCode", citycode));
                go.Operands.Add(new BinaryOperator("City.Country.TeleCode", countrycode));
            }
            go.Operands.Add(new BinaryOperator("PhoneNumber", phone));

            return session.FindObject<DBCTPhoneBook>(go);
        }


        public static DBCTPhoneBook FindByPhoneAndCityOid(Session session, Guid cityoid, string phone)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("PhoneNumber", phone));
            go.Operands.Add(new BinaryOperator("City.Oid", cityoid));
            return session.FindObject<DBCTPhoneBook>(go);
        }

        public static DBCTPhoneBook FindByPhoneAndCountryOid(Session session, Guid countryoid, string phone)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("PhoneNumber", phone));
            go.Operands.Add(new BinaryOperator("City.Country.Oid", countryoid));
            return session.FindObject<DBCTPhoneBook>(go);
        }


        public static DBCTPhoneBook FindByPhoneAndCountry(Session session, string phone, Guid countryGuid)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("Country.Oid", countryGuid));
            go.Operands.Add(new BinaryOperator("PhoneNumber", phone));
            return session.FindObject<DBCTPhoneBook>(go);
        }

        public static DBCTPhoneBook FindByPhoneAndCity(Session session, string phone, Guid cityGuid)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("City.Oid", cityGuid));
            go.Operands.Add(new BinaryOperator("PhoneNumber", phone));
            return session.FindObject<DBCTPhoneBook>(go);
        }


        public static XPCollection<DBCTPhoneBook> GetByContact(Session session, Guid contactOid)
        {
            return new XPCollection<DBCTPhoneBook>(session, new BinaryOperator("Contact.Oid", contactOid));
        }

        internal static Phone PopulateToFake(DBCTPhoneBook dbphone)
        {
            var rv = new Phone();
            rv.PhoneNumber = dbphone.PhoneNumber;
            rv.PhoneType = dbphone.PhoneType;
            rv.CountryTitle = dbphone.Country != null ? dbphone.Country.TitleXX : string.Empty;
            rv.CountryCode = dbphone.Country != null ? dbphone.Country.TeleCode.ToString() : string.Empty;
            rv.CityTitle = dbphone.City != null ? dbphone.City.TitleXX : string.Empty;
            rv.CityCode = dbphone.City != null ? dbphone.City.TeleCodeString : string.Empty;
            rv.Title = dbphone.Title;
            rv.Note = dbphone.Note;
            return rv;
        }

        private string CalculateTelecode2()
        {
            if (Country != null)
                return Country.TeleCode.ToString("D");
            if (City != null)
                return City.Country.TeleCode.ToString("D") + "," + City.PhoneCode.ToString("D");
            return string.Empty;
        }

        #endregion
    }
}
