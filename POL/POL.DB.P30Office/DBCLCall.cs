using System;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using POL.DB.P30Office.GL;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.P30Office
{
    public class DBCLCall : XPGUIDObject
    {
        #region Link - Internals

        [Association("CLCall_CLInternal"), Aggregated]
        [DisplayName("داخلی ها")]
        public XPCollection<DBCLInternal> Internals
        {
            get { return GetCollection<DBCLInternal>("Internals"); }
        }

        #endregion

        #region TeleCode

        [DisplayName("كد مخابراتی")]
        [PersistentAlias("Iif(City is null, Country.TeleCode*10000000, City.PhoneCode + City.Country.TeleCode*10000000)"
            )]
        public decimal TeleCode
        {
            get { return Convert.ToDecimal(EvaluateAlias("TeleCode")); }
        }

        #endregion

        #region FullPhoneString

        [NonPersistent]
        [DisplayName("شماره تماس بصورت كامل")]
        public string FullPhoneString
        {
            get
            {
                var rv = string.Empty;
                if (Country != null)
                    rv = string.Format("(+{0}) {1}", Country.TeleCode, PhoneNumber);
                if (City != null)
                    rv = string.Format("{0} {1}", City.TeleCodeString, PhoneNumber);
                return rv;
            }
        }

        #endregion

        #region CTOR

        public DBCLCall(Session session) : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            RecordRole = 1;
        }

        #endregion Design

        #region CallType

        private EnumCallType _CallType;

        [DisplayName("نوع تماس")]
        public EnumCallType CallType
        {
            get { return _CallType; }
            set { SetPropertyValue("CallType", ref _CallType, value); }
        }

        #endregion CallType

        #region Contact

        private DBCTContact _Contact;
        [DisplayName("پرونده")]
        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
        }

        #endregion

        #region Phone

        private DBCTPhoneBook _Phone;
        [DisplayName("پرونده")]
        public DBCTPhoneBook Phone
        {
            get { return _Phone; }
            set { SetPropertyValue("Phone", ref _Phone, value); }
        }

        #endregion

        #region CallDate

        private DateTime? _CallDate;

        [DisplayName("تاریخ تماس")]
        public DateTime? CallDate
        {
            get { return _CallDate; }
            set
            {
                SetPropertyValue("CallDate", ref _CallDate, value);
                CallDay = _CallDate == null ? 0 : ((DateTime) _CallDate).DayOfWeek;
            }
        }

        #endregion CallDate

        #region CallDay

        private DayOfWeek _CallDay;

        [DisplayName("روز تماس")]
        public DayOfWeek CallDay
        {
            get { return _CallDay; }
            set { SetPropertyValue("CallDay", ref _CallDay, value); }
        }

        #endregion CallDay

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

        #endregion Country

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

        #endregion City

        #region OriginalCallerID

        private string _OriginalCallerID;

        [Size(32)]
        [DisplayName("داده دریافتی")]
        public string OriginalCallerID
        {
            get { return _OriginalCallerID; }
            set { SetPropertyValue("OriginalCallerID", ref _OriginalCallerID, value); }
        }

        #endregion

        #region ModifierUser

        private string _ModifierUser;

        [Size(32)]
        [DisplayName("كاربر ویرایش كننده")]
        public string ModifierUser
        {
            get { return _ModifierUser; }
            set { SetPropertyValue("ModifierUser", ref _ModifierUser, value); }
        }

        #endregion

        #region PhoneNumber

        private string _PhoneNumber;

        [Size(32)]
        [DisplayName("شماره تماس")]
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { SetPropertyValue("PhoneNumber", ref _PhoneNumber, value); }
        }

        #endregion

        #region ExtraDialed

        private string _ExtraDialed;

        [Size(32)]
        [DisplayName("شماره های مازاد")]
        public string ExtraDialed
        {
            get { return _ExtraDialed; }
            set { SetPropertyValue("ExtraDialed", ref _ExtraDialed, value); }
        }

        #endregion

        #region DurationSeconds

        private int _DurationSeconds;

        [DisplayName("مدت مكالمه بر حسب ثانیه")]
        public int DurationSeconds
        {
            get { return _DurationSeconds; }
            set { SetPropertyValue("DurationSeconds", ref _DurationSeconds, value); }
        }

        #endregion

        #region LineNumber

        private int _LineNumber;

        [DisplayName("شماره خط")]
        public int LineNumber
        {
            get { return _LineNumber; }
            set { SetPropertyValue("LineNumber", ref _LineNumber, value); }
        }

        #endregion

        #region Del

        private bool _Del;

        [DisplayName("حذف شده")]
        public bool Del
        {
            get { return _Del; }
            set { SetPropertyValue("Del", ref _Del, value); }
        }

        #endregion

        #region DelUser

        private string _DelUser;

        [Size(32)]
        [DisplayName("حذف كننده")]
        public string DelUser
        {
            get { return _DelUser; }
            set { SetPropertyValue("DelUser", ref _DelUser, value); }
        }

        #endregion

        #region NoteText

        private string _NoteText;

        [Size(128)]
        [PersianString]
        [DisplayName("متن نكته")]
        public string NoteText
        {
            get { return _NoteText; }
            set { SetPropertyValue("NoteText", ref _NoteText, value); }
        }

        #endregion

        #region NoteFlag

        private int _NoteFlag;

        [DisplayName("پرچم نكته")]
        public int NoteFlag
        {
            get { return _NoteFlag; }
            set { SetPropertyValue("NoteFlag", ref _NoteFlag, value); }
        }

        #endregion

        #region NoteWriter

        private string _NoteWriter;

        [Size(64)]
        [DisplayName("نویسنده نكته")]
        public string NoteWriter
        {
            get { return _NoteWriter; }
            set { SetPropertyValue("NoteWriter", ref _NoteWriter, value); }
        }

        #endregion

        #region RecordTag

        private string _RecordTag;

        [Size(36)]
        [DisplayName("تگ ضبط مكالمه")]
        public string RecordTag
        {
            get { return _RecordTag; }
            set { SetPropertyValue("RecordTag", ref _RecordTag, value); }
        }

        #endregion

        #region RecordEnable

        private bool _RecordEnable;

        [DisplayName("ضبط دارد؟")]
        public bool RecordEnable
        {
            get { return _RecordEnable; }
            set { SetPropertyValue("RecordEnable", ref _RecordEnable, value); }
        }

        #endregion

        #region RecordRole

        private int _RecordRole;

        [DisplayName("قانون ضبط")]
        public int RecordRole
        {
            get { return _RecordRole; }
            set { SetPropertyValue("RecordRole", ref _RecordRole, value); }
        }

        #endregion

        #region InternalString

        private string _InternalString;

        [Size(256)]
        [DisplayName("متن داخلی ها")]
        public string InternalString
        {
            get { return _InternalString; }
            set { SetPropertyValue("InternalString", ref _InternalString, value); }
        }

        #endregion

        #region LastExt

        private int _LastExt;

        [DisplayName("آخرین داخلی")]
        public int LastExt
        {
            get { return _LastExt; }
            set { SetPropertyValue("LastExt", ref _LastExt, value); }
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

        #region Methods

        public static DBCLCall FindByOid(Session dxs, Guid oid)
        {
            return dxs.FindObject<DBCLCall>(new BinaryOperator("Oid", oid));
        }

        public static DBCLCall FindByRecordTag(Session dxs, string tag)
        {
            return dxs.FindObject<DBCLCall>(new BinaryOperator("RecordTag", tag));
        }

        public static XPCollection<DBCLCall> GetObsulateRecords(Session dxs, int role1, int role2, int role3)
        {
            var go = new GroupOperator();
            go.Operands.Add(GetCriteriaForRole(1, role1));
            go.Operands.Add(GetCriteriaForRole(2, role2));
            go.Operands.Add(GetCriteriaForRole(3, role3));
            go.Operands.Add(new BinaryOperator("RecordEnable", true));
            return new XPCollection<DBCLCall>(dxs, go, new SortProperty("CallDate", SortingDirection.Descending));
        }

        public static void RemoveContactLink(Session dxs, Guid contact, string phone, Guid countryOid, Guid cityOid)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("Contact.Oid", contact));
            go.Operands.Add(new BinaryOperator("PhoneNumber", phone));
            if (countryOid != Guid.Empty)
                go.Operands.Add(new BinaryOperator("Country.Oid", countryOid));
            if (cityOid != Guid.Empty)
                go.Operands.Add(new BinaryOperator("City.Oid", cityOid));

            using (var uow = new UnitOfWork(dxs.DataLayer))
            {
                uow.Update(
                    () => new DBCLCall(uow)
                    {
                        Contact = null
                    },
                    go);
                uow.CommitChanges();
            }
        }

        public static void FixGroupOperatorToLINQForCall(CriteriaOperator go)
        {
            if (go is GroupOperator)
                (go as GroupOperator).Operands.ForEach(FixGroupOperatorToLINQForCall);
            else
            {
                if (go is BinaryOperator)
                {
                    var bo = go as BinaryOperator;
                    if (bo.LeftOperand is OperandProperty)
                    {
                        var lp = bo.LeftOperand as OperandProperty;
                        if (lp.PropertyName == "CallType")
                        {
                            if (bo.RightOperand is OperandValue)
                            {
                                var rv = bo.RightOperand as OperandValue;
                                rv.Value = (EnumCallType) rv.Value;
                            }
                        }
                    }
                }
            }
        }

        public static CriteriaOperator RemoveEmptyOperand(CriteriaOperator go)
        {
            if (!(go is GroupOperator)) return go;
            var newgo = new GroupOperator(((GroupOperator) go).OperatorType);
            foreach (var op in ((GroupOperator) go).Operands)
            {
                if (ReferenceEquals(op, null)) continue;
                if (op is GroupOperator)
                    newgo.Operands.Add(RemoveEmptyOperand(op));
                else
                    newgo.Operands.Add(op);
            }
            return newgo;
        }

        public static DBCLCall GetFirstCall(Session session)
        {
            var xpc = new XPCollection<DBCLCall>(session, null, new SortProperty("CallDate", SortingDirection.Ascending));
            xpc.TopReturnedObjects = 1;
            if (xpc.Count > 0)
                return xpc[0];
            return null;
        }

        public static DBCLCall GetLastCall(Session session)
        {
            var xpc = new XPCollection<DBCLCall>(session, null,
                new SortProperty("CallDate", SortingDirection.Descending));
            xpc.TopReturnedObjects = 1;
            if (xpc.Count > 0)
                return xpc[0];
            return null;
        }

        private string CalculateInternalString()
        {
            var rv = new StringBuilder();
            if (Internals.Count > 0)
            {
                Internals.ToList().ForEach(i =>
                {
                    if (i.DurationSeconds < 0) return;
                    if (rv.Length != 0)
                        rv.Append(" > ");
                    rv.Append(i.Ext);
                    rv.AppendFormat("({0})",
                        TimeSpan.FromSeconds(i.DurationSeconds).
                            ToString().TrimStart('0').TrimStart('0').
                            TrimStart(':'));
                });
            }


            var j = Internals.Count;
            LastExt = j > 0 ? Internals[j - 1].Ext : -1;

            var ss = rv.ToString();
            if (ss.Length > 255)
                ss = ss.Substring(255);
            return ss;
        }

        private static CriteriaOperator GetCriteriaForRole(int role, int roleindex)
        {
            if (roleindex > 6) return null; 
            var date = DateTime.MaxValue;
            switch (roleindex)
            {
                case 0:
                    date = DateTime.Now.AddDays(-7);
                    break;
                case 1:
                    date = DateTime.Now.AddDays(-14);
                    break;
                case 2:
                    date = DateTime.Now.AddMonths(-1);
                    break;
                case 3:
                    date = DateTime.Now.AddMonths(-2);
                    break; 
                case 4:
                    date = DateTime.Now.AddMonths(-3);
                    break; 
                case 5:
                    date = DateTime.Now.AddMonths(-6);
                    break; 
                case 6:
                    date = DateTime.Now.AddMonths(-12);
                    break; 
            }
            return new GroupOperator(
                new BinaryOperator("CallDate", date, BinaryOperatorType.Less),
                new BinaryOperator("RecordRole", role, BinaryOperatorType.Equal)
                );
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

        #region Override

        protected override XPCollection<T> CreateCollection<T>(XPMemberInfo property)
        {
            var collection = base.CreateCollection<T>(property);
            if (property.Name == "Internals")
            {
                collection.Sorting = new SortingCollection(new SortProperty("InOrder", SortingDirection.Ascending));
            }
            return collection;
        }

        protected override void OnSaving()
        {
            InternalString = CalculateInternalString();
            TeleCode2 = CalculateTelecode2();
            base.OnSaving();
        }

        #endregion
    }
}
