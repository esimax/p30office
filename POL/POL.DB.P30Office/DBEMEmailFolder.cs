using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.P30Office
{
    public class DBEMEmailFolder : XPGUIDLogableObject
    {
        #region Design

        public DBEMEmailFolder(Session session) : base(session)
        {
        }

        #endregion

        #region App - Folders

        [Association("EMEmailFolder_EMEmailFolders"), Aggregated]
        public XPCollection<DBEMEmailFolder> Folders
        {
            get { return GetCollection<DBEMEmailFolder>("Folders"); }
        }

        #endregion

        #region Folder - Inboxs

        [Association("EMEmailFolder_EMEmailInboxs"), Aggregated]
        public XPCollection<DBEMEmailInbox> Inboxs
        {
            get { return GetCollection<DBEMEmailInbox>("Inboxs"); }
        }

        #endregion

        public static DBEMEmailFolder FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBEMEmailFolder>(new BinaryOperator("Oid", oid));
        }

        public static DBEMEmailFolder FindByFolderType(Session session, Guid appOid, EnumEmailFolderType folderType)
        {
            return session.FindObject<DBEMEmailFolder>(
                new GroupOperator(
                    new BinaryOperator("EmailApp.Oid", appOid),
                    new BinaryOperator("FolderType", folderType)
                    ));
        }

        public static XPCollection<DBEMEmailFolder> GetAll(Session session)
        {
            return new XPCollection<DBEMEmailFolder>(session);
        }

        #region EmailApp

        private DBEMEmailApp _EmailApp;

        [Association("EMEmailApp_EMEmailFolder")]
        public DBEMEmailApp EmailApp
        {
            get { return _EmailApp; }
            set { SetPropertyValue("EmailApp", ref _EmailApp, value); }
        }

        #endregion

        #region FolderType

        private EnumEmailFolderType _FolderType;

        public EnumEmailFolderType FolderType
        {
            get { return _FolderType; }
            set { SetPropertyValue("FolderType", ref _FolderType, value); }
        }

        #endregion

        #region Title

        private string _Title;

        [Size(32)]
        [PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region ParentFolder

        private DBEMEmailFolder _ParentFolder;

        [Association("EMEmailFolder_EMEmailFolders")]
        public DBEMEmailFolder ParentFolder
        {
            get { return _ParentFolder; }
            set { SetPropertyValue("ParentFolder", ref _ParentFolder, value); }
        }

        #endregion
    }
}
