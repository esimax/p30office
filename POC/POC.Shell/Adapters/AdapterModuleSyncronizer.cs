using System;
using POL.Lib.Interfaces;

namespace POC.Shell.Adapters
{
    internal class AdapterModuleSyncronizer : IModuleSyncronizer
    {
        public event EventHandler OnModuleFinilize;

        public void RaiseOnModuleFinilize()
        {
            var ev = OnModuleFinilize;
            if (ev != null)
                ev.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<ApplicationExitEventArg> OnApplicationExit;
        public void RaiseOnApplicationExit(EnumApplicationExit resion)
        {
            var ev = OnApplicationExit;
            if (ev != null)
                ev.Invoke(this, new ApplicationExitEventArg { Reason = resion });
        }
    }
}
