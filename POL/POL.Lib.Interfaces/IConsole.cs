using System;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;

namespace POL.Lib.Interfaces
{
    public interface IConsole
    {
        ObservableCollection<ConsoleItem> Collection { get; }
        void Clear();
        void AddItem(DateTime date, Category type, string text);
    }
}
