using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public class DBCLInternal : XPGUIDObject 
    {
        #region CTOR

        public DBCLInternal(Session session) : base(session)
        {
        }


        #endregion Design

        public static DBCLInternal FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCLInternal>(new BinaryOperator("Oid", oid));
        }

        #region Call

        private DBCLCall _Call;

        [Association("CLCall_CLInternal")]
        [DisplayName("تماس")]
        public DBCLCall Call
        {
            get { return _Call; }
            set { SetPropertyValue("Call", ref _Call, value); }
        }

        #endregion

        #region CallDate

        private DateTime _CallDate;

        [DisplayName("تاریخ")]
        public DateTime CallDate
        {
            get { return _CallDate; }
            set { SetPropertyValue("CallDate", ref _CallDate, value); }
        }

        #endregion

        #region Ext

        private int _Ext;

        [DisplayName("كد داخلی")]
        public int Ext
        {
            get { return _Ext; }
            set { SetPropertyValue("Ext", ref _Ext, value); }
        }

        #endregion

        #region DurationSeconds

        private int _DurationSeconds;

        [DisplayName("مدت مكالمه")]
        public int DurationSeconds
        {
            get { return _DurationSeconds; }
            set { SetPropertyValue("DurationSeconds", ref _DurationSeconds, value); }
        }

        #endregion

        #region InOrder

        private int _InOrder;

        [DisplayName("ترتیب")]
        public int InOrder
        {
            get { return _InOrder; }
            set { SetPropertyValue("InOrder", ref _InOrder, value); }
        }

        #endregion
    }
}
