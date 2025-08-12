using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.P30Office.WF;
using POL.DB.Root;
using POL.Lib.Interfaces.PObjects;
using POL.Lib.Utils;

namespace POL.DB.P30Office
{
    public class DBCTContactCat : XPGUIDLogableObject 
    {
        #region Design

        public DBCTContactCat(Session session) : base(session)
        {
        }

        #endregion

        #region Link [n-n] - Contacts

        [Association("CTContactCats_CTContacts")]
        [DisplayName("پرونده ها")]
        public XPCollection<DBCTContact> Contacts
        {
            get { return GetCollection<DBCTContact>("Contacts"); }
        }

        #endregion

        #region Link [n-n] - ProfileRoot

        [Association("CTContactCats_CTProfileRoot")]
        [DisplayName("فرم ها")]
        public XPCollection<DBCTProfileRoot> ProfileRoots
        {
            get { return GetCollection<DBCTProfileRoot>("ProfileRoots"); }
        }

        #endregion

        #region Link [n-n] - Stages

        [Association("CTContactCats_WFStages")]
        [DisplayName("مرحله ها")]
        public XPCollection<DBWFStage> Stages
        {
            get { return GetCollection<DBWFStage>("Stages"); }
        }

        #endregion

        public static DBCTContactCat FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTContactCat>(new BinaryOperator("Oid", oid));
        }

        public static DBCTContactCat FindDuplicateTitleExcept(Session session, DBCTContactCat exceptContactCat,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContactCat != null)
            {
                go.Operands.Add(new BinaryOperator("Oid", exceptContactCat.Oid, BinaryOperatorType.NotEqual));
            }
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBCTContactCat>(go);
        }

        public static XPCollection<DBCTContactCat> GetAll(Session session)
        {
            return new XPCollection<DBCTContactCat>(session, null, new SortProperty("Title", SortingDirection.Ascending));
        }


        public override string ToString()
        {
            return Title;
        }

        public static Category PopulateToFake(DBCTContactCat dbctContactCat)
        {
            var rv = new Category();
            rv.Role = dbctContactCat.Role;
            rv.Title = dbctContactCat.Title;
            return rv;
        }

        #region Title

        private string _Title;

        [PersianString]
        [Size(128)]
        [DisplayName("عنوان دسته")]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Role

        private string _Role;

        [Size(32)]
        [DisplayName("سطح دسترسی")]
        public string Role
        {
            get { return _Role; }
            set { SetPropertyValue("Role", ref _Role, value); }
        }

        #endregion
    }
}
