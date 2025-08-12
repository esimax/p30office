using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using POL.DB.Root;

namespace POL.DB.General
{
    public class DBGLPhoneBook : XPGUIDObject
    {
        #region Design

        public DBGLPhoneBook()
        {
        }

        public DBGLPhoneBook(Session session)
            : base(session)
        {
        }

        protected DBGLPhoneBook(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        #endregion

        #region Country

        private DBGLCountry _Country;

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

        #region PhoneNumber

        private string _PhoneNumber;

        [Size(16)]
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { SetPropertyValue("PhoneNumber", ref _PhoneNumber, value); }
        }

        #endregion

        #region InternalNumber

        private string _InternalNumber;

        [Size(128)]
        public string InternalNumber
        {
            get { return _InternalNumber; }
            set { SetPropertyValue("InternalNumber", ref _InternalNumber, value); }
        }

        #endregion

        #region Title

        private string _Title;

        [Size(32)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        [NonPersistent]
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
    }
}
