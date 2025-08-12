using System.ServiceModel;

namespace POL.Lib.Interfaces
{
    [MessageContract]
    public class VoiceRecordDeleteReturns
    {
        [MessageBodyMember(Order = 1)] public string InnerMessage;

        [MessageHeader(MustUnderstand = true)] public bool Succeed;

        [MessageHeader(MustUnderstand = true)] public string TopException;
    }
}
