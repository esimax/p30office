using System;
using System.Runtime.Serialization;
using POL.Lib.Interfaces.Info;

namespace POL.Lib.Interfaces
{
    [DataContract]
    public class ClientToServerInformation
    {
        #region P30Office

        [DataMember]
        public string ClientVersion { get; set; }

        #endregion

        #region OS Information

        [DataMember]
        public InfoOS OSInformation { get; set; }

        #endregion

        #region Processor Information

        [DataMember]
        public InfoCPU CPUInformation { get; set; }

        #endregion

        #region LogicalDisk Information

        [DataMember]
        public InfoLogicalDisk[] LogicalDiskInformations { get; set; }

        #endregion

        #region RAM Information

        [DataMember]
        public ulong TotalRam { get; set; }

        #endregion

        #region System Information

        [DataMember]
        public InfoSystem SystemInformation { get; set; }

        #endregion

        #region Network Information

        [DataMember]
        public InfoNetwork[] NetworkInformations { get; set; }

        #endregion

        #region Soft Information

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public DateTime ClientDate { get; set; }

        #endregion
    }
}
