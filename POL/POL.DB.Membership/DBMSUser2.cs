using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.Membership
{
    [OptimisticLocking(false)]
    public class DBMSUser2 : XPGUIDLogableObject 
    {
        #region Design
        public DBMSUser2(Session session) : base(session)
        {
        }
        #endregion

        #region Link Roles

        [Association("Users-Role")]
        public XPCollection<DBMSRole2> Roles
        {
            get { return GetCollection<DBMSRole2>("Roles"); }
        }

        #endregion

        #region Link Logs

        public XPCollection<DBMSLog2> Logs
        {
            get { return GetCollection<DBMSLog2>("Logs"); }
        }

        #endregion

        #region Link Settings

        public XPCollection<DBMSSetting2> Settings
        {
            get { return GetCollection<DBMSSetting2>("Settings"); }
        }

        #endregion

        public int GetLoginCount()
        {
            var xpq = new XPQuery<DBMSLog2>(Session);
            var q = from n in xpq where n.User.Oid == Oid && n.State == EnumUserLogState.Login select n;
            return q.Count();
        }

        #region Title

        private string _Title;

        [Size(64)]
        [PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Username

        private string _Username;

        [Size(32)]
        [PersianString]
        public string Username
        {
            get { return _Username; }
            set { SetPropertyValue("Username", ref _Username, value); }
        }

        #endregion

        #region UsernameLower

        private string _UsernameLower;

        [Size(32)]
        [PersianString]
        public string UsernameLower
        {
            get { return _UsernameLower; }
            set { SetPropertyValue("UsernameLower", ref _UsernameLower, value); }
        }

        #endregion

        #region Email

        private string _Email;

        [Size(64)]
        public string Email
        {
            get { return _Email; }
            set { SetPropertyValue("Email", ref _Email, value); }
        }

        #endregion

        #region EmailLower

        private string _EmailLower;

        [Size(64)]
        public string EmailLower
        {
            get { return _EmailLower; }
            set { SetPropertyValue("EmailLower", ref _EmailLower, value); }
        }

        #endregion

        #region Password

        private string _Password;

        [Size(SizeAttribute.Unlimited)]
        public string Password
        {
            get { return _Password; }
            set { SetPropertyValue("Password", ref _Password, value); }
        }

        #endregion

        #region Password2

        private string _Password2;

        [Size(SizeAttribute.Unlimited)]
        public string Password2
        {
            get { return _Password2; }
            set { SetPropertyValue("Password2", ref _Password2, value); }
        }

        #endregion

        #region Profile

        private string _Profile;

        [PersianString]
        [Size(SizeAttribute.Unlimited)]
        public string Profile
        {
            get { return _Profile; }
            set { SetPropertyValue("Profile", ref _Profile, value); }
        }

        #endregion

        #region IsApproved

        private bool _IsApproved;

        public bool IsApproved
        {
            get { return _IsApproved; }
            set { SetPropertyValue("IsApproved", ref _IsApproved, value); }
        }

        #endregion

        #region InternalCode

        private string _InternalCode;

        [Size(SizeAttribute.Unlimited)]
        public string InternalCode
        {
            get { return _InternalCode; }
            set { SetPropertyValue("InternalCode", ref _InternalCode, value); }
        }

        #endregion

        #region Mobile

        private string _Mobile;

        [Size(64)]
        public string Mobile
        {
            get { return _Mobile; }
            set { SetPropertyValue("Mobile", ref _Mobile, value); }
        }

        #endregion

        #region Methods

        public static DBMSUser2 UserCreate(Session session, string username, string password, string email, string title,
            bool isapproved)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);
            HelperValidation.CheckStartingSpace(username, "username");
            HelperValidation.CheckEndingSpace(username, "username");

            HelperValidation.CheckNullEmptySpaceMax(password, "password", 32);
            HelperValidation.CheckEmail(email, "email", 64);

            HelperValidation.CheckNullEmptySpaceMax(title, "title", 64);
            HelperValidation.CheckStartingSpace(title, "title");
            HelperValidation.CheckEndingSpace(title, "title");


            try
            {
                var pass = HelperSecurity.ComputeSaltHashSHA512(password, null);
                var pas2 = HelperSecurity.Encrypt(password, "123");
                var dbu = UserGetByName(session, username);
                if (dbu != null)
                    return null;

                dbu = new DBMSUser2(session)
                {
                    Username = username,
                    Password = pass,
                    Email = email,
                    Title = title,
                    IsApproved = isapproved,
                    Password2 = pas2
                };
                dbu.Save();
                DBMSLog2.AddLogEntry(dbu, EnumUserLogState.Create);
                return dbu;
            }
            catch
            {
                return null;
            }
        }

        public static DBMSUser2 UserUpdate(Session session, string username, string email, string title)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);
            HelperValidation.CheckEmail(email, "email", 64);
            HelperValidation.CheckNullEmptySpaceMax(title, "title", 64);

            var dbu = UserGetByName(session, username);
            if (dbu == null) return null;
            try
            {
                dbu.Email = email;
                dbu.Title = title;
                dbu.Save();
                DBMSLog2.AddLogEntry(dbu, EnumUserLogState.Update);
                return dbu;
            }
            catch
            {
            }
            return null;
        }

        public static bool UserDelete(Session session, string username)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);
            var dbu = UserGetByName(session, username);
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

        public static DBMSUser2 UserChangePassword(Session session, string username, string oldpass, string newpass)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);

            var dbu = UserValidate(session, username, oldpass);
            if (dbu == null) return null;
            var pass = HelperSecurity.ComputeSaltHashSHA512(newpass, null);
            var pas2 = HelperSecurity.Encrypt(newpass, "123");
            try
            {
                dbu.Password = pass;
                dbu.Password2 = pas2;
                dbu.Save();
                DBMSLog2.AddLogEntry(dbu, EnumUserLogState.ChangePassword);
                return dbu;
            }
            catch
            {
                return null;
            }
        }

        public static DBMSUser2 UserLogin(Session session, string username, string password)
        {
            var dbu = UserValidate(session, username, password);
            if (dbu == null) return null;
            DBMSLog2.AddLogEntry(dbu, EnumUserLogState.Login);
            return dbu;
        }

        public static void UserLogout(DBMSUser2 user)
        {
            DBMSLog2.AddLogEntry(user, EnumUserLogState.Logout);
        }

        public static DBMSUser2 UserResetPassword(Session session, string username, string newpass)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);
            HelperValidation.CheckNullEmptySpaceMax(newpass, "newpassword", 32);

            var dbu = UserGetByName(session, username);
            if (dbu == null) return null;

            var pass = HelperSecurity.ComputeSaltHashSHA512(newpass, null);
            var pas2 = HelperSecurity.Encrypt(newpass, "123");
            try
            {
                dbu.Password = pass;
                dbu.Password2 = pas2;
                dbu.Save();
                DBMSLog2.AddLogEntry(dbu, EnumUserLogState.ResetPassword);
                return dbu;
            }
            catch
            {
                return null;
            }
        }

        public static DBMSUser2 UserApprove(Session session, string username)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);

            var dbu = UserGetByName(session, username);
            if (dbu == null) return null;

            try
            {
                dbu.IsApproved = true;
                dbu.Save();
                DBMSLog2.AddLogEntry(dbu, EnumUserLogState.Approved);
                return dbu;
            }
            catch
            {
                return null;
            }
        }

        public static DBMSUser2 UserUnApprove(Session session, string username)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);

            var dbu = UserGetByName(session, username);
            if (dbu == null) return null;

            try
            {
                dbu.IsApproved = false;
                dbu.Save();
                DBMSLog2.AddLogEntry(dbu, EnumUserLogState.Unapproved);
                return dbu;
            }
            catch
            {
                return null;
            }
        }

        public static DBMSUser2 UserGetByOid(Session session, Guid oid)
        {
            HelperValidation.CheckSession(session);
            try
            {
                return session.FindObject<DBMSUser2>(new BinaryOperator("Oid", oid));
            }
            catch
            {
            }
            return null;
        }

        public static DBMSUser2 UserGetByName(Session session, string username)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);
            try
            {
                return session.FindObject<DBMSUser2>(new BinaryOperator("UsernameLower", username.ToLower()));
            }
            catch
            {
            }
            return null;
        }

        public static DBMSUser2 UserValidate(Session session, string username, string password)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);
            HelperValidation.CheckNullEmptySpaceMax(password, "password", 32);

            var dbu = UserGetByName(session, username);
            if (dbu == null) return null;
            return HelperSecurity.VerifySaltHashSHA512(password, dbu.Password) ? dbu : null;
        }

        public static bool UserExistByName(Session session, string username)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(username, "username", 32);
            return UserGetByName(session, username) != null;
        }

        public static DBMSUser2 UserExistByEmail(Session session, string email)
        {
            HelperValidation.CheckSession(session);
            HelperValidation.CheckNullEmptySpaceMax(email, "email", 64);
            try
            {
                return session.FindObject<DBMSUser2>(new BinaryOperator("EmailLower", email.ToLower()));
            }
            catch
            {
            }
            return null;
        }

        public static XPCollection<DBMSUser2> UserGetAll(Session session, string username)
        {
            HelperValidation.CheckSession(session);
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(username))
            {
                var un = username.Replace("*", "");
                un = un.Replace("%", "");
                un = un.Trim();
                if (!string.IsNullOrWhiteSpace(un))
                {
                    un = string.Format("%{0}%", un);
                    bo = new BinaryOperator("UsernameLower", un.ToLower(), BinaryOperatorType.Like);
                }
            }
            var xpc = new XPCollection<DBMSUser2>(session, bo);
            return xpc;
        }

        public static XPCollection<DBMSUser2> UserGetAll(Session session, string username, bool isapproved)
        {
            HelperValidation.CheckSession(session);
            var go = new GroupOperator();
            var boa = new BinaryOperator("IsApproved", isapproved);
            go.Operands.Add(boa);
            if (!string.IsNullOrWhiteSpace(username))
            {
                var un = username.Replace("*", "");
                un = un.Replace("%", "");
                un = un.Trim();
                if (!string.IsNullOrWhiteSpace(un))
                {
                    un = string.Format("%{0}%", un);
                    go.Operands.Add(new BinaryOperator("UsernameLower", un.ToLower(), BinaryOperatorType.Like));
                }
            }
            var xpc = new XPCollection<DBMSUser2>(session, go);
            return xpc;
        }


        public static DBMSUser2 FindDuplicateUserNameExcept(Session session, DBMSUser2 exceptUser, string username)
        {
            var go = new GroupOperator();
            if (exceptUser != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptUser.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("UsernameLower", HelperConvert.CorrectPersianBug(username.ToLower()),
                BinaryOperatorType.Equal));
            return session.FindObject<DBMSUser2>(go);
        }

        #endregion

        #region Override Methods

        public override string ToString()
        {
            return Title;
        }

        protected override void OnSaving()
        {
            UsernameLower = Username.ToLower();
            EmailLower = Email.ToLower();
            base.OnSaving();
        }

        #endregion

    }
}
