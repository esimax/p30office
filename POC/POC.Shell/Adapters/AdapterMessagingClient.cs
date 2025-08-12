using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Practices.Prism.Logging;
using POL.Lib.Interfaces;
using POC.Library.WCF.ProxyMessaging;
using POL.Lib.XOffice;
using POL.Lib.Utils;

namespace POC.Shell.Adapters
{
    internal class AdapterMessagingClient : IMessagingClient
    {
        private POCCore APOCCore { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALog { get; set; }
        private IApplicationSettings ASettings { get; set; }

        private ProtocolMessagingServerClient Proxy { get; set; }
        private List<Tuple<Action<MessagingItem>, EnumMessageKind>> Hooks { get; set; }
        private DateTime LastDateCollect { get; set; }
        private bool IsFirst { get; set; }

        public AdapterMessagingClient(ILoggerFacade log, IMembership membership, POCCore poc, IApplicationSettings settings)
        {
            Hooks = new List<Tuple<Action<MessagingItem>, EnumMessageKind>>();
            APOCCore = poc;
            AMembership = membership;
            ALog = log;
            ASettings = settings;


            LastDateCollect = DateTime.Now;
            IsFirst = true;


            AMembership.OnMembershipStatusChanged += 
                (s, e) =>
                {
                    try
                    {
                        if (e.Status == EnumMembershipStatus.AfterLogin)
                        {
                            StartMessaging();
                            Proxy.Open();
                        }
                        if (e.Status == EnumMembershipStatus.AfterLogout)
                            HelperUtils.Try(() => Proxy.Close());
                    }
                    catch (Exception ex)
                    {
                        ALog.Log(ex.ToString(), Category.Exception, Priority.High);
                    }
                };




            APOCCore.RegisterForOneSecondTimer(
                () =>
                {
                    if (Proxy == null) return;
                    if (Proxy.State == CommunicationState.Opened)
                        Proxy.GetMyMessagesAsync(APOCCore.InstanceGuid, LastDateCollect, IsFirst);
                });
        }

        private void StartMessaging()
        {

            var serverport = ASettings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringServerPort);
            var servername = ASettings.GetValue<string, EnumPOCAppSettings>(EnumPOCAppSettings.StringServerName);

            var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                            servername,
                                                            Convert.ToInt32(serverport) + ConstantGeneral.ProtocolMessagingPortOffset,
                                                            ConstantGeneral.ProtocolMessagingServiceName));
            Proxy = new ProtocolMessagingServerClient(new NetTcpBinding(SecurityMode.None), address);
            Proxy.GetMyMessagesCompleted +=
                (s1, e1) =>
                {
                    if (e1.Error != null)
                    {
                        ALog.Log(e1.Error.ToString(), Category.Exception, Priority.High);
                        return;
                    }
                    if (e1.Result == null) return;
                    e1.Result.ToList().ForEach(
                        msg => Hooks.ForEach(
                            h =>
                            {
                                if (h.Item2 == msg.MessageKind)
                                    h.Item1.Invoke(msg);
                            }));
                    LastDateCollect = (from n in e1.Result select n.MessageDate).Max();
                };

        }

        public void RegisterHookForMessage(Action<MessagingItem> action, EnumMessageKind filter)
        {
            Hooks.Add(new Tuple<Action<MessagingItem>, EnumMessageKind>(action, filter));
        }

        public bool SendMessage(MessagingItem item)
        {
            if (Proxy == null) return false;
            if (Proxy.State != CommunicationState.Opened) return false;
            if (item == null) return false;
            Proxy.SendMyMessageAsync(item);
            return true;
        }
    }
}
