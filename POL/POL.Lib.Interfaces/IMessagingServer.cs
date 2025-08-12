using System;
using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IMessagingServer
    {
        void AddMessageToQueue(MessagingItem mitem);
        IEnumerable<MessagingItem> GetMessageEnumerable();
        void RegisterImplementer(Action<MessagingItem> action);
    }
}
