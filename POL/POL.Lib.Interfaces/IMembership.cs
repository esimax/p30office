using System;

namespace POL.Lib.Interfaces
{
    public interface IMembership
    {
        MembershipUser ActiveUser { get; }
        bool IsAuthorized { get; }
        EnumMembershipStatus Status { get; }

        void LoginUser(string username, string password, Guid id);
        void LogoutUser(Guid id);

        event EventHandler<MembershipStatusEventArg> OnMembershipStatusChanged;
        bool HasPermission(object code);
    }
}
