using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.Membership
{
    [OptimisticLocking(false), DeferredDeletion(false)]
    public class DBMSRole2 : XPGUIDLogableObject
    {
        #region Design
        public DBMSRole2(Session session) : base(session)
        {
        }
        #endregion

        #region Link Users

        [Association("Users-Role")]
        public XPCollection<DBMSUser2> Users
        {
            get { return GetCollection<DBMSUser2>("Users"); }
        }

        #endregion

        #region Title

        private string _Title;

        [Size(32)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region TitleLower

        private string _TitleLower;

        [Size(32)]
        public string TitleLower
        {
            get { return _TitleLower; }
            set { SetPropertyValue("TitleLower", ref _TitleLower, value); }
        }

        #endregion

        #region Permissions

        private string _Permissions;

        [Size(SizeAttribute.Unlimited)]
        public string Permissions
        {
            get { return _Permissions; }
            set { SetPropertyValue("Permissions", ref _Permissions, value); }
        }

        #endregion

        #region Methods

        public static DBMSRole2 RoleCreate(Session session, string title)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(title, "title", 32);

            var dbu = RoleGetByTitle(session, title);
            if (dbu != null)
                return null;
            try
            {
                dbu = new DBMSRole2(session) {Title = title};
                dbu.Save();
                return dbu;
            }
            catch 
            {
                return null;
            }
        }

        public static DBMSRole2 RoleUpdate(Session session, string title)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(title, "title", 32);

            var dbu = RoleGetByTitle(session, title);
            if (dbu == null) return null;
            try
            {
                dbu.Title = title;
                dbu.Save();
                return dbu;
            }
            catch
            {
            }
            return null;
        }

        public static bool RoleDelete(Session session, string title)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(title, "title", 32);

            var dbu = RoleGetByTitle(session, title);
            if (dbu == null) return false;
            try
            {
                dbu.Delete();
                dbu.Save();
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static DBMSRole2 FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBMSRole2>(new BinaryOperator("Oid", oid));
        }

        private static DBMSRole2 RoleGetByTitle(Session session, string title)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(title, "title", 32);
            try
            {
                return session.FindObject<DBMSRole2>(new BinaryOperator("TitleLower", title.ToLower()));
            }
            catch
            {
            }
            return null;
        }

        public static bool RoleExistByTitle(Session session, string title)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(title, "title", 32);
            return RoleGetByTitle(session, title) != null;
        }

        public static XPCollection<DBMSRole2> RoleGetAll(Session session, string title)
        {
            HelperValidation.CheckSession(session);
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(title))
            {
                var un = title.Replace("*", "");
                un = un.Replace("%", "");
                un = un.Trim();
                if (!string.IsNullOrWhiteSpace(un))
                {
                    un = string.Format("%{0}%", un);
                    bo = new BinaryOperator("TitleLower", un.ToLower(), BinaryOperatorType.Like);
                }
            }
            var xpc = new XPCollection<DBMSRole2>(session, bo);
            return xpc;
        }

        public static DBMSRole2 FindByTitleExcept(Session session, string title, DBMSRole2 exceptRole)
        {
            var go = new GroupOperator();
            if (exceptRole != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptRole.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("TitleLower", HelperConvert.CorrectPersianBug(title.ToLower()),
                BinaryOperatorType.Equal));
            return session.FindObject<DBMSRole2>(go);
        }

        #endregion

        #region Override Methods

        protected override void OnSaving()
        {
            TitleLower = Title.ToLower();
            base.OnSaving();
        }

        public override string ToString()
        {
            return Title;
        }

        #endregion
    }
}
