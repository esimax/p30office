using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.WF
{
    public class DBWFStageCondition : XPGUIDLogableObject 
    {
        #region CTOR


        public DBWFStageCondition(Session session)
            : base(session)
        {
        }



        #endregion

        public override string ToString()
        {
            return Title;
        }


        public static DBWFStageCondition FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBWFStageCondition>(new BinaryOperator("Oid", oid));
        }

        public static DBWFStageCondition FindDuplicateTitleExcept(Session session, DBWFStageCondition exceptContact,
            string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBWFStageCondition>(go);
        }

        public static XPCollection<DBWFStageCondition> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBWFStageCondition>(dxs, bo, new SortProperty("Title", SortingDirection.Ascending));
        }

        #region Title

        private string _Title;

        [Size(256), PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region ProfileItem

        private DBCTProfileItem _ProfileItem;

        public DBCTProfileItem ProfileItem
        {
            get { return _ProfileItem; }
            set { SetPropertyValue("ProfileItem", ref _ProfileItem, value); }
        }

        #endregion

        #region OperatorType

        private BinaryOperatorType _OperatorType;

        public BinaryOperatorType OperatorType
        {
            get { return _OperatorType; }
            set { SetPropertyValue("OperatorType", ref _OperatorType, value); }
        }

        #endregion

        #region Value

        private string _Value;

        [Size(SizeAttribute.Unlimited)]
        public string Value
        {
            get { return _Value; }
            set { SetPropertyValue("Value", ref _Value, value); }
        }

        #endregion
    }
}
