
namespace POC.Library.WCF.ProxyMessaging {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ProxyMessaging.IProtocolMessagingServer")]
    public interface IProtocolMessagingServer {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolMessagingServer/GetMyMessages", ReplyAction="http://tempuri.org/IProtocolMessagingServer/GetMyMessagesResponse")]
        POL.Lib.Interfaces.MessagingItem[] GetMyMessages(System.Guid clientOid, System.DateTime from, bool isFirstTime);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolMessagingServer/GetMyMessages", ReplyAction="http://tempuri.org/IProtocolMessagingServer/GetMyMessagesResponse")]
        System.IAsyncResult BeginGetMyMessages(System.Guid clientOid, System.DateTime from, bool isFirstTime, System.AsyncCallback callback, object asyncState);
        
        POL.Lib.Interfaces.MessagingItem[] EndGetMyMessages(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolMessagingServer/SendMyMessage", ReplyAction="http://tempuri.org/IProtocolMessagingServer/SendMyMessageResponse")]
        void SendMyMessage(POL.Lib.Interfaces.MessagingItem message);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolMessagingServer/SendMyMessage", ReplyAction="http://tempuri.org/IProtocolMessagingServer/SendMyMessageResponse")]
        System.IAsyncResult BeginSendMyMessage(POL.Lib.Interfaces.MessagingItem message, System.AsyncCallback callback, object asyncState);
        
        void EndSendMyMessage(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProtocolMessagingServerChannel : POC.Library.WCF.ProxyMessaging.IProtocolMessagingServer, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetMyMessagesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetMyMessagesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public POL.Lib.Interfaces.MessagingItem[] Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((POL.Lib.Interfaces.MessagingItem[])(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProtocolMessagingServerClient : System.ServiceModel.ClientBase<POC.Library.WCF.ProxyMessaging.IProtocolMessagingServer>, POC.Library.WCF.ProxyMessaging.IProtocolMessagingServer {
        
        private BeginOperationDelegate onBeginGetMyMessagesDelegate;
        
        private EndOperationDelegate onEndGetMyMessagesDelegate;
        
        private System.Threading.SendOrPostCallback onGetMyMessagesCompletedDelegate;
        
        private BeginOperationDelegate onBeginSendMyMessageDelegate;
        
        private EndOperationDelegate onEndSendMyMessageDelegate;
        
        private System.Threading.SendOrPostCallback onSendMyMessageCompletedDelegate;
        
        public ProtocolMessagingServerClient() {
        }
        
        public ProtocolMessagingServerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProtocolMessagingServerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolMessagingServerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolMessagingServerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<GetMyMessagesCompletedEventArgs> GetMyMessagesCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SendMyMessageCompleted;
        
        public POL.Lib.Interfaces.MessagingItem[] GetMyMessages(System.Guid clientOid, System.DateTime from, bool isFirstTime) {
            return base.Channel.GetMyMessages(clientOid, from, isFirstTime);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetMyMessages(System.Guid clientOid, System.DateTime from, bool isFirstTime, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetMyMessages(clientOid, from, isFirstTime, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public POL.Lib.Interfaces.MessagingItem[] EndGetMyMessages(System.IAsyncResult result) {
            return base.Channel.EndGetMyMessages(result);
        }
        
        private System.IAsyncResult OnBeginGetMyMessages(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid clientOid = ((System.Guid)(inValues[0]));
            System.DateTime from = ((System.DateTime)(inValues[1]));
            bool isFirstTime = ((bool)(inValues[2]));
            return this.BeginGetMyMessages(clientOid, from, isFirstTime, callback, asyncState);
        }
        
        private object[] OnEndGetMyMessages(System.IAsyncResult result) {
            POL.Lib.Interfaces.MessagingItem[] retVal = this.EndGetMyMessages(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetMyMessagesCompleted(object state) {
            if ((this.GetMyMessagesCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetMyMessagesCompleted(this, new GetMyMessagesCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetMyMessagesAsync(System.Guid clientOid, System.DateTime from, bool isFirstTime) {
            this.GetMyMessagesAsync(clientOid, from, isFirstTime, null);
        }
        
        public void GetMyMessagesAsync(System.Guid clientOid, System.DateTime from, bool isFirstTime, object userState) {
            if ((this.onBeginGetMyMessagesDelegate == null)) {
                this.onBeginGetMyMessagesDelegate = new BeginOperationDelegate(this.OnBeginGetMyMessages);
            }
            if ((this.onEndGetMyMessagesDelegate == null)) {
                this.onEndGetMyMessagesDelegate = new EndOperationDelegate(this.OnEndGetMyMessages);
            }
            if ((this.onGetMyMessagesCompletedDelegate == null)) {
                this.onGetMyMessagesCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetMyMessagesCompleted);
            }
            base.InvokeAsync(this.onBeginGetMyMessagesDelegate, new object[] {
                        clientOid,
                        from,
                        isFirstTime}, this.onEndGetMyMessagesDelegate, this.onGetMyMessagesCompletedDelegate, userState);
        }
        
        public void SendMyMessage(POL.Lib.Interfaces.MessagingItem message) {
            base.Channel.SendMyMessage(message);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginSendMyMessage(POL.Lib.Interfaces.MessagingItem message, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSendMyMessage(message, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndSendMyMessage(System.IAsyncResult result) {
            base.Channel.EndSendMyMessage(result);
        }
        
        private System.IAsyncResult OnBeginSendMyMessage(object[] inValues, System.AsyncCallback callback, object asyncState) {
            POL.Lib.Interfaces.MessagingItem message = ((POL.Lib.Interfaces.MessagingItem)(inValues[0]));
            return this.BeginSendMyMessage(message, callback, asyncState);
        }
        
        private object[] OnEndSendMyMessage(System.IAsyncResult result) {
            this.EndSendMyMessage(result);
            return null;
        }
        
        private void OnSendMyMessageCompleted(object state) {
            if ((this.SendMyMessageCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendMyMessageCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SendMyMessageAsync(POL.Lib.Interfaces.MessagingItem message) {
            this.SendMyMessageAsync(message, null);
        }
        
        public void SendMyMessageAsync(POL.Lib.Interfaces.MessagingItem message, object userState) {
            if ((this.onBeginSendMyMessageDelegate == null)) {
                this.onBeginSendMyMessageDelegate = new BeginOperationDelegate(this.OnBeginSendMyMessage);
            }
            if ((this.onEndSendMyMessageDelegate == null)) {
                this.onEndSendMyMessageDelegate = new EndOperationDelegate(this.OnEndSendMyMessage);
            }
            if ((this.onSendMyMessageCompletedDelegate == null)) {
                this.onSendMyMessageCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSendMyMessageCompleted);
            }
            base.InvokeAsync(this.onBeginSendMyMessageDelegate, new object[] {
                        message}, this.onEndSendMyMessageDelegate, this.onSendMyMessageCompletedDelegate, userState);
        }
    }
}
