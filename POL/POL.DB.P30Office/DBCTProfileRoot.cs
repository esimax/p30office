using System;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTProfileRoot : XPGUIDLogableObject 
    {
        #region Design

        public DBCTProfileRoot(Session session) : base(session)
        {
        }

        #endregion

        #region Link [1-n] - ProfileGroup

        public XPCollection<DBCTProfileGroup> Groups
        {
            get { return GetCollection<DBCTProfileGroup>("Groups"); }
        }

        #endregion

        #region Link [n-n] - ContactCat

        [Association("CTContactCats_CTProfileRoot")]
        public XPCollection<DBCTContactCat> ContactCats
        {
            get { return GetCollection<DBCTContactCat>("ContactCats"); }
        }

        #endregion

        public static DBCTProfileRoot FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTProfileRoot>(new BinaryOperator("Oid", oid));
        }

        public static DBCTProfileRoot FindDuplicateTitleExcept(Session session, DBCTProfileRoot exceptRoot, string title)
        {
            var go = new GroupOperator();
            if (exceptRoot != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptRoot.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTProfileRoot>(go);
        }

        public static XPCollection<DBCTProfileRoot> GetAll(Session session)
        {
            return new XPCollection<DBCTProfileRoot>(session, null,
                new SortProperty("Order", SortingDirection.Ascending),
                new SortProperty("Title", SortingDirection.Ascending));
        }

        public static string GetProfileItemUriString()
        {
            var sb = new StringBuilder();
            sb.Append(UsedConstants.POLImagePath);
            sb.Append("Special/16/_16_TabPage.png");
            return sb.ToString();
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

        #region RoleViewer

        private string _RoleViewer;

        [PersianString]
        [Size(64)]
        public string RoleViewer
        {
            get { return _RoleViewer; }
            set { SetPropertyValue("RoleViewer", ref _RoleViewer, value); }
        }

        #endregion

        #region RoleEditor

        private string _RoleEditor;

        [PersianString]
        [Size(64)]
        public string RoleEditor
        {
            get { return _RoleEditor; }
            set { SetPropertyValue("RoleEditor", ref _RoleEditor, value); }
        }

        #endregion
    }
}
