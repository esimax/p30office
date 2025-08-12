using System;

namespace POL.Lib.Utils.TelNet
{
    public class TelNetDataAvailableEventArgs : EventArgs
    {
        public TelNetDataAvailableEventArgs(string output)
        {
            Data = output;
        }

        public string Data { get; }
    }
}
