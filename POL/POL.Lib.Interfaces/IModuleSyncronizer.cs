using System;

namespace POL.Lib.Interfaces
{
    public interface IModuleSyncronizer
    {
        event EventHandler OnModuleFinilize;
        void RaiseOnModuleFinilize();

        event EventHandler<ApplicationExitEventArg> OnApplicationExit;
        void RaiseOnApplicationExit(EnumApplicationExit resion);
    }
}
