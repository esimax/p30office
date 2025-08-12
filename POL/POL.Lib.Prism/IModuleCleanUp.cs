using System;

namespace POL.Lib.Prism
{
    public interface IModuleCleanUp
    {
        int Count { get; }

        void RegisterCleanAction(string modulename, Action action, int priority);

        void UnregisterCleanAction(string modulename);

        void DoCleanUp();
    }
}
