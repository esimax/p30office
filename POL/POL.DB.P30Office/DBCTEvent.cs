using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public class DBCTEvent : XPGUIDLogableObject 
    {
        #region Design

        public DBCTEvent(Session session) : base(session)
        {
        }
        #endregion

        public static DBCTEvent FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTEvent>(new BinaryOperator("Oid", oid));
        }

        public static XPCollection<DBCTEvent> GetAll(Session session)
        {
            return new XPCollection<DBCTEvent>(session);
        }

        public static XPCollection<DBCTEvent> GetByKey(Session session, string key)
        {
            return new XPCollection<DBCTEvent>(session, new BinaryOperator("Key", key),
                new SortProperty("Priority", SortingDirection.Ascending));
        }

        #region Title

        private string _Title;

        [Size(64)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Key

        private string _Key;

        [Size(64)]
        public string Key
        {
            get { return _Key; }
            set { SetPropertyValue("Key", ref _Key, value); }
        }

        #endregion

        #region Priority

        private int _Priority;

        public int Priority
        {
            get { return _Priority; }
            set { SetPropertyValue("Priority", ref _Priority, value); }
        }

        #endregion

        #region Script

        private string _Script;

        [Size(SizeAttribute.Unlimited)]
        public string Script
        {
            get { return _Script; }
            set { SetPropertyValue("Script", ref _Script, value); }
        }

        #endregion

        #region Errors

        private int _Errors;

        public int Errors
        {
            get { return _Errors; }
            set { SetPropertyValue("Errors", ref _Errors, value); }
        }

        #endregion

        #region RunCount

        private int _RunCount;

        public int RunCount
        {
            get { return _RunCount; }
            set { SetPropertyValue("RunCount", ref _RunCount, value); }
        }

        #endregion
    }
}
