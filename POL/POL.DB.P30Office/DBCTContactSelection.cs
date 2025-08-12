using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using POL.DB.Membership;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTContactSelection : XPGUIDLogableObject 
    {
        #region CTOR

        public DBCTContactSelection(Session session) : base(session)
        {
        }

        #endregion

        #region Link - UsersGUID

        [Association("CSGHs"), Aggregated]
        public XPCollection<DBCTGUIDHolder> UsersGUID
        {
            get
            {
                var xpc = GetCollection<DBCTGUIDHolder>("UsersGUID");
                return xpc;
            }
        }

        #endregion

        #region Link - Contacts

        [Association("CTContactSelections_CTContacts")]
        public XPCollection<DBCTContact> Contacts
        {
            get { return GetCollection<DBCTContact>("Contacts"); }
        }

        #endregion

        public static XPCollection<DBCTContactSelection> GetByUser(Session dxs, Guid userID, bool onlyOwned,
            DBCTContactSelection except)
        {
            var go = new GroupOperator(GroupOperatorType.Or, CriteriaOperator.Parse("User2.Oid == ?", userID));
            if (!onlyOwned)
            {
                go.Operands.Add(new ContainsOperator("UsersGUID", new BinaryOperator("GUID", userID)));
            }
            if (except != null)
            {
                go = new GroupOperator(GroupOperatorType.And, go,
                    new BinaryOperator("Oid", except.Oid, BinaryOperatorType.NotEqual));
            }
            return new XPCollection<DBCTContactSelection>(dxs, go);
        }

        public static DBCTContactSelection FindDuplicateTitleExcept(Session session, DBCTContactSelection exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTContactSelection>(go);
        }

        public static DBCTContactSelection FindDuplicateTitleExcept(Session session, DBCTContactSelection exceptContact,
            string title, Guid userID)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("User2.Oid", userID, BinaryOperatorType.Equal));
            return session.FindObject<DBCTContactSelection>(go);
        }

        public static DBCTContactSelection FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTContactSelection>(new BinaryOperator("Oid", oid));
        }

        protected override XPCollection<T> CreateCollection<T>(XPMemberInfo property)
        {
            var collection = base.CreateCollection<T>(property);
            if (property.Name == "Contacts")
            {
                collection.Sorting = new SortingCollection(new SortProperty("Title", SortingDirection.Ascending));
            }
            return collection;
        }

        public override string ToString()
        {
            return Title;
        }

        #region Title

        private string _Title;

        [Size(64)]
        [PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region User2

        private DBMSUser2 _User2;

        public DBMSUser2 User2
        {
            get { return _User2; }
            set { SetPropertyValue("User2", ref _User2, value); }
        }

        #endregion

        #region NonPersistent

        [NonPersistent]
        public EnumShareStatus ShareStatus
        {
            get
            {
                if (Services.AMembership == null)
                    return EnumShareStatus.None;
                if (!Services.AMembership.IsAuthorized)
                    return EnumShareStatus.None;
                if (User2 == null)
                    return EnumShareStatus.None;
                if (User2.Oid == Services.AMembership.ActiveUser.UserID)
                {
                    return UsersGUID.Count == 0 ? EnumShareStatus.None : EnumShareStatus.SharedForOthers;
                }
                return EnumShareStatus.OthersSharedThis;
            }
        }

        [NonPersistent]
        public string OtherUser
        {
            get
            {
                return ShareStatus == EnumShareStatus.OthersSharedThis
                    ? string.Format("({0})", User2.Title)
                    : string.Empty;
            }
        }

        #endregion
    }
}
