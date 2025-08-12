using System;
using DevExpress.Xpo;
using POL.DB.Root;

namespace POL.DB.P30Office
{
    [OptimisticLocking(false)]
    public class DBCLPacketBad : XPGUIDObject
    {
        #region Design

        public DBCLPacketBad(Session session)
            : base(session)
        {
        }

        #endregion

        public static void LogBadData(Session dxs, DateTime date, int deviceNumber, string linedata)
        {
            try
            {
                var v = new DBCLPacketBad(dxs) {LogDate = date, Device = deviceNumber, BadData = linedata};
                v.Save();
            }
            catch
            {
            }
        }

        #region LogDate

        private DateTime _LogDate;

        public DateTime LogDate
        {
            get { return _LogDate; }
            set { SetPropertyValue("LogDate", ref _LogDate, value); }
        }

        #endregion

        #region Device

        private int _Device;

        public int Device
        {
            get { return _Device; }
            set { SetPropertyValue("Device", ref _Device, value); }
        }

        #endregion

        #region BadData

        private string _BadData;

        [Size(SizeAttribute.Unlimited)]
        public string BadData
        {
            get { return _BadData; }
            set { SetPropertyValue("BadData", ref _BadData, value); }
        }

        #endregion
    }
}
