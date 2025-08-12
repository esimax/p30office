using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.WF
{
    public class DBWFStage : XPGUIDLogableObject 
    {
        #region CTOR


        public DBWFStage(Session session)
            : base(session)
        {
        }



        #endregion

        #region Link [n-n] - Contact Category

        [Association("CTContactCats_WFStages")]
        [DisplayName("دسته ها")]
        public XPCollection<DBCTContactCat> Categories
        {
            get { return GetCollection<DBCTContactCat>("Categories"); }
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }


        public static DBWFStage FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBWFStage>(new BinaryOperator("Oid", oid));
        }

        public static DBWFStage FindDuplicateTitleExcept(Session session, DBWFStage exceptContact, string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBWFStage>(go);
        }

        public static XPCollection<DBWFStage> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBWFStage>(dxs, bo, new SortProperty("Title", SortingDirection.Ascending));
        }

        #region Title

        private string _Title;

        [Size(64), PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Note

        private string _Note;

        [Size(SizeAttribute.Unlimited), PersianString]
        public string Note
        {
            get { return _Note; }
            set { SetPropertyValue("Note", ref _Note, value); }
        }

        #endregion
    }
}
