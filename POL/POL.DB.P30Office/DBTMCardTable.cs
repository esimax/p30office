using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBTMCardTable2 : XPGUIDLogableObject 
    {
        #region Design

        public DBTMCardTable2(Session session) : base(session)
        {
        }
        #endregion

        #region LinkFile

        [Delayed(true)]
        public byte[] LinkFile
        {
            get { return GetDelayedPropertyValue<byte[]>("LinkFile"); }
            set { SetDelayedPropertyValue("LinkFile", value); }
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }


        public static DBTMCardTable2 FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBTMCardTable2>(new BinaryOperator("Oid", oid));
        }

        public static DBTMCardTable2 FindDuplicateTitleExcept(Session session, DBTMCardTable2 exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBTMCardTable2>(go);
        }

        public static XPCollection<DBTMCardTable2> GetAll(Session session)
        {
            return new XPCollection<DBTMCardTable2>(session);
        }

        public static XPCollection<DBTMCardTable2> GetAllWithAutomationStarting(Session session)
        {
            var d1 = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            var goand1 = new GroupOperator();
            goand1.Operands.Add(new BinaryOperator("HasStartingDate", true));
            goand1.Operands.Add(new BinaryOperator("HasStartingAutomation", true));
            goand1.Operands.Add(new BinaryOperator("StartingDate", d1, BinaryOperatorType.GreaterOrEqual));
            goand1.Operands.Add(new BinaryOperator("StartingDate", d1.AddMinutes(1), BinaryOperatorType.Less));
            goand1.Operands.Add(
                new NotOperator(new BinaryOperator("StartingAutomationID", Guid.Empty, BinaryOperatorType.Equal)));
            return new XPCollection<DBTMCardTable2>(session, goand1);
        }

        public static XPCollection<DBTMCardTable2> GetAllWithAutomationEnding(Session session)
        {
            var goand2 = new GroupOperator();
            goand2.Operands.Add(new BinaryOperator("HasEndingDate", true));
            goand2.Operands.Add(new BinaryOperator("HasEndingAutomation", true));
            goand2.Operands.Add(new BinaryOperator("EndingDate", DateTime.Now, BinaryOperatorType.GreaterOrEqual));
            goand2.Operands.Add(new BinaryOperator("EndingDate", DateTime.Now.AddMinutes(1), BinaryOperatorType.Less));
            goand2.Operands.Add(
                new NotOperator(new BinaryOperator("EndingAutomationID", Guid.Empty, BinaryOperatorType.Equal)));
            return new XPCollection<DBTMCardTable2>(session, goand2);
        }

        public static XPCollection<DBTMCardTable2> GetAllWithAutomationResult(Session session)
        {
            var goand2 = new GroupOperator();
            goand2.Operands.Add(new BinaryOperator("HasResultAutomation", true));
            goand2.Operands.Add(new BinaryOperator("ResultAutomationDone", false));
            return new XPCollection<DBTMCardTable2>(session, goand2);
        }

        #region Title

        private string _Title;

        [Size(64)]
        [DisplayName("عنوان كارتابل")]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region CreatorName

        private string _CreatorName;

        public string CreatorName
        {
            get { return _CreatorName; }
            set { SetPropertyValue("CreatorName", ref _CreatorName, value); }
        }

        #endregion

        #region SendToType

        private int _SendToType;

        public int SendToType
        {
            get { return _SendToType; }
            set { SetPropertyValue("SendToType", ref _SendToType, value); }
        }

        #endregion

        #region SendToData

        private Guid _SendToData;

        public Guid SendToData
        {
            get { return _SendToData; }
            set { SetPropertyValue("SendToData", ref _SendToData, value); }
        }

        #endregion

        #region SendTo

        private string _SendTo;

        [Size(64)]
        public string SendTo
        {
            get { return _SendTo; }
            set { SetPropertyValue("SendTo", ref _SendTo, value); }
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

        #region Note

        private string _Note;

        [Size(SizeAttribute.Unlimited)]
        public string Note
        {
            get { return _Note; }
            set { SetPropertyValue("Note", ref _Note, value); }
        }

        #endregion

        #region HasStartingDate

        private bool _HasStartingDate;

        public bool HasStartingDate
        {
            get { return _HasStartingDate; }
            set { SetPropertyValue("HasStartingDate", ref _HasStartingDate, value); }
        }

        #endregion

        #region HasStartingAutomation

        private bool _HasStartingAutomation;

        public bool HasStartingAutomation
        {
            get { return _HasStartingAutomation; }
            set { SetPropertyValue("HasStartingAutomation", ref _HasStartingAutomation, value); }
        }

        #endregion

        #region StartingDate

        private DateTime? _StartingDate;

        public DateTime? StartingDate
        {
            get { return _StartingDate; }
            set { SetPropertyValue("StartingDate", ref _StartingDate, value); }
        }

        #endregion

        #region StartingAutomationID

        private Guid _StartingAutomationID;

        public Guid StartingAutomationID
        {
            get { return _StartingAutomationID; }
            set { SetPropertyValue("StartingAutomationID", ref _StartingAutomationID, value); }
        }

        #endregion

        #region HasEndingDate

        private bool _HasEndingDate;

        public bool HasEndingDate
        {
            get { return _HasEndingDate; }
            set { SetPropertyValue("HasEndingDate", ref _HasEndingDate, value); }
        }

        #endregion

        #region HasEndingAutomation

        private bool _HasEndingAutomation;

        public bool HasEndingAutomation
        {
            get { return _HasEndingAutomation; }
            set { SetPropertyValue("HasEndingAutomation", ref _HasEndingAutomation, value); }
        }

        #endregion

        #region EndingDate

        private DateTime? _EndingDate;

        public DateTime? EndingDate
        {
            get { return _EndingDate; }
            set { SetPropertyValue("EndingDate", ref _EndingDate, value); }
        }

        #endregion

        #region EndingAutomationID

        private Guid _EndingAutomationID;

        public Guid EndingAutomationID
        {
            get { return _EndingAutomationID; }
            set { SetPropertyValue("EndingAutomationID", ref _EndingAutomationID, value); }
        }

        #endregion

        #region HasLinkToCall

        private bool _HasLinkToCall;

        public bool HasLinkToCall
        {
            get { return _HasLinkToCall; }
            set { SetPropertyValue("HasLinkToCall", ref _HasLinkToCall, value); }
        }

        #endregion

        #region HasLinkToEmail

        private bool _HasLinkToEmail;

        public bool HasLinkToEmail
        {
            get { return _HasLinkToEmail; }
            set { SetPropertyValue("HasLinkToEmail", ref _HasLinkToEmail, value); }
        }

        #endregion

        #region HasLinkToSMS

        private bool _HasLinkToSMS;

        public bool HasLinkToSMS
        {
            get { return _HasLinkToSMS; }
            set { SetPropertyValue("HasLinkToSMS", ref _HasLinkToSMS, value); }
        }

        #endregion

        #region HasLinkToContact

        private bool _HasLinkToContact;

        public bool HasLinkToContact
        {
            get { return _HasLinkToContact; }
            set { SetPropertyValue("HasLinkToContact", ref _HasLinkToContact, value); }
        }

        #endregion

        #region HasLinkToCategory

        private bool _HasLinkToCategory;

        public bool HasLinkToCategory
        {
            get { return _HasLinkToCategory; }
            set { SetPropertyValue("HasLinkToCategory", ref _HasLinkToCategory, value); }
        }

        #endregion

        #region HasLinkToFile

        private bool _HasLinkToFile;

        public bool HasLinkToFile
        {
            get { return _HasLinkToFile; }
            set { SetPropertyValue("HasLinkToFile", ref _HasLinkToFile, value); }
        }

        #endregion

        #region LinkCall

        private DBCLCall _LinkCall;

        public DBCLCall LinkCall
        {
            get { return _LinkCall; }
            set { SetPropertyValue("LinkCall", ref _LinkCall, value); }
        }

        #endregion

        #region LinkEmail

        private DBEMEmailInbox _LinkEmail;

        public DBEMEmailInbox LinkEmail
        {
            get { return _LinkEmail; }
            set { SetPropertyValue("LinkEmail", ref _LinkEmail, value); }
        }

        #endregion

        #region LinkSMS2

        private DBSMLog2 _LinkSMS2;

        public DBSMLog2 LinkSMS2
        {
            get { return _LinkSMS2; }
            set { SetPropertyValue("LinkSMS2", ref _LinkSMS2, value); }
        }

        #endregion

        #region LinkContact

        private DBCTContact _LinkContact;

        public DBCTContact LinkContact
        {
            get { return _LinkContact; }
            set { SetPropertyValue("LinkContact", ref _LinkContact, value); }
        }

        #endregion

        #region LinkCategory

        private DBCTContactCat _LinkCategory;

        public DBCTContactCat LinkCategory
        {
            get { return _LinkCategory; }
            set { SetPropertyValue("LinkCategory", ref _LinkCategory, value); }
        }

        #endregion

        #region LinkFileName

        private string _LinkFileName;

        [Size(256)]
        public string LinkFileName
        {
            get { return _LinkFileName; }
            set { SetPropertyValue("LinkFileName", ref _LinkFileName, value); }
        }

        #endregion

        #region HasResultAutomation

        private bool _HasResultAutomation;

        public bool HasResultAutomation
        {
            get { return _HasResultAutomation; }
            set { SetPropertyValue("HasResultAutomation", ref _HasResultAutomation, value); }
        }

        #endregion

        #region ResultAutomationID

        private Guid _ResultAutomationID;

        public Guid ResultAutomationID
        {
            get { return _ResultAutomationID; }
            set { SetPropertyValue("ResultAutomationID", ref _ResultAutomationID, value); }
        }

        #endregion

        #region ResultAutomationDone

        private bool _ResultAutomationDone;

        public bool ResultAutomationDone
        {
            get { return _ResultAutomationDone; }
            set { SetPropertyValue("ResultAutomationDone", ref _ResultAutomationDone, value); }
        }

        #endregion

        #region Result

        private int _Result;

        public int Result
        {
            get { return _Result; }
            set { SetPropertyValue("Result", ref _Result, value); }
        }

        #endregion

        #region ResultDate

        private DateTime _ResultDate;

        public DateTime ResultDate
        {
            get { return _ResultDate; }
            set { SetPropertyValue("ResultDate", ref _ResultDate, value); }
        }

        #endregion

        #region ResultNote

        private string _ResultNote;

        [Size(SizeAttribute.Unlimited)]
        public string ResultNote
        {
            get { return _ResultNote; }
            set { SetPropertyValue("ResultNote", ref _ResultNote, value); }
        }

        #endregion
    }
}
