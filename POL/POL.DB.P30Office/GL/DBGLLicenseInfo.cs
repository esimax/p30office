using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;

namespace POL.DB.P30Office.GL
{
    [OptimisticLocking(false), DeferredDeletion(false)]
    public class DBGLLicenseInfo : XPGUIDObject
    {
        #region CTOR


        public DBGLLicenseInfo(Session session)
            : base(session)
        {
        }


        #endregion

        [Delayed(true)]
        public byte[] PublicKey
        {
            get { return GetDelayedPropertyValue<byte[]>("PublicKey"); }
            set { SetDelayedPropertyValue("PublicKey", value); }
        }

        [Delayed(true)]
        public byte[] LicenseData
        {
            get { return GetDelayedPropertyValue<byte[]>("LicenseData"); }
            set { SetDelayedPropertyValue("LicenseData", value); }
        }




        public static DBGLLicenseInfo GetFirst(Session session)
        {
            var xpc = new XPCollection<DBGLLicenseInfo>(session, null,
                new SortProperty("RequestDate", SortingDirection.Ascending)) {TopReturnedObjects = 1};
            return xpc.Count > 0 ? xpc[0] : null;
        }

        public static DBGLLicenseInfo GetLast(Session session)
        {
            var xpc = new XPCollection<DBGLLicenseInfo>(session, null,
                new SortProperty("RequestDate", SortingDirection.Descending)) {TopReturnedObjects = 1};
            return xpc.Count > 0 ? xpc[0] : null;
        }

        public static int GetCountByDate(Session session, DateTime date)
        {
            var d1 = date.Date;
            var d2 = d1.AddDays(1);

            var xpq = new XPQuery<DBGLLicenseInfo>(session);
            return xpq.Count(n => (n.RequestDate >= d1) && (n.RequestDate < d2));
        }

        public static List<Guid> GetOidByDate(Session session, DateTime date)
        {
            var d1 = date.Date;
            var d2 = d1.AddDays(1);
            var xpq = new XPQuery<DBGLLicenseInfo>(session);
            return xpq.Where(n => (n.RequestDate >= d1) && (n.RequestDate < d2)).Select(n => n.Oid).ToList();
        }

        public static DBGLLicenseInfo FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBGLLicenseInfo>(new BinaryOperator("Oid", oid));
        }

        #region Application

        private string _Application;

        [Size(32)]
        public string Application
        {
            get { return _Application; }
            set { SetPropertyValue("Application", ref _Application, value); }
        }

        #endregion

        #region Serial

        private string _Serial;

        [Size(64)]
        public string Serial
        {
            get { return _Serial; }
            set { SetPropertyValue("Serial", ref _Serial, value); }
        }

        #endregion

        #region RequestDate

        private DateTime _RequestDate;

        public DateTime RequestDate
        {
            get { return _RequestDate; }
            set { SetPropertyValue("RequestDate", ref _RequestDate, value); }
        }

        #endregion

        #region Version

        private string _Version;

        [Size(16)]
        public string Version
        {
            get { return _Version; }
            set { SetPropertyValue("Version", ref _Version, value); }
        }

        #endregion

        #region FirstName

        private string _FirstName;

        [Size(64)]
        public string FirstName
        {
            get { return _FirstName; }
            set { SetPropertyValue("FirstName", ref _FirstName, value); }
        }

        #endregion

        #region LastName

        private string _LastName;

        [Size(64)]
        public string LastName
        {
            get { return _LastName; }
            set { SetPropertyValue("LastName", ref _LastName, value); }
        }

        #endregion

        #region Company

        private string _Company;

        [Size(64)]
        public string Company
        {
            get { return _Company; }
            set { SetPropertyValue("Company", ref _Company, value); }
        }

        #endregion

        #region Mobile

        private string _Mobile;

        [Size(32)]
        public string Mobile
        {
            get { return _Mobile; }
            set { SetPropertyValue("Mobile", ref _Mobile, value); }
        }

        #endregion

        #region Phone

        private string _Phone;

        [Size(32)]
        public string Phone
        {
            get { return _Phone; }
            set { SetPropertyValue("Phone", ref _Phone, value); }
        }

        #endregion

        #region Email

        private string _Email;

        [Size(32)]
        public string Email
        {
            get { return _Email; }
            set { SetPropertyValue("Email", ref _Email, value); }
        }

        #endregion

        #region Address

        private string _Address;

        [Size(512)]
        public string Address
        {
            get { return _Address; }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

        #endregion

        #region UserCount

        private int _UserCount;

        public int UserCount
        {
            get { return _UserCount; }
            set { SetPropertyValue("UserCount", ref _UserCount, value); }
        }

        #endregion

        #region AgentCode

        private int _AgentCode;

        public int AgentCode
        {
            get { return _AgentCode; }
            set { SetPropertyValue("AgentCode", ref _AgentCode, value); }
        }

        #endregion
    }
}
