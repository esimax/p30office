using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.P30Office
{
    public class DBEMEmailInbox : XPGUIDLogableObject
    {
        #region Design

        public DBEMEmailInbox(Session session)
            : base(session)
        {
        }

        #endregion

        #region Body

        [Size(SizeAttribute.Unlimited)]
        [Delayed(true)]
        public string Body
        {
            get
            {
                return GetDelayedPropertyValue<string>("Body");
            }
            set
            {
                SetDelayedPropertyValue("Body", value);
            }
        }

        #endregion

        #region BodyCache

        [Size(SizeAttribute.Unlimited)]
        [Delayed(true)]
        public string BodyCache
        {
            get { return GetDelayedPropertyValue<string>("BodyCache"); }
            set { SetDelayedPropertyValue("BodyCache", value); }
        }

        #endregion

        #region Header

        [Size(SizeAttribute.Unlimited)]
        [PersianString]
        [Delayed(true)]
        public string Header
        {
            get { return GetDelayedPropertyValue<string>("Header"); }
            set
            {
                SetDelayedPropertyValue("Header", value);
            }
        }

        #endregion

        #region Inbox - Attachments

        [Association("EMEmailInboxs_EMEmailAttachments")]
        public XPCollection<DBEMEmailAttachment> Attachments
        {
            get { return GetCollection<DBEMEmailAttachment>("Attachments"); }
        }

        #endregion

        public static DBEMEmailInbox FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBEMEmailInbox>(new BinaryOperator("Oid", oid));
        }
        #region Contact

        private DBCTContact _Contact;
        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
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

        #region Subject

        private string _Subject;

        [Size(256)]
        [PersianString]
        public string Subject
        {
            get { return _Subject; }
            set { SetPropertyValue("Subject", ref _Subject, value); }
        }

        #endregion

        #region UIDL

        private string _UIDL;

        [Size(64)]
        public string UIDL
        {
            get { return _UIDL; }
            set { SetPropertyValue("UIDL", ref _UIDL, value); }
        }

        #endregion

        #region FromAddress

        private string _FromAddress;

        [Size(256)]
        public string FromAddress
        {
            get { return _FromAddress; }
            set { SetPropertyValue("FromAddress", ref _FromAddress, value); }
        }

        #endregion

        #region FromName

        private string _FromName;

        [Size(256)]
        [PersianString]
        public string FromName
        {
            get { return _FromName; }
            set { SetPropertyValue("FromName", ref _FromName, value); }
        }

        #endregion


        #region Size

        private long _Size;

        public long Size
        {
            get { return _Size; }
            set { SetPropertyValue("Size", ref _Size, value); }
        }

        #endregion

        #region ReplyTo

        private string _ReplyTo;

        [Size(SizeAttribute.Unlimited)]
        public string ReplyTo
        {
            get { return _ReplyTo; }
            set { SetPropertyValue("ReplyTo", ref _ReplyTo, value); }
        }

        #endregion

        #region Star

        private bool _Star;

        public bool Star
        {
            get { return _Star; }
            set { SetPropertyValue("Star", ref _Star, value); }
        }

        #endregion

        #region ReadBy

        private string _ReadBy;

        [Size(SizeAttribute.Unlimited)]
        public string ReadBy
        {
            get { return _ReadBy; }
            set { SetPropertyValue("ReadBy", ref _ReadBy, value); }
        }

        #endregion

        #region Priority

        private EnumEmailPriority _Priority;

        public EnumEmailPriority Priority
        {
            get { return _Priority; }
            set { SetPropertyValue("Priority", ref _Priority, value); }
        }

        #endregion

        #region ReturnReceipt

        private string _ReturnReceipt;

        [Size(256)]
        public string ReturnReceipt
        {
            get { return _ReturnReceipt; }
            set { SetPropertyValue("ReturnReceipt", ref _ReturnReceipt, value); }
        }

        #endregion

        #region SendByUser

        private string _SendByUser;

        [Size(32)]
        public string SendByUser
        {
            get { return _SendByUser; }
            set { SetPropertyValue("SendByUser", ref _SendByUser, value); }
        }

        #endregion

        #region SendFailed

        private bool _SendFailed;

        public bool SendFailed
        {
            get { return _SendFailed; }
            set { SetPropertyValue("SendFailed", ref _SendFailed, value); }
        }

        #endregion

        #region ParentFolder

        private DBEMEmailFolder _ParentFolder;

        [Association("EMEmailFolder_EMEmailInboxs")]
        public DBEMEmailFolder ParentFolder
        {
            get { return _ParentFolder; }
            set { SetPropertyValue("ParentFolder", ref _ParentFolder, value); }
        }

        #endregion

        #region AttachmentCount

        private int _AttachmentCount;

        public int AttachmentCount
        {
            get { return _AttachmentCount; }
            set { SetPropertyValue("AttachmentCount", ref _AttachmentCount, value); }
        }

        #endregion

    }
}
