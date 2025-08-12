using System.ServiceModel;

namespace POL.Lib.Interfaces
{
    [ServiceContract]
    public interface IProtocolVoiceRecord
    {
        [OperationContract]
        VoiceRecordDownloadReturns DownloadFile(VoiceRecordDownloadParameter item);

        [OperationContract]
        VoiceRecordDeleteReturns DeleteMapItem(VoiceRecordDownloadParameter item);
    }
}
