using System;

namespace POL.Lib.Interfaces
{
    public class ApplicationExitEventArg : EventArgs
    {
        public EnumApplicationExit Reason { get; set; }
    }
}
