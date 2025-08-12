using System.ServiceModel;

namespace POL.Lib.Interfaces
{
    [ServiceContract]
    public interface IProtocolPhoneMonitoring
    {
        [OperationContract]
        bool BlockLine(string user, int line);

        [OperationContract]
        bool UnBlockLine(string user, int line);

        [OperationContract]
        bool ToggleLine(string user, int line);

        [OperationContract]
        PhoneMonitoringData[] GetMonitoringData();


    }
}
