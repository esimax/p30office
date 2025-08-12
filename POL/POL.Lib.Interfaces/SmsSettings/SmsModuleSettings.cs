using System;
using System.IO.Ports;
using System.Runtime.Serialization;

namespace POL.Lib.Interfaces.SmsSettings
{
    [Serializable]
    [DataContract]
    public class SmsModuleSettings
    {
        public override string ToString()
        {
            return SmsNumber;
        }

        #region تنظیمات مشترك

        [DataMember]
        public bool SmsIsEnable { get; set; }

        [DataMember]
        public EnumSmsDeviceType SmsDeviceType { get; set; }

        [DataMember]
        public string SmsNumber { get; set; }

        [DataMember]
        public bool SmsEnableDelivery { get; set; }

        [DataMember]
        public string SmsForwardOnFailure { get; set; }

        [DataMember]
        public string SmsForwardOnLowCredit { get; set; }

        [DataMember]
        public bool SmsEnableLog { get; set; }

        [DataMember]
        public bool SmsAllowDirectSend { get; set; }

        [DataMember]
        public bool SmsUsageForManager { get; set; }

        [DataMember]
        public bool SmsUsageForCall { get; set; }

        [DataMember]
        public bool SmsUsageForAutomation { get; set; }

        #endregion

        #region تنظیمات نرم افزاری

        [DataMember]
        public string SwProvider { get; set; }

        [DataMember]
        public string SwUrl { get; set; }

        [DataMember]
        public string SwUrlGet { get; set; }

        [DataMember]
        public string SwUsername { get; set; }

        [DataMember]
        public string SwPasword { get; set; }

        [DataMember]
        public EnumSmsSwDeliveryDelay SwDeliveryDelay { get; set; }

        [DataMember]
        public string SwExtra { get; set; }

        #endregion

        #region تنظیمات سخت افزاری

        [DataMember]
        public string HwSerialPort { get; set; }

        [DataMember]
        public string HwBaudRate { get; set; }

        [DataMember]
        public int HwDataBits { get; set; }

        [DataMember]
        public Parity HwParity { get; set; }

        [DataMember]
        public StopBits HwStopBits { get; set; }

        [DataMember]
        public Handshake HwHandshake { get; set; }

        [DataMember]
        public EnumSmsLongMessages HwSmsLongMessages { get; set; }

        [DataMember]
        public EnumSmsEncoding HwSmsEncoding { get; set; }

        [DataMember]
        public bool HwNewMessageIndication { get; set; }

        #endregion
    }
}
