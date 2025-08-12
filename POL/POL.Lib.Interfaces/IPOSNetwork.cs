using System;
using System.Collections.Generic;

namespace POL.Lib.Interfaces
{
    public interface IPOSNetwork
    {
        ServerToClientInformation STCI { get; set; }
        Dictionary<Guid, POSClientInformation> ClientsInfo { get; set; }
    }
}
