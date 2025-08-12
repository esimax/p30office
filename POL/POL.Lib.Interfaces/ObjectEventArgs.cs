using System;

namespace POL.Lib.Interfaces
{
    public class ObjectEventArgs : EventArgs
    {
        public ObjectEventArgs(object parameter)
        {
            Parameter = parameter;
        }

        public object Parameter { get; set; }
    }
}
