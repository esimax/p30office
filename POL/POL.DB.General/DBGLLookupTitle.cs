using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using POL.DB.Root;

namespace POL.DB.General
{
    [OptimisticLocking(false)]
    public class DBGLLookupTitle : XPGUIDObject
    {
        #region Design

        public DBGLLookupTitle()
        {
        }

        public DBGLLookupTitle(Session session)
            : base(session)
        {
        }

        protected DBGLLookupTitle(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        #endregion

        #region Title

        private string _Title;

        [PersianString, Size(32)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region MaximumCount

        private int _MaximumCount;

        public int MaximumCount
        {
            get { return _MaximumCount; }
            set { SetPropertyValue("MaximumCount", ref _MaximumCount, value); }
        }

        #endregion

        #region LookupValues

        [Association("GLLookupTitle-GLLookupValues")]
        public XPCollection<DBGLLookupValue> LookupValues
        {
            get { return GetCollection<DBGLLookupValue>("LookupValues"); }
        }

        #endregion
    }
}
