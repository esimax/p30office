using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office.WF
{
    public class DBWFStageLog : XPGUIDLogableObject 
    {
        #region CTOR


        public DBWFStageLog(Session session)
            : base(session)
        {
        }



        #endregion

        public static DBWFStageLog FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBWFStageLog>(new BinaryOperator("Oid", oid));
        }

        #region From

        private DBWFStage _From;

        public DBWFStage From
        {
            get { return _From; }
            set { SetPropertyValue("From", ref _From, value); }
        }

        #endregion

        #region To

        private DBWFStage _To;

        public DBWFStage To
        {
            get { return _To; }
            set { SetPropertyValue("To", ref _To, value); }
        }

        #endregion

        #region Date

        private DateTime _Date;

        public DateTime Date
        {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }

        #endregion

        #region Username

        private string _Username;

        [Size(32)]
        [PersianString]
        public string Username
        {
            get { return _Username; }
            set { SetPropertyValue("Username", ref _Username, value); }
        }

        #endregion
    }
}
