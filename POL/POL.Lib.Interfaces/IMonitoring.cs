using System;
using System.Collections.ObjectModel;

namespace POL.Lib.Interfaces
{
    public interface IMonitoring
    {
        ObservableCollection<MonitoringItem> Collection { get; }
        void Register(string key, string title, Action<MonitoringItem> updateAction);
        void Update(string key, EnumMonitoringStatus status, string message);
        MonitoringItem GetByKey(string key);

        event EventHandler<MonitoringEventArg> OnStatusUpdated;
    }
}
