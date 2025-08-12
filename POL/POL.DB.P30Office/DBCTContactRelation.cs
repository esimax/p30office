using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public class DBCTContactRelation : XPGUIDLogableObject 
    {
        #region Design

        public DBCTContactRelation(Session session) : base(session)
        {
        }

        #endregion

        public static DBCTContactRelation FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTContactRelation>(new BinaryOperator("Oid", oid));
        }

        public static DBCTContactRelation FindDuplicateByContact(Session session, DBCTContactRelation exceptContactRel,
            Guid contact1Oid, Guid contact2Oid)
        {
            var go = new GroupOperator();
            if (exceptContactRel != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContactRel.Oid, BinaryOperatorType.NotEqual));
            }

            var or = new GroupOperator(GroupOperatorType.Or);
            or.Operands.Add(new GroupOperator(new BinaryOperator("Contact1.Oid", contact1Oid),
                new BinaryOperator("Contact2.Oid", contact2Oid)));
            or.Operands.Add(new GroupOperator(new BinaryOperator("Contact2.Oid", contact1Oid),
                new BinaryOperator("Contact1.Oid", contact2Oid)));

            go.Operands.Add(or);
            return session.FindObject<DBCTContactRelation>(go);
        }

        public static XPCollection<DBCTContactRelation> GetAll(Session session)
        {
            return new XPCollection<DBCTContactRelation>(session);
        }

        public static XPCollection<DBCTContactRelation> GetByContact(Session session, Guid contactOid)
        {
            var or = new GroupOperator(GroupOperatorType.Or);
            or.Operands.Add(new BinaryOperator("Contact1.Oid", contactOid));
            or.Operands.Add(new BinaryOperator("Contact2.Oid", contactOid));
            return new XPCollection<DBCTContactRelation>(session, or);
        }

        #region Contact1

        private DBCTContact _Contact1;

        public DBCTContact Contact1
        {
            get { return _Contact1; }
            set { SetPropertyValue("Contact1", ref _Contact1, value); }
        }

        #endregion

        #region Contact2

        private DBCTContact _Contact2;

        public DBCTContact Contact2
        {
            get { return _Contact2; }
            set { SetPropertyValue("Contact2", ref _Contact2, value); }
        }

        #endregion

        #region TitleMain1

        private string _TitleMain1;

        [PersianString]
        [Size(32)]
        public string TitleMain1
        {
            get { return _TitleMain1; }
            set { SetPropertyValue("TitleMain1", ref _TitleMain1, value); }
        }

        #endregion

        #region TitleSub1

        private string _TitleSub1;

        [PersianString]
        [Size(32)]
        public string TitleSub1
        {
            get { return _TitleSub1; }
            set { SetPropertyValue("TitleSub1", ref _TitleSub1, value); }
        }

        #endregion

        #region TitleMain2

        private string _TitleMain2;

        [PersianString]
        [Size(32)]
        public string TitleMain2
        {
            get { return _TitleMain2; }
            set { SetPropertyValue("TitleMain2", ref _TitleMain2, value); }
        }

        #endregion

        #region TitleSub2

        private string _TitleSub2;

        [PersianString]
        [Size(32)]
        public string TitleSub2
        {
            get { return _TitleSub2; }
            set { SetPropertyValue("TitleSub2", ref _TitleSub2, value); }
        }

        #endregion
    }
}
