using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.ABSettings.Models
{
    public class MApplicationBarSettings : IDisposable, INotifyPropertyChanged
    {
        public MApplicationBarSettings(object mainView)
        {
            APOCSettings = ServiceLocator.Current.GetInstance<IPOCSettings>();

            PopulateLanguageList();
            PopulateDirectionList();
            PopulateFontFamilyList();
            PopulateFontSizeList();
            PopulateKeyboardLayoutList();
            PopulateCalendarList();
            PopulateDateFormatList();
            PopulateAutoStartDelayList();
            PopulateDockWidthList();

            InitCommands();
        }

        #region [METHODS]
        private void InitCommands()
        {
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp03 != "");
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp03);
        }
        #endregion

        private IPOCSettings APOCSettings { get; set; }


        #region ServerName
        public string ServerName
        {
            get { return HelperSettingsClient.ServerName; }
            set
            {
                HelperSettingsClient.ServerName = value;
                OnPropertyChanged("ServerName");
            }
        }
        #endregion
        #region ServerPort
        public string ServerPort
        {
            get { return HelperSettingsClient.ServerPort; }
            set
            {
                HelperSettingsClient.ServerPort = value;
                OnPropertyChanged("ServerPort");
            }
        }
        #endregion
        #region UseMSSQLServer2
        public bool UseMSSQLServer2
        {
            get { return HelperSettingsClient.UseMSSQLServer2; }
            set
            {
                OnPropertyChanged("UseMSSQLServer2");
                HelperSettingsClient.UseMSSQLServer2 = value;
            }
        }
        #endregion


        #region LanguageList
        private List<CultureInfo> _LanguageList;
        public List<CultureInfo> LanguageList
        {
            get
            {
                return _LanguageList;
            }
            set
            {
                if (value == _LanguageList) return;
                _LanguageList = value;
                OnPropertyChanged("LanguageList");
            }
        }
        #endregion
        #region LanguageSelected
        public CultureInfo LanguageSelected
        {
            get
            {
                var lcid = HelperSettingsClient.LanguageLCID;
                if (LanguageList == null) return null;
                var q = from n in LanguageList where n.LCID == lcid select n;
                return !q.Any() ? null : q.First();
            }
            set
            {
                if (value != null)
                    HelperSettingsClient.LanguageLCID = value.LCID;
                OnPropertyChanged("LanguageSelected");
            }
        }
        #endregion

        #region DirectionList
        private List<string> _DirectionList;
        public List<string> DirectionList
        {
            get
            {
                return _DirectionList;
            }
            set
            {
                if (value == _DirectionList) return;
                _DirectionList = value;
                OnPropertyChanged("DirectionList");
            }
        }
        #endregion
        #region DirectionSelected
        public int DirectionSelected
        {
            get
            {
                var dir = HelperSettingsClient.ApplicationDirection;
                return dir == "RightToLeft" ? 0 : 1;
            }
            set
            {
                HelperSettingsClient.ApplicationDirection = (value == 0 ? "RightToLeft" : "LeftToRight");
                OnPropertyChanged("DirectionSelected");
            }
        }
        #endregion

        #region FontFamilyList
        private List<string> _FontFamilyList;
        public List<string> FontFamilyList
        {
            get
            {
                return _FontFamilyList;
            }
            set
            {
                if (value == _FontFamilyList) return;
                _FontFamilyList = value;
                OnPropertyChanged("FontFamilyList");
            }
        }
        #endregion
        #region FontFamilySelected
        public string FontFamilySelected
        {
            get
            {
                return HelperSettingsClient.ApplicationFontFamily;
            }
            set
            {
                if (value != null)
                    HelperSettingsClient.ApplicationFontFamily = value;
                OnPropertyChanged("FontFamilySelected");
            }
        }
        #endregion

        #region FontSizeList
        private List<int> _FontSizeList;
        public List<int> FontSizeList
        {
            get
            {
                return _FontSizeList;
            }
            set
            {
                if (value == _FontSizeList) return;
                _FontSizeList = value;
                OnPropertyChanged("FontSizeList");
            }
        }
        #endregion
        #region FontSizeSelected
        public int FontSizeSelected
        {
            get
            {
                return HelperSettingsClient.ApplicationFontSize;
            }
            set
            {
                HelperSettingsClient.ApplicationFontSize = value;
                OnPropertyChanged("FontSizeSelected");
            }
        }
        #endregion

        #region KeyboardLayoutList
        private List<System.Windows.Forms.InputLanguage> _KeyboardLayoutList;
        public List<System.Windows.Forms.InputLanguage> KeyboardLayoutList
        {
            get
            {
                return _KeyboardLayoutList;
            }
            set
            {
                if (value == _KeyboardLayoutList) return;
                _KeyboardLayoutList = value;
                OnPropertyChanged("KeyboardLayoutList");
            }
        }
        #endregion
        #region KBLayoutDefault
        public System.Windows.Forms.InputLanguage KBLayoutDefault
        {
            get
            {
                var kb = HelperSettingsClient.ApplicationKBLayoutDefault;
                if (KeyboardLayoutList == null) return null;
                var q = from n in KeyboardLayoutList where n.LayoutName == kb select n;
                return !q.Any() ? null : q.First();
            }
            set
            {
                if (value != null)
                    HelperSettingsClient.ApplicationKBLayoutDefault = value.LayoutName;
                OnPropertyChanged("KBLayoutDefault");
            }
        }
        #endregion
        #region KBLayoutRTL
        public System.Windows.Forms.InputLanguage KBLayoutRTL
        {
            get
            {
                var kb = HelperSettingsClient.ApplicationKBLayoutRTL;
                if (KeyboardLayoutList == null) return null;
                var q = from n in KeyboardLayoutList where n.LayoutName == kb select n;
                return !q.Any() ? null : q.First();
            }
            set
            {
                if (value != null)
                    HelperSettingsClient.ApplicationKBLayoutRTL = value.LayoutName;
                OnPropertyChanged("KBLayoutRTL");
            }
        }
        #endregion
        #region KBLayoutLTR
        public System.Windows.Forms.InputLanguage KBLayoutLTR
        {
            get
            {
                var kb = HelperSettingsClient.ApplicationKBLayoutLTR;
                if (KeyboardLayoutList == null) return null;
                var q = from n in KeyboardLayoutList where n.LayoutName == kb select n;
                return !q.Any() ? null : q.First();
            }
            set
            {
                if (value != null)
                    HelperSettingsClient.ApplicationKBLayoutLTR = value.LayoutName;
                OnPropertyChanged("KBLayoutLTR");
            }
        }
        #endregion

        #region CalendarList
        private List<string> _CalendarList;
        public List<string> CalendarList
        {
            get
            {
                return _CalendarList;
            }
            set
            {
                if (value == _CalendarList) return;
                _CalendarList = value;
                OnPropertyChanged("CalendarList");
            }
        }
        #endregion
        #region CalendarSelected
        public int CalendarSelected
        {
            get
            {
                return HelperSettingsClient.ApplicationCalendar;
            }
            set
            {
                HelperSettingsClient.ApplicationCalendar = value;
                OnPropertyChanged("CalendarSelected");
            }
        }
        #endregion
        #region HijriOffset
        public int HijriOffset
        {
            get
            {
                return HelperSettingsClient.HijriCalendarOffset;
            }
            set
            {
                HelperSettingsClient.HijriCalendarOffset = value;
                OnPropertyChanged("HijriOffset");
            }
        }
        #endregion
        #region HijriOffsetList
        public List<int> HijriOffsetList
        {
            get { return Enumerable.Range(-11, 23).ToList(); }
        }
        #endregion


        #region DateFormatList
        private List<string> _DateFormatList;
        public List<string> DateFormatList
        {
            get
            {
                return _DateFormatList;
            }
            set
            {
                if (value == _DateFormatList) return;
                _DateFormatList = value;
                OnPropertyChanged("DateFormatList");
            }
        }
        #endregion
        #region DateFormatSelected
        public string DateFormatSelected
        {
            get
            {
                return HelperSettingsClient.ApplicationDateFormat;
            }
            set
            {
                HelperSettingsClient.ApplicationDateFormat = value;
                OnPropertyChanged("DateFormatSelected");
            }
        }
        #endregion

        #region AutoStartEnable
        public bool AutoStartEnable
        {
            get { return HelperAutoStart.AutoStartStatus("P30Office Client"); }
            set
            {
                OnPropertyChanged("AutoStartEnable");
                HelperAutoStart.AutoStartApplication(value, "P30Office Client", System.Reflection.Assembly.GetEntryAssembly().Location);
            }
        }
        #endregion
        #region AutoStartDelayList
        private List<int> _AutoStartDelayList;
        public List<int> AutoStartDelayList
        {
            get
            {
                return _AutoStartDelayList;
            }
            set
            {
                if (value == _AutoStartDelayList) return;
                _AutoStartDelayList = value;
                OnPropertyChanged("AutoStartDelayList");
            }
        }
        #endregion
        #region AutoStartDelaySelected
        public int AutoStartDelaySelected
        {
            get { return HelperSettingsClient.AutoStartDelay; }
            set
            {
                HelperSettingsClient.AutoStartDelay = value;
                OnPropertyChanged("AutoStartDelaySelected");
            }
        }
        #endregion

        #region DockIsFixed
        public bool DockIsFixed
        {
            get { return HelperSettingsClient.DockIsFixed; }
            set
            {
                HelperSettingsClient.DockIsFixed = value;
                OnPropertyChanged("DockIsFixed");
            }
        }
        #endregion
        #region DockWidthList
        private List<int> _DockWidthList;
        public List<int> DockWidthList
        {
            get
            {
                return _DockWidthList;
            }
            set
            {
                if (value == _DockWidthList) return;
                _DockWidthList = value;
                OnPropertyChanged("DockWidthList");
            }
        }
        #endregion
        #region DockWidthSelected
        public int DockWidthSelected
        {
            get { return HelperSettingsClient.DockWidth; }
            set
            {
                HelperSettingsClient.DockWidth = value;
                OnPropertyChanged("DockWidthSelected");
            }
        }
        #endregion


        public List<UIElement> OtherSettingUserControls { get { return APOCSettings.GetList(); } }


        #region [METHODS]
        private void PopulateLanguageList()
        {
            LanguageList = new List<CultureInfo>();
            var ci = CultureInfo.GetCultureInfo("fa-IR");
            LanguageList.Add(ci);
        }
        private void PopulateDirectionList()
        {
            DirectionList = new List<string> { "راست به چپ", "چپ به راست" };
        }
        private void PopulateFontFamilyList()
        {
            FontFamilyList = new List<string> { "Tahoma", "B Koodak" };
        }
        private void PopulateFontSizeList()
        {
            FontSizeList = new List<int> { 8, 10, 12, 14, 16 };
        }
        private void PopulateKeyboardLayoutList()
        {
            KeyboardLayoutList = new List<System.Windows.Forms.InputLanguage>();
            foreach (System.Windows.Forms.InputLanguage v in System.Windows.Forms.InputLanguage.InstalledInputLanguages)
                KeyboardLayoutList.Add(v);
        }
        private void PopulateCalendarList()
        {
            CalendarList = new List<string> { "قمری", "میلادی", "شمسی" };
        }
        private void PopulateDateFormatList()
        {
            DateFormatList = new List<string> { "yyyy/MM/dd", "dd/MM/yyyy", "yyyy/dd/MM", "MM/dd/yyyy" };
        }
        private void PopulateAutoStartDelayList()
        {
            AutoStartDelayList = new List<int> { 0, 10, 20, 30, 60, 120 };
        }
        private void PopulateDockWidthList()
        {
            DockWidthList = new List<int> { 160, 200, 240, 280 };
        }

        #endregion



        public RelayCommand CommandHelp { get; set; }


        #region IDisposable
        public void Dispose()
        {
            LanguageList = null;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
