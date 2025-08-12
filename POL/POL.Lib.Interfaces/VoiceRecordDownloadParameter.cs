using System.ServiceModel;

namespace POL.Lib.Interfaces
{
    [MessageContract]
    public class VoiceRecordDownloadParameter
    {
        [MessageBodyMember] public string RecordTag;
    }
}
