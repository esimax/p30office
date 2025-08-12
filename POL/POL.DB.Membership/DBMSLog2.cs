using System;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.Membership
{
    [OptimisticLocking(false), DeferredDeletion(false)]
    public class DBMSLog2 : XPGUIDObject
    {
        #region Design
        public DBMSLog2(Session session) : base(session)
        {
        }
        #endregion

        internal static void AddLogEntry(DBMSUser2 user, EnumUserLogState state)
        {
            try
            {
                var l = new DBMSLog2(user.Session) {User = user, State = state, DateLog = DateTime.Now};
                l.Save();
                user.Logs.Add(l);
                user.Save();
            }
            catch
            {
            }
        }

        #region DateLog

        private DateTime? _DateLog;

        public DateTime? DateLog
        {
            get { return _DateLog; }
            set
            {
                if (value == DateTime.MinValue)
                    SetPropertyValue("DateLog", ref _DateLog, null);
                else
                    SetPropertyValue("DateLog", ref _DateLog, value);
            }
        }

        #endregion

        #region State

        private EnumUserLogState _State;

        public EnumUserLogState State
        {
            get { return _State; }
            set { SetPropertyValue("State", ref _State, value); }
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
    }
}
