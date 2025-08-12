using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using POL.DB.Root;

namespace POL.DB.P30Office.GL
{
    public class DBGLUpdateStorage : XPGUIDObject
    {
        #region Design

        public DBGLUpdateStorage(Session session)
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

        public static DBGLUpdateStorage FindFirst(Session dxs)
        {
            var xpc = new XPCollection<DBGLUpdateStorage>(dxs, null,
                new SortProperty("Version", SortingDirection.Descending))
            {TopReturnedObjects = 1};
            if (xpc.Count > 0)
                return xpc[0];
            return null;
        }

        #region Date

        private DateTime _Date;

        public DateTime Date
        {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }

        #endregion

        #region Version

        private string _Version;

        public string Version
        {
            get { return _Version; }
            set { SetPropertyValue("Version", ref _Version, value); }
        }

        #endregion

        #region DeviceInstalled

        private int _DeviceInstalled;

        public int DeviceInstalled
        {
            get { return _DeviceInstalled; }
            set { SetPropertyValue("DeviceInstalled", ref _DeviceInstalled, value); }
        }

        #endregion

        #region Confirmed

        private bool _Confirmed;

        public bool Confirmed
        {
            get { return _Confirmed; }
            set { SetPropertyValue("Confirmed", ref _Confirmed, value); }
        }

        #endregion
    }
}
