using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTContactRelMain : XPGUIDLogableObject 
    {
        #region Design

        public DBCTContactRelMain(Session session) : base(session)
        {
        }
        #endregion

        #region Link [1-n] - Contact Rel Sub

        public XPCollection<DBCTContactRelSub> RelSubs
        {
            get { return GetCollection<DBCTContactRelSub>("RelSubs"); }
        }

        #endregion

        public static DBCTContactRelMain FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTContactRelMain>(new BinaryOperator("Oid", oid));
        }

        public static DBCTContactRelMain FindDuplicateTitleExcept(Session session, DBCTContactRelMain exceptContactCat,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContactCat != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContactCat.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTContactRelMain>(go);
        }

        public static XPCollection<DBCTContactRelMain> GetAll(Session session)
        {
            return new XPCollection<DBCTContactRelMain>(session);
        }

        public override string ToString()
        {
            return Title;
        }

        #region Title

        private string _Title;

        [PersianString]
        [Size(32)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion
    }
}
