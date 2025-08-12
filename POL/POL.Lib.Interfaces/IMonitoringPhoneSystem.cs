using System;

namespace POL.Lib.Interfaces
{
    public interface IMonitoringPhoneSystem
    {
        void RaiseCallerID(CallInfo call, ContactInfo contact);
        void RaiseDial(CallInfo callInfo, string dialed);
        void RaiseRing(CallInfo call);
        void RaiseHookOff(CallInfo call, ContactInfo contact);
        void RaiseHookOn(CallInfo call);
        void RaiseTrans(CallInfo callInfo, string dialed, ContactInfo contact);

        void RegisterImplementer(Action<CallInfo, ContactInfo> callerID, Action<CallInfo, string> dial,
            Action<CallInfo> ring, Action<CallInfo, ContactInfo> hookOff,
            Action<CallInfo> hookOn, Action<CallInfo, string, ContactInfo> trans);
    }
}
