using System;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using POL.DB.Root;

namespace POL.DB.General
{
    [OptimisticLocking(false)]
    public class DBGLCountry : XPGUIDObject
    {
        #region Design

        public DBGLCountry()
        {
        }

        public DBGLCountry(Session session)
            : base(session)
        {
        }

        protected DBGLCountry(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        #endregion

        #region Code_ID

        private int _Code_ID;

        public int Code_ID
        {
            get { return _Code_ID; }
            set { SetPropertyValue("Code_ID", ref _Code_ID, value); }
        }

        #endregion

        #region Code_FIPS104

        private string _Code_FIPS104;

        [Size(4)]
        public string Code_FIPS104
        {
            get { return _Code_FIPS104; }
            set { SetPropertyValue("Code_FIPS104", ref _Code_FIPS104, value); }
        }

        #endregion

        #region ISO2

        private string _ISO2;

        [Size(4)]
        public string ISO2
        {
            get { return _ISO2; }
            set { SetPropertyValue("ISO2", ref _ISO2, value); }
        }

        #endregion

        #region ISO3

        private string _ISO3;

        [Size(4)]
        public string ISO3
        {
            get { return _ISO3; }
            set { SetPropertyValue("ISO3", ref _ISO3, value); }
        }

        #endregion

        #region ISON

        private int _ISON;

        public int ISON
        {
            get { return _ISON; }
            set { SetPropertyValue("ISON", ref _ISON, value); }
        }

        #endregion

        #region Internet

        private string _Internet;

        [Size(4)]
        public string Internet
        {
            get { return _Internet; }
            set { SetPropertyValue("Internet", ref _Internet, value); }
        }

        #endregion

        #region CurrencyName

        private string _CurrencyName;

        [Size(32), PersianString]
        public string CurrencyName
        {
            get { return _CurrencyName; }
            set { SetPropertyValue("CurrencyName", ref _CurrencyName, value); }
        }

        #endregion

        #region CurrencyCode

        private string _CurrencyCode;

        [Size(4)]
        public string CurrencyCode
        {
            get { return _CurrencyCode; }
            set { SetPropertyValue("CurrencyCode", ref _CurrencyCode, value); }
        }

        #endregion

        #region CurrencySymbol

        private string _CurrencySymbol;

        [Size(16), PersianString]
        public string CurrencySymbol
        {
            get { return _CurrencySymbol; }
            set { SetPropertyValue("CurrencySymbol", ref _CurrencySymbol, value); }
        }

        #endregion

        #region TeleCode1

        private int _TeleCode1;

        public int TeleCode1
        {
            get { return _TeleCode1; }
            set { SetPropertyValue("TeleCode1", ref _TeleCode1, value); }
        }

        #endregion

        #region TeleCode2

        private int _TeleCode2;

        public int TeleCode2
        {
            get { return _TeleCode2; }
            set { SetPropertyValue("TeleCode2", ref _TeleCode2, value); }
        }

        #endregion

        #region TeleCode

        private int _TeleCode;

        public int TeleCode
        {
            get { return _TeleCode; }
            set { SetPropertyValue("TeleCode", ref _TeleCode, value); }
        }

        #endregion

        #region TitleEn

        private string _TitleEn;

        [Size(128)]
        public string TitleEn
        {
            get { return _TitleEn; }
            set { SetPropertyValue("TitleEn", ref _TitleEn, value); }
        }

        #endregion

        #region TitleXX

        private string _TitleXX;

        [Size(256), PersianString]
        public string TitleXX
        {
            get { return _TitleXX; }
            set { SetPropertyValue("TitleXX", ref _TitleXX, value); }
        }

        #endregion

        #region FlagImage

        [Delayed]
        public byte[] FlagImage
        {
            get { return GetDelayedPropertyValue<Byte[]>("FlagImage"); }
            set { SetDelayedPropertyValue("FlagImage", value); }
        }

        #endregion

        #region MapImage

        [Delayed]
        public byte[] MapImage
        {
            get { return GetDelayedPropertyValue<Byte[]>("MapImage"); }
            set { SetDelayedPropertyValue("MapImage", value); }
        }

        #endregion

        #region TimeZone

        private long _TimeZone;

        public long TimeZone
        {
            get { return _TimeZone; }
            set { SetPropertyValue("TimeZone", ref _TimeZone, value); }
        }

        #endregion

        #region TimeZoneString

        [NonPersistent]
        public string TimeZoneString
        {
            get
            {
                return _TimeZone >= 0
                           ? "+" + TimeSpan.FromTicks(_TimeZone).ToString(@"hh\:mm")
                           : "-" + TimeSpan.FromTicks(_TimeZone).ToString(@"hh\:mm");
            }
            set
            {
                try
                {
                    string s = value.Trim('+');
                    TimeSpan ts = TimeSpan.Parse(s);
                    TimeZone = ts.Ticks;
                }
                catch
                {
                    _TimeZone = 0;
                }
            }
        }

        #endregion

        #region TeleCodeString

        [NonPersistent]
        public string TeleCodeString
        {
            get
            {
                return string.Format("+({0}{1})", TeleCode1 > 0 ? TeleCode1.ToString(CultureInfo.InvariantCulture) : "",
                                     TeleCode2 > 0 ? TeleCode2.ToString(CultureInfo.InvariantCulture) : "");
            }
        }

        #endregion

        #region Link - Cities

        [Association("GLCountry-GLCities"), Aggregated]
        public XPCollection<DBGLCity> Cities
        {
            get
            {
                XPCollection<DBGLCity> xpc = GetCollection<DBGLCity>("Cities");
                return xpc;
            }
        }
        protected override XPCollection<T> CreateCollection<T>(DevExpress.Xpo.Metadata.XPMemberInfo property)
        {
            XPCollection<T> collection = base.CreateCollection<T>(property);
            if (property.Name == "Cities")
            {
                collection.Sorting = new SortingCollection(new SortProperty("TitleXX", SortingDirection.Ascending));
            }
            return collection;
        }
        #endregion

        protected override void OnSaving()
        {
            base.OnSaving();
            try
            {
                TeleCode = TeleCode2 > 0 ? Convert.ToInt32(string.Format("{0}{1}", TeleCode1, TeleCode2)) : TeleCode1;
            }
            catch
            {
            }
        }


        public static XPCollection<DBGLCountry> GetAll(Session dxs)
        {
            return new XPCollection<DBGLCountry>(dxs, null, new SortProperty("TitleXX", SortingDirection.Ascending));
        }

        public static XPCollection<DBGLCountry> GetAllWithTeleCode(Session dxs)
        {
            return new XPCollection<DBGLCountry>(dxs,
                                                 new BinaryOperator("TeleCode1", 1, BinaryOperatorType.GreaterOrEqual),
                                                 new SortProperty("TitleXX", SortingDirection.Ascending));
        }


        public static DBGLCountry FindByTeleCode(Session dxs, int telecode)
        {
            var rv = dxs.FindObject<DBGLCountry>(new BinaryOperator("TeleCode", telecode));
            return rv;
        }


        public static DBGLCountry FindByOid(Session session, Guid guid)
        {
            return session.FindObject<DBGLCountry>(new BinaryOperator("Oid", guid));
        }

        public static DBGLCountry FindByISO3(Session dxs, string iso3)
        {
            return dxs.FindObject<DBGLCountry>(new BinaryOperator("ISO3", iso3));
        }

        public static DBGLCountry FindByTitleXX(Session dxs, string titleXX)
        {
            return dxs.FindObject<DBGLCountry>(new BinaryOperator("TitleXX", titleXX));
        }
    }
}
