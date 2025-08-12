using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using POL.DB.Root;

namespace POL.DB.General
{
    [OptimisticLocking(false)]
    public class DBGLLookupValue : XPGUIDObject
    {
        #region Design

        public DBGLLookupValue()
        {
        }

        public DBGLLookupValue(Session session)
            : base(session)
        {
        }

        protected DBGLLookupValue(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        #endregion

        #region Val1

        private string _Val1;

        [PersianString, Size(256)]
        public string Val1
        {
            get { return _Val1; }
            set { SetPropertyValue("Val1", ref _Val1, value); }
        }

        #endregion

        #region Val2

        private string _Val2;

        [PersianString, Size(256)]
        public string Val2
        {
            get { return _Val2; }
            set { SetPropertyValue("Val2", ref _Val2, value); }
        }

        #endregion

        #region Val3

        private string _Val3;

        [PersianString, Size(256)]
        public string Val3
        {
            get { return _Val3; }
            set { SetPropertyValue("Val3", ref _Val3, value); }
        }

        #endregion

        #region LookupTitle

        private DBGLLookupTitle _LookupTitle;

        [Association("GLLookupTitle-GLLookupValues")]
        public DBGLLookupTitle LookupTitle
        {
            get { return _LookupTitle; }
            set { SetPropertyValue("LookupTitle", ref _LookupTitle, value); }
        }

        #endregion
    }
}
