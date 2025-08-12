using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.WF
{
    public class DBWFStageArrow : XPGUIDLogableObject 
    {
        #region CTOR


        public DBWFStageArrow(Session session)
            : base(session)
        {
        }



        #endregion



        public override string ToString()
        {
            return Title;
        }


        public static DBWFStageArrow FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBWFStageArrow>(new BinaryOperator("Oid", oid));
        }

        public static DBWFStageArrow FindDuplicateTitleExcept(Session session, DBWFStageArrow exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBWFStageArrow>(go);
        }

        public static XPCollection<DBWFStageArrow> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBWFStageArrow>(dxs, bo, new SortProperty("Title", SortingDirection.Ascending));
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

        #region From

        private DBWFStage _From;

        public DBWFStage From
        {
            get { return _From; }
            set { SetPropertyValue("From", ref _From, value); }
        }

        #endregion

        #region To

        private DBWFStage _To;

        public DBWFStage To
        {
            get { return _To; }
            set { SetPropertyValue("To", ref _To, value); }
        }

        #endregion

        #region TwoWay

        private bool _TwoWay;

        public bool TwoWay
        {
            get { return _TwoWay; }
            set { SetPropertyValue("TwoWay", ref _TwoWay, value); }
        }

        #endregion
    }
}
