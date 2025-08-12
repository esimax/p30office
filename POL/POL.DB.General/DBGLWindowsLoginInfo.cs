using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using POL.DB.Root;

namespace POL.DB.General
{
    [OptimisticLocking(false)]
    public class DBGLWindowsLoginInfo : XPGUIDObject
    {
        #region Design

        public DBGLWindowsLoginInfo()
        {
        }

        public DBGLWindowsLoginInfo(Session session)
            : base(session)
        {
        }

        protected DBGLWindowsLoginInfo(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        #endregion

        #region MachineName

        private string _MachineName;

        [Size(128)]
        public string MachineName
        {
            get { return _MachineName; }
            set { SetPropertyValue("MachineName", ref _MachineName, value); }
        }

        #endregion

        #region UserName

        private string _UserName;

        [Size(128)]
        public string UserName
        {
            get { return _UserName; }
            set { SetPropertyValue("UserName", ref _UserName, value); }
        }

        #endregion

    }
}
