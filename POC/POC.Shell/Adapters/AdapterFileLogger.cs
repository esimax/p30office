using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Logging;

namespace POC.Shell.Adapters
{
    public class AdapterFileLogger : ILoggerFacade
    {
        private Queue<Tuple<string, Category, Priority>> SavedLogs { get; set; }
        public Action<string, Category, Priority> Callback { get; set; }

        #region CTOR
        public AdapterFileLogger()
        {
            SavedLogs = new Queue<Tuple<string, Category, Priority>>();
        } 
        #endregion

        public void Log(string message, Category category, Priority priority)
        {
            if (Callback != null)
            {
                Callback(message, category, priority);
            }
            else
            {
                SavedLogs.Enqueue(new Tuple<string, Category, Priority>(message, category, priority));
            }
        }
        public void ReplaySavedLogs()
        {
            if (Callback == null) return;
            while (SavedLogs.Count > 0)
            {
                var log = SavedLogs.Dequeue();
                Callback(log.Item1, log.Item2, log.Item3);
            }
        }
    }
}
