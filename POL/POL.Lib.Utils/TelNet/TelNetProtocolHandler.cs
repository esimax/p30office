using System;
using System.Drawing;
using System.Text;


namespace POL.Lib.Utils.TelNet
{
    public abstract class TelNetProtocolHandler
    {
        private byte current_sb;

        private byte neg_state;

        private byte[] receivedDX;

        private byte[] receivedWX;

        private byte[] sentDX;

        private byte[] sentWX;

        public TelNetProtocolHandler()
        {
            Reset();

            crlf[0] = 13;
            crlf[1] = 10;
            cr[0] = 13;
            cr[1] = 0;
        }

        protected void InputFeed(byte[] b, int len)
        {
            var bytesTmp = new byte[tempbuf.Length + len];

            Array.Copy(tempbuf, 0, bytesTmp, 0, tempbuf.Length);
            Array.Copy(b, 0, bytesTmp, tempbuf.Length, len);

            tempbuf = bytesTmp;
        }

        #region Globals and properties

        private byte[] tempbuf = new byte[0];

        private byte[] crlf = new byte[2];

        private byte[] cr = new byte[2];

        public string CRLF
        {
            get { return Encoding.ASCII.GetString(crlf); }
            set { crlf = Encoding.ASCII.GetBytes(value); }
        }

        public string CR
        {
            get { return Encoding.ASCII.GetString(cr); }
            set { cr = Encoding.ASCII.GetBytes(value); }
        }

        protected string terminalType = "dumb";

        protected Size windowSize = Size.Empty;

        protected abstract void SetLocalEcho(bool echo);

        protected abstract void NotifyEndOfRecord();

        protected abstract void Write(byte[] b);

        private void Write(byte b)
        {
            Write(new[] {b});
        }

        protected void Reset()
        {
            neg_state = 0;
            receivedDX = new byte[256];
            sentDX = new byte[256];
            receivedWX = new byte[256];
            sentWX = new byte[256];
        }

        #endregion

        #region Telnet protocol codes

        private const byte STATE_DATA = 0;
        private const byte STATE_IAC = 1;
        private const byte STATE_IACSB = 2;
        private const byte STATE_IACWILL = 3;
        private const byte STATE_IACDO = 4;
        private const byte STATE_IACWONT = 5;
        private const byte STATE_IACDONT = 6;
        private const byte STATE_IACSBIAC = 7;
        private const byte STATE_IACSBDATA = 8;
        private const byte STATE_IACSBDATAIAC = 9;

        private const byte IAC = 255;

        private const byte EOR = 239;

        private const byte WILL = 251;

        private const byte WONT = 252;

        private const byte DO = 253;

        private const byte DONT = 254;

        private const byte SB = 250;

        private const byte SE = 240;

        private const byte TELOPT_BINARY = 0; 

        private const byte TELOPT_ECHO = 1; 

        private const byte TELOPT_SGA = 3; 

        private const byte TELOPT_EOR = 25; 

        private const byte TELOPT_NAWS = 31; 

        private const byte TELOPT_TTYPE = 24;

        private static byte[] IACWILL = {IAC, WILL};
        private static byte[] IACWONT = {IAC, WONT};
        private static byte[] IACDO = {IAC, DO};
        private static byte[] IACDONT = {IAC, DONT};
        private static readonly byte[] IACSB = {IAC, SB};
        private static readonly byte[] IACSE = {IAC, SE};

        private static readonly byte TELQUAL_IS = 0;

        private static readonly byte TELQUAL_SEND = 1;

        #endregion

        #region The actual negotiation handling for the telnet protocol

        protected void SendTelnetControl(byte code)
        {
            var b = new byte[2];

            b[0] = IAC;
            b[1] = code;
            Write(b);
        }

        private void HandleSB(byte type, byte[] sbdata, int sbcount)
        {
            switch (type)
            {
                case TELOPT_TTYPE:
                    if (sbcount > 0 && sbdata[0] == TELQUAL_SEND)
                    {
                        Write(IACSB);
                        Write(TELOPT_TTYPE);
                        Write(TELQUAL_IS);
                        
                        Write(Encoding.ASCII.GetBytes(terminalType));
                        Write(IACSE);
                    }
                    break;
            }
        }


        protected void Transpose(byte[] buf)
        {
            int i;

            byte[] nbuf, xbuf;
            var nbufptr = 0;
            nbuf = new byte[buf.Length*2];

            for (i = 0; i < buf.Length; i++)
            {
                switch (buf[i])
                {
                    case IAC:
                        nbuf[nbufptr++] = IAC;
                        nbuf[nbufptr++] = IAC;
                        break;
                    case 10: 
                        if (receivedDX[TELOPT_BINARY + 128] != DO)
                        {
                            while (nbuf.Length - nbufptr < crlf.Length)
                            {
                                xbuf = new byte[nbuf.Length*2];
                                Array.Copy(nbuf, 0, xbuf, 0, nbufptr);
                                nbuf = xbuf;
                            }
                            for (var j = 0; j < crlf.Length; j++)
                                nbuf[nbufptr++] = crlf[j];
                        }
                        else
                        {
                            nbuf[nbufptr++] = buf[i];
                        }
                        break;
                    case 13: 
                        if (receivedDX[TELOPT_BINARY + 128] != DO)
                        {
                            while (nbuf.Length - nbufptr < cr.Length)
                            {
                                xbuf = new byte[nbuf.Length*2];
                                Array.Copy(nbuf, 0, xbuf, 0, nbufptr);
                                nbuf = xbuf;
                            }
                            for (var j = 0; j < cr.Length; j++)
                                nbuf[nbufptr++] = cr[j];
                        }
                        else
                        {
                            nbuf[nbufptr++] = buf[i];
                        }
                        break;
                    default:
                        nbuf[nbufptr++] = buf[i];
                        break;
                }
            }
            xbuf = new byte[nbufptr];
            Array.Copy(nbuf, 0, xbuf, 0, nbufptr);
            Write(xbuf);
        }


        protected int Negotiate(byte[] nbuf)
        {
            var count = tempbuf.Length;
            if (count == 0) 
                return -1;

            var sendbuf = new byte[3];
            var sbbuf = new byte[tempbuf.Length];
            var buf = tempbuf;

            byte b;
            byte reply;

            var sbcount = 0;
            var boffset = 0;
            var noffset = 0;

            var done = false;
            var foundSE = false;


            while (!done && boffset < count && noffset < nbuf.Length)
            {
                b = buf[boffset++];

                if (b >= 128)
                    b = (byte) (b - 256);

                switch (neg_state)
                {
                    case STATE_DATA:
                        if (b == IAC)
                        {
                            neg_state = STATE_IAC;
                        }
                        else
                        {
                            nbuf[noffset++] = b;
                        }
                        break;

                    case STATE_IAC:
                        switch (b)
                        {
                            case IAC:
                                neg_state = STATE_DATA;
                                nbuf[noffset++] = IAC; 
                                break;

                            case WILL:
                                neg_state = STATE_IACWILL;
                                break;

                            case WONT:
                                neg_state = STATE_IACWONT;
                                break;

                            case DONT:
                                neg_state = STATE_IACDONT;
                                break;

                            case DO:
                                neg_state = STATE_IACDO;
                                break;

                            case EOR:
                                NotifyEndOfRecord();
                                neg_state = STATE_DATA;
                                break;

                            case SB:
                                neg_state = STATE_IACSB;
                                sbcount = 0;
                                break;

                            default:
                                neg_state = STATE_DATA;
                                break;
                        }
                        break;

                    case STATE_IACWILL:
                        switch (b)
                        {
                            case TELOPT_ECHO:
                                reply = DO;
                                SetLocalEcho(false);
                                break;

                            case TELOPT_SGA:
                                reply = DO;
                                break;

                            case TELOPT_EOR:
                                reply = DO;
                                break;

                            case TELOPT_BINARY:
                                reply = DO;
                                break;

                            default:
                                reply = DONT;
                                break;
                        }

                        if (reply != sentDX[b + 128] || WILL != receivedWX[b + 128])
                        {
                            sendbuf[0] = IAC;
                            sendbuf[1] = reply;
                            sendbuf[2] = b;
                            Write(sendbuf);

                            sentDX[b + 128] = reply;
                            receivedWX[b + 128] = WILL;
                        }

                        neg_state = STATE_DATA;
                        break;

                    case STATE_IACWONT:

                        switch (b)
                        {
                            case TELOPT_ECHO:
                                SetLocalEcho(true);
                                reply = DONT;
                                break;

                            case TELOPT_SGA:
                                reply = DONT;
                                break;

                            case TELOPT_EOR:
                                reply = DONT;
                                break;

                            case TELOPT_BINARY:
                                reply = DONT;
                                break;

                            default:
                                reply = DONT;
                                break;
                        }

                        if (reply != sentDX[b + 128] || WONT != receivedWX[b + 128])
                        {
                            sendbuf[0] = IAC;
                            sendbuf[1] = reply;
                            sendbuf[2] = b;
                            Write(sendbuf);

                            sentDX[b + 128] = reply;
                            receivedWX[b + 128] = WILL;
                        }

                        neg_state = STATE_DATA;
                        break;

                    case STATE_IACDO:
                        switch (b)
                        {
                            case TELOPT_ECHO:
                                reply = WILL;
                                SetLocalEcho(true);
                                break;

                            case TELOPT_SGA:
                                reply = WILL;
                                break;

                            case TELOPT_TTYPE:
                                reply = WILL;
                                break;

                            case TELOPT_BINARY:
                                reply = WILL;
                                break;

                            case TELOPT_NAWS:
                                var size = windowSize;
                                receivedDX[b] = DO;

                                if (size.GetType() != typeof (Size))
                                {
                                    Write(IAC);
                                    Write(WONT);
                                    Write(TELOPT_NAWS);
                                    reply = WONT;
                                    sentWX[b] = WONT;
                                    break;
                                }

                                reply = WILL;
                                sentWX[b] = WILL;
                                sendbuf[0] = IAC;
                                sendbuf[1] = WILL;
                                sendbuf[2] = TELOPT_NAWS;

                                Write(sendbuf);
                                Write(IAC);
                                Write(SB);
                                Write(TELOPT_NAWS);
                                Write((byte) (size.Width >> 8));
                                Write((byte) (size.Width & 0xff));
                                Write((byte) (size.Height >> 8));
                                Write((byte) (size.Height & 0xff));
                                Write(IAC);
                                Write(SE);
                                break;

                            default:
                                reply = WONT;
                                break;
                        }

                        if (reply != sentWX[128 + b] || DO != receivedDX[128 + b])
                        {
                            sendbuf[0] = IAC;
                            sendbuf[1] = reply;
                            sendbuf[2] = b;
                            Write(sendbuf);

                            sentWX[b + 128] = reply;
                            receivedDX[b + 128] = DO;
                        }

                        neg_state = STATE_DATA;
                        break;

                    case STATE_IACDONT:
                        switch (b)
                        {
                            case TELOPT_ECHO:
                                reply = WONT;
                                SetLocalEcho(false);
                                break;

                            case TELOPT_SGA:
                                reply = WONT;
                                break;

                            case TELOPT_NAWS:
                                reply = WONT;
                                break;

                            case TELOPT_BINARY:
                                reply = WONT;
                                break;

                            default:
                                reply = WONT;
                                break;
                        }

                        if (reply != sentWX[b + 128] || DONT != receivedDX[b + 128])
                        {
                            sendbuf[0] = IAC;
                            sendbuf[1] = reply;
                            sendbuf[2] = b;
                            Write(sendbuf);

                            sentWX[b + 128] = reply;
                            receivedDX[b + 128] = DONT;
                        }

                        neg_state = STATE_DATA;
                        break;

                    case STATE_IACSBIAC:

                        for (var i = boffset; i < tempbuf.Length; i++)
                            if (tempbuf[i] == SE)
                                foundSE = true;

                        if (!foundSE)
                        {
                            boffset--;
                            done = true;
                            break;
                        }

                        foundSE = false;

                        if (b == IAC)
                        {
                            sbcount = 0;
                            current_sb = b;
                            neg_state = STATE_IACSBDATA;
                        }
                        else
                        {
                            neg_state = STATE_DATA;
                        }
                        break;

                    case STATE_IACSB:

                        for (var i = boffset; i < tempbuf.Length; i++)
                            if (tempbuf[i] == SE)
                                foundSE = true;

                        if (!foundSE)
                        {
                            boffset--;
                            done = true;
                            break;
                        }

                        foundSE = false;

                        switch (b)
                        {
                            case IAC:
                                neg_state = STATE_IACSBIAC;
                                break;

                            default:
                                current_sb = b;
                                sbcount = 0;
                                neg_state = STATE_IACSBDATA;
                                break;
                        }
                        break;

                    case STATE_IACSBDATA:

                        for (var i = boffset; i < tempbuf.Length; i++)
                            if (tempbuf[i] == SE)
                                foundSE = true;

                        if (!foundSE)
                        {
                            boffset--;
                            done = true;
                            break;
                        }

                        foundSE = false;

                        switch (b)
                        {
                            case IAC:
                                neg_state = STATE_IACSBDATAIAC;
                                break;
                            default:
                                sbbuf[sbcount++] = b;
                                break;
                        }
                        break;

                    case STATE_IACSBDATAIAC:
                        switch (b)
                        {
                            case IAC:
                                neg_state = STATE_IACSBDATA;
                                sbbuf[sbcount++] = IAC;
                                break;
                            case SE:
                                HandleSB(current_sb, sbbuf, sbcount);
                                current_sb = 0;
                                neg_state = STATE_DATA;
                                break;
                            case SB:
                                HandleSB(current_sb, sbbuf, sbcount);
                                neg_state = STATE_IACSB;
                                break;
                            default:
                                neg_state = STATE_DATA;
                                break;
                        }
                        break;

                    default:
                        neg_state = STATE_DATA;
                        break;
                }
            }

            var xb = new byte[count - boffset];
            Array.Copy(tempbuf, boffset, xb, 0, count - boffset);
            tempbuf = xb;

            return noffset;
        }

        #endregion
    }
}
