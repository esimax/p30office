
namespace POC.Library.WCF.ProxyCore {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ProxyCore.IProtocolCore")]
    public interface IProtocolCore {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolCore/ShareInformation", ReplyAction="http://tempuri.org/IProtocolCore/ShareInformationResponse")]
        POL.Lib.Interfaces.ServerToClientInformation ShareInformation(POL.Lib.Interfaces.ClientToServerInformation c2Si);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolCore/ShareInformation", ReplyAction="http://tempuri.org/IProtocolCore/ShareInformationResponse")]
        System.IAsyncResult BeginShareInformation(POL.Lib.Interfaces.ClientToServerInformation c2Si, System.AsyncCallback callback, object asyncState);
        
        POL.Lib.Interfaces.ServerToClientInformation EndShareInformation(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProtocolCoreChannel : POC.Library.WCF.ProxyCore.IProtocolCore, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ShareInformationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public ShareInformationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public POL.Lib.Interfaces.ServerToClientInformation Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((POL.Lib.Interfaces.ServerToClientInformation)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProtocolCoreClient : System.ServiceModel.ClientBase<POC.Library.WCF.ProxyCore.IProtocolCore>, POC.Library.WCF.ProxyCore.IProtocolCore {
        
        private BeginOperationDelegate onBeginShareInformationDelegate;
        
        private EndOperationDelegate onEndShareInformationDelegate;
        
        private System.Threading.SendOrPostCallback onShareInformationCompletedDelegate;
        
        public ProtocolCoreClient() {
        }
        
        public ProtocolCoreClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProtocolCoreClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolCoreClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolCoreClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<ShareInformationCompletedEventArgs> ShareInformationCompleted;
        
        public POL.Lib.Interfaces.ServerToClientInformation ShareInformation(POL.Lib.Interfaces.ClientToServerInformation c2Si) {
            return base.Channel.ShareInformation(c2Si);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginShareInformation(POL.Lib.Interfaces.ClientToServerInformation c2Si, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginShareInformation(c2Si, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public POL.Lib.Interfaces.ServerToClientInformation EndShareInformation(System.IAsyncResult result) {
            return base.Channel.EndShareInformation(result);
        }
        
        private System.IAsyncResult OnBeginShareInformation(object[] inValues, System.AsyncCallback callback, object asyncState) {
            POL.Lib.Interfaces.ClientToServerInformation c2Si = ((POL.Lib.Interfaces.ClientToServerInformation)(inValues[0]));
            return this.BeginShareInformation(c2Si, callback, asyncState);
        }
        
        private object[] OnEndShareInformation(System.IAsyncResult result) {
            POL.Lib.Interfaces.ServerToClientInformation retVal = this.EndShareInformation(result);
            return new object[] {
                    retVal};
        }
        
        private void OnShareInformationCompleted(object state) {
            if ((this.ShareInformationCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ShareInformationCompleted(this, new ShareInformationCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ShareInformationAsync(POL.Lib.Interfaces.ClientToServerInformation c2Si) {
            this.ShareInformationAsync(c2Si, null);
        }
        
        public void ShareInformationAsync(POL.Lib.Interfaces.ClientToServerInformation c2Si, object userState) {
            if ((this.onBeginShareInformationDelegate == null)) {
                this.onBeginShareInformationDelegate = new BeginOperationDelegate(this.OnBeginShareInformation);
            }
            if ((this.onEndShareInformationDelegate == null)) {
                this.onEndShareInformationDelegate = new EndOperationDelegate(this.OnEndShareInformation);
            }
            if ((this.onShareInformationCompletedDelegate == null)) {
                this.onShareInformationCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnShareInformationCompleted);
            }
            base.InvokeAsync(this.onBeginShareInformationDelegate, new object[] {
                        c2Si}, this.onEndShareInformationDelegate, this.onShareInformationCompletedDelegate, userState);
        }
    }
}
