using System;

namespace POL.Lib.Interfaces
{
    public interface IMessagingClient
    {
        void RegisterHookForMessage(Action<MessagingItem> action, EnumMessageKind filter);
        bool SendMessage(MessagingItem item);
    }
}
