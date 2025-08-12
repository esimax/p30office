using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.BT
{

    [OptimisticLocking(false)]
    public class DBBTEmailTitle2 : XPGUIDLogableObject 
    {
        #region CTOR
        public DBBTEmailTitle2(Session session)
            : base(session)
        {
        }
        #endregion

        public override string ToString()
        {
            return Title;
        }

        public static DBBTEmailTitle2 FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBBTEmailTitle2>(new BinaryOperator("Oid", oid));
        }

        public static DBBTEmailTitle2 FindDuplicateTitleExcept(Session session, DBBTEmailTitle2 exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBBTEmailTitle2>(go);
        }

        public static XPCollection<DBBTEmailTitle2> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBBTEmailTitle2>(dxs, bo, new SortProperty("Title", SortingDirection.Ascending));
        }

        #region Title

        private string _Title;

        [Size(16), PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion
    }
}
