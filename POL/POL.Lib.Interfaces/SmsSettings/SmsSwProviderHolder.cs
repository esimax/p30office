using System;

namespace POL.Lib.Interfaces.SmsSettings
{
    public class SmsSwProviderHolder
    {
        public int Code { get; set; }
        public string Name { get; set; }

        public Action<object, SmsModuleSettings> SendMethod { get; set; }

        public Func<SmsModuleSettings, int> GetMethod { get; set; }

        public Func<string[], SmsModuleSettings, string[]> CheckDeliveryMethod { get; set; }
    }
}
