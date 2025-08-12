using System;
using System.ServiceProcess;

namespace POL.Lib.Interfaces
{
    public interface IServiceMain
    {
        ServiceControllerStatus ServiceStatus { get; }
        bool IsInstalled { get; }
        void Start();
        void Stop();

        event EventHandler OnStatusChanged;

        event EventHandler<AutoUpdateArgs> OnAutoUpdateChanged;
    }
}
