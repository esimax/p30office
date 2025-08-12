using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces.PObjects;

namespace POL.DB.P30Office
{
    public class DBCTEmail : XPGUIDLogableObject 
    {
        #region Design

        public DBCTEmail(Session session) : base(session)
        {
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{0} ({1})", Title, Address);
        }

        protected override void OnSaving()
        {
            AddressLower = Address != null ? Address.ToLower() : null;
            base.OnSaving();
        }

        public static DBCTEmail FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTEmail>(new BinaryOperator("Oid", oid));
        }

        public static DBCTEmail FindByAddressExcept(Session session, DBCTEmail except, string address)
        {
            if (except == null)
                return session.FindObject<DBCTEmail>(new BinaryOperator("AddressLower", address.ToLower()));
            var go = new GroupOperator();
            go.Operands.Add(new BinaryOperator("AddressLower", address.ToLower()));
            go.Operands.Add(new BinaryOperator("Oid", except.Oid, BinaryOperatorType.NotEqual));
            return session.FindObject<DBCTEmail>(go);
        }

        public static XPCollection<DBCTEmail> GetByContact(Session session, Guid contactOid)
        {
            return new XPCollection<DBCTEmail>(session, new BinaryOperator("Contact.Oid", contactOid));
        }

        public static XPCollection<DBCTEmail> GetAll(Session session)
        {
            return new XPCollection<DBCTEmail>(session);
        }

        internal static Email PopulateToFake(DBCTEmail dbemail)
        {
            var rv = new Email();
            rv.Address = dbemail.Address;
            rv.Title = dbemail.Title;
            return rv;
        }

        #region Title

        private string _Title;

        [Size(32)]
        public string Title
        {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }

        #endregion

        #region Address

        private string _Address;

        [Size(64)]
        public string Address
        {
            get { return _Address; }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

        #endregion

        #region AddressLower

        private string _AddressLower;

        [Size(64)]
        public string AddressLower
        {
            get { return _AddressLower; }
            set { SetPropertyValue("AddressLower", ref _AddressLower, value); }
        }

        #endregion

        #region Contact

        private DBCTContact _Contact;

        [Association("CTContact_CTEmails")]
        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
        }

        #endregion
    }
}
