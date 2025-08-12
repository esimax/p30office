using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace POL.Lib.Interfaces
{
    [ServiceContract]
    public interface IProtocolVoiceRecord
    {
        [OperationContract]
        VoiceRecordDownloadReturns DownloadFile(VoiceRecordDownloadParameter item);
       
        [OperationContract]
        VoiceRecordDeleteReturns DeleteMapItem(VoiceRecordDownloadParameter item);
    }

    [MessageContract]
    public class VoiceRecordDeleteReturns
    {
        [MessageHeader(MustUnderstand = true)]
        public string TopException;
        [MessageBodyMember(Order = 1)]
        public string InnerMessage;

        [MessageHeader(MustUnderstand = true)]
        public bool Succeed;
    }

    [MessageContract]
    public class VoiceRecordDownloadParameter
    {
        [MessageBodyMember]
        public string RecordTag;
    }

    [MessageContract]
    public class VoiceRecordDownloadReturns : IDisposable
    {
        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
        
        [MessageHeader(MustUnderstand = true)]
        public string RecordTag;

        [MessageHeader(MustUnderstand = true)]
        public long Length;

        [MessageHeader(MustUnderstand = true)]
        public string TopException;
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
