using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.P30Office.AC;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.BT
{
    [OptimisticLocking(false)]
    public class DBACFactorItem : XPGUIDLogableObject 
    {
        #region CTOR

        public DBACFactorItem(Session session) : base(session)
        {
        }
        #endregion

        [NonPersistent]
        public string PricePersian
        {
            get { return HelperConvert.ConvertToPersianDigit(Price.ToString("N0")); }
        }

        [NonPersistent]
        public string SumPersian
        {
            get { return HelperConvert.ConvertToPersianDigit(((double)Price * Count).ToString("N0")); }
        }

        [NonPersistent]
        public string CountPersian
        {
            get { return HelperConvert.ConvertToPersianDigit(Count.ToString("N0")); }
        }


        public override string ToString()
        {
            return Product == null ? string.Empty : Product.Title;
        }


        public static DBACFactorItem FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBACFactorItem>(new BinaryOperator("Oid", oid));
        }

        public static XPCollection<DBACFactorItem> GetByFactor(Session dxs, DBACFactor dbf)
        {
            return new XPCollection<DBACFactorItem>(dxs, new BinaryOperator("Factor.Oid", dbf.Oid));
        }


        #region Product

        private DBACProduct _Product;

        public DBACProduct Product
        {
            get { return _Product; }
            set { SetPropertyValue("Product", ref _Product, value); }
        }

        #endregion

        #region Count

        private double _Count;

        public double Count
        {
            get { return _Count; }
            set { SetPropertyValue("Count", ref _Count, value); }
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

        #region Sum

        private decimal _Sum;

        public decimal Sum
        {
            get { return _Sum; }
            set { SetPropertyValue("Sum", ref _Sum, value); }
        }

        #endregion

        #region Factor

        private DBACFactor _Factor;

        [DisplayName("فاكتور")]
        [Association("ACFactor_ACFactorItems")]
        public DBACFactor Factor
        {
            get { return _Factor; }
            set { SetPropertyValue("Factor", ref _Factor, value); }
        }

        #endregion

    }
}
