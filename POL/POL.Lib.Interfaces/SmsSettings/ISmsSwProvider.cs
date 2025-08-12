using System.Collections.Generic;

namespace POL.Lib.Interfaces.SmsSettings
{
    public interface ISmsSwProvider
    {
        void RegisterSmsSwProvider(SmsSwProviderHolder holder);
        SmsSwProviderHolder GetProviderByCode(int code);
        SmsSwProviderHolder GetProviderByName(string name);
        List<SmsSwProviderHolder> GetAll();
    }
}
