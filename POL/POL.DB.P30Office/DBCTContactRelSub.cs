using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTContactRelSub : XPGUIDLogableObject 
    {
        #region Design

        public DBCTContactRelSub(Session session) : base(session)
        {
        }
        #endregion

        public static DBCTContactRelSub FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTContactRelSub>(new BinaryOperator("Oid", oid));
        }

        public static DBCTContactRelSub FindDuplicateTitleExcept(Session session, Guid relMainOid,
            DBCTContactRelSub exceptContactCat, string title)
        {
            var go = new GroupOperator();
            if (exceptContactCat != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContactCat.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("RelMain.Oid", relMainOid, BinaryOperatorType.Equal));
            return session.FindObject<DBCTContactRelSub>(go);
        }

        public static XPCollection<DBCTContactRelSub> GetByMainOid(Session session, Guid mainOid)
        {
            return new XPCollection<DBCTContactRelSub>(session, new BinaryOperator("RelMain.Oid", mainOid));
        }

        public static XPCollection<DBCTContactRelSub> GetByMainTitle(Session session, string mainTitle)
        {
            return new XPCollection<DBCTContactRelSub>(session, new BinaryOperator("RelMain.Title", mainTitle));
        }

        public static XPCollection<DBCTContactRelSub> GetAll(Session session)
        {
            return new XPCollection<DBCTContactRelSub>(session);
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

        #region Link [n-1] - Contact Rel Main

        private DBCTContactRelMain _RelMain;

        public DBCTContactRelMain RelMain
        {
            get { return _RelMain; }
            set { SetPropertyValue("RelMain", ref _RelMain, value); }
        }

        #endregion
    }
}
