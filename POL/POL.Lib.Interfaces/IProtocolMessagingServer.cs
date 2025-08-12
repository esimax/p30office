using System;
using System.ServiceModel;

namespace POL.Lib.Interfaces
{
    [ServiceContract]
    public interface IProtocolMessagingServer
    {
        [OperationContract]
        MessagingItem[] GetMyMessages(Guid clientOid, DateTime from, bool isFirstTime);

        [OperationContract]
        void SendMyMessage(MessagingItem message);
    }
}
