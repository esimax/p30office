using DevExpress.Xpo;
using POL.DB.General;

namespace POL.DB.P30Office
{
    public class DBCTGUIDHolder : DBGLGUIDHolder
    {
        #region ctor

        public DBCTGUIDHolder(Session session) : base(session)
        {
        }

        #endregion

        #region ContactSelection

        private DBCTContactSelection _ContactSelection;

        [Association("CSGHs")]
        public DBCTContactSelection ContactSelection
        {
            get { return _ContactSelection; }
            set { SetPropertyValue("ContactSelection", ref _ContactSelection, value); }
        }

        #endregion
    }
}
