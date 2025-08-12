using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTProfileReport : XPGUIDLogableObject 
    {
        #region Design

        public DBCTProfileReport(Session session) : base(session)
        {
        }


        #endregion

        public static DBCTProfileReport FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTProfileReport>(new BinaryOperator("Oid", oid));
        }

        public static DBCTProfileReport FindDuplicateTitleExcept(Session session, DBCTProfileReport exceptContactCat,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContactCat != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContactCat.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTProfileReport>(go);
        }

        public static XPCollection<DBCTProfileReport> GetAll(Session session)
        {
            return new XPCollection<DBCTProfileReport>(session, null,
                new SortProperty("Title", SortingDirection.Ascending));
        }

        public override string ToString()
        {
            return Title;
        }

        #region Title

        private string _Title;

        [PersianString]
        [Size(64)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region CCreatedAt

        private bool _CCreatedAt;

        public bool CCreatedAt
        {
            get { return _CCreatedAt; }
            set { SetPropertyValue("CCreatedAt", ref _CCreatedAt, value); }
        }

        #endregion

        #region CCreator

        private bool _CCreator;

        public bool CCreator
        {
            get { return _CCreator; }
            set { SetPropertyValue("CCreator", ref _CCreator, value); }
        }

        #endregion

        #region CEditedAt

        private bool _CEditedAt;

        public bool CEditedAt
        {
            get { return _CEditedAt; }
            set { SetPropertyValue("CEditedAt", ref _CEditedAt, value); }
        }

        #endregion

        #region CEditor

        private bool _CEditor;

        public bool CEditor
        {
            get { return _CEditor; }
            set { SetPropertyValue("CEditor", ref _CEditor, value); }
        }

        #endregion

        #region CCategory

        private bool _CCategory;

        public bool CCategory
        {
            get { return _CCategory; }
            set { SetPropertyValue("CCategory", ref _CCategory, value); }
        }

        #endregion

        #region CPhone

        private bool _CPhone;

        public bool CPhone
        {
            get { return _CPhone; }
            set { SetPropertyValue("CPhone", ref _CPhone, value); }
        }

        #endregion

        #region CEmail

        private bool _CEmail;

        public bool CEmail
        {
            get { return _CEmail; }
            set { SetPropertyValue("CEmail", ref _CEmail, value); }
        }

        #endregion

        #region CAddress

        private bool _CAddress;

        public bool CAddress
        {
            get { return _CAddress; }
            set { SetPropertyValue("CAddress", ref _CAddress, value); }
        }

        #endregion

        #region MetaData

        private string _MetaData;

        [PersianString]
        [Size(SizeAttribute.Unlimited)]
        public string MetaData
        {
            get { return _MetaData; }
            set { SetPropertyValue("MetaData", ref _MetaData, value); }
        }

        #endregion
    }
}
