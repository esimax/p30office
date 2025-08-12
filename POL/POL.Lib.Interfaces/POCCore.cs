using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Editors.Helpers;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using POL.Lib.Interfaces.SmsSettings;

namespace POL.Lib.Interfaces
{
    public class POCCore
    {
        private string _ApplicationPath;

        private string _LayoutPath;

        private string _SoundTrackPath;

        public POCCore()
        {
            InstanceGuid = new Guid();
            AppBarIsBussy = false;
            OneSecondTimer = new DispatcherTimer(DispatcherPriority.Send) {Interval = TimeSpan.FromSeconds(1)};
            OneSecondTimer.Tick += (s, e) => OneSecondActions.ForEach(a => a.Invoke());
            OneSecondTimer.Start();
            OneSecondActions = new List<Action>();
        }

        public Guid InstanceGuid { get; set; }

        public ServerToClientInformation STCI { get; set; }
        public ClientToServerInformation CTSI { get; set; }

        public string ApplicationPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_ApplicationPath))
                    _ApplicationPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                return _ApplicationPath;
            }
        }

        public string LayoutPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_LayoutPath))
                {
                    _LayoutPath = Path.Combine(ApplicationPath, "Layout");
                    if (!Directory.Exists(_LayoutPath))
                        try
                        {
                            Directory.CreateDirectory(_LayoutPath);
                        }
                        catch
                        {
                        }
                }
                return _LayoutPath;
            }
        }

        public string SoundTrackPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SoundTrackPath))
                {
                    _SoundTrackPath = Path.Combine(ApplicationPath, "SoundTrack");
                    if (!Directory.Exists(_SoundTrackPath))
                        try
                        {
                            Directory.CreateDirectory(_SoundTrackPath);
                        }
                        catch
                        {
                        }
                }
                return _SoundTrackPath;
            }
        }


        public bool AutoSaveProfile { get; set; }
        public string ContactInitModule { get; set; }


        private DispatcherTimer OneSecondTimer { get; }
        private List<Action> OneSecondActions { get; }


        public bool PopupHorizontalAlignmentOnLeft { get; set; }
        public int CallPopupDurationIndex { get; set; }
        public int CallPopupSizeIndex { get; set; }
        public bool CallPopupColorIsFixed { get; set; }
        public Color CallPopupColorFixed { get; set; }
        public Guid CallPopupColorProfile { get; set; }
        public bool CallPopupImageIsActive { get; set; }
        public Guid CallPopupImageProfile { get; set; }
        public bool CallPopupSoundIsActive { get; set; }
        public string CallPopupSoundTrackIndex { get; set; }
        public bool CallPopupCustCol1 { get; set; }
        public bool CallPopupCustCol2 { get; set; }
        public bool CallPopupCustCol3 { get; set; }
        public bool CallPopupCustCol4 { get; set; }
        public bool CallPopupCustCol5 { get; set; }
        public bool CallPopupCustCol6 { get; set; }
        public bool CallPopupCustCol7 { get; set; }
        public bool CallPopupCustCol8 { get; set; }
        public bool CallPopupCustCol9 { get; set; }
        public bool CallPopupCustCol0 { get; set; }

        public bool CallPopupExportAsPopFile { get; set; }

        public bool CallShowTodayCallStat { get; set; }
        public bool CallShowOldCallStat { get; set; }
        public bool CallShowContactCreator { get; set; }
        public bool CallShowContactCreatedDate { get; set; }
        public bool CallShowContactEditor { get; set; }
        public bool CallShowContactEditedDate { get; set; }
        public bool CallShowLastNote { get; set; }
        public bool CallAllowAddNote { get; set; }
        public bool CallOnlyInternal { get; set; }
        public string CallInternalCode { get; set; }
        public bool CallAllowCardTable { get; set; }

        public int EmailReceivePopupDurationIndex { get; set; }
        public int EmailSendPopupDurationIndex { get; set; }
        public bool EmailAllowCardTable { get; set; }
        public event EventHandler OnAppBarIsBussyChanged;

        public void RegisterForOneSecondTimer(Action action)
        {
            OneSecondActions.Add(action);
        }

        #region LineNames

        public Dictionary<int, string> LineNames { get; private set; }

        public void UpdateLineNames()
        {
            if (LineNames != null && LineNames.Count > 0) return;
            try
            {
                if (!string.IsNullOrEmpty(STCI.LineNames))
                {
                    var lineData = STCI.LineNames.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    LineNames = new Dictionary<int, string>();
                    lineData.ToList().ForEach(
                        ld =>
                        {
                            var ss = ld.Split(';');
                            LineNames.Add(Convert.ToInt32(ss[0]), ss[1]);
                        });
                }
            }
            catch
            {
                LineNames = new Dictionary<int, string>();
            }
        }

        #endregion

        #region ExtNames

        public Dictionary<int, string> ExtNames { get; private set; }

        public void UpdateExtNames()
        {
            if (ExtNames != null && ExtNames.Count > 0) return;
            try
            {
                if (!string.IsNullOrEmpty(STCI.ExtNames))
                {
                    var lineData = STCI.ExtNames.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    ExtNames = new Dictionary<int, string>();
                    lineData.ToList().ForEach(
                        ld =>
                        {
                            var ss = ld.Split(';');
                            ExtNames.Add(Convert.ToInt32(ss[0]), ss[1]);
                        });
                }
            }
            catch
            {
                ExtNames = new Dictionary<int, string>();
            }
        }

        #endregion

        #region Sms

        public int SMSReceivePopupDurationIndex { get; set; }
        public int SMSSendPopupDurationIndex { get; set; }
        public bool SMSAllowCardTable { get; set; }

        public List<SmsModuleSettings> SmsAllSettings { get; set; }

        public void UpdateSmsSettings()
        {
            try
            {
                SmsAllSettings = new List<SmsModuleSettings>();
                var data2 = JsonConvert.DeserializeObject<SmsMultiModuleSettings>(STCI.SmsMultiSettingsJsonString);
                data2.Settings.Where(n => n.SmsIsEnable && n.SmsAllowDirectSend)
                    .ForEach(n => { SmsAllSettings.Add(n); });
            }
            catch (Exception ex)
            {
                var log = ServiceLocator.Current.GetInstance<ILoggerFacade>();
                log.Log("A9FE876B-40CB-4FBA-B5DF-AD7B5E1EB4D8", Category.Exception, Priority.Medium);
                log.Log(ex.ToString(), Category.Exception, Priority.Medium);
            }
        }

        #endregion

        #region AppBarIsBussy

        private bool _appBarIsBussy;

        public bool AppBarIsBussy
        {
            get { return _appBarIsBussy; }
            set
            {
                if (value == _appBarIsBussy) return;
                _appBarIsBussy = value;
                var v = OnAppBarIsBussyChanged;
                if (v != null)
                    v.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
