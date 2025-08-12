using System;
using System.ServiceModel;

namespace POL.Lib.Interfaces
{
    [ServiceContract]
    public interface IProtocolMembership
    {
        [OperationContract]
        bool ValidateUser(string username, string password);


        [OperationContract]
        MembershipUser RegisterUser(Guid id, string username, string password);

        [OperationContract]
        bool ReregisterUser(Guid id);

        [OperationContract]
        bool LogoutUser(Guid id);
    }
}
