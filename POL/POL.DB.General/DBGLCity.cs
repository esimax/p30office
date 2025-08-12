using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using POL.DB.Root;

namespace POL.DB.General
{
    [OptimisticLocking(false)]
    public class DBGLCity : XPGUIDObject
    {
        #region Design

        public DBGLCity()
        {
        }

        public DBGLCity(Session session)
            : base(session)
        {
        }

        protected DBGLCity(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        #endregion

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
        public string TitleXX
        {
            get { return _TitleXX; }
            set { SetPropertyValue("TitleXX", ref _TitleXX, value); }
        }

        #endregion

        #region Latitude

        private double _Latitude;

        public double Latitude
        {
            get { return _Latitude; }
            set { SetPropertyValue("Latitude", ref _Latitude, value); }
        }

        #endregion

        #region Longitude

        private double _Longitude;

        public double Longitude
        {
            get { return _Longitude; }
            set { SetPropertyValue("Longitude", ref _Longitude, value); }
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

        #region PhoneCode

        private int _PhoneCode;

        public int PhoneCode
        {
            get { return _PhoneCode; }
            set { SetPropertyValue("PhoneCode", ref _PhoneCode, value); }
        }

        #endregion

        #region HasTeleCode

        private bool _HasTeleCode;

        public bool HasTeleCode
        {
            get { return _HasTeleCode; }
            set { SetPropertyValue("HasTeleCode", ref _HasTeleCode, value); }
        }

        #endregion

        #region StateTitle

        private string _StateTitle;

        [Size(128), PersianString]
        public string StateTitle
        {
            get { return _StateTitle; }
            set { SetPropertyValue("StateTitle", ref _StateTitle, value); }
        }

        #endregion

        #region TeleCodeString

        [NonPersistent]
        public string TeleCodeString
        {
            get
            {
                if (Country != null)
                    return
                        String.Format("{0} {1}", Country.TeleCodeString, (PhoneCode > 0 ? PhoneCode.ToString() : "")).
                            Trim();
                return "";
            }
        }

        #endregion

        #region Country

        private DBGLCountry _Country;

        [Association("GLCountry-GLCities")]
        public DBGLCountry Country
        {
            get { return _Country; }
            set { SetPropertyValue("Country", ref _Country, value); }
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

        public static XPCollection<DBGLCity> GetByCountry(Session dxs, DBGLCountry dbc)
        {
            return new XPCollection<DBGLCity>(dxs,
                                              new GroupOperator(
                                                  new BinaryOperator("Country.Oid", dbc)
                                                  )
                                              , new SortProperty("TitleXX", SortingDirection.Ascending));
        }

        public static XPCollection<DBGLCity> GetByCountryWithTeleCode(Session dxs, DBGLCountry dbc)
        {
            return new XPCollection<DBGLCity>(dxs,
                                              new GroupOperator(
                                                  new BinaryOperator("PhoneCode", 1, BinaryOperatorType.GreaterOrEqual),
                                                  new BinaryOperator("Country.Oid", dbc),
                                                  new BinaryOperator("HasTeleCode", true)
                                                  )
                                              , new SortProperty("TitleXX", SortingDirection.Ascending));
        }

        public static DBGLCity FindByCodes(Session session, int countrycode, int citycode)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("Country.TeleCode", countrycode));
            go.Operands.Add(new BinaryOperator("PhoneCode", citycode));
            return session.FindObject<DBGLCity>(go);
        }

        public static DBGLCity FindByTitleXX(Session session, string titlexx)
        {
            return session.FindObject<DBGLCity>(new BinaryOperator("TitleXX", titlexx));
        }

        public static DBGLCity FindByTitleXX(Session session, string titlexx,Guid countryOid)
        {
            return session.FindObject<DBGLCity>(new GroupOperator( 
                new BinaryOperator("TitleXX", titlexx)
                , new BinaryOperator("Country.Oid",countryOid)
                ));
        }


    }
}
