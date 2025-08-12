using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.ServiceLocation;
using POC.Library.WCF.ProxyCore;
using POL.Lib.Common.POLMembership;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using Ionic.Zip;
using System.Threading;

namespace POC.Module.ABMembership.Models
{
    public class MApplicationBarMembership : IDisposable, INotifyPropertyChanged
    {
        private IMembership AMembership { get; set; }
        private IDatabase ADatabase { get; set; }
        private POCCore APOCCore { get; set; }

        public MApplicationBarMembership(object mainView)
        {

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            AMembership.OnMembershipStatusChanged += 
                (s, e) =>
                {
                    switch (e.Status)
                    {
                        case EnumMembershipStatus.InvalidNetwork:
                            StatusMembership = EnumStatusGeneral.Invalid;
                            TooltipMembership = "بروز خطا در پروتوكل شبكه.";
                            IsBussy = false;
                            APOCCore.AppBarIsBussy = false;
                            break;
                        case EnumMembershipStatus.AccessDenide:
                            StatusMembership = EnumStatusGeneral.Invalid;
                            TooltipMembership = "چنین كاربری مجاز نمی باشد.";
                            IsBussy = false;
                            APOCCore.AppBarIsBussy = false;
                            break;
                        case EnumMembershipStatus.BeforLogin:
                            break;
                        case EnumMembershipStatus.AfterLogin:
                            StatusMembership = EnumStatusGeneral.OK;
                            TooltipMembership = "كاربر : " + UserName;
                            IsBussy = false;
                            APOCCore.AppBarIsBussy = false;
                            break;
                        case EnumMembershipStatus.BeforeLogout:
                            break;
                        case EnumMembershipStatus.AfterLogout:

                            Password = string.Empty;
                            StatusNetwork = EnumStatusGeneral.Undefined;
                            StatusDatabase = EnumStatusGeneral.Undefined;
                            StatusMembership = EnumStatusGeneral.Undefined;
                            StatusServerErrors = EnumStatusGeneral.Undefined;
                            StatusSyncDateTime = EnumStatusGeneral.Undefined;
                            StatusVersion = EnumStatusGeneral.Undefined;

                            TextNetwork = "وضعیت شبكه";

                            TooltipDatabase = null;
                            TooltipMembership = null;
                            TooltipNetwork = null;
                            TooltipServerErrors = null;
                            TooltipSyncDateTime = null;
                            TooltipVersion = null;

                            IsBussy = false;
                            APOCCore.AppBarIsBussy = false;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    OnPropertyChanged("");
                };

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            InitCommands();
            TextNetwork = "وضعیت شبكه";

            Task.Factory.StartNew(
                () =>
                {
                    Thread.Sleep(500);
                    if (AutoLogin)
                        HelperUtils.DoDispatcher(Login);
                });
        }

        #region UserName
        private static string _userName;
        public string UserName
        {
            get { return AMembership.IsAuthorized ? AMembership.ActiveUser.Title : (SaveUserName ? HelperSettingsClient.UserName : _userName); }
            set
            {
                if (SaveUserName)
                    HelperSettingsClient.UserName = value;
                else
                    _userName = value;
                OnPropertyChanged("UserName");
            }
        }
        #endregion
        #region Password
        private string _Password;
        public string Password
        {
            get { return AMembership.IsAuthorized ? string.Empty : (SavePassword ? HelperSettingsClient.Password : _Password); }
            set
            {
                if (SavePassword)
                    HelperSettingsClient.Password = value;
                else
                    _Password = value;
                OnPropertyChanged("Password");
            }
        }
        #endregion
        #region SaveUserName
        public bool SaveUserName
        {
            get { return HelperSettingsClient.SaveUserName; }
            set
            {
                HelperSettingsClient.SaveUserName = value;
                if (value)
                    HelperSettingsClient.UserName = _userName;
                else
                    _userName = HelperSettingsClient.UserName;
                OnPropertyChanged("SaveUserName");
            }
        }
        #endregion
        #region SavePassword
        public bool SavePassword
        {
            get
            {
                return HelperSettingsClient.SavePassword;
            }
            set
            {
                HelperSettingsClient.SavePassword = value;
                if (value)
                    HelperSettingsClient.Password = _Password;
                else
                    _Password = HelperSettingsClient.Password;
                OnPropertyChanged("SavePassword");
            }
        }
        #endregion
        #region AutoLogin
        public bool AutoLogin
        {
            get
            {
                return HelperSettingsClient.AutoLogin;
            }
            set
            {
                HelperSettingsClient.AutoLogin = value;
                OnPropertyChanged("AutoLogin");
            }
        }
        #endregion


        #region LoginDateTime
        public string LoginDateTime
        {
            get
            {
                return AMembership.IsAuthorized ? AMembership.ActiveUser.LoginDate.ToShortTimeString() : string.Empty;
            }
        }
        #endregion
        #region LoginCount
        public string LoginCount
        {
            get
            {
                return AMembership.IsAuthorized
                    ? AMembership.ActiveUser.CountLogin.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }
        #endregion
        #region RegisteredDate
        public string RegisteredDate
        {
            get
            {
                return AMembership.IsAuthorized ?
                    HelperLocalize.DateTimeToString(AMembership.ActiveUser.RegisterDate, HelperLocalize.ApplicationCalendar, "dd MMMM yy")
                         : "-";
            }
        }
        #endregion
        #region InternalCodes
        public string InternalCodes
        {
            get
            {
                return AMembership.IsAuthorized ? AMembership.ActiveUser.InternalPhone : "-";
            }
        }
        #endregion
        #region UserRoles
        public List<string> UserRoles
        {
            get
            {
                return AMembership.IsAuthorized ?
                    AMembership.ActiveUser.Roles.ToList() :
                    null;
            }
        }
        #endregion

        #region CanLogin
        public bool CanLogin
        {
            get { return !AMembership.IsAuthorized && !IsBussy; }
        }
        #endregion
        #region CanLogout
        public bool CanLogout
        {
            get
            {
                return AMembership.IsAuthorized && !IsBussy;
            }
        }
        #endregion
        #region IsBussy
        private bool _IsBussy;
        public bool IsBussy
        {
            get { return _IsBussy; }
            set
            {
                _IsBussy = value;
                OnPropertyChanged("IsBussy");
                OnPropertyChanged("CanLogin");
                OnPropertyChanged("CanLogout");
            }
        }
        #endregion


        #region StatusNetwork
        private EnumStatusGeneral _statusNetwork;
        public EnumStatusGeneral StatusNetwork
        {
            get { return _statusNetwork; }
            set
            {
                _statusNetwork = value;
                OnPropertyChanged("StatusNetwork");
            }
        }
        #endregion
        #region TooltipNetwork
        private string _tooltipNetwork;
        public string TooltipNetwork
        {
            get { return _tooltipNetwork; }
            set
            {
                _tooltipNetwork = value;
                OnPropertyChanged("TooltipNetwork");
            }
        }
        #endregion
        #region TextNetwork
        private string _textNetwork;
        public string TextNetwork
        {
            get { return _textNetwork; }
            set
            {
                _textNetwork = value;
                OnPropertyChanged("TextNetwork");
            }
        }
        #endregion


        #region StatusSyncDateTime
        private EnumStatusGeneral _statusSyncDateTime;
        public EnumStatusGeneral StatusSyncDateTime
        {
            get { return _statusSyncDateTime; }
            set
            {
                _statusSyncDateTime = value;
                OnPropertyChanged("StatusSyncDateTime");
            }
        }
        #endregion
        #region TooltipSyncDateTime
        private string _tooltipSyncDateTime;
        public string TooltipSyncDateTime
        {
            get { return _tooltipSyncDateTime; }
            set
            {
                _tooltipSyncDateTime = value;
                OnPropertyChanged("TooltipSyncDateTime");
            }
        }
        #endregion

        #region StatusVersion
        private EnumStatusGeneral _statusVersion;
        public EnumStatusGeneral StatusVersion
        {
            get { return _statusVersion; }
            set
            {
                _statusVersion = value;
                OnPropertyChanged("StatusVersion");
            }
        }
        #endregion
        #region TooltipVersion
        private string _tooltipVersion;
        public string TooltipVersion
        {
            get { return _tooltipVersion; }
            set
            {
                _tooltipVersion = value;
                OnPropertyChanged("TooltipVersion");
            }
        }
        #endregion

        #region StatusServerErrors
        private EnumStatusGeneral _statusServerErrors;
        public EnumStatusGeneral StatusServerErrors
        {
            get { return _statusServerErrors; }
            set
            {
                _statusServerErrors = value;
                OnPropertyChanged("StatusServerErrors");
            }
        }
        #endregion
        #region TooltipServerErrors
        private string _tooltipServerErrors;
        public string TooltipServerErrors
        {
            get { return _tooltipServerErrors; }
            set
            {
                _tooltipServerErrors = value;
                OnPropertyChanged("TooltipServerErrors");
            }
        }
        #endregion

        #region StatusDatabase
        private EnumStatusGeneral _statusDatabase;
        public EnumStatusGeneral StatusDatabase
        {
            get { return _statusDatabase; }
            set
            {
                _statusDatabase = value;
                OnPropertyChanged("StatusDatabase");
            }
        }
        #endregion
        #region TooltipDatabase
        private string _tooltipDatabase;
        public string TooltipDatabase
        {
            get { return _tooltipDatabase; }
            set
            {
                _tooltipDatabase = value;
                OnPropertyChanged("TooltipDatabase");
            }
        }
        #endregion

        #region StatusMembership
        private EnumStatusGeneral _statusMembership;
        public EnumStatusGeneral StatusMembership
        {
            get { return _statusMembership; }
            set
            {
                _statusMembership = value;
                OnPropertyChanged("StatusMembership");
            }
        }
        #endregion
        #region TooltipMembership
        private string _tooltipMembership;
        public string TooltipMembership
        {
            get { return _tooltipMembership; }
            set
            {
                _tooltipMembership = value;
                OnPropertyChanged("TooltipMembership");
            }
        }
        #endregion



        private void InitCommands()
        {
            CommandLogin = new RelayCommand(Login, () => true);
            CommandLogout = new RelayCommand(Logout, () => true);

            CommandChangePassword = new RelayCommand(ChangePassword, () => AMembership.HasPermission(PCOPermissions.Membership_ActiveUser_AllowChangePassword));
        }


        private void Logout()
        {
            AMembership.LogoutUser(APOCCore.InstanceGuid);
        }
        private void Login()
        {

            APOCCore.AppBarIsBussy = true;

            IsBussy = true;
            StatusNetwork = EnumStatusGeneral.Checking;
            TooltipNetwork = string.Empty;
            TextNetwork = "در حال بررسی";

            StatusSyncDateTime = EnumStatusGeneral.Checking;
            TooltipSyncDateTime = string.Empty;

            StatusVersion = EnumStatusGeneral.Checking;
            TooltipVersion = string.Empty;

            StatusServerErrors = EnumStatusGeneral.Checking;
            TooltipServerErrors = string.Empty;

            StatusDatabase = EnumStatusGeneral.Undefined;
            TooltipDatabase = string.Empty;

            StatusMembership = EnumStatusGeneral.Undefined;
            TooltipMembership = string.Empty;


            try
            {
                if (string.IsNullOrWhiteSpace(UserName))
                    throw new SecurityException("كلمه ورود معتبر نمی باشد.");

                if (string.IsNullOrWhiteSpace(Password))
                    throw new SecurityException("رمز ورود معتبر نمی باشد.");

                var binding = new NetTcpBinding
                {
                    TransferMode = TransferMode.Buffered,
                    Security = { Mode = SecurityMode.None },
                    ReliableSession = { Enabled = false, Ordered = true, InactivityTimeout = new TimeSpan(0, 10, 0) },
                    MaxReceivedMessageSize = 65536L,
                };
                var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                    HelperSettingsClient.ServerName,
                    Convert.ToInt32(HelperSettingsClient.ServerPort) + ConstantGeneral.ProtocolCorePortOffset,
                    ConstantGeneral.ProtocolCoreServiceName));
                var pxCore = new ProtocolCoreClient(binding, address);
                pxCore.Open();


                pxCore.ShareInformationCompleted +=
                    (s, e) =>
                    {
                        if (e.Error != null)
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Send,
                                new Action(
                                    () =>
                                    {
                                        IsBussy = false;
                                        StatusNetwork = EnumStatusGeneral.Invalid;
                                        TextNetwork = "خطا در برقراری ارتباط";
                                        TooltipNetwork = e.Error.Message;
                                    }));
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Send,
                                new Action(
                                    () =>
                                    {
                                        var hasError = false;
                                        APOCCore.STCI = e.Result;
                                        APOCCore.UpdateLineNames();
                                        APOCCore.UpdateExtNames();
                                        APOCCore.UpdateSmsSettings();

                                        StatusNetwork = EnumStatusGeneral.OK;
                                        TextNetwork = "ارتباط برقرار شد";
                                        TooltipNetwork = string.Format("نام سرور : {0}", HelperSettingsClient.ServerName);

                                        #region Date
                                        var dateDelta = APOCCore.CTSI.ClientDate - APOCCore.STCI.ServerDate;
                                        var deltaSeconds = (int)Math.Abs(dateDelta.TotalSeconds);
                                        if (deltaSeconds > 120)
                                        {
                                            hasError = true;
                                            StatusSyncDateTime = EnumStatusGeneral.Invalid;
                                            TooltipSyncDateTime = "اختلاف زمان بین سرور و كلاینت بیش از دو دقیقه می باشد" + Environment.NewLine + "اختلاف :" + (deltaSeconds / 60) + " دقیقه.";
                                        }
                                        else
                                        {
                                            StatusSyncDateTime = EnumStatusGeneral.OK;
                                            TooltipSyncDateTime = string.Format("اختلاف زمان : {0} ثانیه می باشد.", deltaSeconds);
                                        }
                                        #endregion

                                        #region Version
                                        if (APOCCore.CTSI.ClientVersion != APOCCore.STCI.ServerVersion)
                                        {
                                            hasError = false;
                                            StatusVersion = EnumStatusGeneral.Invalid;
                                            TooltipVersion = string.Format("نسخه كلاینت با نسخه سرور یكسان نمی باشد.{0}نسخه سرور : {1}{0}نسخه كلاینت : {2}", Environment.NewLine, APOCCore.STCI.ServerVersion, APOCCore.CTSI.ClientVersion);
                                        }
                                        else
                                        {
                                            StatusVersion = EnumStatusGeneral.OK;
                                            TooltipVersion = string.Format("نسخه : {0}", APOCCore.CTSI.ClientVersion);
                                        }
                                        #endregion

                                        #region Device
                                        if (APOCCore.STCI.StatusDeviceALM != EnumStatusDeviceALM.Disabled && APOCCore.STCI.StatusDeviceALM != EnumStatusDeviceALM.OK)
                                        {
                                            hasError = true;
                                            StatusServerErrors = EnumStatusGeneral.Invalid;
                                            TooltipServerErrors = "خطا در دستگاه كالر آیدی";
                                        }

                                        if (APOCCore.STCI.StatusDevicePana != EnumStatusDevicePana.Disabled && APOCCore.STCI.StatusDevicePana != EnumStatusDevicePana.OK)
                                        {
                                            hasError = true;
                                            StatusServerErrors = EnumStatusGeneral.Invalid;
                                            TooltipServerErrors = "خطا در دستگاه سانترال پاناسونیك";
                                        }


                                        #endregion

                                        #region Telecommunication
                                        if (APOCCore.STCI.StatusTelecommunication != EnumStatusTelecommunication.OK)
                                        {
                                            if (APOCCore.STCI.StatusTelecommunication == EnumStatusTelecommunication.Undefined &&
                                               APOCCore.STCI.Device == EnumDeviceUsed.None)
                                            {
                                                if (!(APOCCore.STCI.CurrentCityGuid != Guid.Empty &&
                                                    APOCCore.STCI.MobileLength > 0 && !string.IsNullOrEmpty(APOCCore.STCI.MobileStartingCode)))
                                                {
                                                    hasError = true;
                                                    StatusServerErrors = EnumStatusGeneral.Invalid;
                                                    TooltipServerErrors = "خطا در تنظیمات مخابراتی";
                                                }
                                            }
                                            else
                                            {
                                                hasError = true;
                                                StatusServerErrors = EnumStatusGeneral.Invalid;
                                                TooltipServerErrors = "خطا در تنظیمات مخابراتی";
                                            }
                                        }
                                        #endregion

                                        #region Membership
                                        if (APOCCore.STCI.StatusMembership != EnumStatusMembership.OK)
                                        {
                                            hasError = true;
                                            StatusServerErrors = EnumStatusGeneral.Invalid;
                                            TooltipServerErrors = "خطا در مدیریت كاربران";
                                        }
                                        #endregion

                                        #region Monitoring
                                        if (APOCCore.STCI.StatusPhoneMonitoring != EnumStatusPhoneMonitoring.OK)
                                        {
                                            hasError = true;
                                            StatusServerErrors = EnumStatusGeneral.Invalid;
                                            TooltipServerErrors = "خطا در مانیتورینگ";
                                        }
                                        #endregion


                                        if (hasError)
                                        {
                                            IsBussy = false;
                                            APOCCore.AppBarIsBussy = false;
                                        }
                                        else
                                        {
                                            StatusServerErrors = EnumStatusGeneral.OK;
                                            TooltipServerErrors = "سرور از سلامت كامل برخوردار می باشد";
                                            CheckDatabaseConnection(APOCCore);
                                        }
                                    }));
                        }
                    };
                APOCCore.CTSI.ClientDate = DateTime.Now;
                pxCore.ShareInformationAsync(APOCCore.CTSI);
            }
            catch (Exception ex)
            {
                IsBussy = false;
                StatusNetwork = EnumStatusGeneral.Invalid;
                TextNetwork = "خطا در برقراری ارتباط";
                if (ex is UriFormatException)
                    TooltipNetwork = "لطفا تنظیمات ارتباط با سرور را بدرستی وارد كنید.";
                else if (ex is EndpointNotFoundException)
                    TooltipNetwork = "سرور با پورت مورد نظر یافت نشد.";
                else if (ex is SecurityException)
                {
                    TooltipNetwork = ex.Message;
                    TextNetwork = TooltipNetwork;
                }
                else
                    TooltipNetwork = ex.Message;
                APOCCore.AppBarIsBussy = false;
            }
        }
        private void CheckDatabaseConnection(POCCore pocCore)
        {
            StatusDatabase = EnumStatusGeneral.Checking;
            if (ADatabase.Dxs == null)
            {
                StatusDatabase = EnumStatusGeneral.Invalid;
                TooltipDatabase = "پایگاه اطلاعات معتبر نمی باشد.";
                IsBussy = false;
                pocCore.AppBarIsBussy = false;
            }
            else
            {
                try
                {
                    ADatabase.Dxs.Connect();
                    ADatabase.Dxs.Disconnect();
                    StatusDatabase = EnumStatusGeneral.OK;
                    TooltipDatabase = "ارتباط با پایگاه اطلاعات برقرار شد.";
                    if (APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL)
                    {
                        RegisterXPOFunctions();
                    }
                    if (APOCCore.CTSI.ClientVersion != APOCCore.STCI.ServerVersion)
                    {
                        var dbs = POL.DB.P30Office.GL.DBGLUpdateStorage.FindFirst(ADatabase.Dxs);
                        if (dbs == null)
                        {
                            IsBussy = false;
                            pocCore.AppBarIsBussy = false;
                            return;
                        }
                        if (dbs.Version != APOCCore.STCI.ServerVersion)
                        {
                            IsBussy = false;
                            pocCore.AppBarIsBussy = false;
                            return;
                        }
                        IsBussy = false;
                        InstallUpdate();
                        return;
                    }



                    LoginUser(pocCore);
                }
                catch 
                {
                    StatusDatabase = EnumStatusGeneral.Invalid;
                    TooltipDatabase = "بروز خطا در برقراری ارتباط با پایگاه داده.";
                    IsBussy = false;
                    pocCore.AppBarIsBussy = false;
                }
            }
        }

        private void InstallUpdate()
        {
            POL.WPF.DXControls.POLProgressBox.Show(1,
                pb =>
                {
                    try
                    {
                        pb.AsyncSetText(1, "بار گزاری فایل ها");
                        var dbs = POL.DB.P30Office.GL.DBGLUpdateStorage.FindFirst(ADatabase.Dxs);
                        var data = dbs.DataByte;

                        var path = string.Format("{0}{1}", System.IO.Path.GetTempPath(), Guid.NewGuid());
                        System.IO.Directory.CreateDirectory(path);
                        var extractFile = string.Format("{0}\\update.zip", path);
                        var dec = HelperSecurity.Decrypt(data, "download_from_p30office");
                        using (var f = System.IO.File.Create(extractFile))
                        {
                            f.Write(dec, 0, dec.Length);
                        }

                        pb.AsyncSetText(1, "بررسی سلامت فایل ها");

                        var unpackDirectory = path + "\\extract";
                        using (var zip1 = ZipFile.Read(extractFile))
                        {
                            foreach (var e in zip1)
                            {
                                e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                            }
                        }
                        var info = new ProcessStartInfo(string.Format("{0}\\pou.shell.exe", unpackDirectory));
                        info.Arguments = string.Format("\"{0}\"", System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                        System.IO.Directory.SetCurrentDirectory(unpackDirectory);
                        Process.Start(info);
                        HelperUtils.DoDispatcher(
                            () => Application.Current.Shutdown());
                    }
                    catch (Exception ex)
                    {
                        POL.WPF.DXControls.POLMessageBox.ShowError(ex.Message, Application.Current.MainWindow);
                    }

                }, pb => { }, Application.Current.MainWindow);
        }
        private void LoginUser(POCCore pocCore)
        {
            StatusMembership = EnumStatusGeneral.Checking;
            AMembership.LoginUser(UserName, Password, pocCore.InstanceGuid);
        }

        private void ChangePassword()
        {
            var w = new WMembershipUserChangePassword(AMembership.ActiveUser.UserID) { Owner = Application.Current.MainWindow };
            w.ShowDialog();
        }

        private void RegisterXPOFunctions()
        {
            var v = DevExpress.Data.Filtering.CriteriaOperator.GetCustomFunction("FullTextContains");
            if (v == null)
                DevExpress.Data.Filtering.CriteriaOperator.RegisterCustomFunction(new FullTextContainsFunction());
        }

        #region [COMMANDS]
        public RelayCommand CommandLogin { get; set; }
        public RelayCommand CommandChangePassword { get; set; }
        public RelayCommand CommandLogout { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Send,
                    new Action(() =>
                    {
                        handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    }));

        }
        #endregion
    }
}
