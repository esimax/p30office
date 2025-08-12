using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.AC
{
    [OptimisticLocking(false)]
    public class DBACFactorTemplate : XPGUIDLogableObject
    {
        #region CTOR

        public DBACFactorTemplate(Session session)
            : base(session)
        {
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }

        public static DBACFactorTemplate FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBACFactorTemplate>(new BinaryOperator("Oid", oid));
        }

        public static DBACFactorTemplate FindDuplicateTitleExcept(Session session, DBACFactorTemplate exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBACFactorTemplate>(go);
        }

        public static XPCollection<DBACFactorTemplate> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBACFactorTemplate>(dxs, bo, new SortProperty("Title", SortingDirection.Ascending));
        }

        #region Title

        private string _Title;

        [Size(100), PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Value

        private string _Value;

        [Size(SizeAttribute.Unlimited), PersianString]
        public string Value
        {
            get { return _Value; }
            set { SetPropertyValue("Value", ref _Value, value); }
        }

        #endregion
    }
}
