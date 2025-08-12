using System;
using System.Collections.ObjectModel;

namespace POL.Lib.Interfaces
{
    public interface IStatistics
    {
        ObservableCollection<StatisticsItem> Collection { get; }
        void Register(string key, string title, Action<StatisticsItem> updateAction);
        void Update(string key, string value);
        StatisticsItem GetByKey(string key);

        event EventHandler<StatisticsEventArg> OnStatusUpdated;
    }
}
