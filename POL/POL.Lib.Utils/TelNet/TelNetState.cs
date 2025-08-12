using System.Net.Sockets;

namespace POL.Lib.Utils.TelNet
{
    public class TelNetState
    {
        public const int BufferSize = 256;

        public byte[] Buffer = new byte[BufferSize];

        public Socket WorkSocket;
    }
}
