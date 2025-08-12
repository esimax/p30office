using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    public class DBSMTextStatic : XPGUIDLogableObject
    {
        #region Design

        public DBSMTextStatic(Session session) : base(session)
        {
        }
        #endregion

        public static XPCollection<DBSMTextStatic> GetAll(Session dxs)
        {
            return new XPCollection<DBSMTextStatic>(dxs, null,
                new SortProperty("DateCreated", SortingDirection.Descending));
        }

        public static DBSMTextStatic FindByOid(Session session, Guid oid)
        {
            return session.FindObject<DBSMTextStatic>(new BinaryOperator("Oid", oid));
        }

        #region Body

        private string _Body;

        [Size(1600)]
        [PersianString]
        public string Body
        {
            get { return _Body; }
            set { SetPropertyValue("Body", ref _Body, value); }
        }

        #endregion

        #region IsRTL

        private bool _IsRTL;

        public bool IsRTL
        {
            get { return _IsRTL; }
            set { SetPropertyValue("IsRTL", ref _IsRTL, value); }
        }

        #endregion
    }
}
