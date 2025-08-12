using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace POL.Lib.Utils.TelNet
{
    public class TelNetWrapper : TelNetProtocolHandler
    {
        public Exception ErrorException { get; set; }

        protected override void SetLocalEcho(bool echo)
        {
        }

        protected override void NotifyEndOfRecord()
        {
        }

        #region Globals and properties

        private readonly ManualResetEvent _ConnectDone = new ManualResetEvent(false);
        private readonly ManualResetEvent _SendDone = new ManualResetEvent(false);

        public event TelNetDisconnectedEventHandler Disconnected;
        public event TelNetDataAvailableEventHandler DataAvailable;

        private Socket _Socket;

        protected string Hostname;
        protected int Hostport;

        public string HostName
        {
            set { Hostname = value; }
        }

        public int Port
        {
            set
            {
                if (value > 0)
                    Hostport = value;
                else
                    throw new ArgumentException("Port number must be greater than 0.", "Port");
            }
        }

        public int TerminalWidth
        {
            set { windowSize.Width = value; }
        }

        public int TerminalHeight
        {
            set { windowSize.Height = value; }
        }

        public string TerminalType
        {
            set { terminalType = value; }
        }

        public bool Connected
        {
            get { return _Socket.Connected; }
        }

        #endregion

        #region Public interface

        public void Connect()
        {
            Connect(Hostname, Hostport);
        }

        public void Connect(string host, int portInt)
        {
            try
            {
                IPAddress ipAddress;

                if (!IPAddress.TryParse(host, out ipAddress))
                {
                    var ipHostInfo = Dns.GetHostEntry(host);
                    ipAddress = ipHostInfo.AddressList[0];
                }



                var remoteEP = new IPEndPoint(ipAddress, portInt);

                _Socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                _Socket.BeginConnect(remoteEP,
                    ConnectCallback, _Socket);
                _ConnectDone.WaitOne();

                Reset();
            }
            catch
            {
                Disconnect();
                throw;
            }
        }

        public string Send(string cmd)
        {
            try
            {
                var arr = Encoding.ASCII.GetBytes(cmd);
                Transpose(arr);
                return null;
            }
            catch (Exception e)
            {
                Disconnect();
                throw new ApplicationException("Error writing to socket.", e);
            }
        }

        public void Receive()
        {
            Receive(_Socket);
        }

        public void Disconnect()
        {
            if (_Socket == null || !_Socket.Connected) return;
            _Socket.Shutdown(SocketShutdown.Both);
            _Socket.Close();
            if (Disconnected != null) Disconnected(this, new EventArgs());
        }

        #endregion

        #region IO methods

        protected override void Write(byte[] b)
        {
            if (_Socket.Connected)
                Send(_Socket, b);
            _SendDone.WaitOne();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Socket client = null;

            try
            {
                client = (Socket) ar.AsyncState;

                client.EndConnect(ar);

                _ConnectDone.Set();
            }
            catch (Exception e)
            {
                Disconnect();
                if (client != null)
                    ErrorException = e;
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                var state = new TelNetState {WorkSocket = client};

                client.BeginReceive(state.Buffer, 0, TelNetState.BufferSize, 0,
                    ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Disconnect();
                ErrorException = e;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (TelNetState) ar.AsyncState;
                var client = state.WorkSocket;

                var bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    InputFeed(state.Buffer, bytesRead);
                    Negotiate(state.Buffer);

                    if (DataAvailable != null)
                        DataAvailable(this,
                            new TelNetDataAvailableEventArgs(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead)));

                    client.BeginReceive(state.Buffer, 0, TelNetState.BufferSize, 0,
                        ReceiveCallback, state);
                }
                else
                {
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                Disconnect();
                ErrorException = e;
            }
        }

        private void Send(Socket client, byte[] byteData)
        {
            client.BeginSend(byteData, 0, byteData.Length, 0,
                SendCallback, client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            var client = (Socket) ar.AsyncState;

            client.EndSend(ar);

            _SendDone.Set();
        }

        #endregion

        #region Cleanup

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
                Disconnect();
        }

        ~TelNetWrapper()
        {
            Dispose(false);
        }

        #endregion
    }
}
