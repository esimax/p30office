using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.P30Office
{
    [Obsolete]
    public class DBSMLog : XPGUIDObject
    {
        #region Design

        public DBSMLog(Session session)
            : base(session)
        {
        }

        #endregion

        public static XPCollection<DBSMLog> GetReadyToSend(Session dxs, DateTime untill)
        {
            var go = new GroupOperator();

            go.Operands.Add(new BinaryOperator("SMSType", EnumSMSType.RequestToSend, BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("TransDate", untill, BinaryOperatorType.LessOrEqual));

            return new XPCollection<DBSMLog>(dxs, go);
        }

        public static XPCollection<DBSMLog> GetNeedDelivaryCheck(Session dxs, DateTime untill)
        {
            var go = new GroupOperator();

            go.Operands.Add(new BinaryOperator("SMSType", EnumSMSType.Send, BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("SMSDelivery", EnumSMSDelivery.NeedToCheck, BinaryOperatorType.Equal));
            go.Operands.Add(new BinaryOperator("TransDate", untill, BinaryOperatorType.LessOrEqual));
            go.Operands.Add(new BinaryOperator("DelivaryCount", 6, BinaryOperatorType.LessOrEqual));

            return new XPCollection<DBSMLog>(dxs, go);
        }

        public static DBSMLog FindByOid(Session dxs, Guid oid)
        {
            return dxs.FindObject<DBSMLog>(new BinaryOperator("Oid", oid));
        }

        #region From

        private string _From;

        [Size(32)]
        public string From
        {
            get { return _From; }
            set { SetPropertyValue("From", ref _From, value); }
        }

        #endregion

        #region To

        private string _To;

        [Size(32)]
        public string To
        {
            get { return _To; }
            set { SetPropertyValue("To", ref _To, value); }
        }

        #endregion

        #region TransDate

        private DateTime _TransDate;

        public DateTime TransDate
        {
            get { return _TransDate; }
            set { SetPropertyValue("TransDate", ref _TransDate, value); }
        }

        #endregion

        #region TransID

        private long _TransID;

        public long TransID
        {
            get { return _TransID; }
            set { SetPropertyValue("TransID", ref _TransID, value); }
        }

        #endregion

        #region SMSType

        private EnumSMSType _SMSType;

        public EnumSMSType SMSType
        {
            get { return _SMSType; }
            set { SetPropertyValue("SMSType", ref _SMSType, value); }
        }

        #endregion

        #region SMSSendResult

        private EnumSMSSendResult _SMSSendResult;

        public EnumSMSSendResult SMSSendResult
        {
            get { return _SMSSendResult; }
            set { SetPropertyValue("SMSSendResult", ref _SMSSendResult, value); }
        }

        #endregion

        #region SMSDelivery

        private EnumSMSDelivery _SMSDelivery;

        public EnumSMSDelivery SMSDelivery
        {
            get { return _SMSDelivery; }
            set { SetPropertyValue("SMSDelivery", ref _SMSDelivery, value); }
        }

        #endregion

        #region Body

        private string _Body;

        [Size(2048)]
        [PersianString]
        public string Body
        {
            get { return _Body; }
            set { SetPropertyValue("Body", ref _Body, value); }
        }

        #endregion

        #region Phone

        private DBCTPhoneBook _Phone;

        public DBCTPhoneBook Phone
        {
            get { return _Phone; }
            set { SetPropertyValue("Phone", ref _Phone, value); }
        }

        #endregion

        #region IsRTL

        private bool _IsRTL;

        public bool IsRTL
        {
            get { return _IsRTL; }
            set { SetPropertyValue("IsRTL", ref _IsRTL, value); }
        }

        #endregion

        #region NoteText

        private string _NoteText;

        [Size(128)]
        [PersianString]
        [DisplayName("متن نكته")]
        public string NoteText
        {
            get { return _NoteText; }
            set { SetPropertyValue("NoteText", ref _NoteText, value); }
        }

        #endregion

        #region NoteFlag

        private int _NoteFlag;

        [DisplayName("پرچم نكته")]
        public int NoteFlag
        {
            get { return _NoteFlag; }
            set { SetPropertyValue("NoteFlag", ref _NoteFlag, value); }
        }

        #endregion

        #region NoteWriter

        private string _NoteWriter;

        [Size(64)]
        [DisplayName("نویسنده نكته")]
        public string NoteWriter
        {
            get { return _NoteWriter; }
            set { SetPropertyValue("NoteWriter", ref _NoteWriter, value); }
        }

        #endregion

        #region DelivaryCount

        private int _DelivaryCount;

        public int DelivaryCount
        {
            get { return _DelivaryCount; }
            set { SetPropertyValue("DelivaryCount", ref _DelivaryCount, value); }
        }

        #endregion

        #region UpdateCode

        private int _UpdateCode;

        public int UpdateCode
        {
            get { return _UpdateCode; }
            set { SetPropertyValue("UpdateCode", ref _UpdateCode, value); }
        }

        #endregion
    }
}
