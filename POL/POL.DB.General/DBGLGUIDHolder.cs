using System;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.General
{
    [OptimisticLocking(false), DeferredDeletion(false)]
    public class DBGLGUIDHolder : XPGUIDObject
    {
        #region Design
        public DBGLGUIDHolder(Session session) : base(session)
        {
        }
        #endregion

        #region GUID

        private Guid _GUID;

        public Guid GUID
        {
            get { return _GUID; }
            set { SetPropertyValue("GUID", ref _GUID, value); }
        }

        #endregion
    }
}
