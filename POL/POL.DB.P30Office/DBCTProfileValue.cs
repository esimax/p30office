using System;
using System.Globalization;
using System.Windows.Media;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTProfileValue : XPGUIDLogableObject 
    {
        private const string CCS_BoolTrueValue = "+";
        private const string CCS_BoolFalseValue = "-";
        private const string CCS_BoolNullValue = "?";
        private const EnumCalendarType CCS_AppCalendarType = EnumCalendarType.Shamsi;
        private const string CCS_DateTimeFormat = "yy/MM/dd";

        #region Design

        public DBCTProfileValue(Session session) : base(session)
        {
        }
        #endregion

        #region LayoutItemHolder

        [NonPersistent]
        public object LayoutItemHolder { get; set; }

        #endregion

        #region Guid1NP

        [NonPersistent]
        public Guid Guid1NP { get; set; }

        #endregion

        #region Int1NP

        [NonPersistent]
        public int Int1NP { get; set; }

        #endregion

        #region Int2NP

        [NonPersistent]
        public int Int2NP { get; set; }

        #endregion

        #region Double1NP

        [NonPersistent]
        public double Double1NP { get; set; }

        #endregion

        #region Double2NP

        [NonPersistent]
        public double Double2NP { get; set; }

        #endregion

        #region String1NP

        [NonPersistent]
        public string String1NP { get; set; }

        #endregion

        #region String2NP

        [NonPersistent]
        public string String2NP { get; set; }

        #endregion

        #region Byte1NP

        [NonPersistent]
        public byte[] Byte1NP { get; set; }

        #endregion

        #region DateTime1NP

        [NonPersistent]
        public DateTime DateTime1NP { get; set; }

        #endregion

        public static DBCTProfileValue FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTProfileValue>(new BinaryOperator("Oid", oid));
        }

        public static DBCTProfileValue FindByContactAndItem(Session session, Guid contactOid, Guid profileItemOid)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("Contact.Oid", contactOid));
            go.Operands.Add(new BinaryOperator("ProfileItem.Oid", profileItemOid));
            return session.FindObject<DBCTProfileValue>(go);
        }

        public static DBCTProfileValue FindByContactCodeAndItem(Session session, int contactCode, Guid profileItemOid)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("Contact.Code", contactCode));
            go.Operands.Add(new BinaryOperator("ProfileItem.Oid", profileItemOid));
            return session.FindObject<DBCTProfileValue>(go);
        }

        public static XPCollection<DBCTProfileValue> GetAll(Session session, Guid contactOid)
        {
            return new XPCollection<DBCTProfileValue>(session,
                new BinaryOperator("Contact.Oid", contactOid, BinaryOperatorType.Equal),
                new SortProperty("Order", SortingDirection.Ascending));
        }

        public static XPCollection<DBCTProfileValue> GetByProfileItem(Session session, Guid itemOid)
        {
            return new XPCollection<DBCTProfileValue>(session,
                new BinaryOperator("ProfileItem.Oid", itemOid, BinaryOperatorType.Equal));
        }

        public override string ToString()
        {
            return ProfileItem == null ? string.Empty : ProfileItem.Title;
        }

        public Color GetColorValue()
        {
            var val = Convert.ToInt64(Double1);
            return Color.FromArgb((byte) (val >> 24), (byte) ((val << 8) >> 24), (byte) ((val << 16) >> 24),
                (byte) ((val << 24) >> 24));
        }


        public string GetCustomColumnValue()
        {
            switch (ProfileItem.ItemType)
            {
                case EnumProfileItemType.Bool:
                    return GetFromBool();
                case EnumProfileItemType.Double:
                    return GetFromDouble();
                case EnumProfileItemType.Country:
                    return GetFromCountry();
                case EnumProfileItemType.City:
                    return GetFromCity();
                case EnumProfileItemType.Location:
                    return GetFromLocation(); 
                case EnumProfileItemType.String:
                    return GetFromString();
                case EnumProfileItemType.Memo:
                    return GetFromMemo();
                case EnumProfileItemType.StringCombo:
                    return GetFromCombo();
                case EnumProfileItemType.StringCheckList:
                    return GetFromCheckList(); 
                case EnumProfileItemType.Color:
                    return GetFromColor();
                case EnumProfileItemType.File:
                    return GetFromFile();
                case EnumProfileItemType.Image:
                    return GetFromImage();
                case EnumProfileItemType.Date:
                    return GetFromDate();
                case EnumProfileItemType.Time:
                    return GetFromTime();
                case EnumProfileItemType.DateTime:
                    return GetFromDateTime();
                case EnumProfileItemType.Contact:
                    return GetFromContact();
                case EnumProfileItemType.List:
                    return GetFromList();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private string GetFromBool()
        {
            switch (Int1)
            {
                case 0:
                    return CCS_BoolFalseValue;
                case 1:
                    return CCS_BoolTrueValue;
                case 2:
                    return CCS_BoolNullValue;
            }
            return CCS_BoolNullValue;
        }

        private string GetFromDouble()
        {
            return Double1.ToString(ProfileItem.String1);
        }

        private string GetFromCountry()
        {
            return String1;
        }

        private string GetFromCity()
        {
            return string.Format("{0}, {1}", String1, String2);
        }

        private string GetFromLocation()
        {
            return String1;
        }

        private string GetFromString()
        {
            return String1;
        }

        private string GetFromMemo()
        {
            return String1;
        }

        private string GetFromCombo()
        {
            return String1;
        }

        private string GetFromCheckList()
        {
            if (string.IsNullOrEmpty(String1))
                return string.Empty;
            return String1.Length > 256 ? String1.Substring(0, 256) : String1;
        }

        private string GetFromColor()
        {
            return "#" + Convert.ToInt64(Double1).ToString("X").PadLeft(8, '0');
        }

        private string GetFromFile()
        {
            return String1;
        }

        private string GetFromImage()
        {
            return String1;
        }

        private string GetFromDate()
        {
            return Int1 == 0
                ? "خالی"
                : HelperLocalize.DateTimeToString(DateTime1, GetCalendar(ProfileItem.Int1), CCS_DateTimeFormat);
        }

        private string GetFromTime()
        {
            var h = Int1/100;
            var m = Int1%100;
            return string.Format("{0}:{1}", h.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                m.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
        }

        private string GetFromDateTime()
        {
            if (Int1 == 0) return "خالی";
            var i = DateTime1;
            return string.Format("{0} - {1}:{2}",
                HelperLocalize.DateTimeToString(i, GetCalendar(ProfileItem.Int1), CCS_DateTimeFormat),
                i.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                i.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
        }

        private string GetFromContact()
        {
            return String1;
        }

        private string GetFromList()
        {
            return String1;
        }


        private EnumCalendarType GetCalendar(int i)
        {
            if (i == 1) return EnumCalendarType.Hijri;
            if (i == 2) return EnumCalendarType.Gregorian;
            if (i == 3) return EnumCalendarType.Shamsi;
            return CCS_AppCalendarType;
        }

        #region Order

        private int _Order;

        public int Order
        {
            get { return _Order; }
            set { SetPropertyValue("Order", ref _Order, value); }
        }

        #endregion

        #region ProfileItem

        private DBCTProfileItem _ProfileItem;

        public DBCTProfileItem ProfileItem
        {
            get { return _ProfileItem; }
            set { SetPropertyValue("ProfileItem", ref _ProfileItem, value); }
        }

        #endregion

        #region Contact

        private DBCTContact _Contact;

        [Association("CTContact_CTProfileValues")]
        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
        }

        #endregion

        #region Guid1

        private Guid _Guid1;

        public Guid Guid1
        {
            get { return _Guid1; }
            set { SetPropertyValue("Guid1", ref _Guid1, value); }
        }

        #endregion

        #region Int1

        private int _Int1;

        public int Int1
        {
            get { return _Int1; }
            set { SetPropertyValue("Int1", ref _Int1, value); }
        }

        #endregion

        #region Int2

        private int _Int2;

        public int Int2
        {
            get { return _Int2; }
            set { SetPropertyValue("Int2", ref _Int2, value); }
        }

        #endregion

        #region Double1

        private double _Double1;

        public double Double1
        {
            get { return _Double1; }
            set { SetPropertyValue("Double1", ref _Double1, value); }
        }

        #endregion

        #region Double2

        private double _Double2;

        public double Double2
        {
            get { return _Double2; }
            set { SetPropertyValue("Double2", ref _Double2, value); }
        }

        #endregion

        #region String1

        private string _String1;

        [Size(SizeAttribute.Unlimited)]
        public string String1
        {
            get { return _String1; }
            set { SetPropertyValue("String1", ref _String1, value); }
        }

        #endregion

        #region String2

        private string _String2;

        [Size(SizeAttribute.Unlimited)]
        public string String2
        {
            get { return _String2; }
            set { SetPropertyValue("String2", ref _String2, value); }
        }

        #endregion

        #region DateTime1

        private DateTime _DateTime1;

        public DateTime DateTime1
        {
            get { return _DateTime1; }
            set { SetPropertyValue("DateTime1", ref _DateTime1, value); }
        }

        #endregion

        #region NeedToSave

        private bool _NeedToSave;

        [NonPersistent]
        public bool NeedToSave
        {
            get { return _NeedToSave; }
            set
            {
                _NeedToSave = value;
                RaisePropertyChangedEvent("NeedToSave");
            }
        }

        #endregion
    }

}
