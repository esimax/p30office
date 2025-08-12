using System;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.GL
{
    [OptimisticLocking(false)]
    public class DBGLCity : XPGUIDLogableObject 
    {
        #region CTOR

        public DBGLCity(Session session) : base(session)
        {
        }

        #endregion

        #region TeleCodeString

        [NonPersistent]
        [DisplayName("بصورت كامل")]
        public string TeleCodeString
        {
            get
            {
                if (Country != null)
                    return
                        string.Format("{0} {1}", Country.TeleCodeString,
                            PhoneCode > 0 ? PhoneCode.ToString(CultureInfo.InvariantCulture) : "").
                            Trim();
                return "";
            }
        }

        #endregion

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(TitleXX) ? TitleXX : TitleEn;
        }


        public static DBGLCity FindByOid(Session session, Guid guid)
        {
            return session.FindObject<DBGLCity>(new BinaryOperator("Oid", guid));
        }

        public static DBGLCity FindDuplicateTitleXXExcept(Session session, DBGLCity exceptContact, string titleXX,
            bool allowPhoneCode)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("PhoneCode", 0,
                allowPhoneCode ? BinaryOperatorType.Greater : BinaryOperatorType.LessOrEqual));
            go.Operands.Add(new BinaryOperator("TitleXX", HelperConvert.CorrectPersianBug(titleXX),
                BinaryOperatorType.Equal));
            return session.FindObject<DBGLCity>(go);
        }

        public static DBGLCity FindByCodes(Session session, int countrycode, int citycode)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("Country.TeleCode", countrycode));
            go.Operands.Add(new BinaryOperator("PhoneCode", citycode));
            return session.FindObject<DBGLCity>(go);
        }

        public static XPCollection<DBGLCity> GetByCountryWithoutTeleCode(Session dxs, DBGLCountry dbc,
            string searchInTitleXX)
        {
            var go = new GroupOperator(
                new BinaryOperator("PhoneCode", 0, BinaryOperatorType.LessOrEqual),
                new BinaryOperator("Country.Oid", dbc) 
                );
            if (!string.IsNullOrWhiteSpace(searchInTitleXX))
                go.Operands.Add(new BinaryOperator("TitleXX", string.Format("%{0}%", searchInTitleXX),
                    BinaryOperatorType.Like));
            return new XPCollection<DBGLCity>(dxs, go,
                new SortProperty("StateTitle", SortingDirection.Ascending),
                new SortProperty("TitleXX", SortingDirection.Ascending));
        }

        public static XPCollection<DBGLCity> GetByCountryWithTeleCode(Session dxs, DBGLCountry dbc,
            string searchInTitleXX)
        {
            var go = new GroupOperator(
                new BinaryOperator("PhoneCode", 0, BinaryOperatorType.Greater),
                new BinaryOperator("Country.Oid", dbc) 
                );
            if (!string.IsNullOrWhiteSpace(searchInTitleXX))
                go.Operands.Add(new BinaryOperator("TitleXX", string.Format("%{0}%", searchInTitleXX),
                    BinaryOperatorType.Like));
            return new XPCollection<DBGLCity>(dxs, go,
                new SortProperty("StateTitle", SortingDirection.Ascending),
                new SortProperty("TitleXX", SortingDirection.Ascending));
        }

        public static XPCollection<DBGLCity> GetByCountryAll(Session dxs, DBGLCountry dbc)
        {
            var go = new GroupOperator(
                new BinaryOperator("Country.Oid", dbc) 
                );
            return new XPCollection<DBGLCity>(dxs, go,
                new SortProperty("StateTitle", SortingDirection.Ascending),
                new SortProperty("TitleXX", SortingDirection.Ascending));
        }

        #region TitleEn

        private string _TitleEn;

        [Size(256)]
        public string TitleEn
        {
            get { return _TitleEn; }
            set { SetPropertyValue("TitleEn", ref _TitleEn, value); }
        }

        #endregion

        #region TitleXX

        private string _TitleXX;

        [Size(256), PersianString]
        [DisplayName("عنوان شهر")]
        public string TitleXX
        {
            get { return _TitleXX; }
            set { SetPropertyValue("TitleXX", ref _TitleXX, value); }
        }

        #endregion

        #region Latitude

        private double _Latitude;

        [DisplayName("عرض جغرافیایی")]
        public double Latitude
        {
            get { return _Latitude; }
            set { SetPropertyValue("Latitude", ref _Latitude, value); }
        }

        #endregion

        #region Longitude

        private double _Longitude;

        [DisplayName("طول جغرافیایی")]
        public double Longitude
        {
            get { return _Longitude; }
            set { SetPropertyValue("Longitude", ref _Longitude, value); }
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

        #region PhoneCode

        private int _PhoneCode;

        [DisplayName("كد مخابراتی")]
        public int PhoneCode
        {
            get { return _PhoneCode; }
            set { SetPropertyValue("PhoneCode", ref _PhoneCode, value); }
        }

        #endregion

        #region HasTeleCode

        private bool _HasTeleCode;

        [DisplayName("كد دارد؟")]
        public bool HasTeleCode
        {
            get { return _HasTeleCode; }
            set { SetPropertyValue("HasTeleCode", ref _HasTeleCode, value); }
        }

        #endregion

        #region StateTitle

        private string _StateTitle;

        [Size(128), PersianString]
        [DisplayName("نام استان")]
        public string StateTitle
        {
            get { return _StateTitle; }
            set { SetPropertyValue("StateTitle", ref _StateTitle, value); }
        }

        #endregion

        #region PhoneLen

        private int _PhoneLen;

        [DisplayName("اندازه شماره")]
        public int PhoneLen
        {
            get { return _PhoneLen; }
            set { SetPropertyValue("PhoneLen", ref _PhoneLen, value); }
        }

        #endregion

        #region Country

        private DBGLCountry _Country;
        [DisplayName("كشور")]
        public DBGLCountry Country
        {
            get { return _Country; }
            set { SetPropertyValue("Country", ref _Country, value); }
        }

        #endregion

    }
}
