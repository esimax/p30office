using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTProfileTValue : XPGUIDLogableObject 
    {
        #region Design

        public DBCTProfileTValue(Session session) : base(session)
        {
        }
        #endregion

        #region Link [1-n] - ProfileTValue(Children)

        [Association("ProfileTValue-ProfileTValues")]
        [Aggregated]
        public XPCollection<DBCTProfileTValue> Children
        {
            get { return GetCollection<DBCTProfileTValue>("Children"); }
        }

        #endregion

        public static DBCTProfileTValue FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTProfileTValue>(new BinaryOperator("Oid", oid));
        }

        public static DBCTProfileTValue FindDuplicateTitleExcept(Session session, Guid tableOid,
            DBCTProfileTValue exceptContactCat, string title, DBCTProfileTValue parent)
        {
            var go = new GroupOperator();
            if (exceptContactCat != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContactCat.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Table.Oid", tableOid, BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            if (parent == null)
                go.Operands.Add(new NullOperator("Parent"));
            else
                go.Operands.Add(new BinaryOperator("Parent.Oid", parent.Oid, BinaryOperatorType.Equal));

            return session.FindObject<DBCTProfileTValue>(go);
        }

        public static XPCollection<DBCTProfileTValue> GetAll(Session session, Guid tableOid)
        {
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("Table.Oid", tableOid, BinaryOperatorType.Equal));
            go.Operands.Add(new NullOperator("Parent"));
            return new XPCollection<DBCTProfileTValue>(session,
                go,
                new SortProperty("Order", SortingDirection.Ascending),
                new SortProperty("Title", SortingDirection.Ascending));
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

        #region Order

        private int _Order;

        public int Order
        {
            get { return _Order; }
            set { SetPropertyValue("Order", ref _Order, value); }
        }

        #endregion

        #region Table

        private DBCTProfileTable _Table;

        public DBCTProfileTable Table
        {
            get { return _Table; }
            set { SetPropertyValue("Table", ref _Table, value); }
        }

        #endregion

        #region ProfileTValue(Parent)

        private DBCTProfileTValue _Parent;

        [Association("ProfileTValue-ProfileTValues")]
        public DBCTProfileTValue Parent
        {
            get { return _Parent; }
            set { SetPropertyValue("Parent", ref _Parent, value); }
        }

        #endregion
    }
}
