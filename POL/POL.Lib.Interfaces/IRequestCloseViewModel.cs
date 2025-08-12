using System;

namespace POL.Lib.Interfaces
{
    public interface IRequestCloseViewModel
    {
        event EventHandler<RequestCloseEventArgs> RequestClose;
    }
}
