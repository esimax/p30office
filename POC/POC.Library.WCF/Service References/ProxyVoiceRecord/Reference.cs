
namespace POC.Library.WCF.ProxyVoiceRecord {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ProxyVoiceRecord.IProtocolVoiceRecord")]
    public interface IProtocolVoiceRecord {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolVoiceRecord/DownloadFile", ReplyAction="http://tempuri.org/IProtocolVoiceRecord/DownloadFileResponse")]
        POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadReturns DownloadFile(POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolVoiceRecord/DeleteMapItem", ReplyAction="http://tempuri.org/IProtocolVoiceRecord/DeleteMapItemResponse")]
        POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDeleteReturns DeleteMapItem(POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="VoiceRecordDownloadParameter", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class VoiceRecordDownloadParameter {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string RecordTag;
        
        public VoiceRecordDownloadParameter() {
        }
        
        public VoiceRecordDownloadParameter(string RecordTag) {
            this.RecordTag = RecordTag;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="VoiceRecordDownloadReturns", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class VoiceRecordDownloadReturns {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public long Length;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string RecordTag;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string TopException;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public System.IO.Stream FileByteStream;
        
        public VoiceRecordDownloadReturns() {
        }
        
        public VoiceRecordDownloadReturns(long Length, string RecordTag, string TopException, System.IO.Stream FileByteStream) {
            this.Length = Length;
            this.RecordTag = RecordTag;
            this.TopException = TopException;
            this.FileByteStream = FileByteStream;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="VoiceRecordDeleteReturns", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class VoiceRecordDeleteReturns {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public bool Succeed;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string TopException;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string InnerMessage;
        
        public VoiceRecordDeleteReturns() {
        }
        
        public VoiceRecordDeleteReturns(bool Succeed, string TopException, string InnerMessage) {
            this.Succeed = Succeed;
            this.TopException = TopException;
            this.InnerMessage = InnerMessage;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProtocolVoiceRecordChannel : POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProtocolVoiceRecordClient : System.ServiceModel.ClientBase<POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord>, POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord {
        
        public ProtocolVoiceRecordClient() {
        }
        
        public ProtocolVoiceRecordClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProtocolVoiceRecordClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolVoiceRecordClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolVoiceRecordClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadReturns POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord.DownloadFile(POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter request) {
            return base.Channel.DownloadFile(request);
        }
        
        public long DownloadFile(ref string RecordTag, out string TopException, out System.IO.Stream FileByteStream) {
            POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter inValue = new POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter();
            inValue.RecordTag = RecordTag;
            POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadReturns retVal = ((POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord)(this)).DownloadFile(inValue);
            RecordTag = retVal.RecordTag;
            TopException = retVal.TopException;
            FileByteStream = retVal.FileByteStream;
            return retVal.Length;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDeleteReturns POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord.DeleteMapItem(POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter request) {
            return base.Channel.DeleteMapItem(request);
        }
        
        public bool DeleteMapItem(string RecordTag, out string TopException, out string InnerMessage) {
            POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter inValue = new POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter();
            inValue.RecordTag = RecordTag;
            POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDeleteReturns retVal = ((POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord)(this)).DeleteMapItem(inValue);
            TopException = retVal.TopException;
            InnerMessage = retVal.InnerMessage;
            return retVal.Succeed;
        }
    }
}
