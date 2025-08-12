using System;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.P30Office
{
    public class DBSMLog2 : XPGUIDObject
    {
        #region Design

        public DBSMLog2(Session session) : base(session)
        {
        }

        #endregion

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

        #region TransId

        private long _TransId;

        public long TransId
        {
            get { return _TransId; }
            set { SetPropertyValue("TransId", ref _TransId, value); }
        }

        #endregion

        #region SmsType

        private EnumSmsType _SmsType;

        public EnumSmsType SmsType
        {
            get { return _SmsType; }
            set { SetPropertyValue("SMSType", ref _SmsType, value); }
        }

        #endregion

        #region SmsPriority

        private EnumSmsPriority _SmsPriority;

        public EnumSmsPriority SmsPriority
        {
            get { return _SmsPriority; }
            set { SetPropertyValue("SmsPriority", ref _SmsPriority, value); }
        }

        #endregion

        #region SmsResult

        private string _SmsResult;

        [Size(SizeAttribute.Unlimited)]
        public string SmsResult
        {
            get { return _SmsResult; }
            set { SetPropertyValue("SmsResult", ref _SmsResult, value); }
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

        #region DelivaryResult

        private string _DelivaryResult;

        [Size(SizeAttribute.Unlimited)]
        public string DelivaryResult
        {
            get { return _DelivaryResult; }
            set { SetPropertyValue("DelivaryResult", ref _DelivaryResult, value); }
        }

        #endregion

        #region Sender

        private string _Sender;

        [Size(64), PersianString]
        public string Sender
        {
            get { return _Sender; }
            set { SetPropertyValue("Sender", ref _Sender, value); }
        }

        #endregion

    }
}
