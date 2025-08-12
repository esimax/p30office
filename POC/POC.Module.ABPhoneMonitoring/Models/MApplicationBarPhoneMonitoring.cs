using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.POLPhoneItem;
using POL.WPF.DXControls.MVVM;
using System.Collections.ObjectModel;
using POC.Library.WCF.ProxyPhoneMonitoring;
using System.Windows.Controls;

namespace POC.Module.ABPhoneMonitoring.Models
{
    public class MApplicationBarPhoneMonitoring : NotifyObjectBase, IDisposable
    {

        private IMembership AMembership { get; set; }
        private IDatabase ADatabase { get; set; }
        private POCCore APOCCore { get; set; }

        private dynamic MainView { get; set; }
        private UserControl DynamicUserControl { get; set; }
        private List<POLPhoneItem> DynamicPhoneItems { get; set; }
        private ProtocolPhoneMonitoringClient Proxy { get; set; }


        public MApplicationBarPhoneMonitoring(object mainView)
        {
            MainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogin)
                    {
                        if (AMembership.HasPermission(PCOPermissions.Tools_ViewMonitoring))
                            HelperUtils.Try(() => Proxy.Open());
                    }
                    if (e.Status == EnumMembershipStatus.AfterLogout)
                        HelperUtils.Try(() => Proxy.Close());
                    RaisePropertyChanged("AllowMonitoring");
                };
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();






            AllData = new ObservableCollection<PhoneMonitoringDataWrapper>();
            for (var i = 0; i < ConstantGeneral.MaximumPhoneLineSupported; i++)
                AllData.Add(new PhoneMonitoringDataWrapper { Data = new PhoneMonitoringData() });

            InitDynamics();
            InitCommands();

            try
            {
                var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                    HelperSettingsClient.ServerName,
                    Convert.ToInt32(HelperSettingsClient.ServerPort) +
                    ConstantGeneral.ProtocolPhoneMonitoringPortOffset,
                    ConstantGeneral.ProtocolPhoneMonitoringServiceName));
                Proxy = new ProtocolPhoneMonitoringClient(new NetTcpBinding(SecurityMode.None), address);
                Proxy.GetMonitoringDataCompleted +=
                    (s1, e1) =>
                    {
                        if (e1.Error != null)
                        {
                            foreach (PhoneMonitoringDataWrapper pmdw in AllData)
                                pmdw.Data.Status = EnumCallSystemLineStatus.Unkown;
                            return;
                        }
                        if (e1.Result == null) return;
                        for (var i = 0; i < e1.Result.Count(); i++)
                            AllData[i].SetData(e1.Result[i]);
                    };

                APOCCore.RegisterForOneSecondTimer(
                    () =>
                    {
                        if (Proxy.State == CommunicationState.Opened)
                        {
                            if ((int)DynamicUserControl.GetValue(Panel.ZIndexProperty) == 1000)
                                Proxy.GetMonitoringDataAsync();
                        }
                    });
            }
            catch
            {

            }

            ShowAll = HelperSettingsClient.ABPhoneMonShowAll;
            ShowCallIn = HelperSettingsClient.ABPhoneMonShowIn;
            ShowCallOut = HelperSettingsClient.ABPhoneMonShowOut;
            ShowExtra = HelperSettingsClient.ABPhoneMonShowExtra;
        }




        #region AllData
        public static ObservableCollection<PhoneMonitoringDataWrapper> AllData { get; set; }
        #endregion

        #region ShowAll
        private bool _ShowAll;
        public bool ShowAll
        {
            get { return _ShowAll; }
            set
            {
                if (_ShowAll == value) return;
                _ShowAll = value;
                RaisePropertyChanged("ShowAll");
                for (var i = 0; i < ConstantGeneral.MaximumPhoneLineSupported; i++)
                    AllData[i].SetForceShow(value);
                HelperSettingsClient.ABPhoneMonShowAll = value;
            }
        }
        #endregion

        #region ShowCallOut
        private bool _ShowCallOut;
        public bool ShowCallOut
        {
            get { return _ShowCallOut; }
            set
            {
                if (_ShowCallOut == value) return;
                _ShowCallOut = value;
                RaisePropertyChanged("ShowCallOut");
                RaisePropertyChanged("CallOutVisibility");
                HelperSettingsClient.ABPhoneMonShowOut = value;
            }
        }
        #endregion

        #region ShowCallIn
        private bool _ShowCallIn;
        public bool ShowCallIn
        {
            get { return _ShowCallIn; }
            set
            {
                if (_ShowCallIn == value) return;
                _ShowCallIn = value;
                RaisePropertyChanged("ShowCallIn");
                RaisePropertyChanged("CallInVisibility");
                HelperSettingsClient.ABPhoneMonShowIn = value;
            }
        }
        #endregion

        #region ShowExtra
        private bool _ShowExtra;
        public bool ShowExtra
        {
            get { return _ShowExtra; }
            set
            {
                if (_ShowExtra == value) return;
                _ShowExtra = value;
                RaisePropertyChanged("ShowExtra");
                RaisePropertyChanged("ExtraVisibility");
                HelperSettingsClient.ABPhoneMonShowExtra = value;
            }
        }
        #endregion

        #region CallOutVisibility
        public Visibility CallOutVisibility
        {
            get { return ShowCallOut ? Visibility.Visible : Visibility.Collapsed; }
        }
        #endregion

        #region CallInVisibility
        public Visibility CallInVisibility
        {
            get { return ShowCallIn ? Visibility.Visible : Visibility.Collapsed; }
        }
        #endregion

        #region ExtraVisibility
        public Visibility ExtraVisibility
        {
            get { return ShowExtra ? Visibility.Visible : Visibility.Collapsed; }
        }
        #endregion



        public bool AllowMonitoring
        {
            get
            {
                return AMembership.HasPermission(PCOPermissions.Tools_ViewMonitoring);
            }
        }







        #region [METHODS]
        private void InitDynamics()
        {
            DynamicPhoneItems = MainView.DynamicPhoneItems;
            DynamicUserControl = MainView as UserControl;
        }
        private void InitCommands()
        {
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp02 != "");
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp02);
        }
        #endregion



        #region [COMMANDS]
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion
    }
}
