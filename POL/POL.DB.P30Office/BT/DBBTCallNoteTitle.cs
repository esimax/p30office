using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.BT
{
    [OptimisticLocking(false)]
    public class DBBTCallNoteTitle2 : XPGUIDLogableObject 
    {
        #region CTOR

        public DBBTCallNoteTitle2(Session session)
            : base(session)
        {
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }


        public static DBBTCallNoteTitle2 FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBBTCallNoteTitle2>(new BinaryOperator("Oid", oid));
        }

        public static DBBTCallNoteTitle2 FindDuplicateTitleExcept(Session session, DBBTCallNoteTitle2 exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBBTCallNoteTitle2>(go);
        }

        public static XPCollection<DBBTCallNoteTitle2> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBBTCallNoteTitle2>(dxs, bo, new SortProperty("Title", SortingDirection.Ascending));
        }

        public static void DeleteMoreThan(Session dxs, int count)
        {
            var xpq = new XPQuery<DBBTCallNoteTitle2>(dxs);
            var q = (from n in xpq orderby n.DateModified select n).Skip(count);
            if (!q.Any()) return;
            var list = q.ToList();
            list.ForEach(n =>
            {
                n.Delete();
                n.Save();
            });
        }

        #region Title

        private string _Title;

        [Size(128), PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region LastColorIndex

        private int _LastColorIndex;

        public int LastColorIndex
        {
            get { return _LastColorIndex; }
            set { SetPropertyValue("LastColorIndex", ref _LastColorIndex, value); }
        }

        #endregion
    }
}
