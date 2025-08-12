using System;
using System.Linq;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public class DBUpdatePatch : XPGUIDObject
    {
        #region CTOR

        public DBUpdatePatch(Session session)
            : base(session)
        {
        }

        #endregion

        #region PatchCode

        private Guid _PatchCode;

        public Guid PatchCode
        {
            get { return _PatchCode; }
            set
            {
                _PatchCode = value;
                SetPropertyValue("PatchCode", ref _PatchCode, value);
            }
        }

        #endregion

        #region Methods

        public static bool HasUpdated(Session dxs, Guid guid)
        {
            var q = new XPQuery<DBUpdatePatch>(dxs);
            return q.Any(n => n.PatchCode == guid);
        }

        public static void PatchDone(Session dxs, Guid patchCode)
        {
            var db = new DBUpdatePatch(dxs)
            {
                PatchCode = patchCode
            };
            db.Save();
        }

        #endregion
    }
}
