using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBTMAutomation : XPGUIDLogableObject 
    {
        #region Design

        public DBTMAutomation(Session session) : base(session)
        {
        }
        #endregion

        public override string ToString()
        {
            return Title;
        }


        public static DBTMAutomation FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBTMAutomation>(new BinaryOperator("Oid", oid));
        }

        public static DBTMAutomation FindDuplicateTitleExcept(Session session, DBTMAutomation exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBTMAutomation>(go);
        }

        public static XPCollection<DBTMAutomation> GetAll(Session session)
        {
            return new XPCollection<DBTMAutomation>(session);
        }

        #region Title

        private string _Title;

        [Size(64)]
        [DisplayName("عنوان اتوماسیون")]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region PopupEnable

        private bool _PopupEnable;

        public bool PopupEnable
        {
            get { return _PopupEnable; }
            set { SetPropertyValue("PopupEnable", ref _PopupEnable, value); }
        }

        #endregion

        #region PopupText

        private string _PopupText;

        [Size(1024)]
        public string PopupText
        {
            get { return _PopupText; }
            set { SetPropertyValue("PopupText", ref _PopupText, value); }
        }

        #endregion

        #region EmailEnable

        private bool _EmailEnable;

        public bool EmailEnable
        {
            get { return _EmailEnable; }
            set { SetPropertyValue("EmailEnable", ref _EmailEnable, value); }
        }

        #endregion

        #region EmailAccount

        private DBEMEmailApp _EmailAccount;

        public DBEMEmailApp EmailAccount
        {
            get { return _EmailAccount; }
            set { SetPropertyValue("EmailAccount", ref _EmailAccount, value); }
        }

        #endregion

        #region EmailTemplate

        private DBEMTemplate _EmailTemplate;

        public DBEMTemplate EmailTemplate
        {
            get { return _EmailTemplate; }
            set { SetPropertyValue("EmailTemplate", ref _EmailTemplate, value); }
        }

        #endregion

        #region EmailType

        private int _EmailType;

        public int EmailType
        {
            get { return _EmailType; }
            set { SetPropertyValue("EmailType", ref _EmailType, value); }
        }

        #endregion

        #region EmailData

        private string _EmailData;

        [Size(1024)]
        public string EmailData
        {
            get { return _EmailData; }
            set { SetPropertyValue("EmailData", ref _EmailData, value); }
        }

        #endregion

        #region SMSEnable

        private bool _SMSEnable;

        public bool SMSEnable
        {
            get { return _SMSEnable; }
            set { SetPropertyValue("SMSEnable", ref _SMSEnable, value); }
        }

        #endregion

        #region SMSText

        private string _SMSText;

        [Size(2048)]
        public string SMSText
        {
            get { return _SMSText; }
            set { SetPropertyValue("SMSText", ref _SMSText, value); }
        }

        #endregion

        #region SMSType

        private int _SMSType;

        public int SMSType
        {
            get { return _SMSType; }
            set { SetPropertyValue("SMSType", ref _SMSType, value); }
        }

        #endregion

        #region SMSData

        private string _SMSData;

        [Size(1024)]
        public string SMSData
        {
            get { return _SMSData; }
            set { SetPropertyValue("SMSData", ref _SMSData, value); }
        }

        #endregion

        #region SMSSendToAnyOne

        private bool _SMSSendToAnyOne;

        public bool SMSSendToAnyOne
        {
            get { return _SMSEnable; }
            set { SetPropertyValue("SMSSendToAnyOne", ref _SMSSendToAnyOne, value); }
        }

        #endregion

        #region SMSNumbersAnyone

        private string _SMSNumbersAnyone;

        [Size(1024)]
        public string SMSNumbersAnyone
        {
            get { return _SMSNumbersAnyone; }
            set { SetPropertyValue("SMSNumbersAnyone", ref _SMSNumbersAnyone, value); }
        }

        #endregion
    }
}
