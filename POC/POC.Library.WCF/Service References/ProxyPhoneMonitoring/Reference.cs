
namespace POC.Library.WCF.ProxyPhoneMonitoring {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ProxyPhoneMonitoring.IProtocolPhoneMonitoring")]
    public interface IProtocolPhoneMonitoring {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolPhoneMonitoring/BlockLine", ReplyAction="http://tempuri.org/IProtocolPhoneMonitoring/BlockLineResponse")]
        bool BlockLine(string user, int line);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolPhoneMonitoring/BlockLine", ReplyAction="http://tempuri.org/IProtocolPhoneMonitoring/BlockLineResponse")]
        System.IAsyncResult BeginBlockLine(string user, int line, System.AsyncCallback callback, object asyncState);
        
        bool EndBlockLine(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolPhoneMonitoring/UnBlockLine", ReplyAction="http://tempuri.org/IProtocolPhoneMonitoring/UnBlockLineResponse")]
        bool UnBlockLine(string user, int line);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolPhoneMonitoring/UnBlockLine", ReplyAction="http://tempuri.org/IProtocolPhoneMonitoring/UnBlockLineResponse")]
        System.IAsyncResult BeginUnBlockLine(string user, int line, System.AsyncCallback callback, object asyncState);
        
        bool EndUnBlockLine(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolPhoneMonitoring/ToggleLine", ReplyAction="http://tempuri.org/IProtocolPhoneMonitoring/ToggleLineResponse")]
        bool ToggleLine(string user, int line);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolPhoneMonitoring/ToggleLine", ReplyAction="http://tempuri.org/IProtocolPhoneMonitoring/ToggleLineResponse")]
        System.IAsyncResult BeginToggleLine(string user, int line, System.AsyncCallback callback, object asyncState);
        
        bool EndToggleLine(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolPhoneMonitoring/GetMonitoringData", ReplyAction="http://tempuri.org/IProtocolPhoneMonitoring/GetMonitoringDataResponse")]
        POL.Lib.Interfaces.PhoneMonitoringData[] GetMonitoringData();
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolPhoneMonitoring/GetMonitoringData", ReplyAction="http://tempuri.org/IProtocolPhoneMonitoring/GetMonitoringDataResponse")]
        System.IAsyncResult BeginGetMonitoringData(System.AsyncCallback callback, object asyncState);
        
        POL.Lib.Interfaces.PhoneMonitoringData[] EndGetMonitoringData(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProtocolPhoneMonitoringChannel : POC.Library.WCF.ProxyPhoneMonitoring.IProtocolPhoneMonitoring, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BlockLineCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public BlockLineCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UnBlockLineCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public UnBlockLineCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ToggleLineCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public ToggleLineCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetMonitoringDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetMonitoringDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public POL.Lib.Interfaces.PhoneMonitoringData[] Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((POL.Lib.Interfaces.PhoneMonitoringData[])(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProtocolPhoneMonitoringClient : System.ServiceModel.ClientBase<POC.Library.WCF.ProxyPhoneMonitoring.IProtocolPhoneMonitoring>, POC.Library.WCF.ProxyPhoneMonitoring.IProtocolPhoneMonitoring {
        
        private BeginOperationDelegate onBeginBlockLineDelegate;
        
        private EndOperationDelegate onEndBlockLineDelegate;
        
        private System.Threading.SendOrPostCallback onBlockLineCompletedDelegate;
        
        private BeginOperationDelegate onBeginUnBlockLineDelegate;
        
        private EndOperationDelegate onEndUnBlockLineDelegate;
        
        private System.Threading.SendOrPostCallback onUnBlockLineCompletedDelegate;
        
        private BeginOperationDelegate onBeginToggleLineDelegate;
        
        private EndOperationDelegate onEndToggleLineDelegate;
        
        private System.Threading.SendOrPostCallback onToggleLineCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetMonitoringDataDelegate;
        
        private EndOperationDelegate onEndGetMonitoringDataDelegate;
        
        private System.Threading.SendOrPostCallback onGetMonitoringDataCompletedDelegate;
        
        public ProtocolPhoneMonitoringClient() {
        }
        
        public ProtocolPhoneMonitoringClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProtocolPhoneMonitoringClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolPhoneMonitoringClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolPhoneMonitoringClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<BlockLineCompletedEventArgs> BlockLineCompleted;
        
        public event System.EventHandler<UnBlockLineCompletedEventArgs> UnBlockLineCompleted;
        
        public event System.EventHandler<ToggleLineCompletedEventArgs> ToggleLineCompleted;
        
        public event System.EventHandler<GetMonitoringDataCompletedEventArgs> GetMonitoringDataCompleted;
        
        public bool BlockLine(string user, int line) {
            return base.Channel.BlockLine(user, line);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginBlockLine(string user, int line, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginBlockLine(user, line, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndBlockLine(System.IAsyncResult result) {
            return base.Channel.EndBlockLine(result);
        }
        
        private System.IAsyncResult OnBeginBlockLine(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string user = ((string)(inValues[0]));
            int line = ((int)(inValues[1]));
            return this.BeginBlockLine(user, line, callback, asyncState);
        }
        
        private object[] OnEndBlockLine(System.IAsyncResult result) {
            bool retVal = this.EndBlockLine(result);
            return new object[] {
                    retVal};
        }
        
        private void OnBlockLineCompleted(object state) {
            if ((this.BlockLineCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.BlockLineCompleted(this, new BlockLineCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void BlockLineAsync(string user, int line) {
            this.BlockLineAsync(user, line, null);
        }
        
        public void BlockLineAsync(string user, int line, object userState) {
            if ((this.onBeginBlockLineDelegate == null)) {
                this.onBeginBlockLineDelegate = new BeginOperationDelegate(this.OnBeginBlockLine);
            }
            if ((this.onEndBlockLineDelegate == null)) {
                this.onEndBlockLineDelegate = new EndOperationDelegate(this.OnEndBlockLine);
            }
            if ((this.onBlockLineCompletedDelegate == null)) {
                this.onBlockLineCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnBlockLineCompleted);
            }
            base.InvokeAsync(this.onBeginBlockLineDelegate, new object[] {
                        user,
                        line}, this.onEndBlockLineDelegate, this.onBlockLineCompletedDelegate, userState);
        }
        
        public bool UnBlockLine(string user, int line) {
            return base.Channel.UnBlockLine(user, line);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginUnBlockLine(string user, int line, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginUnBlockLine(user, line, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndUnBlockLine(System.IAsyncResult result) {
            return base.Channel.EndUnBlockLine(result);
        }
        
        private System.IAsyncResult OnBeginUnBlockLine(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string user = ((string)(inValues[0]));
            int line = ((int)(inValues[1]));
            return this.BeginUnBlockLine(user, line, callback, asyncState);
        }
        
        private object[] OnEndUnBlockLine(System.IAsyncResult result) {
            bool retVal = this.EndUnBlockLine(result);
            return new object[] {
                    retVal};
        }
        
        private void OnUnBlockLineCompleted(object state) {
            if ((this.UnBlockLineCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.UnBlockLineCompleted(this, new UnBlockLineCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void UnBlockLineAsync(string user, int line) {
            this.UnBlockLineAsync(user, line, null);
        }
        
        public void UnBlockLineAsync(string user, int line, object userState) {
            if ((this.onBeginUnBlockLineDelegate == null)) {
                this.onBeginUnBlockLineDelegate = new BeginOperationDelegate(this.OnBeginUnBlockLine);
            }
            if ((this.onEndUnBlockLineDelegate == null)) {
                this.onEndUnBlockLineDelegate = new EndOperationDelegate(this.OnEndUnBlockLine);
            }
            if ((this.onUnBlockLineCompletedDelegate == null)) {
                this.onUnBlockLineCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnUnBlockLineCompleted);
            }
            base.InvokeAsync(this.onBeginUnBlockLineDelegate, new object[] {
                        user,
                        line}, this.onEndUnBlockLineDelegate, this.onUnBlockLineCompletedDelegate, userState);
        }
        
        public bool ToggleLine(string user, int line) {
            return base.Channel.ToggleLine(user, line);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginToggleLine(string user, int line, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginToggleLine(user, line, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndToggleLine(System.IAsyncResult result) {
            return base.Channel.EndToggleLine(result);
        }
        
        private System.IAsyncResult OnBeginToggleLine(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string user = ((string)(inValues[0]));
            int line = ((int)(inValues[1]));
            return this.BeginToggleLine(user, line, callback, asyncState);
        }
        
        private object[] OnEndToggleLine(System.IAsyncResult result) {
            bool retVal = this.EndToggleLine(result);
            return new object[] {
                    retVal};
        }
        
        private void OnToggleLineCompleted(object state) {
            if ((this.ToggleLineCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ToggleLineCompleted(this, new ToggleLineCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ToggleLineAsync(string user, int line) {
            this.ToggleLineAsync(user, line, null);
        }
        
        public void ToggleLineAsync(string user, int line, object userState) {
            if ((this.onBeginToggleLineDelegate == null)) {
                this.onBeginToggleLineDelegate = new BeginOperationDelegate(this.OnBeginToggleLine);
            }
            if ((this.onEndToggleLineDelegate == null)) {
                this.onEndToggleLineDelegate = new EndOperationDelegate(this.OnEndToggleLine);
            }
            if ((this.onToggleLineCompletedDelegate == null)) {
                this.onToggleLineCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnToggleLineCompleted);
            }
            base.InvokeAsync(this.onBeginToggleLineDelegate, new object[] {
                        user,
                        line}, this.onEndToggleLineDelegate, this.onToggleLineCompletedDelegate, userState);
        }
        
        public POL.Lib.Interfaces.PhoneMonitoringData[] GetMonitoringData() {
            return base.Channel.GetMonitoringData();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetMonitoringData(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetMonitoringData(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public POL.Lib.Interfaces.PhoneMonitoringData[] EndGetMonitoringData(System.IAsyncResult result) {
            return base.Channel.EndGetMonitoringData(result);
        }
        
        private System.IAsyncResult OnBeginGetMonitoringData(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return this.BeginGetMonitoringData(callback, asyncState);
        }
        
        private object[] OnEndGetMonitoringData(System.IAsyncResult result) {
            POL.Lib.Interfaces.PhoneMonitoringData[] retVal = this.EndGetMonitoringData(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetMonitoringDataCompleted(object state) {
            if ((this.GetMonitoringDataCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetMonitoringDataCompleted(this, new GetMonitoringDataCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetMonitoringDataAsync() {
            this.GetMonitoringDataAsync(null);
        }
        
        public void GetMonitoringDataAsync(object userState) {
            if ((this.onBeginGetMonitoringDataDelegate == null)) {
                this.onBeginGetMonitoringDataDelegate = new BeginOperationDelegate(this.OnBeginGetMonitoringData);
            }
            if ((this.onEndGetMonitoringDataDelegate == null)) {
                this.onEndGetMonitoringDataDelegate = new EndOperationDelegate(this.OnEndGetMonitoringData);
            }
            if ((this.onGetMonitoringDataCompletedDelegate == null)) {
                this.onGetMonitoringDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetMonitoringDataCompleted);
            }
            base.InvokeAsync(this.onBeginGetMonitoringDataDelegate, null, this.onEndGetMonitoringDataDelegate, this.onGetMonitoringDataCompletedDelegate, userState);
        }
    }
}
