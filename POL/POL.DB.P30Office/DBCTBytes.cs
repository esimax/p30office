using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POL.DB.P30Office
{
    public class DBCTBytes : XPGUIDObject 
    {
        #region Design

        public DBCTBytes(Session session)
            : base(session)
        {
        }


        #endregion

        #region DataByte

        [Delayed(true)]
        public byte[] DataByte
        {
            get { return GetDelayedPropertyValue<byte[]>("DataByte"); }
            set { SetDelayedPropertyValue("DataByte", value); }
        }

        #endregion

        public static DBCTBytes FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBCTBytes>(new BinaryOperator("Oid", oid));
        }

        #region Contact

        private DBCTContact _Contact;

        public DBCTContact Contact
        {
            get { return _Contact; }
            set { SetPropertyValue("Contact", ref _Contact, value); }
        }

        #endregion

        #region ByteDataType

        private EnumByteDataType _ByteDataType;

        public EnumByteDataType ByteDataType
        {
            get { return _ByteDataType; }
            set { SetPropertyValue("ByteDataType", ref _ByteDataType, value); }
        }

        #endregion
    }
}
