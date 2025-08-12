using System;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTProfileGroup : XPGUIDLogableObject 
    {
        #region Design

        public DBCTProfileGroup(Session session) : base(session)
        {
        }
        #endregion

        #region Link [1-n] - ProfileItem

        public XPCollection<DBCTProfileItem> Items
        {
            get { return GetCollection<DBCTProfileItem>("Items"); }
        }

        #endregion

        public static DBCTProfileGroup FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTProfileGroup>(new BinaryOperator("Oid", oid));
        }

        public static DBCTProfileGroup FindDuplicateTitleExcept(Session session, Guid rootOid,
            DBCTProfileGroup exceptContactCat, string title)
        {
            var go = new GroupOperator();
            if (exceptContactCat != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContactCat.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("ProfileRoot.Oid", rootOid, BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTProfileGroup>(go);
        }

        public static int GetLastOrder(Session session, Guid rootOid)
        {
            var xpc = new XPCollection<DBCTProfileGroup>(session,
                new BinaryOperator("ProfileRoot.Oid", rootOid, BinaryOperatorType.Equal),
                new SortProperty("Order", SortingDirection.Descending))
            {TopReturnedObjects = 1};
            return xpc.Count == 0 ? 0 : xpc[0].Order;
        }

        public static string GetProfileItemUriString()
        {
            var sb = new StringBuilder();
            sb.Append(UsedConstants.POLImagePath);
            sb.Append("Standard/16/_16_Group.png");
            return sb.ToString();
        }


        public static XPCollection<DBCTProfileGroup> GetAll(Session session, Guid rootOid)
        {
            return new XPCollection<DBCTProfileGroup>(session,
                new BinaryOperator("ProfileRoot.Oid", rootOid, BinaryOperatorType.Equal),
                new SortProperty("Order", SortingDirection.Ascending));
        }

        public override string ToString()
        {
            return Title;
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

        #region IsRequired

        private bool _IsRequired;

        public bool IsRequired
        {
            get { return _IsRequired; }
            set { SetPropertyValue("IsRequired", ref _IsRequired, value); }
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

        #region ProfileRoot

        private DBCTProfileRoot _ProfileRoot;

        public DBCTProfileRoot ProfileRoot
        {
            get { return _ProfileRoot; }
            set { SetPropertyValue("ProfileRoot", ref _ProfileRoot, value); }
        }

        #endregion

    }
}
