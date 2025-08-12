using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.P30Office
{
    [OptimisticLocking(false), DeferredDeletion(false)]
    public class DBCLStatPhone : XPGUIDObject
    {
        #region CTOR

        public DBCLStatPhone(Session session) : base(session)
        {
        }

        #endregion Design

        #region CountryCode

        private int _CountryCode;

        public int CountryCode
        {
            get { return _CountryCode; }
            set { SetPropertyValue("CountryCode", ref _CountryCode, value); }
        }

        #endregion

        #region CityCode

        private int _CityCode;

        public int CityCode
        {
            get { return _CityCode; }
            set { SetPropertyValue("CityCode", ref _CityCode, value); }
        }

        #endregion CityCode

        #region PhoneNumber

        private string _PhoneNumber;

        [Size(32)]
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { SetPropertyValue("PhoneNumber", ref _PhoneNumber, value); }
        }

        #endregion

        #region CountOut

        private int _CountOut;

        public int CountOut
        {
            get { return _CountOut; }
            set { SetPropertyValue("CountOut", ref _CountOut, value); }
        }

        #endregion

        #region CountIn

        private int _CountIn;

        public int CountIn
        {
            get { return _CountIn; }
            set { SetPropertyValue("CountIn", ref _CountIn, value); }
        }

        #endregion

        #region CountTotal

        private int _CountTotal;

        public int CountTotal
        {
            get { return _CountTotal; }
            set { SetPropertyValue("CountTotal", ref _CountTotal, value); }
        }

        #endregion

        #region DurationOut

        private int _DurationOut;

        public int DurationOut
        {
            get { return _DurationOut; }
            set { SetPropertyValue("DurationOut", ref _DurationOut, value); }
        }

        #endregion

        #region DurationIn

        private int _DurationIn;

        public int DurationIn
        {
            get { return _DurationIn; }
            set { SetPropertyValue("DurationIn", ref _DurationIn, value); }
        }

        #endregion

        #region DurationTotal

        private int _DurationTotal;

        public int DurationTotal
        {
            get { return _DurationTotal; }
            set { SetPropertyValue("DurationTotal", ref _DurationTotal, value); }
        }

        #endregion

        #region MissCall

        private int _MissCall;

        public int MissCall
        {
            get { return _MissCall; }
            set { SetPropertyValue("MissCall", ref _MissCall, value); }
        }

        #endregion

        #region LastCallType

        private EnumCallType _LastCallType;

        public EnumCallType LastCallType
        {
            get { return _LastCallType; }
            set { SetPropertyValue("LastCallType", ref _LastCallType, value); }
        }

        #endregion LastCallType

        #region LastCallDate

        private DateTime? _LastCallDate;

        public DateTime? LastCallDate
        {
            get { return _LastCallDate; }
            set { SetPropertyValue("LastCallDate", ref _LastCallDate, value); }
        }

        #endregion

        #region LastDuration

        private int _LastDuration;

        public int LastDuration
        {
            get { return _LastDuration; }
            set { SetPropertyValue("LastDuration", ref _LastDuration, value); }
        }

        #endregion

        #region LastLine

        private int _LastLine;

        public int LastLine
        {
            get { return _LastLine; }
            set { SetPropertyValue("LastLine", ref _LastLine, value); }
        }

        #endregion

        #region LastInternal

        private int _LastInternal;

        public int LastInternal
        {
            get { return _LastInternal; }
            set { SetPropertyValue("LastInternal", ref _LastInternal, value); }
        }

        #endregion

        #region Contact

        private DBCTContact _Contact;
        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
        }

        #endregion

        #region Phone

        private DBCTPhoneBook _Phone;

        public DBCTPhoneBook Phone
        {
            get { return _Phone; }
            set { SetPropertyValue("Phone", ref _Phone, value); }
        }

        #endregion

        #region CalcRequired

        private bool _CalcRequired;

        public bool CalcRequired
        {
            get { return _CalcRequired; }
            set { SetPropertyValue("CalcRequired", ref _CalcRequired, value); }
        }

        #endregion

        #region Methods

        public static void ClearAllData(Session dxs)
        {
            using (var uow = new UnitOfWork(dxs.DataLayer))
            {
                uow.Delete<DBCLStatPhone>(null); 
                uow.CommitChanges();
            }
        }

        public static DBCLStatPhone FindByOid(Session dxs, Guid oid)
        {
            return dxs.FindObject<DBCLStatPhone>(new BinaryOperator("Oid", oid));
        }

        public static DBCLStatPhone FindByCodeAndPhone(Session dxs, int countryCode, int cityCode, string phone)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("CountryCode", countryCode));
            go.Operands.Add(new BinaryOperator("CityCode", cityCode));
            go.Operands.Add(new BinaryOperator("PhoneNumber", phone));
            return dxs.FindObject<DBCLStatPhone>(go);
        }

        #endregion
    }
}
