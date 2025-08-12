using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.P30Office.BT;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.AC
{
    [OptimisticLocking(false)]
    public class DBACProduct : XPGUIDLogableObject
    {
        #region CTOR

        public DBACProduct(Session session) : base(session)
        {
        }

        #endregion

        [NonPersistent]
        public string PricePersian
        {
            get { return HelperConvert.ConvertToPersianDigit(Price.ToString("N0")); }
        }

        public override string ToString()
        {
            return Title;
        }


        public static int GetNextCode(Session dxs)
        {
            try
            {
                var maxCode = (int) dxs.Evaluate<DBACProduct>(CriteriaOperator.Parse("Max(Code)"), null);
                return maxCode + 1;
            }
            catch
            {
                return 0;
            }
        }

        public static DBACProduct FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBACProduct>(new BinaryOperator("Oid", oid));
        }

        public static DBACProduct FindByCode(Session session, int code)
        {
            var xpc = new XPCollection<DBACProduct>(session, new BinaryOperator("Code", code));
            return xpc.Count == 0 ? null : xpc[0];
        }

        public static XPCollection<DBACProduct> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBACProduct>(dxs, bo, new SortProperty("Title", SortingDirection.Ascending));
        }

        #region Title

        private string _Title;

        [Size(200), PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
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

        #region Price

        private decimal _Price;

        public decimal Price
        {
            get { return _Price; }
            set { SetPropertyValue("Price", ref _Price, value); }
        }

        #endregion

        #region Unit

        private DBBTUnit _Unit;

        public DBBTUnit Unit
        {
            get { return _Unit; }
            set { SetPropertyValue("Unit", ref _Unit, value); }
        }

        #endregion
    }
}
