using System;

namespace POL.Lib.Interfaces
{
    public class RequestCloseEventArgs : EventArgs
    {
        public bool? DialogResult { get; set; }
    }
}
