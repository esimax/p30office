using System.Runtime.InteropServices;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POC.Module.Call.Views;
using POL.DB.P30Office;
using POL.DB.P30Office.GL;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GemBox.Spreadsheet;

namespace POC.Module.Call.Models
{
    public class MCall : NotifyObjectBase, IDisposable
    {
        #region Private Properties
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private DispatcherTimer CallUpdateTimer { get; set; }
        private DispatcherTimer AutoRefreshTimer { get; set; }

        private dynamic MainView { get; set; }
        private RibbonControl MainRibbonControl { get; set; }
        private GridControl DynamicGrid { get; set; }
        private ChartControl DynamicChartControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private UserControl ActiveView { get; set; }
        private DevExpress.Xpf.Editors.Filtering.FilterControl ActiveFilterControl { get; set; }

        private GroupOperator MainSearchCriteria { get; set; }
        private bool HasLoadedLayout { get; set; }
        private const string ModuleID = "C1CB7E75-1AD2-450E-AF6A-F3F61C1AF162";
        #endregion



        public MCall(object mainView)
        {
            _IsFilterByDate = true;
            _IsShowAll = true;

            MainView = mainView;
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogin)
                        PopulateContactCat();
                };

            APOCContactModule.OnSelectedDateChanged += APOCContactModule_OnSelectedDateChanged;

            InitDynamics();
            PopulateContactCat();
            PopulateCallFilterList();
            PopulateCallReportList();

            _DayOfWeekFilter = 0;
            _BeginTime = "00:00";
            _EndTime = "23:59";

            UpdateSearch();
            InitCommands();

            AutoRefreshTimer = new DispatcherTimer();
            AutoRefreshIndex = HelperSettingsClient.CallAutoRefreshIndex;
            AutoRefreshTimer.IsEnabled = HelperSettingsClient.CallIsAutoRefresh;
            AutoRefreshTimer.Tick += (s, e) =>
                                         {
                                             if (SelectedTabIndex == 0 &&
                                                 IsFilterByDate &&
                                                 APOCContactModule.SelectedDate.Date == DateTime.Now.Date &&
                                                 ActiveView.IsVisible
                                                 )
                                                 Refresh();
                                         };



            _SelectedChartAxisY = (from n in ChartAxisYList select n).First();
            _SelectedChartAxisX = (from n in ChartAxisXList select n).First();
            _SelectedChartSeparation = (from n in ChartSeparationList select n).First();
            _SelectedPaperKind = (from n in PaperKindList select n).First();

            
            
        }
        void APOCContactModule_OnSelectedDateChanged(object sender, EventArgs e)
        {
            UpdateSearchWithDelay();
        }




        #region SelectedTabIndex
        private int _SelectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                RaisePropertyChanged("SelectedTabIndex");
                RaisePropertyChanged("CanSetChartSettings");
            }
        }
        #endregion
        #region SideSelectedTabIndex
        private int _SideSelectedTabIndex;
        public int SideSelectedTabIndex
        {
            get { return _SideSelectedTabIndex; }
            set
            {
                _SideSelectedTabIndex = value;
                RaisePropertyChanged("SideSelectedTabIndex");
            }
        }
        #endregion



        #region IsFilterByDate
        private bool _IsFilterByDate;
        public bool IsFilterByDate
        {
            get { return _IsFilterByDate; }
            set
            {
                if (!_IsFilterByDate || value || IsFilterByFilter)
                {
                    _IsFilterByDate = value;
                    _IsFilterByFilter = !value;

                    SideSelectedTabIndex = 0;

                    UpdateSearch();
                }
                RaisePropertyChanged("IsFilterByDate");
                RaisePropertyChanged("IsFilterByFilter");

                RaisePropertyChanged("FilterTabVisibility");

                DynamicGrid.Columns.ToList().ForEach(c => DynamicGrid.ClearColumnFilter(c.FieldName));
            }
        }
        #endregion
        #region IsShowAll
        private bool _IsShowAll;
        public bool IsShowAll
        {
            get { return _IsShowAll; }
            set
            {
                if (!_IsShowAll || value || IsShowCallIn || IsShowCallOut || IsShowMissCall)
                {
                    _IsShowAll = value;
                    _IsShowCallIn = !value;
                    _IsShowCallOut = !value;
                    _IsShowMissCall = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowCallIn");
                RaisePropertyChanged("IsShowCallOut");
                RaisePropertyChanged("IsShowMissCall");
            }
        }
        #endregion
        #region IsShowCallOut
        private bool _IsShowCallOut;
        public bool IsShowCallOut
        {
            get { return _IsShowCallOut; }
            set
            {
                if (!_IsShowCallOut || value || IsShowCallIn || IsShowAll || IsShowMissCall)
                {
                    _IsShowCallOut = value;
                    _IsShowCallIn = !value;
                    _IsShowAll = !value;
                    _IsShowMissCall = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowCallIn");
                RaisePropertyChanged("IsShowCallOut");
                RaisePropertyChanged("IsShowMissCall");
            }
        }
        #endregion
        #region IsShowCallIn
        private bool _IsShowCallIn;
        public bool IsShowCallIn
        {
            get { return _IsShowCallIn; }
            set
            {
                if (!_IsShowCallIn || value || IsShowAll || IsShowCallOut || IsShowMissCall)
                {
                    _IsShowCallIn = value;
                    _IsShowAll = !value;
                    _IsShowCallOut = !value;
                    _IsShowMissCall = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowCallIn");
                RaisePropertyChanged("IsShowCallOut");
                RaisePropertyChanged("IsShowMissCall");
            }
        }
        #endregion
        #region IsShowMissCall
        private bool _IsShowMissCall;
        public bool IsShowMissCall
        {
            get { return _IsShowMissCall; }
            set
            {
                if (!_IsShowMissCall || value || IsShowCallIn || IsShowCallOut || IsShowAll)
                {
                    _IsShowMissCall = value;
                    _IsShowCallIn = !value;
                    _IsShowCallOut = !value;
                    _IsShowAll = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowMissCall");
                RaisePropertyChanged("IsShowCallIn");
                RaisePropertyChanged("IsShowCallOut");
                RaisePropertyChanged("IsShowAll");
            }
        }
        #endregion
        #region IsFilterByFilter
        private bool _IsFilterByFilter;
        public bool IsFilterByFilter
        {
            get { return _IsFilterByFilter; }
            set
            {
                if (!_IsFilterByFilter || value || _IsFilterByDate)
                {
                    _IsFilterByFilter = value;
                    _IsFilterByDate = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsFilterByFilter");
                RaisePropertyChanged("IsFilterByDate");

                RaisePropertyChanged("FilterTabVisibility");

            }
        }
        #endregion

        #region FilterTabVisibility
        public Visibility FilterTabVisibility
        {
            get { return IsFilterByFilter ? Visibility.Visible : Visibility.Collapsed; }
        }
        #endregion

        #region CallList
        private XPServerCollectionSource _CallList;
        public XPServerCollectionSource CallList
        {
            get { return _CallList; }
            set
            {
                _CallList = value;
                RaisePropertyChanged("CallList");
            }
        }
        #endregion
        #region FocusedCall
        private DBCLCall _FocusedCall;
        public DBCLCall FocusedCall
        {
            get
            {
                return _FocusedCall;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedCall)) return;
                _FocusedCall = value;
                RaisePropertyChanged("FocusedCall");
            }
        }
        #endregion FocusedCall

        #region SelectedRowCount
        public int SelectedRowCount
        {
            get
            {
                return DynamicGrid.GetSelectedRowHandles().Length;
            }
        }
        #endregion
        #region DeviceHasInternal
        public bool DeviceHasInternal
        {
            get { return APOCCore.STCI.Device == EnumDeviceUsed.Panasonic; }
        }
        #endregion
        #region DeviceHasRecord
        public bool DeviceHasRecord
        {
            get { return APOCCore.STCI.DeviceHasRecord; }
        }
        #endregion
        #region DeviceHasVoiceMessage
        public bool DeviceHasVoiceMessage
        {
            get { return APOCCore.STCI.DeviceHasVoiceMessage; }
        }
        #endregion

        #region CustomColumnTextX
        public string CustomColumnText1
        {
            get { return APOCCore.STCI.ContactCustColTitle1; }
        }
        public string CustomColumnText2
        {
            get { return APOCCore.STCI.ContactCustColTitle2; }
        }
        public string CustomColumnText3
        {
            get { return APOCCore.STCI.ContactCustColTitle3; }
        }
        public string CustomColumnText4
        {
            get { return APOCCore.STCI.ContactCustColTitle4; }
        }
        public string CustomColumnText5
        {
            get { return APOCCore.STCI.ContactCustColTitle5; }
        }
        public string CustomColumnText6
        {
            get { return APOCCore.STCI.ContactCustColTitle6; }
        }
        public string CustomColumnText7
        {
            get { return APOCCore.STCI.ContactCustColTitle7; }
        }
        public string CustomColumnText8
        {
            get { return APOCCore.STCI.ContactCustColTitle8; }
        }
        public string CustomColumnText9
        {
            get { return APOCCore.STCI.ContactCustColTitle9; }
        }
        public string CustomColumnText0
        {
            get { return APOCCore.STCI.ContactCustColTitle0; }
        }
        #endregion

        #region CallLog Settings
        #region G1OnlyDate
        private bool _G1OnlyDate;
        public bool G1OnlyDate
        {
            get
            {
                return _G1OnlyDate;
            }
            set
            {
                if (_G1OnlyDate != value)
                {
                    _G1OnlyDate = value;
                    RaisePropertyChanged("G1OnlyDate");
                }
                if (value)
                    HelperSettingsClient.CallDateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            }
        }
        #endregion
        #region G1OnlyTime
        private bool _G1OnlyTime;
        public bool G1OnlyTime
        {
            get
            {
                return _G1OnlyTime;
            }
            set
            {
                if (_G1OnlyTime != value)
                {
                    _G1OnlyTime = value;
                    RaisePropertyChanged("G1OnlyTime");
                }
                if (value)
                    HelperSettingsClient.CallDateFormat = "HH:mm:ss";
            }
        }
        #endregion
        #region G1Both
        private bool _G1Both;
        public bool G1Both
        {
            get
            {
                return _G1Both;
            }
            set
            {
                if (_G1Both != value)
                {
                    _G1Both = value;
                    RaisePropertyChanged("G1Both");
                }
                if (value)
                    HelperSettingsClient.CallDateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm:ss";
            }
        }
        #endregion

        #region G2Code
        private bool _G2Code;
        public bool G2Code
        {
            get
            {
                return _G2Code;
            }
            set
            {
                if (_G2Code != value)
                {
                    _G2Code = value;
                    RaisePropertyChanged("G2Code");
                }
                if (value)
                    HelperSettingsClient.CallTeleCodeDisplayMode = EnumCallTeleCodeDisplayMode.CodeAll;
            }
        }
        #endregion
        #region G2Title
        private bool _G2Title;
        public bool G2Title
        {
            get
            {
                return _G2Title;
            }
            set
            {
                if (_G2Title != value)
                {
                    _G2Title = value;
                    RaisePropertyChanged("G2Title");
                }
                if (value)
                    HelperSettingsClient.CallTeleCodeDisplayMode = EnumCallTeleCodeDisplayMode.NameAll;
            }
        }
        #endregion
        #region G2Both
        private bool _G2Both;
        public bool G2Both
        {
            get
            {
                return _G2Both;
            }
            set
            {
                if (_G2Both != value)
                {
                    _G2Both = value;
                    RaisePropertyChanged("G2Both");
                }
                if (value)
                    HelperSettingsClient.CallTeleCodeDisplayMode = EnumCallTeleCodeDisplayMode.CodeNameAll;
            }
        }
        #endregion

        #region G3Code
        private bool _G3Code;
        public bool G3Code
        {
            get
            {
                return _G3Code;
            }
            set
            {
                if (_G3Code != value)
                {
                    _G3Code = value;
                    RaisePropertyChanged("G3Code");
                }
                if (value)
                    HelperSettingsClient.CallLineDisplayMode = EnumCallLineDisplayMode.Code;
            }
        }
        #endregion
        #region G3Title
        private bool _G3Title;
        public bool G3Title
        {
            get
            {
                return _G3Title;
            }
            set
            {
                if (_G3Title != value)
                {
                    _G3Title = value;
                    RaisePropertyChanged("G3Title");
                }
                if (value)
                    HelperSettingsClient.CallLineDisplayMode = EnumCallLineDisplayMode.Name;
            }
        }
        #endregion
        #region G3Both
        private bool _G3Both;
        public bool G3Both
        {
            get
            {
                return _G3Both;
            }
            set
            {
                if (_G3Both != value)
                {
                    _G3Both = value;
                    RaisePropertyChanged("G3Both");
                }
                if (value)
                    HelperSettingsClient.CallLineDisplayMode = EnumCallLineDisplayMode.Both;
            }
        }
        #endregion

        #region G4Code
        #region G4Code
        private bool _G4Code;
        public bool G4Code
        {
            get
            {
                return _G4Code;
            }
            set
            {
                if (_G4Code != value)
                {
                    _G4Code = value;
                    RaisePropertyChanged("G4Code");
                }
                if (value)
                    HelperSettingsClient.CallExtDisplayMode = EnumCallLineDisplayMode.Code;
            }
        }
        #endregion
        #region G4Title
        private bool _G4Title;
        public bool G4Title
        {
            get
            {
                return _G4Title;
            }
            set
            {
                if (_G4Title != value)
                {
                    _G4Title = value;
                    RaisePropertyChanged("G4Title");
                }
                if (value)
                    HelperSettingsClient.CallExtDisplayMode = EnumCallLineDisplayMode.Name;
            }
        }
        #endregion
        #region G4Both
        private bool _G4Both;
        public bool G4Both
        {
            get
            {
                return _G4Both;
            }
            set
            {
                if (_G4Both != value)
                {
                    _G4Both = value;
                    RaisePropertyChanged("G4Both");
                }
                if (value)
                    HelperSettingsClient.CallExtDisplayMode = EnumCallLineDisplayMode.Both;
            }
        }
        #endregion
        #endregion
        #endregion



        #region IsAutoRefresh
        public bool IsAutoRefresh
        {
            get { return AutoRefreshTimer.IsEnabled; }
            set
            {
                HelperSettingsClient.CallIsAutoRefresh = value;
                AutoRefreshTimer.IsEnabled = value;
                RaisePropertyChanged("IsAutoRefresh");
            }
        }
        #endregion
        #region AutoRefreshIndex
        public int AutoRefreshIndex
        {
            get { return HelperSettingsClient.CallAutoRefreshIndex; }
            set
            {
                HelperSettingsClient.CallAutoRefreshIndex = value;
                var b = AutoRefreshTimer.IsEnabled;
                AutoRefreshTimer.IsEnabled = false;
                switch (value)
                {
                    case 0:
                        AutoRefreshTimer.Interval = TimeSpan.FromSeconds(10);
                        break;
                    case 1:
                        AutoRefreshTimer.Interval = TimeSpan.FromSeconds(30);
                        break;
                    case 2:
                        AutoRefreshTimer.Interval = TimeSpan.FromMinutes(1);
                        break;
                    case 3:
                        AutoRefreshTimer.Interval = TimeSpan.FromMinutes(5);
                        break;
                    case 4:
                        AutoRefreshTimer.Interval = TimeSpan.FromMinutes(10);
                        break;
                }
                AutoRefreshTimer.IsEnabled = b;
                RaisePropertyChanged("AutoRefreshIndex");
            }
        }
        #endregion

        #region CallFilterList
        private XPCollection<DBCLCallFilter> _CallFilterList;
        public XPCollection<DBCLCallFilter> CallFilterList
        {
            get { return _CallFilterList; }
            set
            {
                _CallFilterList = value;
                RaisePropertyChanged("CallFilterList");
            }
        }
        #endregion
        #region SelectedCallFilter
        private DBCLCallFilter _SelectedCallFilter;
        public DBCLCallFilter SelectedCallFilter
        {
            get
            {
                return _SelectedCallFilter;
            }
            set
            {
                if (ReferenceEquals(value, _SelectedCallFilter)) return;
                _SelectedCallFilter = value;
                RaisePropertyChanged("SelectedCallFilter");

                if (value != null)
                {
                    try
                    {
                        ActiveFilterControl.FilterCriteria = CriteriaOperator.Parse(value.FilterString, null);
                    }
                    catch
                    {
                    }
                }
            }
        }
        #endregion SelectedCallFilter



        #region QuickSearchText
        private string _QuickSearchText;
        public string QuickSearchText
        {
            get { return _QuickSearchText; }
            set
            {
                _QuickSearchText = value;
                RaisePropertyChanged("QuickSearchText");
                UpdateSearchWithDelay();
            }
        }
        #endregion

        #region BeginTime
        private string _BeginTime;
        public string BeginTime
        {
            get { return _BeginTime; }
            set
            {
                _BeginTime = value;
                RaisePropertyChanged("BeginTime");
                UpdateSearchWithDelay();
            }
        }
        #endregion
        #region EndTime
        private string _EndTime;
        public string EndTime
        {
            get { return _EndTime; }
            set
            {
                _EndTime = value;
                RaisePropertyChanged("EndTime");
                UpdateSearchWithDelay();
            }
        }
        #endregion
        #region BeginTimeErrorBrush
        private Brush _BeginTimeErrorBrush;
        public Brush BeginTimeErrorBrush
        {
            get { return _BeginTimeErrorBrush; }
            set
            {
                _BeginTimeErrorBrush = value;
                RaisePropertyChanged("BeginTimeErrorBrush");
            }
        }
        #endregion
        #region EndTimeErrorBrush
        private Brush _EndTimeErrorBrush;
        public Brush EndTimeErrorBrush
        {
            get { return _EndTimeErrorBrush; }
            set
            {
                _EndTimeErrorBrush = value;
                RaisePropertyChanged("EndTimeErrorBrush");
            }
        }
        #endregion
        #region DayOfWeekFilter
        private int _DayOfWeekFilter;
        public int DayOfWeekFilter
        {
            get { return _DayOfWeekFilter; }
            set
            {
                _DayOfWeekFilter = value;
                RaisePropertyChanged("DayOfWeekFilter");
                UpdateSearchWithDelay();
            }
        }
        #endregion









        #region CanSetChartSettings
        public Visibility CanSetChartSettings
        {
            get { return SelectedTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed; }
        }
        #endregion

        #region CallReportList
        private List<DBCLCallReport> _CallReportList;
        public List<DBCLCallReport> CallReportList
        {
            get { return _CallReportList; }
        }
        #endregion
        #region SelectedCallReport
        private DBCLCallReport _SelectedCallReport;
        public DBCLCallReport SelectedCallReport
        {
            get
            {
                return _SelectedCallReport;
            }
            set
            {
                if (ReferenceEquals(value, _SelectedCallReport)) return;
                if (_SelectedCallReport != null)
                {
                    _SelectedCallReport.Save();
                }
                _SelectedCallReport = value;
                RaisePropertyChanged("SelectedCallReport");
                RaisePropertyChanged("CallReportCanChange");

                if (value == null) return;
                SelectedChartAxisY = (from n in ChartAxisYList where n.Value == value.ChartAxisYType select n).FirstOrDefault();
                SelectedChartAxisX = (from n in ChartAxisXList where n.Value == value.ChartAxisXType select n).FirstOrDefault();
                SelectedChartSeparation = (from n in ChartSeparationList where n.Value == value.ChartSeparation select n).FirstOrDefault();
                SelectedPaperKind = (from n in PaperKindList where n.Value == value.Paper select n).FirstOrDefault();

            }
        }
        #endregion
        #region CallReportCanChange
        public bool CallReportCanChange
        {
            get
            {
                return (SelectedCallReport != null && SelectedCallReport.UserCreated == AMembership.ActiveUser.UserName);
            }
        }
        #endregion

        #region ChartAxisYList
        public List<KeyValuePair<string, EnumChartAxisYType>> ChartAxisYList
        {
            get { return GetEnumList<EnumChartAxisYType>(new[] { "تعداد", "مدت مكالمه" }); }
        }
        #endregion
        #region SelectedChartAxisY
        private KeyValuePair<string, EnumChartAxisYType> _SelectedChartAxisY;
        public KeyValuePair<string, EnumChartAxisYType> SelectedChartAxisY
        {
            get
            {
                return _SelectedChartAxisY;
            }
            set
            {
                _SelectedChartAxisY = value;
                if (SelectedCallReport != null)
                    SelectedCallReport.ChartAxisYType = value.Value;
                RaisePropertyChanged("SelectedChartAxisY");
            }
        }
        #endregion

        #region ChartAxisXList
        public List<KeyValuePair<string, EnumChartAxisXType>> ChartAxisXList
        {
            get
            {
                var data = new List<string>();
                data.AddRange(new[]{"زمان (ساعت)", 
                                    "زمان (روز)", 
                                    "زمان (هفته)", 
                                    "زمان (ماه)", 
                                    "زمان (سال)",
                                    "روزهای هفته",
                                    "كد مخابراتی",
                                    "خطوط تلفن",
                                });
                data.Add(DeviceHasInternal ? "خطوط داخلی" : "");
                data.Add(APOCCore.STCI.ContactCustColEnable0 ? APOCCore.STCI.ContactCustColTitle0 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable1 ? APOCCore.STCI.ContactCustColTitle1 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable2 ? APOCCore.STCI.ContactCustColTitle2 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable3 ? APOCCore.STCI.ContactCustColTitle3 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable4 ? APOCCore.STCI.ContactCustColTitle4 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable5 ? APOCCore.STCI.ContactCustColTitle5 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable6 ? APOCCore.STCI.ContactCustColTitle6 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable7 ? APOCCore.STCI.ContactCustColTitle7 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable8 ? APOCCore.STCI.ContactCustColTitle8 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable9 ? APOCCore.STCI.ContactCustColTitle9 : string.Empty);
                return GetEnumList<EnumChartAxisXType>(data.ToArray());
            }
        }
        #endregion
        #region SelectedChartAxisX
        private KeyValuePair<string, EnumChartAxisXType> _SelectedChartAxisX;
        public KeyValuePair<string, EnumChartAxisXType> SelectedChartAxisX
        {
            get { return _SelectedChartAxisX; }
            set
            {
                _SelectedChartAxisX = value;
                if (SelectedCallReport != null)
                    SelectedCallReport.ChartAxisXType = (EnumChartAxisXType)value.Value;
                RaisePropertyChanged("SelectedChartAxisX");
            }
        }
        #endregion

        #region ChartSeparationList
        public List<KeyValuePair<string, EnumChartSeparation>> ChartSeparationList
        {
            get
            {
                var data = new List<string>();
                data.AddRange(new[]{"بدون تفكیك", 
                                    "نوع تماس",
                                    "پرونده",
                                    "كد مخابراتی",
                                    "خطوط تلفن",
                                });
                data.Add(DeviceHasInternal ? "خطوط داخلی" : "");
                data.Add(APOCCore.STCI.ContactCustColEnable0 ? APOCCore.STCI.ContactCustColTitle0 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable1 ? APOCCore.STCI.ContactCustColTitle1 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable2 ? APOCCore.STCI.ContactCustColTitle2 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable3 ? APOCCore.STCI.ContactCustColTitle3 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable4 ? APOCCore.STCI.ContactCustColTitle4 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable5 ? APOCCore.STCI.ContactCustColTitle5 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable6 ? APOCCore.STCI.ContactCustColTitle6 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable7 ? APOCCore.STCI.ContactCustColTitle7 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable8 ? APOCCore.STCI.ContactCustColTitle8 : string.Empty);
                data.Add(APOCCore.STCI.ContactCustColEnable9 ? APOCCore.STCI.ContactCustColTitle9 : string.Empty);
                return GetEnumList<EnumChartSeparation>(data.ToArray());
            }
        }
        #endregion
        #region SelectedChartSeparation
        private KeyValuePair<string, EnumChartSeparation> _SelectedChartSeparation;
        public KeyValuePair<string, EnumChartSeparation> SelectedChartSeparation
        {
            get { return _SelectedChartSeparation; }
            set
            {
                _SelectedChartSeparation = value;
                if (SelectedCallReport != null)
                    SelectedCallReport.ChartSeparation = (EnumChartSeparation)value.Value;
                RaisePropertyChanged("SelectedChartSeparation");
            }
        }
        #endregion

        #region PaperKindList
        public List<KeyValuePair<string, PaperKind>> PaperKindList
        {
            get
            {
                var data = Enum.GetNames(typeof(PaperKind));
                var max = (from n in data select (int)Enum.Parse(typeof(PaperKind), n)).Max();
                var data2 = new string[max + 1];
                for (var i = 0; i < max + 1; i++) data2[i] = string.Empty;

                for (var i = 0; i < data.Count(); i++)
                {
                    var index = (int)Enum.Parse(typeof(PaperKind), data[i]);
                    data2[index] = data[i];
                    if (data2[index] == "Custom")
                        data2[index] = string.Empty;
                }
                return GetEnumList<PaperKind>(data2.ToArray());
            }
        }
        #endregion
        #region SelectedPaperKind
        private KeyValuePair<string, PaperKind> _SelectedPaperKind;
        public KeyValuePair<string, PaperKind> SelectedPaperKind
        {
            get { return _SelectedPaperKind; }
            set
            {
                _SelectedPaperKind = value;
                if (SelectedCallReport != null)
                    SelectedCallReport.Paper = (PaperKind)value.Value;
                RaisePropertyChanged("SelectedPaperKind");
            }
        }
        #endregion

        public DateTime SelectedDate
        {
            get { return APOCContactModule.SelectedDate.Date; }
            set
            {
                RaisePropertyChanged("SelectedDate");
                if (value == APOCContactModule.SelectedDate.Date) return;
                APOCContactModule.SelectedDate = value;
                APOCContactModule.RaiseOnSelectedDateChanged();
            }
        }

        #region ContactCatList
        private List<object> _ContactCatList;
        public List<object> ContactCatList
        {
            get
            {
                return _ContactCatList;
            }
            set
            {
                if (_ContactCatList == value)
                    return;
                _ContactCatList = value;
                RaisePropertyChanged("ContactCatList");
            }
        }
        #endregion
        #region SelectedContactCat
        private object _SelectedContactCat;
        public object SelectedContactCat
        {
            get
            {
                return _SelectedContactCat;
            }
            set
            {
                if (ReferenceEquals(_SelectedContactCat, value)) return;
                _SelectedContactCat = value;
                RaisePropertyChanged("SelectedContactCat");
                UpdateSearch();
            }
        }
        #endregion

        private List<KeyValuePair<string, T>> GetEnumList<T>(string[] values) where T : struct, IConvertible
        {
            var array = Enum.GetValues(typeof(T));
            return (from object v in array
                    let index = (int)v
                    let value = values[index]
                    where !string.IsNullOrWhiteSpace(value)
                    select new KeyValuePair<string, T>(value, (T)v)).ToList();
        }






        #region [METHODS]
        private void InitDynamics()
        {
            MainRibbonControl = MainView.DynamicRibbonControl;
            DynamicGrid = MainView.DynamicDynamicGrid;
            DynamicTableView = DynamicGrid.View as TableView;
            ActiveFilterControl = MainView.ActiveFilterControl;
            ActiveView = MainView as UserControl;
            DynamicChartControl = MainView.DynamicChartControl;
            InitGrid();
            CallLogLoadSettings();


            DynamicGrid.CustomGroupDisplayText += (s, e)
                =>
                {
                    if (e.Column.FieldName == "CallType")
                    {
                        var groupLevel = DynamicGrid.GetRowLevelByRowHandle(e.RowHandle);
                        if (groupLevel != e.Column.GroupIndex) return;
                        e.DisplayText = ((EnumCallType)e.Value) == EnumCallType.CallIn ? "دریافتی" : "ارسالی";
                    }
                    if (e.Column.FieldName == "DurationSeconds")
                    {
                        var groupLevel = DynamicGrid.GetRowLevelByRowHandle(e.RowHandle);
                        if (groupLevel != e.Column.GroupIndex) return;
                        var conv = new POL.Lib.Common.ConvCallDuration();
                        e.DisplayText = conv.Convert(e.Value, null, null, null).ToString();
                    }
                };

            DynamicGrid.CustomColumnGroup += (s, e)
                =>
                {
                };
            if (DynamicTableView != null)
            {
                DynamicTableView.MouseDoubleClick += (s1, e1) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e1);
                    if (i < 0) return;
                    if (CommandGotoContact.CanExecute(null))
                        CommandGotoContact.Execute(null);
                    e1.Handled = true;
                };

                DynamicTableView.PreviewKeyDown += (s1, e1) =>
                {
                };
            }

            APOCMainWindow.GetWindow().Closing +=
                (s, e) => SaveCallGridLayout();
            if (ActiveView != null)
                ActiveView.Loaded +=
                    (s, e) =>
                    {
                        if (HasLoadedLayout) return;
                        RestoreCallGridLayout();
                        HasLoadedLayout = true;
                    };

            DynamicChartControl.CustomDrawSeriesPoint +=
            (s, e) =>
            {
                if (SelectedChartAxisY.Value != EnumChartAxisYType.CallDuration) return;
                e.LabelText = e.SeriesPoint.Value < 0 ? "??:??:??" : TimeSpan.FromSeconds(e.SeriesPoint.Value).ToString();
            };

        }

        private void RestoreCallGridLayout()
        {
            HelperUtils.Try(
                () =>
                {
                    var fn = System.IO.Path.Combine(APOCCore.LayoutPath, ModuleID + "_CallLog.XML");
                    DynamicGrid.RestoreLayoutFromXml(fn);
                });
        }
        private void SaveCallGridLayout()
        {
            HelperUtils.Try(
                () =>
                {
                    var fn = System.IO.Path.Combine(APOCCore.LayoutPath, ModuleID + "_CallLog.XML");
                    DynamicGrid.SaveLayoutToXml(fn);
                });
        }
        private void CallLogLoadSettings()
        {
            if (HelperSettingsClient.CallDateFormat == "HH:mm:ss")
                G1OnlyTime = true;
            else if (HelperSettingsClient.CallDateFormat == Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern)
                G1OnlyDate = true;
            else
                G1Both = true;


            if (HelperSettingsClient.CallTeleCodeDisplayMode == EnumCallTeleCodeDisplayMode.CodeAll)
                G2Code = true;
            else if (HelperSettingsClient.CallTeleCodeDisplayMode == EnumCallTeleCodeDisplayMode.NameAll)
                G2Title = true;
            else
                G2Both = true;

            if (HelperSettingsClient.CallLineDisplayMode == EnumCallLineDisplayMode.Code)
                G3Code = true;
            else if (HelperSettingsClient.CallLineDisplayMode == EnumCallLineDisplayMode.Name)
                G3Title = true;
            else
                G3Both = true;

            if (HelperSettingsClient.CallExtDisplayMode == EnumCallLineDisplayMode.Code)
                G4Code = true;
            else if (HelperSettingsClient.CallExtDisplayMode == EnumCallLineDisplayMode.Name)
                G4Title = true;
            else
                G4Both = true;
        }
        private void InitGrid()
        {
            if (!DeviceHasInternal)
            {
                var q = from n in DynamicGrid.Columns
                        where n.FieldName == "InternalString" || n.FieldName == "LastExt"
                        select n;
                if (q.Any())
                {
                    q.ToList().ForEach(n => DynamicGrid.Columns.Remove(n));
                }
            }
            if (!APOCCore.STCI.ContactCustColEnable0) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText0"]);
            if (!APOCCore.STCI.ContactCustColEnable1) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText1"]);
            if (!APOCCore.STCI.ContactCustColEnable2) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText2"]);
            if (!APOCCore.STCI.ContactCustColEnable3) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText3"]);
            if (!APOCCore.STCI.ContactCustColEnable4) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText4"]);
            if (!APOCCore.STCI.ContactCustColEnable5) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText5"]);
            if (!APOCCore.STCI.ContactCustColEnable6) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText6"]);
            if (!APOCCore.STCI.ContactCustColEnable7) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText7"]);
            if (!APOCCore.STCI.ContactCustColEnable8) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText8"]);
            if (!APOCCore.STCI.ContactCustColEnable9) DynamicGrid.Columns.Remove(DynamicGrid.Columns["Contact.CCText9"]);

            if (!APOCCore.STCI.DeviceHasRecord)
            {
                DynamicGrid.Columns.Remove(DynamicGrid.Columns["RecordEnable"]);
                DynamicGrid.Columns.Remove(DynamicGrid.Columns["RecordRole"]);
            }
        }

        private DateTime LastUpdateSearch = DateTime.MinValue;
        private void UpdateSearch()
        {
            if (!AMembership.IsAuthorized) return;
            if (BeginTime == null) return;
            if (ReferenceEquals(MainSearchCriteria, null))
                MainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            MainSearchCriteria.Operands.Clear();

            if ((DateTime.Now - LastUpdateSearch).TotalMilliseconds < 500)
            {
                LastUpdateSearch = DateTime.Now;
                return;
            }
            LastUpdateSearch = DateTime.Now;
            try
            {
                var t1 = DateTime.ParseExact(BeginTime, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay.Ticks;
                if (t1 != 0)
                {
                    var co1 = CriteriaOperator.Parse("GetTimeOfDay(CallDate) > " + t1);
                    MainSearchCriteria.Operands.Add(co1);
                }
                BeginTimeErrorBrush = Brushes.White;
            }
            catch
            {
                BeginTimeErrorBrush = Brushes.LightPink;
            }
            try
            {
                var t2 = DateTime.ParseExact(EndTime, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay.Ticks;
                if (t2 < TimeSpan.FromMinutes(23 * 60 + 59).Ticks)
                {
                    var co2 = CriteriaOperator.Parse("GetTimeOfDay(CallDate) < " + t2);
                    MainSearchCriteria.Operands.Add(co2);
                }
                EndTimeErrorBrush = Brushes.White;
            }
            catch
            {
                HelperUtils.DoDispatcher(() =>
                {
                    EndTimeErrorBrush = Brushes.LightPink;
                });
            }

            if (DayOfWeekFilter > 0)
            {
                var day = DayOfWeek.Saturday;
                if (DayOfWeekFilter == 2)
                    day = DayOfWeek.Sunday;
                if (DayOfWeekFilter == 3)
                    day = DayOfWeek.Monday;
                if (DayOfWeekFilter == 4)
                    day = DayOfWeek.Thursday;
                if (DayOfWeekFilter == 5)
                    day = DayOfWeek.Wednesday;
                if (DayOfWeekFilter == 6)
                    day = DayOfWeek.Thursday;
                if (DayOfWeekFilter == 7)
                    day = DayOfWeek.Friday;
                var co1 = CriteriaOperator.Parse("GetDayOfWeek(CallDate) = " + ((int)day).ToString());
                MainSearchCriteria.Operands.Add(co1);
            }











            MainSearchCriteria.Operands.Add(new BinaryOperator("Del", false));

            if (IsFilterByDate)
            {
                SelectedDate = APOCContactModule.SelectedDate.Date;
                MainSearchCriteria.Operands.Add(new BinaryOperator("CallDate", APOCContactModule.SelectedDate.Date, BinaryOperatorType.GreaterOrEqual));
                MainSearchCriteria.Operands.Add(new BinaryOperator("CallDate", APOCContactModule.SelectedDate.Date.AddDays(1), BinaryOperatorType.Less));
                if (IsShowCallIn)
                    MainSearchCriteria.Operands.Add(new BinaryOperator("CallType", EnumCallType.CallIn));
                if (IsShowCallOut)
                    MainSearchCriteria.Operands.Add(new BinaryOperator("CallType", EnumCallType.CallOut));
                if (IsShowMissCall)
                {
                    MainSearchCriteria.Operands.Add(new BinaryOperator("CallType", EnumCallType.CallIn));
                    MainSearchCriteria.Operands.Add(new BinaryOperator("DurationSeconds", 0, BinaryOperatorType.Less));
                }
            }
            if (IsFilterByFilter)
            {
                MainSearchCriteria.Operands.Add(GetCallFilter());
            }

            if (!AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_1))
                MainSearchCriteria.Operands.Add(new BinaryOperator("LineNumber", 1, BinaryOperatorType.NotEqual));
            if (!AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_2))
                MainSearchCriteria.Operands.Add(new BinaryOperator("LineNumber", 2, BinaryOperatorType.NotEqual));
            if (!AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_3))
                MainSearchCriteria.Operands.Add(new BinaryOperator("LineNumber", 3, BinaryOperatorType.NotEqual));
            if (!AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_4))
                MainSearchCriteria.Operands.Add(new BinaryOperator("LineNumber", 4, BinaryOperatorType.NotEqual));

            if (!string.IsNullOrWhiteSpace(AMembership.ActiveUser.InternalPhone))
            {
                var selfInternal = AMembership.ActiveUser.InternalPhone.Split(new char[] { ' ', ',', ';' },
                    StringSplitOptions.RemoveEmptyEntries);
                if (AMembership.ActiveUser.HasPermission((int)PCOPermissions.Call_Internal_Only) &&
                    selfInternal.Length != 0)
                {
                    var gointernal = new GroupOperator(GroupOperatorType.Or);
                    foreach (var internalCode in selfInternal)
                    {
                        var boInternal = new BinaryOperator("LastExt", internalCode, BinaryOperatorType.Equal);
                        gointernal.Operands.Add(boInternal);
                    }
                    MainSearchCriteria.Operands.Add(gointernal);
                }
            }

            if (AMembership.ActiveUser.UserName != "admin" && !(SelectedContactCat is DBCTContactCat))
            {
                var goCat = new GroupOperator(GroupOperatorType.Or);
                goCat.Operands.Add(new NullOperator("Contact"));
                foreach (var cat in ContactCatList)
                {
                    if (cat is DBCTContactCat)
                        goCat.Operands.Add(CriteriaOperator.Parse("[Contact.Categories][Oid == {" + ((DBCTContactCat)cat).Oid + "}]"));
                }
                if (goCat.Operands.Count > 0)
                    MainSearchCriteria.Operands.Add(goCat);
            }
            if (SelectedContactCat is DBCTContactCat)
                MainSearchCriteria.Operands.Add(CriteriaOperator.Parse("[Contact.Categories][Oid == {" + ((DBCTContactCat)SelectedContactCat).Oid + "}]"));

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBCLCall))
            ;


            if (!string.IsNullOrWhiteSpace(QuickSearchText))
            {
                QuickSearchText = QuickSearchText.Replace("%", "").Replace("*", "");
                var go = new GroupOperator(GroupOperatorType.Or);
                go.Operands.Add(new BinaryOperator("Contact.Title", "%" + QuickSearchText + "%", BinaryOperatorType.Like));
                go.Operands.Add(new BinaryOperator("PhoneNumber", "%" + QuickSearchText + "%", BinaryOperatorType.Like));
                MainSearchCriteria.Operands.Add(go);
            }

            xpi.FixedFilterCriteria = MainSearchCriteria;
            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.Dxs;
            };
            CallList = null;
            CallList = xpi;
        }
        private void UpdateSearchWithDelay()
        {
            if (CallUpdateTimer == null)
            {
                CallUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                CallUpdateTimer.Tick += (s, e) =>
                {
                    CallUpdateTimer.Stop();
                    UpdateSearch();
                };
            }
            CallUpdateTimer.Stop();
            CallUpdateTimer.Start();
        }
        private GroupOperator GetCallFilter()
        {
            return null;
        }
        private void PopulateCallFilterList()
        {
            CallFilterList = null;
            CallFilterList = DBCLCallFilter.GetAll(ADatabase.Dxs);
        }
        private void PopulateCallReportList()
        {
            _CallReportList = null;
            _CallReportList = DBCLCallReport.GetAllByMembership(ADatabase.Dxs, AMembership.ActiveUser).ToList();
            RaisePropertyChanged("CallReportList");
        }


        private void InitCommands()
        {
            CommandFilterByDate = new RelayCommand(() => { }, () => true);
            CommandShowAll = new RelayCommand(() => { }, () => IsFilterByDate && AMembership.HasPermission(PCOPermissions.Call_AllowCallIn) && AMembership.HasPermission(PCOPermissions.Call_AllowCallOut));
            CommandShowCallOut = new RelayCommand(() => { }, () => IsFilterByDate && AMembership.HasPermission(PCOPermissions.Call_AllowCallOut));
            CommandShowCallIn = new RelayCommand(() => { }, () => IsFilterByDate && AMembership.HasPermission(PCOPermissions.Call_AllowCallIn));
            CommandShowMissCall = new RelayCommand(() => { }, () => IsFilterByDate && AMembership.HasPermission(PCOPermissions.Call_AllowCallIn));
            CommandFilterByFilter = new RelayCommand(() => { }, () => AMembership.HasPermission(PCOPermissions.Call_AllowFilter));

            CommandCallDeleteSingle = new RelayCommand(CallDeleteSingle, () => FocusedCall != null && AMembership.HasPermission(PCOPermissions.Call_AllowDelete));
            CommandCallDeleteAll = new RelayCommand(CallDeleteAll, () => FocusedCall != null && AMembership.HasPermission(PCOPermissions.Call_AllowDelete));
            CommandCallCorrect = new RelayCommand(CallCorrect, () => FocusedCall != null && AMembership.HasPermission(PCOPermissions.Call_AllowCorrection));

            CommandSyncSingle = new RelayCommand(SyncSingle, () => FocusedCall != null && AMembership.HasPermission(PCOPermissions.Call_AllowSync));
            CommandSyncAll = new RelayCommand(SyncAll, () => AMembership.HasPermission(PCOPermissions.Call_AllowSync));
            CommandGotoContact = new RelayCommand(GotoContact, () => FocusedCall != null && FocusedCall.Contact != null);

            CommandRefresh = new RelayCommand(Refresh, () => true);
            CommandSetCallNote = new RelayCommand(SetCallNote, () => FocusedCall != null && AMembership.HasPermission(PCOPermissions.Call_TagAdd));

            CommandApplyCallFilter = new RelayCommand(ApplyCallFilter, () => true);
            CommandSaveCallFilter = new RelayCommand(SaveCallFilter, () => true);
            CommandCallFilterClear = new RelayCommand(CallFilterClear, () => true);
            CommandCallFilterDelete = new RelayCommand(CallFilterDelete, () => true);
            CommandCallFilterRefresh = new RelayCommand(CallFilterRefresh, () => true);

            CommandCallReportAdd = new RelayCommand(CallReportAdd, () => AMembership.HasPermission(PCOPermissions.Call_AllowChart));
            CommandCallReportEdit = new RelayCommand(CallReportEdit, () => AMembership.HasPermission(PCOPermissions.Call_AllowChart));
            CommandCallReportDelete = new RelayCommand(CallReportDelete, () => AMembership.HasPermission(PCOPermissions.Call_AllowChart));
            CommandCallReportRefresh = new RelayCommand(CallReportRefresh, () => AMembership.HasPermission(PCOPermissions.Call_AllowChart));
            CommandCallReportRender = new RelayCommand(CallReportRender, () => SelectedCallReport != null && AMembership.HasPermission(PCOPermissions.Call_AllowChart));

            CommandPrintChart = new RelayCommand(PrintChartBar, () => AMembership.HasPermission(PCOPermissions.Call_AllowPrint) && SelectedTabIndex == 1);
            CommandPrint1 = new RelayCommand(Print1, () => AMembership.HasPermission(PCOPermissions.Call_AllowPrint) && SelectedTabIndex == 0);
            CommandPrint2 = new RelayCommand(Print2, () => AMembership.HasPermission(PCOPermissions.Call_AllowPrint) && SelectedTabIndex == 0);
            CommandPrint3 = new RelayCommand(Print3, () => AMembership.HasPermission(PCOPermissions.Call_AllowPrint) && SelectedTabIndex == 0);
            CommandPrint4 = new RelayCommand(Print4, () => AMembership.HasPermission(PCOPermissions.Call_AllowPrint) && SelectedTabIndex == 0);
            CommandPrint5 = new RelayCommand(Print5, () => AMembership.HasPermission(PCOPermissions.Call_AllowPrint) && SelectedTabIndex == 0);
            CommandPrint6 = new RelayCommand(Print6, () => AMembership.HasPermission(PCOPermissions.Call_AllowPrint) && SelectedTabIndex == 0);

            CommandVoicePlay = new RelayCommand(VoicePlay, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Call_Record_Play));
            CommandVoiceDelete = new RelayCommand(VoiceDelete, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Call_Record_Delete));
            CommandVoiceSave = new RelayCommand(VoiceSave, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Call_Record_Save));
            CommandVoiceRole1 = new RelayCommand(VoiceRole1, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Call_Record_SetRole));
            CommandVoiceRole2 = new RelayCommand(VoiceRole2, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Call_Record_SetRole));
            CommandVoiceRole3 = new RelayCommand(VoiceRole3, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Call_Record_SetRole));

            CommandNextDay = new RelayCommand(NextDay, () => IsFilterByDate);
            CommandPrevDay = new RelayCommand(PrevDay, () => IsFilterByDate);

            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);

            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);

            CommandQuickSearchClear = new RelayCommand(QuickSearchClear, () => true);

            CommandClearFilterByDay = new RelayCommand(ClearFilterByDay, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp22 != "");
        }

        private void ClearFilterByDay()
        {
            BeginTime = "00:00";
            EndTime = "23:59";
            DayOfWeekFilter = 0;
        }

        private void QuickSearchClear()
        {
            QuickSearchText = null;
            UpdateSearch();
        }








        private void Refresh()
        {
            APOCMainWindow.ShowBusyIndicator();
            var i = DynamicTableView.TopRowIndex;
            var srhs = DynamicGrid.GetSelectedRowHandles();
            UpdateSearch();
            DynamicGrid.UnselectAll();

            srhs.ToList().ForEach(r => DynamicGrid.SelectItem(r));
            DynamicTableView.TopRowIndex = i;
            APOCMainWindow.HideBusyIndicator();
        }
        private void SetCallNote()
        {
            APOCMainWindow.ShowSetCallNote(APOCMainWindow.GetWindow(), FocusedCall);
        }

        private void CallCorrect()
        {
            var w = new WCallCorrection(FocusedCall) { Owner = APOCMainWindow.GetWindow() };
            w.ShowDialog();
        }
        private void CallDeleteSingle()
        {
            if (SelectedRowCount <= 0) return;
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("تعداد {0} تماس حذف شود؟", SelectedRowCount), APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;
            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("حذف تماس", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCLCall> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCLCall).ToList();
                        }));

                    w.AsyncSetText(1, "در حال حذف");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.PhoneNumber);
                            v.Del = true;
                            v.DelUser = AMembership.IsAuthorized ? AMembership.ActiveUser.UserName : string.Empty;
                            v.Save();
                            successCount++;
                        }
                        catch
                        {
                            failedCount++;
                        }
                        w.AsyncSetText(3, string.Format("موفقیت : {0}  - خطا : {1}", successCount, failedCount));
                    }
                },
                w =>
                {
                    POLMessageBox.ShowInformation(String.Format("تعداد {0} تماس با موفقیت حذف شد.{1}تعداد خطا ها : {2}", successCount, Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, APOCMainWindow.GetWindow());
        }
        private void CallDeleteAll()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("تمام تماس های نمایش داده شده حذف شود؟", APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;
            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("حذف تماس", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال حذف");

                    try
                    {
                        var fc = MainSearchCriteria;
                        HelperUtils.DoDispatcher(
                            () =>
                            {
                                var opEmpty = MainSearchCriteria.Operands.Where(opGo => ReferenceEquals(opGo, null) || (opGo is GroupOperator && ((GroupOperator)opGo).Operands.Count == 0)).ToList();
                                opEmpty.ForEach(op => MainSearchCriteria.Operands.Remove(op));
                                if (!ReferenceEquals(DynamicGrid.FilterCriteria, null))
                                    fc = new GroupOperator(MainSearchCriteria, DynamicGrid.FilterCriteria);
                            });

                        using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                        {
                            uow.Update<DBCLCall>(
                                  () => new DBCLCall(uow)
                                  {
                                      Del = true,
                                      DelUser = AMembership.ActiveUser.UserName,
                                  },
                                  fc);
                            uow.CommitChanges();
                        }
                        successCount = 1;
                    }
                    catch
                    {
                        failedCount = 1;

                    }
                },
                w =>
                {
                    if (successCount > 0)
                        POLMessageBox.ShowInformation("عملیات انجام شد.", w);
                    if (failedCount > 0)
                        POLMessageBox.ShowError("بروز خطا در حذف اطلاعات.", w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, APOCMainWindow.GetWindow());
        }

        private void SyncSingle()
        {
            CallSync(false, false);
        }
        private void SyncAll()
        {
            CallSync(true, true);
        }
        private void GotoContact()
        {
            if (FocusedCall == null) return;
            if (FocusedCall.Contact == null) return;
            var roles = FocusedCall.Contact.Categories.ToList().Select(n => n.Role).Where(r => r != null);
            if (AMembership.ActiveUser.Roles.Select(r => r.ToLower()).Intersect(roles.Select(r => r.ToLower())).Any() || AMembership.ActiveUser.UserName.ToLower() == "admin")
                APOCContactModule.GotoContactByCode(FocusedCall.Contact.Code);
            else
            {
                POLMessageBox.ShowWarning("خطا : سطوح دسترسی كافی جهت مشاهده پرونده وجود ندارد.", APOCMainWindow.GetWindow());
            }
        }


        private void ApplyCallFilter()
        {
            ActiveFilterControl.ApplyFilter();
        }
        private void SaveCallFilter()
        {
            ApplyCallFilter();
            if (SelectedCallFilter == null)
            {
                var w = new WCallFilterAddEdit(null)
                            {
                                Owner = APOCMainWindow.GetWindow(),
                            };
                if (w.ShowDialog() == true)
                {
                    if (w.DynamicDBCallFilter != null)
                    {
                        var db = DBCLCallFilter.FindByOid(ADatabase.Dxs, w.DynamicDBCallFilter.Oid);
                        db.FilterString = !ReferenceEquals(ActiveFilterControl.FilterCriteria, null)
                                              ? ActiveFilterControl.FilterCriteria.ToString()
                                              : null;
                        db.Save();
                        PopulateCallFilterList();
                    }
                }
            }
            else
            {
                var dr = POLMessageBox.ShowQuestionYesNo(string.Format("تغییرات در فیلتر زیر ذخیره شود؟ {0}{0}{1}", Environment.NewLine, SelectedCallFilter),
                APOCMainWindow.GetWindow());
                if (dr != MessageBoxResult.Yes) return;
                try
                {
                    SelectedCallFilter.FilterString = !ReferenceEquals(ActiveFilterControl.FilterCriteria, null)
                                                          ? ActiveFilterControl.FilterCriteria.ToString()
                                                          : null;
                    SelectedCallFilter.Save();
                }
                catch (Exception ex)
                {
                    POLMessageBox.ShowError(ex.Message, APOCMainWindow.GetWindow());
                }
            }
        }
        private void CallFilterClear()
        {
            SelectedCallFilter = null;
        }
        private void CallFilterDelete()
        {
            if (SelectedCallFilter == null) return;
            var db = SelectedCallFilter;
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("فیلتر زیر حذف شود؟{0}{0}{1}", Environment.NewLine, db),
                APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;
            try
            {
                SelectedCallFilter = null;
                db.Delete();
                db.Save();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, APOCMainWindow.GetWindow());
            }
            PopulateCallFilterList();
        }
        private void CallFilterRefresh()
        {
            PopulateCallFilterList();
            SelectedCallFilter = null;
        }

        private void CallSync(bool ask, bool fromSearchResult)
        {
            if (FocusedCall == null) return;
            var selectedcalls = DynamicGrid.GetSelectedRowHandles();

            POLProgressBox.Show(3, pw
                =>
                {
                    if (!fromSearchResult)
                    {
                        var dxs = ADatabase.GetNewSession();
                        if (selectedcalls == null || selectedcalls.Length == 0)
                            return;
                        var items = (from n in selectedcalls select DBCLCall.FindByOid(dxs, ((DBCLCall)DynamicGrid.GetRow(n)).Oid));
                        if (items.Count() == 1)
                            SyncSingle(items.First());
                        else if (items.Count() > 1)
                            SyncMulti(pw, items.ToList());
                    }
                    else
                    {
                        var proceed = false;
                        var fc = MainSearchCriteria;
                        POL.Lib.Utils.HelperUtils.DoDispatcher(
                            () =>
                            {
                                var opEmpty = MainSearchCriteria.Operands.Where(opGo => ReferenceEquals(opGo, null) || (opGo is GroupOperator && ((GroupOperator)opGo).Operands.Count == 0)).ToList();
                                opEmpty.ForEach(op => MainSearchCriteria.Operands.Remove(op));
                                if (!ReferenceEquals(DynamicGrid.FilterCriteria, null))
                                    fc = new GroupOperator(MainSearchCriteria, DynamicGrid.FilterCriteria);
                            });

                        var dxs = ADatabase.GetNewSession();

                        var xpq = new XPQuery<DBCLCall>(dxs);
                        var converter = new CriteriaToExpressionConverter();
                        DBCLCall.FixGroupOperatorToLINQForCall(fc);

                        var xpq2 = xpq.AppendWhere(converter, fc) as XPQuery<DBCLCall>;
                        var count = xpq2.Count();

                        if (ask)
                        {
                            var doo = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Func<object>)(() =>
                            {
                                var dr = POLMessageBox.ShowQuestionYesNo(String.Format("تطبیق برای {0} ركورد تماس انجام شود؟", count));
                                return dr;
                            }));

                            while (doo.Status != DispatcherOperationStatus.Completed)
                            {
                                Thread.Sleep(1);
                            }

                            var v = (MessageBoxResult)doo.Result;
                            if (v == MessageBoxResult.Yes)
                                proceed = true;
                        }
                        else
                            proceed = true;

                        if (proceed)
                        {
                            pw.AsyncEnableCancel();
                            pw.AsyncSetMax(count);
                            pw.AsyncSetText(1, String.Format("تعداد : {0}", count));

                            var synced = 0;

                            for (var i = 0; i < count; i = i + 100)
                            {
                                var list = (from n in xpq2 orderby n.Oid select n).Skip(i).Take(100).ToList();
                                var counter = 0;
                                foreach (var dbcall in list)
                                {
                                    if (pw.NeedToCancel) return;
                                    counter++;
                                    pw.AsyncSetText(2, String.Format("شمارش : {0}", i + counter));
                                    if (dbcall.Contact == null)
                                    {
                                        if (FindProperContact(dbcall))
                                            synced++;
                                    }
                                    else
                                    {
                                        if (dbcall.Contact.IsDeleted)
                                        {
                                            if (RefindContact(dbcall))
                                                synced++;
                                        }
                                        else
                                        {
                                            var found = false;
                                            var find = DBCTPhoneBook.GetByContact(dbcall.Session, dbcall.Contact.Oid).ToList();
                                            if (dbcall.City != null)
                                            {
                                                if (find.Count(n => n.City != null && n.City.Oid == dbcall.City.Oid) > 0)
                                                    found = true;
                                            }
                                            else if (dbcall.Country != null)
                                                if (find.Count(n => n.Country != null && n.Country.Oid == dbcall.Country.Oid) > 0)
                                                    found = true;
                                            if (!found)
                                                RefindContact(dbcall);
                                        }
                                    }
                                    pw.AsyncSetText(3, string.Format("تطبیق : {0}", synced));
                                    pw.AsyncSetValue(counter + i - 1);
                                }
                            }
                        }
                    }
                }, pw =>
                       {
                           UpdateSearch();
                       }, APOCMainWindow.GetWindow());


        }
        private void SyncSingle(DBCLCall dbc)
        {
            if (dbc.Contact == null)
            {
                var foundContact = false;

                DBCTPhoneBook dbp = null;
                if (dbc.City != null)
                    dbp = DBCTPhoneBook.FindByPhoneAndCity(dbc.Session, dbc.PhoneNumber, dbc.City.Oid);
                else if (dbc.Country != null)
                    dbp = DBCTPhoneBook.FindByPhoneAndCountry(dbc.Session, dbc.PhoneNumber, dbc.Country.Oid);

                if (dbp != null)
                {
                    if (dbp.Contact != null)
                        foundContact = true;
                }

                if (foundContact)
                {
                    dbc.Contact = dbp.Contact;
                    dbc.Save();
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, (Action)(() =>
                    {

                        APOCMainWindow.ShowCallSync(APOCMainWindow.GetWindow(), dbc);
                        UpdateSearch();
                    }));
                }
            }
            else
            {
                if (dbc.Contact.IsDeleted)
                {
                    POL.Lib.Utils.HelperUtils.DoDispatcher(
                        () =>
                        {
                            var tdr = POLMessageBox.ShowQuestionYesNo("اطلاعات قبلی تماس انتخاب شده، حذف شده اند" + "\n" + "اطلاعات سوخته از نو بررسی شود؟");
                            if (tdr != MessageBoxResult.Yes) return;
                            dbc.Contact = null;
                            dbc.Save();
                            SyncSingle(dbc);
                        });

                }
                else
                {
                    var find = DBCTPhoneBook.GetByContact(dbc.Session, dbc.Oid).ToList();
                    if (dbc.City != null)
                    {
                        if (find.Count(n => n.City != null && n.City.Oid == dbc.City.Oid) > 0)
                            return;
                    }
                    else if (dbc.Country != null)
                        if (find.Count(n => n.Country != null && n.Country.Oid == dbc.Country.Oid) > 0)
                            return;
                    dbc.Contact = null;
                    dbc.Save();
                    SyncSingle(dbc);
                }
            }
        }
        private void SyncMulti(POLProgressBox e2, List<DBCLCall> calllist)
        {
            if (calllist == null) return;
            if (calllist.Count == 0) return;
            e2.AsyncEnableCancel();
            e2.AsyncSetMax(calllist.Count);

            var synced = 0;
            var counter = 0;
            e2.AsyncSetText(1, String.Format("تعداد : {0}", calllist.Count));
            foreach (var dbcall in calllist)
            {
                counter++;
                e2.AsyncSetText(2, String.Format("شمارش : {0}", counter));
                if (dbcall.Contact == null)
                {
                    if (FindProperContact(dbcall))
                        synced++;
                }
                else
                {
                    if (dbcall.Contact.IsDeleted)
                    {
                        if (RefindContact(dbcall))
                            synced++;
                    }
                    else
                    {
                        var found = false;
                        var find = DBCTPhoneBook.GetByContact(dbcall.Session, dbcall.Contact.Oid).ToList();
                        if (dbcall.City != null)
                        {
                            if (find.Count(n => n.City != null && n.City.Oid == dbcall.City.Oid) > 0)
                                found = true;
                        }
                        else if (dbcall.Country != null)
                            if (find.Count(n => n.Country != null && n.Country.Oid == dbcall.Country.Oid) > 0)
                                found = true;
                        if (!found)
                            RefindContact(dbcall);
                    }
                }
                e2.AsyncSetText(3, string.Format("تطبیق : {0}", synced));
                e2.AsyncSetValue(counter - 1);
            }
        }
        private bool RefindContact(DBCLCall dbcall)
        {
            try
            {
                dbcall.Contact = null;
                dbcall.Phone = null;
                dbcall.Save();
                return FindProperContact(dbcall);
            }
            catch { }
            return false;
        }
        private bool FindProperContact(DBCLCall dbcall)
        {
            if (dbcall == null) return false;
            DBCTPhoneBook dbp = null;
            if (dbcall.Country != null)
                dbp = DBCTPhoneBook.FindByPhoneAndCountry(dbcall.Session, dbcall.PhoneNumber, dbcall.Country.Oid);
            else if (dbcall.City != null)
                dbp = DBCTPhoneBook.FindByPhoneAndCity(dbcall.Session, dbcall.PhoneNumber, dbcall.City.Oid);
            if (dbp == null) return false;
            try
            {
                dbcall.Contact = dbp.Contact;
                dbcall.Phone = dbp;
                dbcall.Save();
                return true;
            }
            catch { }
            return false;
        }




        private void CallReportAdd()
        {
            var w = new WCallReportAddEdit(null)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                CallReportRefresh();
        }
        private void CallReportEdit()
        {
            if (SelectedCallReport == null) return;
            var w = new WCallReportAddEdit(SelectedCallReport)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                CallReportRefresh();
        }
        private void CallReportDelete()
        {
            if (SelectedCallReport == null) return;
            SelectedCallReport.Reload();
            if (SelectedCallReport.UserCreated != AMembership.ActiveUser.UserName) return;
            if (SelectedCallReport.IsDeleted)
            {
                CallReportRefresh();
                return;
            }
            var db = SelectedCallReport;
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("گزارش زیر حذف شود؟{0}{0}{1}", Environment.NewLine, db),
                APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;
            try
            {
                SelectedCallReport = null;
                db.Delete();
                db.Save();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, APOCMainWindow.GetWindow());
            }
            CallReportRefresh();
        }
        private void CallReportRefresh()
        {
            PopulateCallReportList();
        }
        private void CallReportRender()
        {
            if (SelectedCallReport == null) return;
            ChartDataHolder = new List<List<KeyValuePair<string, int>>>();
            HelperUtils.Try(() =>
            {
                SelectedCallReport.Save();
            });

            POLProgressBox.Show("بررسی اطلاعات", false, 0, 10, 4,
                pb =>
                {
                    var dxs = ADatabase.GetNewSession();
                    var fc = MainSearchCriteria;
                    HelperUtils.DoDispatcher(
                        () =>
                        {
                            var opEmpty = MainSearchCriteria.Operands.Where(opGo => ReferenceEquals(opGo, null) || (opGo is GroupOperator && ((GroupOperator)opGo).Operands.Count == 0)).ToList();
                            opEmpty.ForEach(op => MainSearchCriteria.Operands.Remove(op));
                            if (!ReferenceEquals(DynamicGrid.FilterCriteria, null))
                                fc = new GroupOperator(MainSearchCriteria, DynamicGrid.FilterCriteria);
                        });

                    var xpqAll = new XPQuery<DBCLCall>(dxs);
                    var converter = new CriteriaToExpressionConverter();
                    DBCLCall.FixGroupOperatorToLINQForCall(fc);
                    var xpq = xpqAll.AppendWhere(converter, fc) as XPQuery<DBCLCall>;
                    var count = xpq.Count();
                    if (count == 0)
                    {
                        HelperUtils.DoDispatcher(() => POLMessageBox.ShowError("هیچ گونه اطلاعاتی جهت نمایش وجود ندارد.", pb));
                        return;
                    }
                    pb.AsyncSetText(1, "تعداد ركورد ها : " + count);
                    if (count == 0)
                    {
                        HelperUtils.DoDispatcher(() => POLMessageBox.ShowError("هیچ گونه اطلاعاتی جهت نمایش وجود ندارد.", pb));
                        return;
                    }

                    var minDate = (from n in xpq orderby n.CallDate select n).First().CallDate.Value;
                    var maxDate = (from n in xpq orderby n.CallDate descending select n).First().CallDate.Value;
                    var xCount = GetChartAxisXCount(xpq, minDate, maxDate);
                    if (xCount == 0) xCount = 1;
                    if (!CheckChartAxisXCount(pb, xCount))
                        return;

                    pb.AsyncSetText(2, "تعداد در محور افقی : " + xCount);


                    var zCount = GetChartAxisZCount(xpq, minDate, maxDate);
                    pb.AsyncSetText(3, "تعداد تفكیك : " + zCount);

                    if (xCount * zCount > 128)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : تعداد ستون بیش از حد مجاز می باشد." + Environment.NewLine +
                                                                               "حد مجاز 128 عدد می باشد." + Environment.NewLine + Environment.NewLine +
                                                                               "تعداد ستون اطلاعاتی : " + xCount + Environment.NewLine +
                                                                               "تعداد تفكیك : " + zCount + Environment.NewLine +
                                                                               "تعداد ستون : " + xCount * zCount
                                                                               , pb));
                        return;
                    }

                    pb.AsyncSetMax(zCount);
                    for (var i = 0; i < zCount; i++)
                    {
                        pb.AsyncSetValue(i + 1);
                        var dataSource = GenerateDatasourceForChart(xpq, i, minDate, maxDate);
                        ChartDataHolder.Add(dataSource);
                        pb.AsyncSetText(4, "محاسبه تفكیك : " + i);
                    }
                    HelperUtils.DoDispatcher(() =>
                    {
                        var gs = 1;
                        var maxValue = 0;
                        foreach (var cdh in ChartDataHolder)
                        {
                            for (int i = 0; i < cdh.Count; i++)
                            {
                                var val = cdh[i].Value;
                                if (maxValue < val)
                                    maxValue = val;
                            }
                        }
                        if (maxValue > 10)
                            gs = 10;
                        if (maxValue > 100)
                            gs = 100;
                        if (maxValue > 1000)
                            gs = 1000;
                        if (maxValue > 10000)
                            gs = 10000;
                        if (maxValue > 100000)
                            gs = 100000;
                        if (maxValue > 1000000)
                            gs = 1000000;
                        var diagram = new XYDiagram2D
                               {
                                   Rotated = SelectedCallReport.ChartRotate,
                                   AxisX = new AxisX2D { Title = new AxisTitle { Content = SelectedChartAxisX.Key }, GridSpacing = 1 },
                                   AxisY = new AxisY2D { Title = new AxisTitle { Content = SelectedChartAxisY.Key }, GridSpacing = gs }
                               };
                        if (SelectedChartAxisY.Value == EnumChartAxisYType.CallDuration)
                        {
                            diagram.AxisY.MinorCount = 3;
                            var maxVal = (from ds in ChartDataHolder
                                          from v in ds
                                          select v.Value).Max();
                            var sep = 24 * 60 * 60;
                            var text = "روز";


                            if (maxVal < 3 * 24 * 60 * 60)
                            {
                                sep = 12 * 60 * 60;
                                text = "ساعت";
                            }
                            if (maxVal < 24 * 60 * 60)
                            {
                                sep = 2 * 60 * 60;
                                text = "ساعت";
                            }
                            if (maxVal < 10 * 60 * 60)
                            {
                                sep = 60 * 60;
                                text = "ساعت";
                            }
                            if (maxVal < 60 * 60)
                            {
                                sep = 10 * 60;
                                text = "دقیقه";
                            }
                            if (maxVal < 30 * 60) 
                            {
                                sep = 5 * 60;
                                text = "دقیقه";
                            }
                            if (maxVal < 10 * 60) 
                            {
                                sep = 60;
                                text = "دقیقه";
                            }
                            if (maxVal < 60) 
                            {
                                sep = 10;
                                text = "ثانیه";
                                diagram.AxisY.MinorCount = 9;
                            }

                            var c = ((maxVal / sep) + 1) * sep;

                            for (var i = 0; i <= c; i = i + sep)
                            {
                                var cal = new CustomAxisLabel
                                   {
                                       Value = i,
                                       Content = string.Format("{0} {1}", ((int)Math.Round((double)i / sep)), text),
                                   };
                                diagram.AxisY.CustomLabels.Add(cal);
                            }
                        }
                        diagram.Series.Clear();

                        for (var i = 0; i < zCount; i++)
                        {
                            var series = new BarSideBySideSeries2D
                                             {
                                                 DisplayName = GetSeriesDisplayName(i),
                                                 ShowInLegend = true,
                                                 DataSource = ChartDataHolder[i],
                                                 ValueDataMember = "Value",
                                                 ArgumentDataMember = "Key",
                                                 LabelsVisibility = SelectedCallReport.ChartLable,
                                                 Model = new SimpleBar2DModel(),
                                             };
                            diagram.Series.Add(series);
                        }
                        var chart = DynamicChartControl;
                        chart.CrosshairEnabled = false;
                        chart.Diagram = diagram;
                        chart.Legend.Visibility = SelectedCallReport.ChartLegend ? Visibility.Visible : Visibility.Collapsed;
                    });
                },
                pb => { },
                APOCMainWindow.GetWindow());
        }

        private List<KeyValuePair<string, int>> GenerateDatasourceForChart(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            switch (SelectedChartAxisX.Value)
            {
                case EnumChartAxisXType.TimeHour:
                    return GenerateDatasourceFromTimeHour(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.TimeDay:
                    return GenerateDatasourceFromTimeDay(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.TimeWeek:
                    return GenerateDatasourceFromTimeWeek(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.TimeMonth:
                    return GenerateDatasourceFromTimeMonth(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.TimeYear:
                    return GenerateDatasourceFromTimeYear(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.DayOfWeek:
                    return GenerateDatasourceFromDayOfWeek(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.TeleCode:
                    return GenerateDatasourceFromTeleCode(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.Line:
                    return GenerateDatasourceFromLine(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.ExtLine:
                    return GenerateDatasourceFromExtLine(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol1:
                    return GenerateDatasourceFromCustCol1(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol2:
                    return GenerateDatasourceFromCustCol2(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol3:
                    return GenerateDatasourceFromCustCol3(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol4:
                    return GenerateDatasourceFromCustCol4(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol5:
                    return GenerateDatasourceFromCustCol5(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol6:
                    return GenerateDatasourceFromCustCol6(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol7:
                    return GenerateDatasourceFromCustCol7(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol8:
                    return GenerateDatasourceFromCustCol8(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol9:
                    return GenerateDatasourceFromCustCol9(xpq, zCount, minDate, maxDate);
                case EnumChartAxisXType.CustCol0:
                    return GenerateDatasourceFromCustCol0(xpq, zCount, minDate, maxDate);
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromTimeHour(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var def = (int)Math.Ceiling((maxDate - minDate).TotalHours);
            var dStart = new DateTime(minDate.Year, minDate.Month, minDate.Day, minDate.Hour, 0, 0);
            for (var i = 0; i <= def; i++)
            {
                var d1 = dStart.AddHours(i);
                var d2 = dStart.AddHours(i + 1);
                var q = from n in xpq where n.CallDate >= d1 && n.CallDate < d2 select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);

                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(d1.Hour.ToString(CultureInfo.InvariantCulture), val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(d1.Hour.ToString(CultureInfo.InvariantCulture), val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromTimeDay(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var def = (int)Math.Ceiling((maxDate - minDate).TotalDays);
            var dStart = minDate.Date;
            for (var i = 0; i <= def; i++)
            {
                var d1 = dStart.AddDays(i);
                var d2 = dStart.AddDays(i + 1);
                var q = from n in xpq where n.CallDate >= d1 && n.CallDate < d2 select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var dtf = "";
                HelperUtils.DoDispatcher(() => { dtf = HelperLocalize.ApplicationDateTimeFormat; });

                var valx = HelperConvert.ConvertToPersianDigit(HelperLocalize.DateTimeToString(d1, HelperLocalize.ApplicationCalendar, dtf));
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromTimeWeek(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var def = (int)Math.Ceiling((maxDate - minDate).TotalDays);
            var dStart = minDate.Date;
            while (dStart.DayOfWeek != DayOfWeek.Saturday)
            {
                dStart = dStart.AddDays(-1);
            }

            for (var i = 0; i <= def; i = i + 7)
            {
                var d1 = dStart.AddDays(i);
                var d2 = dStart.AddDays(i + 7 + 1);
                var q = from n in xpq where n.CallDate >= d1 && n.CallDate < d2 select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var dtf = "";
                HelperUtils.DoDispatcher(() => { dtf = HelperLocalize.ApplicationDateTimeFormat; });

                var valx = HelperConvert.ConvertToPersianDigit(HelperLocalize.DateTimeToString(d1, HelperLocalize.ApplicationCalendar, dtf));
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromTimeMonth(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var def = (int)Math.Ceiling((maxDate - minDate).TotalDays);
            var dStart = minDate.Date;

            var cc = HelperLocalize.GetCalendar(HelperLocalize.ApplicationCalendar);
            {
                while (cc.GetDayOfMonth(dStart) != 1)
                {
                    dStart = dStart.AddDays(-1);
                }
            }

            var dEnd = maxDate.Date;
            {
                var y = cc.GetYear(maxDate);
                var m = cc.GetMonth(maxDate) + 1;
                if (m > 12)
                {
                    m = 1;
                    y++;
                }
                dEnd = cc.ToDateTime(y, m, 1, 1, 0, 0, 0);
            }


            while (dStart < dEnd)
            {
                var d1 = dStart;
                var y2 = cc.GetYear(dStart);
                var m2 = cc.GetMonth(dStart) + 1;
                if (m2 > 12)
                {
                    m2 = 1;
                    y2++;
                }
                var d2 = cc.ToDateTime(y2, m2, 1, 1, 0, 0, 0);
                var q = from n in xpq where n.CallDate >= d1 && n.CallDate < d2 select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var dtf = "";
                HelperUtils.DoDispatcher(() => { dtf = HelperLocalize.ApplicationDateTimeFormat; });
                dtf = dtf.Replace("d", "").Trim('\\').Trim('-').Trim('/');
                dtf = dtf.Replace("MMMM", "MM").Replace("MM", "MMMM").Replace("/", " ").Replace("-", " ");
                var valx = HelperConvert.ConvertToPersianDigit(HelperLocalize.DateTimeToString(d1, HelperLocalize.ApplicationCalendar, dtf));

                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
                dStart = d2;
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromTimeYear(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var dStart = minDate.Date;

            var cc = HelperLocalize.GetCalendar(HelperLocalize.ApplicationCalendar);
            {
                while (cc.GetDayOfMonth(dStart) != 1 && cc.GetMonth(dStart) != 1)
                {
                    dStart = dStart.AddDays(-1);
                }
            }

            var dEnd = maxDate.Date;
            {
                var y = cc.GetYear(maxDate) + 1;
                dEnd = cc.ToDateTime(y, 1, 1, 1, 0, 0, 0);
            }


            while (dStart < dEnd)
            {
                var d1 = dStart;
                var y2 = cc.GetYear(dStart) + 1;
                var d2 = cc.ToDateTime(y2, 1, 1, 1, 0, 0, 0);
                var q = from n in xpq where n.CallDate >= d1 && n.CallDate < d2 select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var dtf = "yyyy";
                var valx = HelperConvert.ConvertToPersianDigit(HelperLocalize.DateTimeToString(d1, HelperLocalize.ApplicationCalendar, dtf));

                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
                dStart = d2;
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromDayOfWeek(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            for (var i = 0; i < 7; i++)
            {
                var q = from n in xpq where n.CallDay == (DayOfWeek)i select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = HelperLocalize.GetDayName(HelperLocalize.ApplicationCalendar, (DayOfWeek)i);
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromTeleCode(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {


            var rv = new List<KeyValuePair<string, int>>();

            var qTeleCode = (from n in xpq select n.TeleCode2).Distinct().ToList();
            foreach (var code in qTeleCode)
            {
                var q = from n in xpq where n.TeleCode2 == code select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = "بدون كد";
                var ss = code.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length == 1)
                {
                    var dbc = DBGLCountry.FindByTeleCode(ADatabase.Dxs, Convert.ToInt32(ss[0]));
                    if (dbc != null)
                        valx = dbc.TitleXX;
                }
                if (ss.Length == 2)
                {
                    var dbc = DBGLCity.FindByCodes(ADatabase.Dxs, Convert.ToInt32(ss[0]), Convert.ToInt32(ss[1]));
                    if (dbc != null)
                        valx = dbc.TitleXX;
                }

                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromLine(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq select n.LineNumber).Distinct().ToList();
            foreach (var line in qLine)
            {
                var q = from n in xpq where n.LineNumber == line select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var conv = new POL.Lib.Common.ConvCallLine();
                var valx = Convert.ToString(conv.Convert(line, null, null, null));
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromExtLine(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq select n.LastExt).Distinct().ToList();
            foreach (var line in qLine)
            {
                var q = from n in xpq where n.LineNumber == line select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var conv = new POL.Lib.Common.ConvCallExtToName();
                var valx = Convert.ToString(conv.Convert(line, null, null, null));
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol1(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText1).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText1 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol2(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText2).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText2 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol3(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText3).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText3 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol4(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText4).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText4 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol5(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText5).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText5 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol6(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText6).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText6 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol7(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText7).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText7 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol8(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText8).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText8 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol9(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText9).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText9 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private List<KeyValuePair<string, int>> GenerateDatasourceFromCustCol0(XPQuery<DBCLCall> xpq, int zCount, DateTime minDate, DateTime maxDate)
        {
            var rv = new List<KeyValuePair<string, int>>();
            var qLine = (from n in xpq where n.Contact != null select n.Contact.CCText0).Distinct().ToList();
            foreach (var data in qLine)
            {
                var q = from n in xpq where n.Contact != null && n.Contact.CCText0 == data select n;
                q = GetChartFilterWithSeparation(q, SeriseDisplayName[zCount]);
                var valx = data;
                switch (SelectedChartAxisY.Value)
                {
                    case EnumChartAxisYType.CallCount:
                        {
                            var val = q.Count();
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                    case EnumChartAxisYType.CallDuration:
                        {
                            var val = q.Sum(n => n.DurationSeconds);
                            rv.Add(new KeyValuePair<string, int>(valx, val));
                        }
                        break;
                }
            }
            return rv;
        }
        private IQueryable<DBCLCall> GetChartFilterWithSeparation(IQueryable<DBCLCall> q, string zdata)
        {
            switch (SelectedChartSeparation.Value)
            {
                case EnumChartSeparation.None:
                    return q;
                case EnumChartSeparation.CallType:
                    {
                        var code = (EnumCallType)Convert.ToInt32(zdata);
                        return q.Where(n => n.CallType == code);
                    }
                case EnumChartSeparation.Contact:
                    {
                        var code = Convert.ToInt32(zdata);
                        return q.Where(n => n.Contact.Code == code);
                    }
                case EnumChartSeparation.TeleCode:
                    {
                        var code = Convert.ToDecimal(zdata);
                        return q.Where(n => n.TeleCode == code);
                    }
                case EnumChartSeparation.Line:
                    {
                        var line = Convert.ToInt32(zdata);
                        return q.Where(n => n.LineNumber == line);
                    }
                case EnumChartSeparation.ExtLine:
                    {
                        var extline = Convert.ToInt32(zdata);
                        return q.Where(n => n.LastExt == extline);
                    }
                case EnumChartSeparation.CustCol1:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText1 == cc);
                    }
                case EnumChartSeparation.CustCol2:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText2 == cc);
                    }
                case EnumChartSeparation.CustCol3:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText3 == cc);
                    }
                case EnumChartSeparation.CustCol4:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText4 == cc);
                    }
                case EnumChartSeparation.CustCol5:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText5 == cc);
                    }
                case EnumChartSeparation.CustCol6:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText6 == cc);
                    }
                case EnumChartSeparation.CustCol7:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText7 == cc);
                    }
                case EnumChartSeparation.CustCol8:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText8 == cc);
                    }
                case EnumChartSeparation.CustCol9:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText9 == cc);
                    }
                case EnumChartSeparation.CustCol0:
                    {
                        var cc = zdata;
                        return q.Where(n => n.Contact.CCText0 == cc);
                    }
            }
            return q;
        }
        private string GetSeriesDisplayName(int i)
        {
            if (SelectedChartSeparation.Value == EnumChartSeparation.CallType)
            {
                if (SeriseDisplayName[i] == "0") return "دریافتی";
                if (SeriseDisplayName[i] == "1") return "ارسالی";
            }
            if (SelectedChartSeparation.Value == EnumChartSeparation.Contact)
            {
                try
                {
                    var code = Convert.ToInt32(SeriseDisplayName[i]);
                    var dbc = DBCTContact.FindByCodeAndOrTitle(ADatabase.Dxs, code, null);
                    return dbc.Title;
                }
                catch
                {
                }
            }
            if (SelectedChartSeparation.Value == EnumChartSeparation.TeleCode)
            {
                return string.IsNullOrEmpty(SeriseDisplayName[i]) ? "بدون كد" : SeriseDisplayName[i];
            }
            if (SelectedChartSeparation.Value == EnumChartSeparation.Line)
            {
                return "خط : " + SeriseDisplayName[i];
            }
            if (SelectedChartSeparation.Value == EnumChartSeparation.ExtLine)
            {
                return "داخلی : " + SeriseDisplayName[i];
            }
            return SeriseDisplayName[i];
        }

        private List<string> SeriseDisplayName { get; set; }
        private List<List<KeyValuePair<string, int>>> ChartDataHolder { get; set; }

        private int GetChartAxisZCount(XPQuery<DBCLCall> xpq, DateTime minDate, DateTime maxDate)
        {
            SeriseDisplayName = new List<string>();
            switch (SelectedChartSeparation.Value)
            {
                case EnumChartSeparation.None:
                    SeriseDisplayName.Add(string.Format("{0} - {1}", SelectedChartAxisY.Key, SelectedChartAxisX.Key));
                    return 1;
                case EnumChartSeparation.CallType:
                    {
                        SeriseDisplayName.Add("0");
                        SeriseDisplayName.Add("1");
                        return 2;
                    }
                case EnumChartSeparation.Contact:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.Code).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.TeleCode:
                    {
                        var q = (from n in xpq
                                 select
                                    new
                                    {
                                        country = (n.Country == null ? n.City.Country.TeleCode : n.Country.TeleCode),
                                        city = (n.City == null ? 0 : n.City.PhoneCode)
                                    }).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.Format("{0}0000{1}", n.country, n.city.ToString().PadLeft(3, '0'))));
                        return q.Count();
                    }
                case EnumChartSeparation.Line:
                    {
                        var q = (from n in xpq select n.LineNumber).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.ExtLine:
                    {
                        var q = (from n in xpq select n.LastExt).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol1:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText1).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol2:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText2).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol3:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText3).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol4:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText4).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol5:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText5).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol6:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText6).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol7:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText7).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol8:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText8).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol9:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText9).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
                case EnumChartSeparation.CustCol0:
                    {
                        var q = (from n in xpq where n.Contact != null select n.Contact.CCText0).Distinct();
                        var list = q.ToList();
                        list.ForEach(n => SeriseDisplayName.Add(string.IsNullOrEmpty(n) ? "{خالی}" : n.ToString(CultureInfo.InvariantCulture)));
                        return q.Count();
                    }
            }
            return 1;
        }
        private bool CheckChartAxisXCount(POLProgressBox pb, int xCount)
        {
            switch (SelectedChartAxisX.Value)
            {
                case EnumChartAxisXType.TimeHour:
                    if (xCount > 24)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : حد اكثر بازه زمانی مجاز 24 ساعت می باشد.", pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.TimeDay:
                    if (xCount > 62)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : حداكثر بازه زمانی مجاز 62 روز می باشد.", pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.TimeWeek:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : حد اكثر بازه زمانی مجاز 64 هفته می باشد.", pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.TimeMonth:
                    if (xCount > 24)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : حداكثر بازه زمانی مجاز 24 ماه می باشد.", pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.TimeYear:
                    return true;
                case EnumChartAxisXType.DayOfWeek:
                    if (xCount > 7)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : یك هفته هفت روز دارد.", pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.TeleCode:
                    if (xCount > 128)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : حد اكثر تعداد كدهای مخابراتی مجاز 128 عدد می باشد.", pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.Line:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : حد اكثر تعداد خطوط مجاز 64 عدد می باشد.", pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.ExtLine:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError("خطا : حد اكثر تعداد خطوط داخلی مجاز 64 عدد می باشد.", pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol1:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle1), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol2:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle2), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol3:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle3), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol4:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle4), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol5:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle5), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol6:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle6), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol7:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle7), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol8:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle8), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol9:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle9), pb));
                        return false;
                    }
                    break;
                case EnumChartAxisXType.CustCol0:
                    if (xCount > 64)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(string.Format("خطا : حد اكثر تعداد {0} مجاز 64 عدد می باشد.", APOCCore.STCI.ContactCustColTitle0), pb));
                        return false;
                    }
                    break;
            }
            return true;
        }
        private int GetChartAxisXCount(XPQuery<DBCLCall> xpq, DateTime minDate, DateTime maxDate)
        {
            switch (SelectedChartAxisX.Value)
            {
                case EnumChartAxisXType.TimeHour: return (int)Math.Ceiling((maxDate - minDate).TotalHours);
                case EnumChartAxisXType.TimeDay: return (int)Math.Ceiling((maxDate - minDate).TotalDays);
                case EnumChartAxisXType.TimeWeek: return (int)Math.Ceiling((maxDate - minDate).TotalDays / 7);
                case EnumChartAxisXType.TimeMonth: return (int)Math.Ceiling((maxDate - minDate).TotalDays / 31);
                case EnumChartAxisXType.TimeYear:
                    {
                        var diy = HelperLocalize.GetCalendar(HelperLocalize.ApplicationCalendar).GetDaysInYear(maxDate.Year);
                        return (int)Math.Ceiling((maxDate - minDate).TotalDays / diy);
                    }
                case EnumChartAxisXType.DayOfWeek: return 7;
                case EnumChartAxisXType.TeleCode: return (from n in xpq select n.TeleCode2).Distinct().Count();
                case EnumChartAxisXType.Line: return (from n in xpq select n.LineNumber).Distinct().Count();
                case EnumChartAxisXType.ExtLine: return (from n in xpq select n.LastExt).Distinct().Count();
                case EnumChartAxisXType.CustCol1: return (from n in xpq select n.Contact.CCText1).Distinct().Count();
                case EnumChartAxisXType.CustCol2: return (from n in xpq select n.Contact.CCText2).Distinct().Count();
                case EnumChartAxisXType.CustCol3: return (from n in xpq select n.Contact.CCText3).Distinct().Count();
                case EnumChartAxisXType.CustCol4: return (from n in xpq select n.Contact.CCText4).Distinct().Count();
                case EnumChartAxisXType.CustCol5: return (from n in xpq select n.Contact.CCText5).Distinct().Count();
                case EnumChartAxisXType.CustCol6: return (from n in xpq select n.Contact.CCText6).Distinct().Count();
                case EnumChartAxisXType.CustCol7: return (from n in xpq select n.Contact.CCText7).Distinct().Count();
                case EnumChartAxisXType.CustCol8: return (from n in xpq select n.Contact.CCText8).Distinct().Count();
                case EnumChartAxisXType.CustCol9: return (from n in xpq select n.Contact.CCText9).Distinct().Count();
                case EnumChartAxisXType.CustCol0: return (from n in xpq select n.Contact.CCText0).Distinct().Count();
            }
            return 1000;
        }








        private void PrintChartBar()
        {
            if (SelectedCallReport == null) return;
            HelperUtils.Try(() =>
            {
                SelectedCallReport.Save();
            });

            MainView.BeginPrepareChart();

            var oldwidth = DynamicChartControl.Width;
            var oldhight = DynamicChartControl.Height;

            var ratio = 2.8;
            var mL = SelectedCallReport.MarginLeft;
            var mR = SelectedCallReport.MarginRight;
            var mT = SelectedCallReport.MarginTop;
            var mB = SelectedCallReport.MarginBottom;

            var landscape = SelectedCallReport.IsLandscape;

            var p = HelperPrinter.GetPaperKindDimensions(SelectedCallReport.Paper);
            var w = (p.X / ratio) - mL - mR;
            var h = (p.Y / ratio) - mT - mB;
            DynamicChartControl.Height = landscape ? w : h;
            DynamicChartControl.Width = landscape ? h : w;

            var sl = new SimpleLink
            {
                DetailCount = 1,
                DetailTemplate = (DataTemplate)MainView.Resources["ChartTemplate"]
            };
            sl.CreateDetail +=
                (s, e) =>
                {
                    var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
                    var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

                    var dpiX = (int)dpiXProperty.GetValue(null, null);
                    var dpiY = (int)dpiYProperty.GetValue(null, null);

                    var brush = new VisualBrush(DynamicChartControl);
                    var visual = new DrawingVisual();
                    var context = visual.RenderOpen();

                    context.DrawRectangle(brush, null,
                        new Rect(0, 0, DynamicChartControl.ActualWidth, DynamicChartControl.ActualHeight));
                    context.Close();

                    var bmp = new RenderTargetBitmap(
                        (int)DynamicChartControl.ActualWidth + 1,
                        (int)DynamicChartControl.ActualHeight + 1,
                        dpiX, dpiY, PixelFormats.Pbgra32);
                    bmp.Render(visual);

                    e.Data = HelperLocalize.ApplicationFlowDirection == FlowDirection.RightToLeft ? HelperImage.RTLTransform(bmp, -1, bmp.Width) : bmp;
                };
            sl.CreateDocument(true);
            sl.PaperKind = SelectedCallReport.Paper;
            sl.Landscape = landscape;
            sl.Margins = new Margins((int)mL, (int)mR, (int)mT, (int)mB);



            var preview = new DocumentPreview { Model = new LinkPreviewModel(sl) };
            var v = (DataTemplate)ActiveView.FindResource("toolbarCustomization");
            var barManagerCustomizer = new TemplatedBarManagerController { Template = v };
            preview.BarManager.Controllers.Add(barManagerCustomizer);
            preview.FlowDirection = FlowDirection.LeftToRight;
            preview.FontFamily = new FontFamily("Tahoma");
            preview.FontSize = 12.0;

            var previewWindow = new DocumentPreviewWindow
            {
                Owner = APOCMainWindow.GetWindow(),
                Content = preview,
                FlowDirection = HelperLocalize.ApplicationFlowDirection,
                FontFamily = new FontFamily(HelperLocalize.ApplicationFontName),
                FontSize = HelperLocalize.ApplicationFontSize,
                Title = "پیش نمایش",
            };


            sl.CreateDocument(true);
            previewWindow.ShowDialog();






            DynamicChartControl.Width = oldwidth;
            DynamicChartControl.Height = oldhight;

            MainView.EndPrepareChart();
        }
        private void Print1()
        {
            try
            {
                var report = new POC.Module.Call.Reports.XRCallList();
                report.Name = "ReportCallList";
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "POC.Module.Call.Resources.ReportCallSimple.repx";
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                report.LoadLayout(stream);
                report.DataSource = DynamicGrid.ItemsSource;
                var printTool = new DevExpress.XtraReports.UI.ReportPrintTool(report);
                printTool.ShowPreviewDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Print2()
        {
            try
            {
                var report = new POC.Module.Call.Reports.XRCallList();
                report.Name = "ReportCallNote";
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "POC.Module.Call.Resources.ReportCallNote.repx";
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                report.LoadLayout(stream);
                report.DataSource = DynamicGrid.ItemsSource;
                var printTool = new DevExpress.XtraReports.UI.ReportPrintTool(report);
                printTool.ShowPreviewDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Print3()
        {

            try
            {
                var od = new Microsoft.Win32.OpenFileDialog();
                od.CheckFileExists = true;
                od.DefaultExt = "*.repx";
                var dr = od.ShowDialog();
                if (dr == true)
                {
                    var report = new POC.Module.Call.Reports.XRCallList();
                    report.LoadLayout(od.FileName);
                    report.DataSource = DynamicGrid.ItemsSource;
                    var printTool = new DevExpress.XtraReports.UI.ReportPrintTool(report);
                    printTool.ShowPreviewDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }






        }
        private void Print4()
        {
            var criteria = new GroupOperator();
            if (!ReferenceEquals(DynamicGrid.FilterCriteria, null))
                criteria.Operands.Add(DynamicGrid.FilterCriteria);
            if (!ReferenceEquals(CallList.FixedFilterCriteria, null))
                criteria.Operands.Add(CallList.FixedFilterCriteria);
            DBCLCall.FixGroupOperatorToLINQForCall(criteria);
            var criteria2 = DBCLCall.RemoveEmptyOperand(criteria);

            var xpq = new XPQuery<DBCLCall>(ADatabase.Dxs);
            var xpq2 = xpq.AppendWhere(new CriteriaToExpressionConverter(), criteria2) as XPQuery<DBCLCall>;
            var count = xpq2.Count();
            if (count == 0) return;
            if (count >= 64 * 1024)
            {
                POLMessageBox.ShowError("حداكثر ركورد های قابل استخراج 65 هزارتا می باشد.", Window.GetWindow(DynamicGrid));
                return;
            }

            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xls",
                Filter = "Microsoft Excel 2003 files (*.xls)|*.xls",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "ExportCall.xls"
            };
            if (sf.ShowDialog() != true)
                return;

            var filename = sf.FileName;
            var eFile = new ExcelFile
            {
                DefaultFontName = HelperLocalize.ApplicationFontName,
                DefaultFontSize = (int)HelperLocalize.ApplicationFontSize
            };

            var ws = eFile.Worksheets.Add("تماس ها");
            ws.ViewOptions.ShowColumnsFromRightToLeft = HelperLocalize.ApplicationFlowDirection == FlowDirection.RightToLeft;

            var row = 0;
            #region Columns Title
            var colIndex = 0;
            foreach (var col in DynamicGrid.Columns.OrderBy(n => n.VisibleIndex))
            {
                if (col.Visible)
                {
                    ws.Cells[row, colIndex].Value = col.HeaderCaption;
                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                    colIndex++;
                }
            }
            #endregion

            var colfields = (from n in DynamicGrid.Columns where n.Visible orderby n.VisibleIndex select n.FieldName).ToList();
            var colOrders = (from n in DynamicGrid.Columns where n.Visible && n.SortOrder != ColumnSortOrder.None orderby n.SortIndex select new { n.FieldName, n.SortOrder }).ToList();
            row++;

            POLProgressBox.Show("استخراج اطلاعات تماس ها", true, 0, count, 2,
                pw =>
                {
                    var convCalldate = new POL.Lib.Common.ConvCallDate();
                    var convCallDuration = new POL.Lib.Common.ConvCallDuration();
                    var convCallLine = new POL.Lib.Common.ConvCallLine();
                    var convCallExtToName = new POL.Lib.Common.ConvCallExtToName();
                    var q = from n in xpq2 select n;
                    var oCount = 0;
                    foreach (var colOrder in colOrders)
                    {
                        #region Order First Time

                        if (colOrder.FieldName == "CallType")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.CallType)
                                : q.OrderByDescending(n => n.CallType);
                            oCount++;
                        }
                        if (colOrder.FieldName == "Contact.Code")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Contact.Code)
                                : q.OrderByDescending(n => n.Contact.Code);
                            oCount++;
                        }
                        if (colOrder.FieldName == "Contact.Title")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Contact.Title)
                                : q.OrderByDescending(n => n.Contact.Title);
                            oCount++;
                        }
                        if (colOrder.FieldName == "Contact.Cats")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Contact.Title)
                                : q.OrderByDescending(n => n.Contact.Title);
                            oCount++;
                        }
                        if (colOrder.FieldName == "CallDate")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.CallDate)
                                : q.OrderByDescending(n => n.CallDate);
                            oCount++;
                        }
                        if (colOrder.FieldName == "PhoneNumber")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.PhoneNumber)
                                : q.OrderByDescending(n => n.PhoneNumber);
                            oCount++;
                        }
                        if (colOrder.FieldName == "Phone.Titler")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Phone.Title)
                                : q.OrderByDescending(n => n.Phone.Title);
                            oCount++;
                        }
                        if (colOrder.FieldName == "Phone.Note")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Phone.Note)
                                : q.OrderByDescending(n => n.Phone.Note);
                            oCount++;
                        }
                        if (colOrder.FieldName == "TeleCode2")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.TeleCode2)
                                : q.OrderByDescending(n => n.TeleCode2);
                            oCount++;
                        }
                        if (colOrder.FieldName == "ExtraDialed")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.ExtraDialed)
                                : q.OrderByDescending(n => n.ExtraDialed);
                            oCount++;
                        }
                        if (colOrder.FieldName == "DurationSeconds")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.DurationSeconds)
                                : q.OrderByDescending(n => n.DurationSeconds);
                            oCount++;
                        }
                        if (colOrder.FieldName == "LineNumber")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.LineNumber)
                                : q.OrderByDescending(n => n.LineNumber);
                            oCount++;
                        }
                        if (colOrder.FieldName == "InternalString")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.InternalString)
                                : q.OrderByDescending(n => n.InternalString);
                            oCount++;
                        }
                        if (colOrder.FieldName == "LastExt")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.LastExt)
                                : q.OrderByDescending(n => n.LastExt);
                            oCount++;
                        }
                        if (colOrder.FieldName == "RecordEnable")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.RecordEnable)
                                : q.OrderByDescending(n => n.RecordEnable);
                            oCount++;
                        }
                        if (colOrder.FieldName == "RecordRole")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.RecordRole)
                                : q.OrderByDescending(n => n.RecordRole);
                            oCount++;
                        }
                        if (colOrder.FieldName == "NoteText")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.NoteText)
                                : q.OrderByDescending(n => n.NoteText);
                            oCount++;
                        }
                        if (colOrder.FieldName == "NoteFlag")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.NoteFlag)
                                : q.OrderByDescending(n => n.NoteFlag);
                            oCount++;
                        }

                        #endregion
                    }


                    foreach (var db in q)
                    {

                        colIndex = 0;
                        foreach (var field in colfields)
                        {
                            try
                            {
                                #region CallType
                                if (field == "CallType")
                                {
                                    ws.Cells[row, colIndex].Value = db.CallType == EnumCallType.CallIn ? "دریافتی" : "ارسالی";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region CallDate
                                if (field == "CallDate")
                                {
                                    ws.Cells[row, colIndex].Value = convCalldate.Convert(db.CallDate, null, null, null);
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.Code
                                if (field == "Contact.Code")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.Code.ToString() : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.Title
                                if (field == "Contact.Title")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.Title : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.Cats
                                if (field == "Contact.Cats")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.Cats : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region PhoneNumber
                                if (field == "PhoneNumber")
                                {
                                    ws.Cells[row, colIndex].Value = db.PhoneNumber;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Phone.Title
                                if (field == "Phone.Title")
                                {
                                    ws.Cells[row, colIndex].Value = db.Phone.Title;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Phone.Note
                                if (field == "Phone.Note")
                                {
                                    ws.Cells[row, colIndex].Value = db.Phone.Note;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region TeleCode2
                                if (field == "TeleCode2")
                                {
                                    ws.Cells[row, colIndex].Value = db.TeleCode2;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region ExtraDialed
                                if (field == "ExtraDialed")
                                {
                                    ws.Cells[row, colIndex].Value = db.ExtraDialed;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region DurationSeconds
                                if (field == "DurationSeconds")
                                {
                                    ws.Cells[row, colIndex].Value = convCallDuration.Convert(db.DurationSeconds, null, null, null);
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region LineNumber
                                if (field == "LineNumber")
                                {
                                    ws.Cells[row, colIndex].Value = convCallLine.Convert(db.LineNumber, null, null, null);
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region InternalString
                                if (field == "InternalString")
                                {
                                    ws.Cells[row, colIndex].Value = db.InternalString;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region LastExt
                                if (field == "LastExt")
                                {
                                    ws.Cells[row, colIndex].Value = convCallExtToName.Convert(db.LastExt, null, null, null);
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region RecordEnable
                                if (field == "RecordEnable")
                                {
                                    ws.Cells[row, colIndex].Value = db.RecordEnable ? "دارد" : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region RecordRole
                                if (field == "RecordRole")
                                {
                                    ws.Cells[row, colIndex].Value = ConvertRecordRoleToString(db.RecordRole);
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region NoteText
                                if (field == "NoteText")
                                {
                                    ws.Cells[row, colIndex].Value = db.NoteText;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region NoteFlag
                                if (field == "NoteFlag")
                                {
                                    ws.Cells[row, colIndex].Value = ConvertNoteFlagToString(db.NoteFlag);
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region NoteWriter
                                if (field == "NoteWriter")
                                {
                                    ws.Cells[row, colIndex].Value = db.NoteWriter;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion

                                #region Contact.CCText1
                                if (field == "Contact.CCText1")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText1 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText2
                                if (field == "Contact.CCText2")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText2 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText3
                                if (field == "Contact.CCText3")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText3 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText4
                                if (field == "Contact.CCText4")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText4 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText5
                                if (field == "Contact.CCText5")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText5 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText6
                                if (field == "Contact.CCText6")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText6 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText7
                                if (field == "Contact.CCText7")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText7 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText8
                                if (field == "Contact.CCText8")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText8 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText9
                                if (field == "Contact.CCText9")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText9 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Contact.CCText0
                                if (field == "Contact.CCText0")
                                {
                                    ws.Cells[row, colIndex].Value = db.Contact != null ? db.Contact.CCText0 : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                ws.Cells[row, colIndex].Value = ex.Message;
                                ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                colIndex++;
                            }
                        }

                        pw.AsyncSetText(1, db.PhoneNumber ?? string.Empty);
                        pw.AsyncSetValue(row);
                        row++;

                        if (pw.NeedToCancel)
                        {
                            return;
                        }
                    }
                },
                pw =>
                {
                    eFile.SaveXls(filename);
                    TryToDisplayGeneratedFile(filename);
                }, Window.GetWindow(DynamicGrid));
        }
        private void Print5()
        {
            var wr5 = new WReport5()
            {
                Owner = Window.GetWindow(MainView),
            };
            wr5.Show();
        }
        private void Print6()
        {
            var wr6 = new WReport6()
            {
                Owner = Window.GetWindow(MainView),
            };
            wr6.Show();
        }

        private string ConvertNoteFlagToString(int p)
        {
            switch (p)
            {
                case 1:
                    return "خاكستری";

                case 2:
                    return "زرد";

                case 3:
                    return "قرمز";
                case 4:
                    return "سبز";
                case 5:
                    return "آبی";
            }
            return string.Empty;
        }
        private string ConvertRecordRoleToString(int p)
        {
            switch (p)
            {
                case 1:
                    return "سفید";

                case 2:
                    return "زرد";

                case 3:
                    return "قرمز";
            }
            return string.Empty;
        }
        private void TryToDisplayGeneratedFile(string fileName)
        {
            try
            {
                var p = System.Diagnostics.Process.Start(fileName);
                if (p != null)
                    SetForegroundWindow(p.MainWindowHandle);
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message);
            }
        }



        private void VoicePlay()
        {
            var serverport = HelperSettingsClient.ServerPort;
            var servername = HelperSettingsClient.ServerName;

            var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                            servername,
                                                            Convert.ToInt32(serverport) + ConstantGeneral.ProtocolVoiceRecordPortOffset,
                                                            ConstantGeneral.ProtocolVoiceRecordServiceName));
            var binding = new NetTcpBinding
            {
                TransferMode = TransferMode.Streamed,
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                MaxReceivedMessageSize = long.MaxValue,
                Security = { Mode = SecurityMode.None }
            };

            var channel = new ChannelFactory<POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord>(binding, address);

            var proxy = channel.CreateChannel();

            var fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".wav";
            POLProgressBox.Show("ذخیره صدا", false, 0, 100, 1,
                pb =>
                {
                    try
                    {
                        var v2 = proxy.DownloadFile(new POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter { RecordTag = FocusedCall.RecordTag });
                        using (Stream file = File.Create(fileName))
                        {
                            CopyStream(v2.FileByteStream, file, v2.Length, pb);
                        }
                    }
                    catch (Exception ex)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(ex.Message, pb));
                    }
                },
                pb =>
                {
                    var fi = new FileInfo(fileName);
                    if (fi.Exists)
                    {
                        System.Diagnostics.Process.Start(fileName);
                    }
                }, APOCMainWindow.GetWindow());
        }
        private void VoiceDelete()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات صوتی برای تماس های انتخاب شده حذف شود؟", APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;

            var serverport = HelperSettingsClient.ServerPort;
            var servername = HelperSettingsClient.ServerName;

            var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                            servername,
                                                            Convert.ToInt32(serverport) + ConstantGeneral.ProtocolVoiceRecordPortOffset,
                                                            ConstantGeneral.ProtocolVoiceRecordServiceName));
            var binding = new NetTcpBinding
            {
                TransferMode = TransferMode.Streamed,
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                MaxReceivedMessageSize = long.MaxValue,
                Security = { Mode = SecurityMode.None }
            };

            var channel = new ChannelFactory<POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord>(binding, address);

            var proxy = channel.CreateChannel();

            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("حذف", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCLCall> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCLCall).ToList();
                        }));

                    w.AsyncSetText(1, "در حال حذف");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.PhoneNumber);


                            if (!v.RecordEnable && v.RecordRole == 1 && string.IsNullOrEmpty(v.RecordTag)) continue;

                            var v2 = proxy.DeleteMapItem(new POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter { RecordTag = v.RecordTag });
                            if (v2.Succeed)
                                successCount++;
                            else
                                failedCount++;
                        }
                        catch
                        {
                            failedCount++;
                        }
                        w.AsyncSetText(3, string.Format("موفقیت : {0}  - خطا : {1}", successCount, failedCount));
                    }
                },
                w =>
                {
                    POLMessageBox.ShowInformation(String.Format("تعداد {0} مكالمه با موفقیت حذف شد.{1}تعداد خطا ها : {2}", successCount, Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, APOCMainWindow.GetWindow());
        }
        private void VoiceSave()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK) return;
            var path = dialog.SelectedPath;

            var serverport = HelperSettingsClient.ServerPort;
            var servername = HelperSettingsClient.ServerName;

            var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                            servername,
                                                            Convert.ToInt32(serverport) + ConstantGeneral.ProtocolVoiceRecordPortOffset,
                                                            ConstantGeneral.ProtocolVoiceRecordServiceName));
            var binding = new NetTcpBinding
            {
                TransferMode = TransferMode.Streamed,
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                MaxReceivedMessageSize = long.MaxValue,
                Security = { Mode = SecurityMode.None }
            };

            var channel = new ChannelFactory<POC.Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord>(binding, address);

            var proxy = channel.CreateChannel();

            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("ذخیره", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCLCall> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCLCall).ToList();
                        }));

                    w.AsyncSetText(1, "در حال ذخیره");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.PhoneNumber);
                            if (!v.RecordEnable) continue;
                            var fn = string.Format("{0}\\{1}-{2}-{3}.wav",
                                path,
                                HelperLocalize.DateTimeToString(v.CallDate.Value, HelperLocalize.ApplicationCalendar, "yyMMdd"),
                                string.Format("{0:HHmmss}", v.CallDate.Value),
                                v.LineNumber);

                            var v2 = proxy.DownloadFile(new POC.Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter { RecordTag = v.RecordTag });
                            using (Stream file = File.Create(fn))
                            {
                                CopyStream(v2.FileByteStream, file, v2.Length, w);
                            }
                            successCount++;
                        }
                        catch
                        {
                            failedCount++;
                        }
                        w.AsyncSetText(3, string.Format("موفقیت : {0}  - خطا : {1}", successCount, failedCount));
                    }
                },
                w =>
                {
                    POLMessageBox.ShowInformation(String.Format("تعداد {0} مكالمه با موفقیت ذخیره شد.{1}تعداد خطا ها : {2}", successCount, Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                    HelperUtils.Try(() => System.Diagnostics.Process.Start(path));
                }, APOCMainWindow.GetWindow());
        }
        private void VoiceRole1()
        {
            SetVoiceRole(1);
        }
        private void VoiceRole2()
        {
            SetVoiceRole(2);
        }
        private void VoiceRole3()
        {
            SetVoiceRole(3);
        }

        private void SetVoiceRole(int role)
        {
            if (SelectedRowCount <= 0) return;
            if (SelectedRowCount > 1)
            {
                var dr = POLMessageBox.ShowQuestionYesNo(string.Format("تعداد {0} تماس از قانون شماره 3 پیروی كنند؟", SelectedRowCount),
                                                         APOCMainWindow.GetWindow());
                if (dr != MessageBoxResult.Yes) return;
            }
            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("تغییر قانون حذف", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCLCall> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCLCall).ToList();
                        }));

                    w.AsyncSetText(1, "در حال تغییر");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.PhoneNumber);
                            if (!v.RecordEnable) continue;
                            if (v.RecordRole == role) continue;
                            v.RecordRole = role;
                            v.Save();
                            successCount++;
                        }
                        catch
                        {
                            failedCount++;
                        }
                        w.AsyncSetText(3, string.Format("موفقیت : {0}  - خطا : {1}", successCount, failedCount));
                    }
                },
                w =>
                {
                    if (SelectedRowCount == 1) return;
                    POLMessageBox.ShowInformation(
                        String.Format("تعداد {0} تماس با موفقیت تغییر كرد.{1}تعداد خطا ها : {2}", successCount,
                                      Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, APOCMainWindow.GetWindow());
        }
        private void CopyStream(Stream input, System.IO.Stream output, long size, POLProgressBox pb)
        {
            long current = 0;
            var buffer = new byte[10 * 1024];
            int len;
            var lastp = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
                current += len;
                var p = (int)((current * 100) / size);
                if (p == lastp) continue;
                pb.AsyncSetValue(p);
                lastp = p;
            }
        }


        private void NextDay()
        {
            SelectedDate = SelectedDate.AddDays(1);
        }

        private void PrevDay()
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }


        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp22);
        }

        private void PopulateContactCat()
        {
            if (!AMembership.IsAuthorized) return;
            var xpc = (from n in ACacheData.GetContactCatList() let cat = n.Tag as DBCTContactCat orderby cat.Title select cat).ToList();
            var dbc = SelectedContactCat as DBCTContactCat;
            ContactCatList = new List<object> { "(همه دسته ها)" };
            if (AMembership.ActiveUser.UserName.ToLower() == "admin")
            {
                xpc.ToList().ForEach(n => ContactCatList.Add(n));
            }
            else
            {
                xpc.ToList().Where(n => n.Role != null && AMembership.ActiveUser.Roles.Select(r => r.ToLower()).Contains(n.Role.ToLower())).ToList().ForEach(n => ContactCatList.Add(n));
            }

            SelectedContactCat = dbc != null ? ContactCatList.FirstOrDefault(n => (n is DBCTContactCat) && ((DBCTContactCat)n).Title == dbc.Title) : ContactCatList.FirstOrDefault();
        }
        private void CategoryRefresh()
        {
            PopulateContactCat();
        }
        #endregion





        #region IDisposable
        public void Dispose()
        {
            APOCContactModule.OnSelectedDateChanged -= APOCContactModule_OnSelectedDateChanged;
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandFilterByDate { get; set; }
        public RelayCommand CommandShowAll { get; set; }
        public RelayCommand CommandShowCallOut { get; set; }
        public RelayCommand CommandShowCallIn { get; set; }
        public RelayCommand CommandShowMissCall { get; set; }
        public RelayCommand CommandFilterByFilter { get; set; }

        public RelayCommand CommandCallDeleteSingle { get; set; }
        public RelayCommand CommandCallDeleteAll { get; set; }
        public RelayCommand CommandCallCorrect { get; set; }

        public RelayCommand CommandSyncSingle { get; set; }
        public RelayCommand CommandSyncAll { get; set; }
        public RelayCommand CommandGotoContact { get; set; }

        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandSetCallNote { get; set; }

        public RelayCommand CommandApplyCallFilter { get; set; }
        public RelayCommand CommandSaveCallFilter { get; set; }

        public RelayCommand CommandCallFilterClear { get; set; }
        public RelayCommand CommandCallFilterRefresh { get; set; }
        public RelayCommand CommandCallFilterDelete { get; set; }

        public RelayCommand CommandCallReportAdd { get; set; }
        public RelayCommand CommandCallReportEdit { get; set; }
        public RelayCommand CommandCallReportDelete { get; set; }
        public RelayCommand CommandCallReportRefresh { get; set; }
        public RelayCommand CommandCallReportRender { get; set; }

        public RelayCommand CommandPrintChart { get; set; }
        public RelayCommand CommandPrint1 { get; set; }
        public RelayCommand CommandPrint2 { get; set; }
        public RelayCommand CommandPrint3 { get; set; }
        public RelayCommand CommandPrint4 { get; set; }
        public RelayCommand CommandPrint5 { get; set; }
        public RelayCommand CommandPrint6 { get; set; }

        public RelayCommand CommandVoicePlay { get; set; }
        public RelayCommand CommandVoiceDelete { get; set; }
        public RelayCommand CommandVoiceSave { get; set; }
        public RelayCommand CommandVoiceRole1 { get; set; }
        public RelayCommand CommandVoiceRole2 { get; set; }
        public RelayCommand CommandVoiceRole3 { get; set; }

        public RelayCommand CommandNextDay { get; set; }
        public RelayCommand CommandPrevDay { get; set; }

        public RelayCommand CommandCategoryRefresh { get; set; }

        public RelayCommand CommandQuickSearchClear { get; set; }

        public RelayCommand CommandClearFilterByDay { get; set; }

        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region [DllImport]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion
    }
}
