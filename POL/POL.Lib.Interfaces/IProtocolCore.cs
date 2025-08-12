using System.ServiceModel;

namespace POL.Lib.Interfaces
{
    [ServiceContract]
    public interface IProtocolCore
    {
        [OperationContract]
        ServerToClientInformation ShareInformation(ClientToServerInformation c2Si);
    }
}
