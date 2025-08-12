using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public class DBEMEmailAttachment : XPGUIDLogableObject
    {
        #region Design

        public DBEMEmailAttachment(Session session) : base(session)
        {
        }

        #endregion

        #region Inboxs - Attachments

        [Association("EMEmailInboxs_EMEmailAttachments")]
        public XPCollection<DBEMEmailInbox> Inboxes
        {
            get { return GetCollection<DBEMEmailInbox>("Inboxes"); }
        }

        #endregion

        public static DBEMEmailAttachment FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBEMEmailAttachment>(new BinaryOperator("Oid", oid));
        }

        #region FileName

        private string _FileName;

        [Size(256)]
        [PersianString]
        public string FileName
        {
            get { return _FileName; }
            set { SetPropertyValue("FileName", ref _FileName, value); }
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
    }
}
