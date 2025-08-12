using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;
using POL.Lib.Utils;

namespace POL.DB.P30Office.AC
{
    [OptimisticLocking(false)]
    public class DBACFactorReportTemplate : XPGUIDLogableObject
    {
        #region CTOR

        public DBACFactorReportTemplate(Session session)
            : base(session)
        {
        }

        #endregion

        #region FileData

        [Delayed(true)]
        [Size(SizeAttribute.Unlimited)]
        public string FileData
        {
            get { return GetDelayedPropertyValue<string>("FileData"); }
            set { SetDelayedPropertyValue("FileData", value); }
        }

        #endregion

        public override string ToString()
        {
            return Title;
        }

        public static DBACFactorReportTemplate FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBACFactorReportTemplate>(new BinaryOperator("Oid", oid));
        }

        public static DBACFactorReportTemplate FindDuplicateTitleExcept(Session session,
            DBACFactorReportTemplate exceptContact, string title)
        {
            var go = new GroupOperator();
            if (exceptContact != null)
                go.Operands.Add(new BinaryOperator("Oid", exceptContact.Oid, BinaryOperatorType.NotEqual));
            go.Operands.Add(new BinaryOperator("Title", HelperConvert.CorrectPersianBug(title), BinaryOperatorType.Equal));
            return session.FindObject<DBACFactorReportTemplate>(go);
        }

        public static XPCollection<DBACFactorReportTemplate> GetAll(Session dxs, string searchInTitle = null)
        {
            BinaryOperator bo = null;
            if (!string.IsNullOrWhiteSpace(searchInTitle))
                bo = new BinaryOperator("Title", string.Format("%{0}%", searchInTitle), BinaryOperatorType.Like);
            return new XPCollection<DBACFactorReportTemplate>(dxs, bo,
                new SortProperty("Title", SortingDirection.Ascending));
        }

        #region Title

        private string _Title;

        [Size(100), PersianString]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region FileName

        private string _FileName;

        [Size(256)]
        public string FileName
        {
            get { return _FileName; }
            set { SetPropertyValue("FileName", ref _FileName, value); }
        }

        #endregion
    }
}
