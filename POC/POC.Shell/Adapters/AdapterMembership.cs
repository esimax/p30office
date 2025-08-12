using System;
using System.ServiceModel;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;

namespace POC.Shell.Adapters
{
    internal class AdapterMembership : IMembership
    {
        #region ADatabase
        private IDatabase _ADatabase;
        private IDatabase ADatabase
        {
            get
            {
                if (ServiceLocator.Current == null)
                    return null;
                if (_ADatabase != null)
                    return _ADatabase;
                try
                {
                    _ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                    return _ADatabase;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion
        #region ALogger
        private ILoggerFacade _ALogger;
        private ILoggerFacade ALogger
        {
            get
            {
                if (ServiceLocator.Current == null)
                    return null;
                if (_ALogger != null)
                    return _ALogger;
                try
                {
                    _ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                    return _ALogger;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion

        #region APOCCore
        private POCCore _APOCCore;
        private POCCore APOCCore
        {
            get
            {
                if (ServiceLocator.Current == null)
                    return null;
                if (_APOCCore != null)
                    return _APOCCore;
                try
                {
                    _APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
                    return _APOCCore;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion

        #region ClientHeartTimer
        private Thread ClientHeartThread { get; set; }
        #endregion

        public AdapterMembership()
        {

            OnMembershipStatusChanged +=
                (s, e) =>
                {
                    switch (e.Status)
                    {
                        case EnumMembershipStatus.AfterLogin:
                            ClientHeartThread = new Thread(HeartBeep) { IsBackground = true };
                            ClientHeartThread.Start();
                            break;
                        default:
                            if (ClientHeartThread != null)
                                try
                                {
                                    ClientHeartThread.Abort();
                                    ClientHeartThread = null;
                                }
                                catch
                                {

                                }
                            break;
                    }
                };

        }

        private void HeartBeep(object obj)
        {
            var binding = new NetTcpBinding
            {
                TransferMode = TransferMode.Buffered,
                Security = { Mode = SecurityMode.None },
                ReliableSession = { Enabled = false, Ordered = true, InactivityTimeout = new TimeSpan(0, 10, 0) },
                MaxReceivedMessageSize = 65536L,
            };
            var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                            HelperSettingsClient.ServerName,
                                                            Convert.ToInt32(HelperSettingsClient.ServerPort) +
                                                            ConstantGeneral.ProtocolMembershipPortOffset,
                                                                            ConstantGeneral.ProtocolMembershipServiceName));
            POCCore pocCore = null;
            try
            {
                pocCore = ServiceLocator.Current.GetInstance<POCCore>();
            }
            catch { }

            try
            {
                var pxCore = new Library.WCF.ProxyMembership.ProtocolMembershipClient(binding, address);
                pxCore.Open();
                while (pocCore != null)
                {
                    if (IsAuthorized)
                    {
                        pxCore.ReregisterUser(pocCore.InstanceGuid);
                    }
                    Thread.Sleep(6000);
                }
            }
            catch
            {
                LogoutUser(APOCCore.CTSI.ID);
            }
        }


        public bool IsAuthorized
        {
            get { return ActiveUser != null && Status == EnumMembershipStatus.AfterLogin; }
        }

        public MembershipUser ActiveUser { get; private set; }
        public bool HasPermission(object code)
        {
            try
            {
                return IsAuthorized && ActiveUser.HasPermission(Convert.ToInt32(code));
            }
            catch
            {
                return false;
            }
        }
        public EnumMembershipStatus Status { get; private set; }

        public event EventHandler<MembershipStatusEventArg> OnMembershipStatusChanged;
        private void RaiseOnMembershipStatusChanged(EnumMembershipStatus status)
        {

            if (OnMembershipStatusChanged == null) return;
            var v = OnMembershipStatusChanged;
            v.Invoke(this, new MembershipStatusEventArg(status));

        }

        public void LoginUser(string username, string password, Guid id)
        {
            try
            {
                var binding = new NetTcpBinding
                                  {
                                      TransferMode = TransferMode.Buffered,
                                      Security = { Mode = SecurityMode.None },
                                      ReliableSession = { Enabled = false, Ordered = true, InactivityTimeout = new TimeSpan(0, 10, 0) },
                                      MaxReceivedMessageSize = 65536L,
                                  };
                var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                                HelperSettingsClient.ServerName,
                                                                Convert.ToInt32(HelperSettingsClient.ServerPort) +
                                                                ConstantGeneral.ProtocolMembershipPortOffset,
                                                                ConstantGeneral.ProtocolMembershipServiceName));
                var pxCore = new Library.WCF.ProxyMembership.ProtocolMembershipClient(binding, address);
                pxCore.Open();


                pxCore.RegisterUserCompleted +=
                    (s, e) =>
                    {
                        if (e.Error == null)
                        {
                            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Send,
                                new Action(
                                    () =>
                                    {
                                        if (e.Result != null)
                                        {
                                            ActiveUser = e.Result;
                                            Status = EnumMembershipStatus.AfterLogin;
                                            RaiseOnMembershipStatusChanged(Status);
                                        }
                                        else
                                        {
                                            ActiveUser = null;
                                            Status = EnumMembershipStatus.AccessDenide;
                                            RaiseOnMembershipStatusChanged(Status);
                                        }
                                    }
                                    ));
                        }
                        else
                        {
                            RaiseOnMembershipStatusChanged(EnumMembershipStatus.InvalidNetwork);
                        }
                    };

                pxCore.ValidateUserCompleted +=
                    (s, e) =>
                    {
                        if (e.Error == null)
                        {
                            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Send,
                                new Action(
                                    () =>
                                    {
                                        if (e.Result)
                                        {
                                            Status = EnumMembershipStatus.BeforLogin;
                                            RaiseOnMembershipStatusChanged(Status);
                                            pxCore.RegisterUserAsync(id, username, password);
                                        }
                                        else
                                        {
                                            Status = EnumMembershipStatus.AccessDenide;
                                            RaiseOnMembershipStatusChanged(Status);
                                        }
                                    }));
                        }
                        else
                        {
                            System.Windows.Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Send,
                                new Action(
                                    () =>
                                    {
                                        Status = EnumMembershipStatus.InvalidNetwork;
                                        RaiseOnMembershipStatusChanged(Status);
                                    }));
                        }
                    };
                pxCore.ValidateUserAsync(username, password, id);
            }
            catch
            {
                if (OnMembershipStatusChanged == null) return;
                var v = OnMembershipStatusChanged;
                v.Invoke(this, new MembershipStatusEventArg(EnumMembershipStatus.InvalidNetwork));
            }
        }
        public void LogoutUser(Guid instanceid)
        {
            try
            {
                var binding = new NetTcpBinding
                {
                    TransferMode = TransferMode.Buffered,
                    Security = { Mode = SecurityMode.None },
                    ReliableSession = { Enabled = false, Ordered = true, InactivityTimeout = new TimeSpan(0, 10, 0) },
                    MaxReceivedMessageSize = 65536L,
                };
                var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                                HelperSettingsClient.ServerName,
                                                                Convert.ToInt32(HelperSettingsClient.ServerPort) +
                                                                ConstantGeneral.ProtocolMembershipPortOffset,
                                                                ConstantGeneral.ProtocolMembershipServiceName));
                var pxCore = new Library.WCF.ProxyMembership.ProtocolMembershipClient(binding, address);
                pxCore.Open();

                pxCore.LogoutUserCompleted +=
                    (s, e) =>
                    {
                        if (e.Error == null)
                        {
                            POL.Lib.Utils.HelperUtils.DoDispatcher(
                                    () =>
                                    {
                                        if (e.Result)
                                        {
                                            ActiveUser = null;
                                            Status = EnumMembershipStatus.AfterLogout;
                                            RaiseOnMembershipStatusChanged(Status);
                                        }
                                        else
                                        {
                                            Status = EnumMembershipStatus.InvalidNetwork;
                                            RaiseOnMembershipStatusChanged(Status);
                                        }
                                    }
                                    );
                        }
                        else
                        {
                            POL.Lib.Utils.HelperUtils.DoDispatcher(() => RaiseOnMembershipStatusChanged(EnumMembershipStatus.InvalidNetwork));
                        }
                    };
                pxCore.LogoutUserAsync(instanceid);
            }
            catch
            {
                POL.Lib.Utils.HelperUtils.DoDispatcher(
                    () =>
                    {
                        ActiveUser = null;
                        Status = EnumMembershipStatus.AfterLogout;
                        RaiseOnMembershipStatusChanged(Status);

                    }, DispatcherPriority.Normal);
            }
        }
    }
}
