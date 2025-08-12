using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;

namespace POL.DB.P30Office.BT
{
    [OptimisticLocking(false)]
    public class DBACFactor : XPGUIDLogableObject 
    {
        #region CTOR
        public DBACFactor(Session session) : base(session)
        {
        }
        #endregion

        #region Link - FactoreItems

        [Association("ACFactor_ACFactorItems"), Aggregated]
        public XPCollection<DBACFactorItem> FactoreItems
        {
            get { return GetCollection<DBACFactorItem>("FactoreItems"); }
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }


        public static int GetNextCode(Session dxs)
        {
            try
            {
                var maxCode = (int) dxs.Evaluate<DBACFactor>(CriteriaOperator.Parse("Max(Code)"), null);
                return maxCode + 1;
            }
            catch
            {
                return 0;
            }
        }

        public static DBACFactor FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBACFactor>(new BinaryOperator("Oid", oid));
        }

        public static DBACFactor FindByCode(Session session, int code)
        {
            var xpc = new XPCollection<DBACFactor>(session, new BinaryOperator("Code", code));
            return xpc.Count == 0 ? null : xpc[0];
        }

        public static XPCollection<DBACFactor> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBACFactor>(dxs, bo, new SortProperty("Date", SortingDirection.Descending));
        }

        public static XPCollection<DBACFactor> GetByContact(Session session, Guid contactOid)
        {
            return new XPCollection<DBACFactor>(session, new BinaryOperator("Contact.Oid", contactOid));
        }

        public void UpdateSums()
        {
            SumSum = 0;
            SumPercentIncrease = 0;
            SumDiscount = 0;

            var fitems = FactoreItems;
            foreach (var dbi in fitems)
            {
                var cost = ((double)dbi.Price)*dbi.Count;
                SumSum += (decimal)cost;
            }
            SumDiscount = AmountDiscount + PercentDiscount*SumSum;
            SumSum -= SumDiscount;
            SumPercentIncrease = PercentIncrease*SumSum;
            SumSum += SumPercentIncrease;
        }

        #region Date

        private DateTime _Date;

        public DateTime Date
        {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }

        #endregion

        #region Code

        private int _Code;

        public int Code
        {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        #endregion

        #region CodeShow

        private bool _CodeShow;

        public bool CodeShow
        {
            get { return _CodeShow; }
            set { SetPropertyValue("CodeShow", ref _CodeShow, value); }
        }

        #endregion

        #region TitleOfTitle

        private string _TitleOfTitle;

        [Size(64), PersianString]
        public string TitleOfTitle
        {
            get { return _TitleOfTitle; }
            set { SetPropertyValue("TitleOfTitle", ref _TitleOfTitle, value); }
        }

        #endregion

        #region Title

        private string _Title;

        [Size(64), PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region IsPreFactor

        private bool _IsPreFactor;

        public bool IsPreFactor
        {
            get { return _IsPreFactor; }
            set { SetPropertyValue("IsPreFactor", ref _IsPreFactor, value); }
        }

        #endregion

        #region Contact

        private DBCTContact _Contact;

        [DisplayName("پرونده")]
        [Association("CTContact_ACFactors")]
        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
        }

        #endregion

        #region PercentIncrease

        private decimal _PercentIncrease;

        public decimal PercentIncrease
        {
            get { return _PercentIncrease; }
            set { SetPropertyValue("PercentIncrease", ref _PercentIncrease, value); }
        }

        #endregion

        #region PercentDiscount

        private decimal _PercentDiscount;

        public decimal PercentDiscount
        {
            get { return _PercentDiscount; }
            set { SetPropertyValue("PercentDiscount", ref _PercentDiscount, value); }
        }

        #endregion

        #region AmountDiscount

        private decimal _AmountDiscount;

        public decimal AmountDiscount
        {
            get { return _AmountDiscount; }
            set { SetPropertyValue("AmountDiscount", ref _AmountDiscount, value); }
        }

        #endregion

        #region SumDiscount

        private decimal _SumDiscount;

        public decimal SumDiscount
        {
            get { return _SumDiscount; }
            set { SetPropertyValue("SumDiscount", ref _SumDiscount, value); }
        }

        #endregion

        #region SumPercentIncrease

        private decimal _SumPercentIncrease;

        public decimal SumPercentIncrease
        {
            get { return _SumPercentIncrease; }
            set { SetPropertyValue("SumPercentIncrease", ref _SumPercentIncrease, value); }
        }

        #endregion

        #region SumSum

        private decimal _SumSum;

        public decimal SumSum
        {
            get { return _SumSum; }
            set { SetPropertyValue("SumSum", ref _SumSum, value); }
        }

        #endregion

        #region Note

        private string _Note;

        [Size(1024), PersianString]
        public string Note
        {
            get { return _Note; }
            set { SetPropertyValue("Note", ref _Note, value); }
        }

        #endregion

        #region TemplateValues

        private string _TemplateValues;

        [Size(SizeAttribute.Unlimited), PersianString]
        public string TemplateValues
        {
            get { return _TemplateValues; }
            set { SetPropertyValue("TemplateValues", ref _TemplateValues, value); }
        }

        #endregion
    }
}
