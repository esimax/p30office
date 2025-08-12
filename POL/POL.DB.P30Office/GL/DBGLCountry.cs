using System;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.GL
{
    [OptimisticLocking(false)]
    public class DBGLCountry : XPGUIDLogableObject 
    {
        #region CTOR

        public DBGLCountry(Session session)
            : base(session)
        {
        }

        #endregion

        #region FlagImage

        [Delayed]
        [DisplayName("پرچم")]
        public byte[] FlagImage
        {
            get { return GetDelayedPropertyValue<byte[]>("FlagImage"); }
            set { SetDelayedPropertyValue("FlagImage", value); }
        }

        #endregion

        #region MapImage

        [Delayed]
        [DisplayName("نقشه")]
        public byte[] MapImage
        {
            get { return GetDelayedPropertyValue<byte[]>("MapImage"); }
            set { SetDelayedPropertyValue("MapImage", value); }
        }

        #endregion

        #region TimeZoneString

        [NonPersistent]
        [DisplayName("نام منطقه زمانی")]
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
                    var s = value.Trim('+');
                    var ts = TimeSpan.Parse(s);
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
        [DisplayName("متن كامل كد مخابراتی")]
        public string TeleCodeString
        {
            get
            {
                return string.Format("+({0}{1})", TeleCode1 > 0 ? TeleCode1.ToString(CultureInfo.InvariantCulture) : "",
                    TeleCode2 > 0 ? TeleCode2.ToString(CultureInfo.InvariantCulture) : "");
            }
        }

        #endregion

        public override string ToString()
        {
            return TitleXX;
        }

        protected override void OnSaving()
        {
            HelperUtils.Try(
                () =>
                {
                    TeleCode = TeleCode2 > 0 ? Convert.ToInt32(string.Format("{0}{1}", TeleCode1, TeleCode2)) : TeleCode1;
                });
            base.OnSaving();
        }

        protected override XPCollection<T> CreateCollection<T>(XPMemberInfo property)
        {
            var collection = base.CreateCollection<T>(property);
            if (property.Name == "Cities")
            {
                collection.Sorting = new SortingCollection(new SortProperty("TitleXX", SortingDirection.Ascending));
            }
            return collection;
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

        public static DBGLCountry FindDuplicateTitleXXExcept(Session session, DBGLCountry exceptContact, string titleXX)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("TitleXX", HelperConvert.CorrectPersianBug(titleXX),
                BinaryOperatorType.Equal));
            return session.FindObject<DBGLCountry>(go);
        }

        #region Code_ID

        private int _Code_ID;

        [DisplayName("كد كشور")]
        public int Code_ID
        {
            get { return _Code_ID; }
            set { SetPropertyValue("Code_ID", ref _Code_ID, value); }
        }

        #endregion

        [DisplayName("كد استاندارد FPIS104")]

        #region Code_FIPS104

            private string _Code_FIPS104;

        [Size(4)]
        public string Code_FIPS104
        {
            get { return _Code_FIPS104; }
            set { SetPropertyValue("Code_FIPS104", ref _Code_FIPS104, value); }
        }

        #endregion

        [DisplayName("كد استاندارد ISO2")]

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
        [DisplayName("كد استاندارد ISO3")]
        public string ISO3
        {
            get { return _ISO3; }
            set { SetPropertyValue("ISO3", ref _ISO3, value); }
        }

        #endregion

        #region ISON

        [DisplayName("كد استاندارد ایزو")] private int _ISON;

        public int ISON
        {
            get { return _ISON; }
            set { SetPropertyValue("ISON", ref _ISON, value); }
        }

        #endregion

        #region Internet

        private string _Internet;

        [Size(4)]
        [DisplayName("كد استاندارد اینترنت")]
        public string Internet
        {
            get { return _Internet; }
            set { SetPropertyValue("Internet", ref _Internet, value); }
        }

        #endregion

        #region CurrencyName

        private string _CurrencyName;

        [DisplayName("نام واحد مالی")]
        [Size(32), PersianString]
        public string CurrencyName
        {
            get { return _CurrencyName; }
            set { SetPropertyValue("CurrencyName", ref _CurrencyName, value); }
        }

        #endregion

        #region CurrencyCode

        private string _CurrencyCode;

        [DisplayName("كد واحد مالی")]
        [Size(4)]
        public string CurrencyCode
        {
            get { return _CurrencyCode; }
            set { SetPropertyValue("CurrencyCode", ref _CurrencyCode, value); }
        }

        #endregion

        #region CurrencySymbol

        private string _CurrencySymbol;

        [DisplayName("علامت واحد مالی")]
        [Size(16), PersianString]
        public string CurrencySymbol
        {
            get { return _CurrencySymbol; }
            set { SetPropertyValue("CurrencySymbol", ref _CurrencySymbol, value); }
        }

        #endregion

        #region TeleCode1

        private int _TeleCode1;

        [DisplayName("كد مخابراتی اولیه")]
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

        [DisplayName("كد مخابرات ثانویه")]
        public int TeleCode
        {
            get { return _TeleCode; }
            set { SetPropertyValue("TeleCode", ref _TeleCode, value); }
        }

        #endregion

        #region TitleEn

        private string _TitleEn;

        [DisplayName("عنوان لاتین")]
        [Size(128)]
        public string TitleEn
        {
            get { return _TitleEn; }
            set { SetPropertyValue("TitleEn", ref _TitleEn, value); }
        }

        #endregion

        #region TitleXX

        private string _TitleXX;

        [DisplayName("عنوان فارسی")]
        [Size(256), PersianString]
        public string TitleXX
        {
            get { return _TitleXX; }
            set { SetPropertyValue("TitleXX", ref _TitleXX, value); }
        }

        #endregion

        #region TimeZone

        private long _TimeZone;

        [DisplayName("منطقه زمانی")]
        public long TimeZone
        {
            get { return _TimeZone; }
            set { SetPropertyValue("TimeZone", ref _TimeZone, value); }
        }

        #endregion

        #region Link - Cities


        #endregion
    }
}
