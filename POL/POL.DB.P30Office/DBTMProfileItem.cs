using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public class DBTMProfileItem : XPGUIDLogableObject 
    {
        #region Design

        public DBTMProfileItem(Session session) : base(session)
        {
        }
        #endregion

        public static DBTMProfileItem FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBTMProfileItem>(new BinaryOperator("Oid", oid));
        }

        public static DBTMProfileItem FindByProfileItemOid(Session session, Guid oid)
        {
            return session.FindObject<DBTMProfileItem>(new BinaryOperator("DataItemOid", oid));
        }

        public static XPCollection<DBTMProfileItem> GetAllSatisfiedItems(Session session)
        {
            var now = DateTime.Now;
            var h = now.Hour;
            var m = now.Minute;
            var time = h.ToString().PadLeft(2, '0') + ":" + m.ToString().PadLeft(2, '0');
            var bo = new BinaryOperator("ReactionTime", time);
            var xpc = new XPCollection<DBTMProfileItem>(session, bo);
            return xpc;
        }

        #region DataItemOid

        private Guid _DataItemOid;

        public Guid DataItemOid
        {
            get { return _DataItemOid; }
            set { SetPropertyValue("DataItemOid", ref _DataItemOid, value); }
        }

        #endregion

        #region AutomationOid

        private Guid _AutomationOid;

        public Guid AutomationOid
        {
            get { return _AutomationOid; }
            set { SetPropertyValue("AutomationOid", ref _AutomationOid, value); }
        }

        #endregion

        #region ReactionTime

        private string _ReactionTime;

        [Size(5)]
        public string ReactionTime
        {
            get { return _ReactionTime; }
            set { SetPropertyValue("ReactionTime", ref _ReactionTime, value); }
        }

        #endregion

        #region ReactionPeriod

        private int _ReactionPeriod;

        public int ReactionPeriod
        {
            get { return _ReactionPeriod; }
            set { SetPropertyValue("ReactionPeriod", ref _ReactionPeriod, value); }
        }

        #endregion

        #region ReactionLast

        private DateTime _ReactionLast;

        public DateTime ReactionLast
        {
            get { return _ReactionLast; }
            set { SetPropertyValue("ReactionLast", ref _ReactionLast, value); }
        }

        #endregion

        #region OnOnce

        private bool _OnOnce;

        public bool OnOnce
        {
            get { return _OnOnce; }
            set { SetPropertyValue("OnOnce", ref _OnOnce, value); }
        }

        #endregion

        #region OnYear

        private bool _OnYear;

        public bool OnYear
        {
            get { return _OnYear; }
            set { SetPropertyValue("OnYear", ref _OnYear, value); }
        }

        #endregion

        #region OnMonth

        private bool _OnMonth;

        public bool OnMonth
        {
            get { return _OnMonth; }
            set { SetPropertyValue("OnMonth", ref _OnMonth, value); }
        }

        #endregion

        #region OnChange

        private bool _OnChange;

        public bool OnChange
        {
            get { return _OnChange; }
            set { SetPropertyValue("OnChange", ref _OnChange, value); }
        }

        #endregion

        #region OnChangeValue

        private string _OnChangeValue;

        [Size(128)]
        public string OnChangeValue
        {
            get { return _OnChangeValue; }
            set { SetPropertyValue("OnChangeValue", ref _OnChangeValue, value); }
        }

        #endregion
    }
}
