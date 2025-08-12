using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBEMTemplate : XPGUIDLogableObject
    {
        #region Design

        public DBEMTemplate(Session session) : base(session)
        {
        }

        #endregion

        #region Template - Parameters

        [Association("EMTemplate_EMTempParams"), Aggregated]
        public XPCollection<DBEMTempParams> Parameters
        {
            get { return GetCollection<DBEMTempParams>("Parameters"); }
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }


        public static DBEMTemplate FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBEMTemplate>(new BinaryOperator("Oid", oid));
        }

        public static DBEMTemplate FindDuplicateTitleExcept(Session session, DBEMTemplate exceptContact, string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBEMTemplate>(go);
        }

        public static XPCollection<DBEMTemplate> GetAll(Session session)
        {
            return new XPCollection<DBEMTemplate>(session);
        }

        #region Title

        private string _Title;

        [Size(128)]
        [PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region HTMLBody

        private string _HTMLBody;

        [Size(SizeAttribute.Unlimited)]
        public string HTMLBody
        {
            get { return _HTMLBody; }
            set { SetPropertyValue("HTMLBody", ref _HTMLBody, value); }
        }

        #endregion
    }
}
