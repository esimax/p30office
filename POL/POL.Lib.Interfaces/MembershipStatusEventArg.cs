using System;

namespace POL.Lib.Interfaces
{
    public class MembershipStatusEventArg : EventArgs
    {
        public MembershipStatusEventArg(EnumMembershipStatus status)
        {
            Status = status;
        }

        public EnumMembershipStatus Status { get; set; }
    }
}
