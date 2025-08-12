using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTProfileItem : XPGUIDLogableObject 
    {
        #region Design

        public DBCTProfileItem(Session session) : base(session)
        {
        }

        #endregion

        [NonPersistent]
        public string FullPathString
        {
            get { return ProfileGroup.ProfileRoot.Title + " > " + ProfileGroup.Title + " > " + Title; }
        }

        public static DBCTProfileItem FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTProfileItem>(new BinaryOperator("Oid", oid));
        }

        public static DBCTProfileItem FindDuplicateTitleExcept(Session session, Guid groupOid,
            DBCTProfileItem exceptItem, string title)
        {
            var go = new GroupOperator();
            if (exceptItem != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptItem.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("ProfileGroup.Oid", groupOid, BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTProfileItem>(go);
        }

        public static DBCTProfileItem FindByAllTitles(Session session, string rootTitle, string groupTitle,
            string itemTitle)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("ProfileGroup.ProfileRoot.Title", rootTitle, BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("ProfileGroup.Title", groupTitle, BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(itemTitle),
                BinaryOperatorType.Equal));
            return session.FindObject<DBCTProfileItem>(go);
        }

        public static int GetLastOrder(Session session, Guid groupOid)
        {
            var xpc = new XPCollection<DBCTProfileItem>(session,
                new BinaryOperator("ProfileGroup.Oid", groupOid, BinaryOperatorType.Equal),
                new SortProperty("Order", SortingDirection.Descending)) {TopReturnedObjects = 1};
            return xpc.Count == 0 ? 0 : xpc[0].Order;
        }

        public static XPCollection<DBCTProfileItem> GetAll(Session session, Guid groupOid)
        {
            return new XPCollection<DBCTProfileItem>(session,
                new BinaryOperator("ProfileGroup.Oid", groupOid, BinaryOperatorType.Equal),
                new SortProperty("Order", SortingDirection.Ascending));
        }

        public override string ToString()
        {
            return Title;
        }

        protected override void OnSaving()
        {
            if (string.IsNullOrEmpty(UnicCode))
            {
                try
                {
                    UnicCode = DateTime.Now.Ticks.ToString("X").PadLeft(16, '0');
                }
                catch
                {
                }
            }
            base.OnSaving();
        }

        #region Title

        private string _Title;

        [PersianString]
        [Size(64)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Order

        private int _Order;

        public int Order
        {
            get { return _Order; }
            set { SetPropertyValue("Order", ref _Order, value); }
        }

        #endregion

        #region UnicCode

        private string _UnicCode;

        [Size(16)]
        public string UnicCode
        {
            get { return _UnicCode; }
            set { SetPropertyValue("UnicCode", ref _UnicCode, value); }
        }

        #endregion

        #region ItemType

        private EnumProfileItemType _ItemType;

        public EnumProfileItemType ItemType
        {
            get { return _ItemType; }
            set { SetPropertyValue("ItemType", ref _ItemType, value); }
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

        #region Int3

        private int _Int3;

        public int Int3
        {
            get { return _Int3; }
            set { SetPropertyValue("Int3", ref _Int3, value); }
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

        public string String1
        {
            get { return _String1; }
            set { SetPropertyValue("String1", ref _String1, value); }
        }

        #endregion

        #region String2

        private string _String2;

        public string String2
        {
            get { return _String2; }
            set { SetPropertyValue("String2", ref _String2, value); }
        }

        #endregion

        #region String3

        private string _String3;

        public string String3
        {
            get { return _String3; }
            set { SetPropertyValue("String3", ref _String3, value); }
        }

        #endregion

        #region DefaultValue

        private string _DefaultValue;

        [Size(SizeAttribute.Unlimited)]
        public string DefaultValue
        {
            get { return _DefaultValue; }
            set { SetPropertyValue("DefaultValue", ref _DefaultValue, value); }
        }

        #endregion

        #region IsRequired

        private bool _IsRequired;

        public bool IsRequired
        {
            get { return _IsRequired; }
            set { SetPropertyValue("IsRequired", ref _IsRequired, value); }
        }

        #endregion

        #region ProfileGroup

        private DBCTProfileGroup _ProfileGroup;

        public DBCTProfileGroup ProfileGroup
        {
            get { return _ProfileGroup; }
            set { SetPropertyValue("ProfileGroup", ref _ProfileGroup, value); }
        }

        #endregion

    }

}
