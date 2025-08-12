using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler.Localization;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using NLog;
using NLog.Config;
using POC.Shell.Adapters;
using POC.Shell.Localizers.fa;
using POC.Shell.Views;
using POL.Lib.Common.POLMembership;
using POL.Lib.Interfaces;
using POL.Lib.Prism;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.DXControls;
using POL.WPF.DXControls.POLControls.Licensor;
using POL.WPF.DXControls.POLDateEdit;

namespace POC.Shell
{
    public class Bootstrapper : UnityBootstrapper
    {
        private readonly AdapterApplicationBar _AApplicationBar = new AdapterApplicationBar();
        private readonly AdapterApplicationSettings _AApplicationSettings = new AdapterApplicationSettings();
        private readonly AdapterFileLogger _ALogger = new AdapterFileLogger();
        private readonly AdapterModuleSyncronizer _AModuleSyncronizer = new AdapterModuleSyncronizer();
        private readonly AdapterMembership _AMembership = new AdapterMembership();
        private readonly AdapterDatabase _ADatabase = new AdapterDatabase();
        private readonly AdapterPOCSettings _APOCSetting = new AdapterPOCSettings();
        private readonly AdapterPOCRootTools _APOCRootTools = new AdapterPOCRootTools();
        private readonly AdapterPOCContactModule _APOCContactModule = new AdapterPOCContactModule();
        private readonly AdapterPOCMainWindow _APOCMainWindow = new AdapterPOCMainWindow();
        private readonly AdapterBaseTable _ABaseTable = new AdapterBaseTable();
        private readonly AdapterPopup _APopup = new AdapterPopup();
        private readonly POCCore _POCCore = new POCCore();
        private IMessagingClient _AMessagingClient;
        private readonly AdapterDataFieldManager _ADataField = new AdapterDataFieldManager();
        private readonly AdapterPOCDashboardUnit _APOCDashboardUnit = new AdapterPOCDashboardUnit();
        private readonly AdapterPOCFastContactUnit _APOCFastContactUnit = new AdapterPOCFastContactUnit();


        private Logger Nlog { get; set; }

        protected override ILoggerFacade CreateLogger()
        {
            Nlog = LogManager.GetCurrentClassLogger();

            var tn = LogManager.Configuration.FindTargetByName("filePoc");
            var loggingRule = new LoggingRule("*", LogLevel.Debug, tn);
            LogManager.Configuration.LoggingRules.Add(loggingRule);

            LogManager.Configuration.Reload();

            _ALogger.Callback = (message, category, priority) =>
            {
                var s = string.Format("[{0}] [{1}][{2}] {3}",
                        DateTime.Now.ToString("MMdd HH:mm:ss"),
                        category.ToString().PadRight(6),
                        priority.ToString().PadRight(6),
                        message);
                switch (category)
                {
                    case Category.Debug:
                        Nlog.Log(LogLevel.Debug, s);
                        break;
                    case Category.Exception:
                        Nlog.Log(LogLevel.Error, s);
                        break;
                    case Category.Info:
                        Nlog.Log(LogLevel.Info, s);
                        break;
                    case Category.Warn:
                        Nlog.Log(LogLevel.Warn, s);
                        break;
                }
            };

            return _ALogger;
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var appPath = HelperAssembly.GetApplicationPath();
            try
            {
                Directory.SetCurrentDirectory(appPath);
            }
            catch (Exception ex)
            {
                _ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
            }
            appPath = Path.Combine(appPath, ConstantGeneral.ModuleFolderForPoc);
            if (!Directory.Exists(appPath))
            {
                try
                {
                    Directory.CreateDirectory(appPath);
                }
                catch (Exception ex)
                {
                    _ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                }
            }
            var moduleCatalog = new PrioritizedDirectoryModuleCatalog { ModulePath = appPath };
            return moduleCatalog;
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterInstance<IApplicationBar>(_AApplicationBar);
            Container.RegisterInstance<IApplicationSettings>(_AApplicationSettings);
            Container.RegisterInstance<ILoggerFacade>(_ALogger);
            Container.RegisterInstance<IModuleSyncronizer>(_AModuleSyncronizer);
            Container.RegisterInstance<IMembership>(_AMembership);
            Container.RegisterInstance<IDatabase>(_ADatabase);
            Container.RegisterInstance<IPOCSettings>(_APOCSetting);
            Container.RegisterInstance<IPOCRootTools>(_APOCRootTools);
            Container.RegisterInstance<IPOCMainWindow>(_APOCMainWindow);
            Container.RegisterInstance<IPOCContactModule>(_APOCContactModule);
            Container.RegisterInstance<IBaseTable>(_ABaseTable);
            Container.RegisterInstance<IPopup>(_APopup);
            Container.RegisterInstance<IDataFieldManager>(_ADataField);
            Container.RegisterInstance<IPOCDashboardUnit>(_APOCDashboardUnit);
            Container.RegisterInstance<IPOCFastContactUnit>(_APOCFastContactUnit);


            _POCCore.InstanceGuid = Guid.NewGuid();
            var ctsi = new ClientToServerInformation();
            HelperUtils.Try(() => { ctsi.SystemInformation = HelperComputerInfo.GetSystemInformation(); });
            ctsi.ClientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ctsi.ClientDate = DateTime.Now;
            ctsi.ID = _POCCore.InstanceGuid;
            _POCCore.CTSI = ctsi;
            Container.RegisterInstance(_POCCore);

            _AMessagingClient = new AdapterMessagingClient(_ALogger, _AMembership, _POCCore, _AApplicationSettings);
            Container.RegisterInstance(_AMessagingClient);
            Container.RegisterInstance<ICacheData>(new AdapterCacheData(_ALogger, _AMembership, _POCCore, _AMessagingClient, _ADatabase, _ADataField));


            DataControlBase.AllowInfiniteGridSize = true;
        }
        protected override DependencyObject CreateShell()
        {
            LogicNPLicensor.License();

            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata(200));
            InitApplicationTheme();
            InitApplicationDirection();
            InitApplicationFontFamily();
            InitApplicationFontSize();
            InitCulture();
            InitCalendar();
            InitGlobalDateTimeFormat();
            InitGlobalKBLayoutDefault();
            InitGlobalKBLayoutRTL();
            InitGlobalKBLayoutLTR();
            InitAutoStart();
            InitDock();

            var shell = new WShellView();
            return shell;
        }

        private void InitApplicationTheme()
        {
            ThemeManager.ApplicationThemeName = Theme.Office2007Blue.Name;
            OptionsXBAP.SuppressNotSupportedException = true;
            ApplicationThemeHelper.UpdateApplicationThemeName();
        }
        private void InitApplicationDirection()
        {
            const FlowDirection defaultDirection = FlowDirection.RightToLeft;
            Application.Current.Resources["ApplicationFlowDirection"] = defaultDirection;
            HelperLocalize.ApplicationFlowDirection = defaultDirection;
        }
        private void InitApplicationFontSize()
        {
            Application.Current.Resources["ApplicationFontSize"] = 12.0;
            HelperLocalize.ApplicationFontSize = 12.0;
        }
        private void InitApplicationFontFamily()
        {
            Application.Current.Resources["ApplicationFontFamily"] = new FontFamily("Tahoma");
            HelperLocalize.ApplicationFontName = "Tahoma";
        }
        private void InitCulture()
        {
            HelperSettingsClient.LanguageLCID = Thread.CurrentThread.CurrentCulture.LCID;
            var culture = new PersianCulture();
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            LocalizeToPersian();
            MyDateEdit.RegisterEditor();
        }
        private void InitCalendar()
        {
            HelperLocalize.ApplicationCalendar = EnumCalendarType.Shamsi;
        }
        private void InitGlobalDateTimeFormat()
        {
            const string defaultLDateTimeFormat = "yyyy/MM/dd";
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = defaultLDateTimeFormat;
            Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern = defaultLDateTimeFormat;
        }
        private void InitGlobalKBLayoutDefault()
        {
            const string defaultKBLayout = "Persian";
            try
            {
                var lang = HelperSettingsClient.ApplicationKBLayoutDefault;
                if (string.IsNullOrWhiteSpace(lang))
                    lang = defaultKBLayout;
                HelperLocalize.ApplicationDefKeyboardLeyout = lang;
            }
            catch (Exception ex)
            {
                _ALogger.Log(ex.ToString(), Category.Exception, Priority.None);
            }

            try
            {
                HelperSettingsClient.ApplicationKBLayoutDefault = HelperLocalize.ApplicationDefKeyboardLeyout;
            }
            catch (Exception ex)
            {
                _ALogger.Log(ex.ToString(), Category.Exception, Priority.Low);
            }

        }
        private void InitGlobalKBLayoutRTL()
        {
            const string defaultKBLayout = "Persian";
            try
            {
                var lang = HelperSettingsClient.ApplicationKBLayoutRTL;
                if (string.IsNullOrWhiteSpace(lang))
                    lang = defaultKBLayout;
                HelperLocalize.ApplicationRTLKeyboardLeyout = lang;
            }
            catch (Exception ex)
            {
                _ALogger.Log(ex.ToString(), Category.Exception, Priority.None);
            }

            try
            {
                HelperSettingsClient.ApplicationKBLayoutRTL = HelperLocalize.ApplicationRTLKeyboardLeyout;
            }
            catch (Exception ex)
            {
                _ALogger.Log(ex.ToString(), Category.Exception, Priority.Low);
            }

        }
        private void InitGlobalKBLayoutLTR()
        {
            const string defaultKBLayout = "US";
            try
            {
                var lang = HelperSettingsClient.ApplicationKBLayoutLTR;
                if (string.IsNullOrWhiteSpace(lang))
                    lang = defaultKBLayout;
                HelperLocalize.ApplicationLTRKeyboardLeyout = lang;
            }
            catch (Exception ex)
            {
                _ALogger.Log(ex.ToString(), Category.Exception, Priority.None);
            }

            try
            {
                HelperSettingsClient.ApplicationKBLayoutLTR = HelperLocalize.ApplicationLTRKeyboardLeyout;
            }
            catch (Exception ex)
            {
                _ALogger.Log(ex.ToString(), Category.Exception, Priority.Low);
            }

        }
        private void InitAutoStart()
        {
            try
            {
                var ase = HelperSettingsClient.AutoStartDelay;
            }
            catch
            {
                HelperSettingsClient.AutoStartDelay = 0;
            }
        }
        private void InitDock()
        {
            try
            {
                var b = HelperSettingsClient.DockIsFixed;
            }
            catch 
            {
                HelperSettingsClient.DockIsFixed = false;
            }
            try
            {
                var b = HelperSettingsClient.DockWidth;
                if (b < 160 || b > 280) throw new Exception();
            }
            catch 
            {
                HelperSettingsClient.DockWidth = 200;
            }
        }

        private void LocalizeToPersian()
        {
            POLMessageBoxLocalizer.Active = new PersianPOLMessageBoxLocalizer();
            POLProgressBoxLocalizer.Active = new PersianPOLProgressBoxLocalizer();

            DXMessageBoxLocalizer.Active = new PersianDXMessageBoxLocalizer();
            EditorLocalizer.Active = new PersianEditorLocalizer();
            GridControlLocalizer.Active = new PersianGridControlLocalizer();

            SchedulerControlLocalizer.Active = new PersianSchedulerControlLocalizer();
            SchedulerLocalizer.Active = new PersianXtraSchedulerLocalizer();
        }

        protected override void InitializeShell()
        {
            UMembershipUserManagment.GetPermissionsEnum = () => typeof(PCOPermissions);

            base.InitializeShell();

            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }
        protected override void InitializeModules()
        {
            _AModuleSyncronizer.OnApplicationExit += (s, e) => Application.Current.Shutdown(0);
            base.InitializeModules();
        }


    }
}
