
namespace POC.Library.WCF.ProxyMembership {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ProxyMembership.IProtocolMembership")]
    public interface IProtocolMembership {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolMembership/ValidateUser", ReplyAction="http://tempuri.org/IProtocolMembership/ValidateUserResponse")]
        bool ValidateUser(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolMembership/ValidateUser", ReplyAction="http://tempuri.org/IProtocolMembership/ValidateUserResponse")]
        System.IAsyncResult BeginValidateUser(string username, string password, System.AsyncCallback callback, object asyncState);
        
        bool EndValidateUser(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolMembership/RegisterUser", ReplyAction="http://tempuri.org/IProtocolMembership/RegisterUserResponse")]
        POL.Lib.Interfaces.MembershipUser RegisterUser(System.Guid id, string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolMembership/RegisterUser", ReplyAction="http://tempuri.org/IProtocolMembership/RegisterUserResponse")]
        System.IAsyncResult BeginRegisterUser(System.Guid id, string username, string password, System.AsyncCallback callback, object asyncState);
        
        POL.Lib.Interfaces.MembershipUser EndRegisterUser(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolMembership/ReregisterUser", ReplyAction="http://tempuri.org/IProtocolMembership/ReregisterUserResponse")]
        bool ReregisterUser(System.Guid id);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolMembership/ReregisterUser", ReplyAction="http://tempuri.org/IProtocolMembership/ReregisterUserResponse")]
        System.IAsyncResult BeginReregisterUser(System.Guid id, System.AsyncCallback callback, object asyncState);
        
        bool EndReregisterUser(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProtocolMembership/LogoutUser", ReplyAction="http://tempuri.org/IProtocolMembership/LogoutUserResponse")]
        bool LogoutUser(System.Guid id);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IProtocolMembership/LogoutUser", ReplyAction="http://tempuri.org/IProtocolMembership/LogoutUserResponse")]
        System.IAsyncResult BeginLogoutUser(System.Guid id, System.AsyncCallback callback, object asyncState);
        
        bool EndLogoutUser(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProtocolMembershipChannel : POC.Library.WCF.ProxyMembership.IProtocolMembership, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ValidateUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public ValidateUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public partial class RegisterUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public RegisterUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public POL.Lib.Interfaces.MembershipUser Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((POL.Lib.Interfaces.MembershipUser)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ReregisterUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public ReregisterUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public partial class LogoutUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public LogoutUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public partial class ProtocolMembershipClient : System.ServiceModel.ClientBase<POC.Library.WCF.ProxyMembership.IProtocolMembership>, POC.Library.WCF.ProxyMembership.IProtocolMembership {
        
        private BeginOperationDelegate onBeginValidateUserDelegate;
        
        private EndOperationDelegate onEndValidateUserDelegate;
        
        private System.Threading.SendOrPostCallback onValidateUserCompletedDelegate;
        
        private BeginOperationDelegate onBeginRegisterUserDelegate;
        
        private EndOperationDelegate onEndRegisterUserDelegate;
        
        private System.Threading.SendOrPostCallback onRegisterUserCompletedDelegate;
        
        private BeginOperationDelegate onBeginReregisterUserDelegate;
        
        private EndOperationDelegate onEndReregisterUserDelegate;
        
        private System.Threading.SendOrPostCallback onReregisterUserCompletedDelegate;
        
        private BeginOperationDelegate onBeginLogoutUserDelegate;
        
        private EndOperationDelegate onEndLogoutUserDelegate;
        
        private System.Threading.SendOrPostCallback onLogoutUserCompletedDelegate;
        
        public ProtocolMembershipClient() {
        }
        
        public ProtocolMembershipClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProtocolMembershipClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolMembershipClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProtocolMembershipClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<ValidateUserCompletedEventArgs> ValidateUserCompleted;
        
        public event System.EventHandler<RegisterUserCompletedEventArgs> RegisterUserCompleted;
        
        public event System.EventHandler<ReregisterUserCompletedEventArgs> ReregisterUserCompleted;
        
        public event System.EventHandler<LogoutUserCompletedEventArgs> LogoutUserCompleted;
        
        public bool ValidateUser(string username, string password) {
            return base.Channel.ValidateUser(username, password);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginValidateUser(string username, string password, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginValidateUser(username, password, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndValidateUser(System.IAsyncResult result) {
            return base.Channel.EndValidateUser(result);
        }
        
        private System.IAsyncResult OnBeginValidateUser(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string username = ((string)(inValues[0]));
            string password = ((string)(inValues[1]));
            return this.BeginValidateUser(username, password, callback, asyncState);
        }
        
        private object[] OnEndValidateUser(System.IAsyncResult result) {
            bool retVal = this.EndValidateUser(result);
            return new object[] {
                    retVal};
        }
        
        private void OnValidateUserCompleted(object state) {
            if ((this.ValidateUserCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ValidateUserCompleted(this, new ValidateUserCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ValidateUserAsync(string username, string password) {
            this.ValidateUserAsync(username, password, null);
        }
        
        public void ValidateUserAsync(string username, string password, object userState) {
            if ((this.onBeginValidateUserDelegate == null)) {
                this.onBeginValidateUserDelegate = new BeginOperationDelegate(this.OnBeginValidateUser);
            }
            if ((this.onEndValidateUserDelegate == null)) {
                this.onEndValidateUserDelegate = new EndOperationDelegate(this.OnEndValidateUser);
            }
            if ((this.onValidateUserCompletedDelegate == null)) {
                this.onValidateUserCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnValidateUserCompleted);
            }
            base.InvokeAsync(this.onBeginValidateUserDelegate, new object[] {
                        username,
                        password}, this.onEndValidateUserDelegate, this.onValidateUserCompletedDelegate, userState);
        }
        
        public POL.Lib.Interfaces.MembershipUser RegisterUser(System.Guid id, string username, string password) {
            return base.Channel.RegisterUser(id, username, password);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginRegisterUser(System.Guid id, string username, string password, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginRegisterUser(id, username, password, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public POL.Lib.Interfaces.MembershipUser EndRegisterUser(System.IAsyncResult result) {
            return base.Channel.EndRegisterUser(result);
        }
        
        private System.IAsyncResult OnBeginRegisterUser(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid id = ((System.Guid)(inValues[0]));
            string username = ((string)(inValues[1]));
            string password = ((string)(inValues[2]));
            return this.BeginRegisterUser(id, username, password, callback, asyncState);
        }
        
        private object[] OnEndRegisterUser(System.IAsyncResult result) {
            POL.Lib.Interfaces.MembershipUser retVal = this.EndRegisterUser(result);
            return new object[] {
                    retVal};
        }
        
        private void OnRegisterUserCompleted(object state) {
            if ((this.RegisterUserCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RegisterUserCompleted(this, new RegisterUserCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void RegisterUserAsync(System.Guid id, string username, string password) {
            this.RegisterUserAsync(id, username, password, null);
        }
        
        public void RegisterUserAsync(System.Guid id, string username, string password, object userState) {
            if ((this.onBeginRegisterUserDelegate == null)) {
                this.onBeginRegisterUserDelegate = new BeginOperationDelegate(this.OnBeginRegisterUser);
            }
            if ((this.onEndRegisterUserDelegate == null)) {
                this.onEndRegisterUserDelegate = new EndOperationDelegate(this.OnEndRegisterUser);
            }
            if ((this.onRegisterUserCompletedDelegate == null)) {
                this.onRegisterUserCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRegisterUserCompleted);
            }
            base.InvokeAsync(this.onBeginRegisterUserDelegate, new object[] {
                        id,
                        username,
                        password}, this.onEndRegisterUserDelegate, this.onRegisterUserCompletedDelegate, userState);
        }
        
        public bool ReregisterUser(System.Guid id) {
            return base.Channel.ReregisterUser(id);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginReregisterUser(System.Guid id, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginReregisterUser(id, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndReregisterUser(System.IAsyncResult result) {
            return base.Channel.EndReregisterUser(result);
        }
        
        private System.IAsyncResult OnBeginReregisterUser(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid id = ((System.Guid)(inValues[0]));
            return this.BeginReregisterUser(id, callback, asyncState);
        }
        
        private object[] OnEndReregisterUser(System.IAsyncResult result) {
            bool retVal = this.EndReregisterUser(result);
            return new object[] {
                    retVal};
        }
        
        private void OnReregisterUserCompleted(object state) {
            if ((this.ReregisterUserCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ReregisterUserCompleted(this, new ReregisterUserCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ReregisterUserAsync(System.Guid id) {
            this.ReregisterUserAsync(id, null);
        }
        
        public void ReregisterUserAsync(System.Guid id, object userState) {
            if ((this.onBeginReregisterUserDelegate == null)) {
                this.onBeginReregisterUserDelegate = new BeginOperationDelegate(this.OnBeginReregisterUser);
            }
            if ((this.onEndReregisterUserDelegate == null)) {
                this.onEndReregisterUserDelegate = new EndOperationDelegate(this.OnEndReregisterUser);
            }
            if ((this.onReregisterUserCompletedDelegate == null)) {
                this.onReregisterUserCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnReregisterUserCompleted);
            }
            base.InvokeAsync(this.onBeginReregisterUserDelegate, new object[] {
                        id}, this.onEndReregisterUserDelegate, this.onReregisterUserCompletedDelegate, userState);
        }
        
        public bool LogoutUser(System.Guid id) {
            return base.Channel.LogoutUser(id);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginLogoutUser(System.Guid id, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginLogoutUser(id, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndLogoutUser(System.IAsyncResult result) {
            return base.Channel.EndLogoutUser(result);
        }
        
        private System.IAsyncResult OnBeginLogoutUser(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid id = ((System.Guid)(inValues[0]));
            return this.BeginLogoutUser(id, callback, asyncState);
        }
        
        private object[] OnEndLogoutUser(System.IAsyncResult result) {
            bool retVal = this.EndLogoutUser(result);
            return new object[] {
                    retVal};
        }
        
        private void OnLogoutUserCompleted(object state) {
            if ((this.LogoutUserCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.LogoutUserCompleted(this, new LogoutUserCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void LogoutUserAsync(System.Guid id) {
            this.LogoutUserAsync(id, null);
        }
        
        public void LogoutUserAsync(System.Guid id, object userState) {
            if ((this.onBeginLogoutUserDelegate == null)) {
                this.onBeginLogoutUserDelegate = new BeginOperationDelegate(this.OnBeginLogoutUser);
            }
            if ((this.onEndLogoutUserDelegate == null)) {
                this.onEndLogoutUserDelegate = new EndOperationDelegate(this.OnEndLogoutUser);
            }
            if ((this.onLogoutUserCompletedDelegate == null)) {
                this.onLogoutUserCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnLogoutUserCompleted);
            }
            base.InvokeAsync(this.onBeginLogoutUserDelegate, new object[] {
                        id}, this.onEndLogoutUserDelegate, this.onLogoutUserCompletedDelegate, userState);
        }
    }
}
