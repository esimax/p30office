using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCLCallFilter : XPGUIDLogableObject 
    {
        #region Design

        public DBCLCallFilter(Session session) : base(session)
        {
        }
        #endregion

        public static DBCLCallFilter FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCLCallFilter>(new BinaryOperator("Oid", oid));
        }

        public static DBCLCallFilter FindDuplicateTitleExcept(Session session, DBCLCallFilter exceptFilter, string title)
        {
            var go = new GroupOperator();
            if (exceptFilter != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptFilter.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCLCallFilter>(go);
        }

        public static XPCollection<DBCLCallFilter> GetAll(Session session)
        {
            return new XPCollection<DBCLCallFilter>(session);
        }

        public override string ToString()
        {
            return Title;
        }

        #region Title

        private string _Title;

        [PersianString]
        [Size(64)]
        [DisplayName("عنوان")]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region FilterString

        private string _FilterString;

        [Size(SizeAttribute.Unlimited)]
        [DisplayName("متن فیلتر")]
        public string FilterString
        {
            get { return _FilterString; }
            set { SetPropertyValue("FilterString", ref _FilterString, value); }
        }

        #endregion
    }
}
