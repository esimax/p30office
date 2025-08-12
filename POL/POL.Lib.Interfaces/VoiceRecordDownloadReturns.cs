using System;
using System.IO;
using System.ServiceModel;

namespace POL.Lib.Interfaces
{
    [MessageContract]
    public class VoiceRecordDownloadReturns : IDisposable
    {
        [MessageBodyMember(Order = 1)] public Stream FileByteStream;

        [MessageHeader(MustUnderstand = true)] public long Length;

        [MessageHeader(MustUnderstand = true)] public string RecordTag;

        [MessageHeader(MustUnderstand = true)] public string TopException;

        #region IDisposable Members

        public void Dispose()
        {
            if (FileByteStream != null)
            {
                FileByteStream.Close();
                FileByteStream = null;
            }
        }

        #endregion
    }
}
