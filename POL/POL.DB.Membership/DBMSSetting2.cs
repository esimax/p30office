using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.Membership
{
    [OptimisticLocking(false), DeferredDeletion(false)]
    public class DBMSSetting2 : XPGUIDObject
    {
        static DBMSSetting2()
        {
            InLoadMode = false;
        }

        #region Design

        public DBMSSetting2(Session session) : base(session)
        {
        #endregion

        public static List<DBMSSetting2> CachStatics { get; set; }

        public static bool InLoadMode { get; set; }

        #region Title

        private string _Title;

        [Size(32)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Value

        private string _Value;

        [Size(SizeAttribute.Unlimited)]
        [PersianString]
        public string Value
        {
            get { return _Value; }
            set { SetPropertyValue("Value", ref _Value, value); }
        }

        #endregion

        #region User

        private DBMSUser2 _User;
        public DBMSUser2 User
        {
            get { return _User; }
            set { SetPropertyValue("User", ref _User, value); }
        }

        #endregion

        #region Methods

        public static void SaveSetting<T>(Session session, Guid userID, string title, T value)
        {
            if (InLoadMode) return;
            try
            {
                var db = session.FindObject<DBMSSetting2>(
                    new GroupOperator(
                        new BinaryOperator("User.Oid", userID),
                        new BinaryOperator("Title", title)
                        ));

                if (db == null)
                {
                    using (var uow = new UnitOfWork(session.DataLayer))
                    {
                        var u = uow.FindObject<DBMSUser2>(new BinaryOperator("Oid", userID));
                        if (u == null)
                            throw new Exception(string.Format("Can not find user with Oid :{0}", userID));
                        db = new DBMSSetting2(uow)
                        {
                            Title = title,
                            Value = ReferenceEquals(value, null) ? string.Empty : value.ToString(),
                            User = u
                        };
                        db.Save();
                        uow.CommitChanges();

                        CachStatics =
                            new XPCollection<DBMSSetting2>(session, new BinaryOperator("User.Oid", userID)).ToList();
                    }
                }
                else
                {
                    db.Value = ReferenceEquals(value, null) ? string.Empty : value.ToString();
                    db.Save();

                    CachStatics.First(n => n.Title == title).Value = db.Value;
                }
            }
            catch (Exception ex)
            {
                HelperUtils.Try(() =>
                {
                    var log = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                    log.Log(ex.ToString(), Category.Exception, Priority.Medium);
                });
            }
        }

        public static T LoadSettings<T>(Session session, Guid userID, string title)
        {
            if (CachStatics == null)
            {
                CachStatics = new XPCollection<DBMSSetting2>(session, new BinaryOperator("User.Oid", userID)).ToList();
            }
            var db = CachStatics.FirstOrDefault(n => n.Title == title);
            return db == null ? default(T) : (T) Convert.ChangeType(db.Value, typeof (T));
        }

        #endregion
    }
}
