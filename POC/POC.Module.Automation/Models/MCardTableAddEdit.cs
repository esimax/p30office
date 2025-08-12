using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.DB.P30Office;
using POL.DB.Membership;

namespace POC.Module.Automation.Models
{
    public class MCardTableAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBTMCardTable2 DynamicSelectedData { get; set; }
        private DBCTContact DynamicContact { get; set; }


        #region CTOR
        public MCardTableAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            GetDynamicData();

            PopulateUserList();
            PopulateRoleList();
            PopulateAutomationList();
            PopulateData();

            InitCommands();
        }

        public MCardTableAddEdit(object mainView, string title, string note, object category, object contact, object sms, object email, object call)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            GetDynamicData();

            PopulateUserList();
            PopulateRoleList();
            PopulateAutomationList();
            PopulateData();

            InitCommands();

            Title = title;
            Notes = note;
            AttachmentCategory = category as DBCTContactCat;
            AttachmentContact = contact as DBCTContact;
            AttachmentSMS = sms as DBSMLog2;
            AttachmentEmail = email as DBEMEmailInbox;
            AttachmentCall = call as DBCLCall;
        }
        #endregion


        public List<DBMSUser2> UserList { get; set; }
        public List<DBMSRole2> RoleList { get; set; }
        public List<DBTMAutomation> AutomationList { get; set; }

        DateTime sdate = DateTime.MinValue;
        DateTime edate = DateTime.MaxValue;

        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات كارتابل"; }
        }
        #endregion

        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion

        #region Tab 1 : Dispatch
        #region SendToType
        private int _SendToType;
        public int SendToType
        {
            get { return _SendToType; }
            set
            {
                _SendToType = value;
                RaisePropertyChanged("SendToType");
                RaisePropertyChanged("SendToEnabled");
                PopulateSendToList();
                if (SendToType == 2) 
                {
                    foreach (var stl in SendToList)
                    {
                        if ((stl as DBMSUser2) == null) continue;
                        if (((DBMSUser2)stl).Username == AMembership.ActiveUser.UserName)
                        {
                            SelectedSendTo = stl;
                        }
                    }
                }
            }
        }
        #endregion
        public bool SendToEnabled
        {
            get { return SendToType > 0; }
        }

        #region SendToList
        private List<object> _SendToList;
        public List<object> SendToList
        {
            get { return _SendToList; }
            set
            {
                _SendToList = value;
                RaisePropertyChanged("SendToList");
            }
        }
        #endregion
        #region SelectedSendTo
        private object _SelectedSendTo;
        public object SelectedSendTo
        {
            get { return _SelectedSendTo; }
            set
            {
                _SelectedSendTo = value;
                RaisePropertyChanged("SelectedSendTo");
            }
        }
        #endregion

        #region Priority
        private int _Priority;
        public int Priority
        {
            get { return _Priority; }
            set
            {
                _Priority = value;
                RaisePropertyChanged("Priority");
            }
        }
        #endregion
        #region Notes
        private string _Notes;
        public string Notes
        {
            get { return _Notes; }
            set
            {
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        #endregion
        #endregion

        #region Tab 2 : Schedule
        #region HasStartTime
        private bool _HasStartTime;
        public bool HasStartTime
        {
            get { return _HasStartTime; }
            set
            {
                _HasStartTime = value;
                RaisePropertyChanged("HasStartTime");
                RaisePropertyChanged("StartAutomationEnabled");
            }
        }
        #endregion
        #region StartTime
        private string _StartTime;
        public string StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
                RaisePropertyChanged("StartTime");
            }
        }
        #endregion
        #region StartDate
        private DateTime? _StartDate;
        public DateTime? StartDate
        {
            get { return _StartDate; }
            set
            {
                _StartDate = value;
                RaisePropertyChanged("StartDate");
            }
        }
        #endregion
        #region HasStartAutomation
        private bool _HasStartAutomation;
        public bool HasStartAutomation
        {
            get { return _HasStartAutomation; }
            set
            {
                _HasStartAutomation = value;
                RaisePropertyChanged("HasStartAutomation");
                RaisePropertyChanged("StartAutomationEnabled");
            }
        }
        #endregion
        #region StartAutomationEnabled
        public bool StartAutomationEnabled
        {
            get { return HasStartTime && HasStartAutomation; }
        }
        #endregion
        #region SelectedStartAutomation
        private DBTMAutomation _SelectedStartAutomation;
        public DBTMAutomation SelectedStartAutomation
        {
            get { return _SelectedStartAutomation; }
            set
            {
                _SelectedStartAutomation = value;
                RaisePropertyChanged("SelectedStartAutomation");
            }
        }
        #endregion

        #region HasEndTime
        private bool _HasEndTime;
        public bool HasEndTime
        {
            get { return _HasEndTime; }
            set
            {
                _HasEndTime = value;
                RaisePropertyChanged("HasEndTime");
                RaisePropertyChanged("EndAutomationEnabled");
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
            }
        }
        #endregion
        #region EndDate
        private DateTime? _EndDate;
        public DateTime? EndDate
        {
            get { return _EndDate; }
            set
            {
                _EndDate = value;
                RaisePropertyChanged("EndDate");
            }
        }
        #endregion
        #region HasEndAutomation
        private bool _HasEndAutomation;
        public bool HasEndAutomation
        {
            get { return _HasEndAutomation; }
            set
            {
                _HasEndAutomation = value;
                RaisePropertyChanged("HasEndAutomation");
                RaisePropertyChanged("EndAutomationEnabled");
            }
        }
        #endregion
        #region EndAutomationEnabled
        public bool EndAutomationEnabled
        {
            get { return HasEndTime && HasEndAutomation; }
        }
        #endregion
        #region SelectedEndAutomation
        private DBTMAutomation _SelectedEndAutomation;
        public DBTMAutomation SelectedEndAutomation
        {
            get { return _SelectedEndAutomation; }
            set
            {
                _SelectedEndAutomation = value;
                RaisePropertyChanged("SelectedEndAutomation");
            }
        }
        #endregion
        #endregion

        #region Tab 3 : Attachments
        #region HasAttachmentCall
        private bool _HasAttachmentCall;
        public bool HasAttachmentCall
        {
            get { return _HasAttachmentCall; }
            set
            {
                _HasAttachmentCall = value;
                RaisePropertyChanged("HasAttachmentCall");
            }
        }
        #endregion
        #region AttachmentCallEnable
        private bool _AttachmentCallEnable;
        public bool AttachmentCallEnable
        {
            get { return _AttachmentCallEnable; }
            set
            {
                _AttachmentCallEnable = value;
                RaisePropertyChanged("AttachmentCallEnable");
            }
        }
        #endregion
        #region AttachmentCall
        private DBCLCall _AttachmentCall;
        public DBCLCall AttachmentCall
        {
            get { return _AttachmentCall; }
            set
            {
                _AttachmentCall = value;
                RaisePropertyChanged("AttachmentCall");
            }
        }
        #endregion

        #region HasAttachmentEmail
        private bool _HasAttachmentEmail;
        public bool HasAttachmentEmail
        {
            get { return _HasAttachmentEmail; }
            set
            {
                _HasAttachmentEmail = value;
                RaisePropertyChanged("HasAttachmentEmail");
            }
        }
        #endregion
        #region AttachmentEmailEnable
        private bool _AttachmentEmailEnable;
        public bool AttachmentEmailEnable
        {
            get { return _AttachmentEmailEnable; }
            set
            {
                _AttachmentEmailEnable = value;
                RaisePropertyChanged("AttachmentEmailEnable");
            }
        }
        #endregion
        #region AttachmentEmail
        private DBEMEmailInbox _AttachmentEmail;
        public DBEMEmailInbox AttachmentEmail
        {
            get { return _AttachmentEmail; }
            set
            {
                _AttachmentEmail = value;
                RaisePropertyChanged("AttachmentEmail");
            }
        }
        #endregion

        #region HasAttachmentSMS
        private bool _HasAttachmentSMS;
        public bool HasAttachmentSMS
        {
            get { return _HasAttachmentSMS; }
            set
            {
                _HasAttachmentSMS = value;
                RaisePropertyChanged("HasAttachmentSMS");
            }
        }
        #endregion
        #region AttachmentSMSEnable
        private bool _AttachmentSMSEnable;
        public bool AttachmentSMSEnable
        {
            get { return _AttachmentSMSEnable; }
            set
            {
                _AttachmentSMSEnable = value;
                RaisePropertyChanged("AttachmentSMSEnable");
            }
        }
        #endregion
        #region AttachmentSMS
        private DBSMLog2 _AttachmentSMS;
        public DBSMLog2 AttachmentSMS
        {
            get { return _AttachmentSMS; }
            set
            {
                _AttachmentSMS = value;
                RaisePropertyChanged("AttachmentSMS");
            }
        }
        #endregion

        #region HasAttachmentContact
        private bool _HasAttachmentContact;
        public bool HasAttachmentContact
        {
            get { return _HasAttachmentContact; }
            set
            {
                _HasAttachmentContact = value;
                RaisePropertyChanged("HasAttachmentContact");
            }
        }
        #endregion
        #region AttachmentContactEnable
        private bool _AttachmentContactEnable;
        public bool AttachmentContactEnable
        {
            get { return _AttachmentContactEnable; }
            set
            {
                _AttachmentContactEnable = value;
                RaisePropertyChanged("AttachmentContactEnable");
            }
        }
        #endregion
        #region AttachmentContact
        private DBCTContact _AttachmentContact;
        public DBCTContact AttachmentContact
        {
            get { return _AttachmentContact; }
            set
            {
                _AttachmentContact = value;
                RaisePropertyChanged("AttachmentContact");
            }
        }
        #endregion

        #region HasAttachmentCategory
        private bool _HasAttachmentCategory;
        public bool HasAttachmentCategory
        {
            get { return _HasAttachmentCategory; }
            set
            {
                _HasAttachmentCategory = value;
                RaisePropertyChanged("HasAttachmentCategory");
            }
        }
        #endregion
        #region AttachmentCategoryEnable
        private bool _AttachmentCategoryEnable;
        public bool AttachmentCategoryEnable
        {
            get { return _AttachmentCategoryEnable; }
            set
            {
                _AttachmentCategoryEnable = value;
                RaisePropertyChanged("AttachmentCategoryEnable");
            }
        }
        #endregion
        #region AttachmentCategory
        private DBCTContactCat _AttachmentCategory;
        public DBCTContactCat AttachmentCategory
        {
            get { return _AttachmentCategory; }
            set
            {
                _AttachmentCategory = value;
                RaisePropertyChanged("AttachmentCategory");
            }
        }
        #endregion

        #region HasAttachmentFile
        private bool _HasAttachmentFile;
        public bool HasAttachmentFile
        {
            get { return _HasAttachmentFile; }
            set
            {
                _HasAttachmentFile = value;
                RaisePropertyChanged("HasAttachmentFile");
            }
        }
        #endregion
        #region AttachmentFileEnable
        private bool _AttachmentFileEnable;
        public bool AttachmentFileEnable
        {
            get { return _AttachmentFileEnable; }
            set
            {
                _AttachmentFileEnable = value;
                RaisePropertyChanged("AttachmentFileEnable");
            }
        }
        #endregion
        #region AttachmentFile
        private byte[] _AttachmentFile;
        public byte[] AttachmentFile
        {
            get { return _AttachmentFile; }
            set
            {
                _AttachmentFile = value;
                RaisePropertyChanged("AttachmentFile");
            }
        }
        #endregion
        #region AttachmentFileName
        private string _AttachmentFileName;
        public string AttachmentFileName
        {
            get { return _AttachmentFileName; }
            set
            {
                _AttachmentFileName = value;
                RaisePropertyChanged("AttachmentFileName");
            }
        }
        #endregion
        #endregion

        #region Tab 4 : Result
        #region Creator
        private string _Creator;
        public string Creator
        {
            get { return _Creator; }
            set
            {
                _Creator = value;
                RaisePropertyChanged("Creator");
            }
        }
        #endregion

        #region CreatedDate
        private string _CreatedDate;
        public string CreatedDate
        {
            get { return _CreatedDate; }
            set
            {
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        #endregion

        #region ResultDate
        private string _ResultDate;
        public string ResultDate
        {
            get { return _ResultDate; }
            set
            {
                _ResultDate = value;
                RaisePropertyChanged("ResultDate");
            }
        }
        #endregion

        #region ResultUser
        private string _ResultUser;
        public string ResultUser
        {
            get { return _ResultUser; }
            set
            {
                _ResultUser = value;
                RaisePropertyChanged("ResultUser");
            }
        }
        #endregion

        #region SelectedResult
        private int _SelectedResult;
        public int SelectedResult
        {
            get { return _SelectedResult; }
            set
            {
                _SelectedResult = value;
                RaisePropertyChanged("SelectedResult");
            }
        }
        #endregion

        #region ResultText
        private string _ResultText;
        public string ResultText
        {
            get { return _ResultText; }
            set
            {
                _ResultText = value;
                RaisePropertyChanged("ResultText");
            }
        }
        #endregion

        #region ResultAutomationEnable
        private bool _ResultAutomationEnable;
        public bool ResultAutomationEnable
        {
            get { return _ResultAutomationEnable; }
            set
            {
                _ResultAutomationEnable = value;
                RaisePropertyChanged("ResultAutomationEnable");
            }
        }
        #endregion

        #region HasResultAutomation
        private bool _HasResultAutomation;
        public bool HasResultAutomation
        {
            get { return _HasResultAutomation; }
            set
            {
                _HasResultAutomation = value;
                RaisePropertyChanged("HasResultAutomation");
            }
        }
        #endregion

        #region SelectedResultAutomation
        private DBTMAutomation _SelectedResultAutomation;
        public DBTMAutomation SelectedResultAutomation
        {
            get { return _SelectedResultAutomation; }
            set
            {
                _SelectedResultAutomation = value;
                RaisePropertyChanged("SelectedResultAutomation");
            }
        }
        #endregion

        #endregion


        #region [METHODS]

        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
            DynamicContact = MainView.DynamicContact;
        }

        private void PopulateUserList()
        {
            var v = DBMSUser2.UserGetAll(ADatabase.Dxs, null, true);
            UserList = (from u in v select u).ToList();
        }
        private void PopulateRoleList()
        {
            var v = DBMSRole2.RoleGetAll(ADatabase.Dxs, null);
            RoleList = (from u in v select u).ToList();
        }
        private void PopulateAutomationList()
        {
            var v = DBTMAutomation.GetAll(ADatabase.Dxs);
            AutomationList = (from u in v select u).ToList();
        }
        private void PopulateSendToList()
        {
            switch (SendToType)
            {
                case 1:
                    SendToList = (from n in RoleList select n as object).ToList();
                    break;
                case 2:
                    SendToList = (from n in UserList select n as object).ToList();
                    break;
                default:
                    SendToList = null;
                    break;
            }
            SelectedSendTo = null;
        }

        private void PopulateData()
        {
            GlobalResult = 0;
            SendToType = 0;
            Priority = 3;

            StartDate = DateTime.Now.Date;
            StartTime = string.Format("{0}:{1}", DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'));

            EndDate = DateTime.Now.Date;

            AttachmentCallEnable = true;
            AttachmentEmailEnable = true;
            AttachmentSMSEnable = true;
            AttachmentContactEnable = true;
            AttachmentCategoryEnable = true;
            AttachmentFileEnable = true;

            SelectedResult = 0;
            ResultAutomationEnable = true;

            if (DynamicSelectedData == null)
            {
                MainView.IsNew();
                AttachmentContact = DynamicContact;
                HasAttachmentContact = (AttachmentContact != null);
                return;
            }

            #region Tab 1
            Title = DynamicSelectedData.Title;
            SendToType = DynamicSelectedData.SendToType;
            switch (SendToType)
            {
                case 1:
                    SendToList = (from n in RoleList select (object)n).ToList();
                    SelectedSendTo = DBMSRole2.FindByOid(ADatabase.Dxs, DynamicSelectedData.SendToData);
                    break;
                case 2:
                    SendToList = (from n in UserList select (object)n).ToList();
                    SelectedSendTo = DBMSUser2.UserGetByOid(ADatabase.Dxs, DynamicSelectedData.SendToData);
                    break;
                default:
                    SendToList = null;
                    SelectedSendTo = null;
                    break;
            }
            Priority = DynamicSelectedData.Priority;
            Notes = DynamicSelectedData.Note;
            #endregion

            #region Tab 2
            HasStartTime = DynamicSelectedData.HasStartingDate;
            HasStartAutomation = DynamicSelectedData.HasStartingAutomation;
            if (HasStartTime)
            {
                StartTime = string.Format("{0}:{1}", DynamicSelectedData.StartingDate.Value.Hour.ToString().PadLeft(2, '0'), DynamicSelectedData.StartingDate.Value.Minute.ToString().PadLeft(2, '0'));
                StartDate = DynamicSelectedData.StartingDate.Value.Date;
                if (HasStartAutomation)
                {
                    SelectedStartAutomation = DBTMAutomation.FindByOid(ADatabase.Dxs,
                                                                       DynamicSelectedData.StartingAutomationID);
                }
            }

            HasEndTime = DynamicSelectedData.HasEndingDate;
            HasEndAutomation = DynamicSelectedData.HasEndingAutomation;
            if (HasEndTime)
            {
                EndTime = string.Format("{0}:{1}", DynamicSelectedData.EndingDate.Value.Hour, DynamicSelectedData.EndingDate.Value.Minute);
                EndDate = DynamicSelectedData.EndingDate.Value.Date;
                if (_HasEndAutomation)
                {
                    SelectedEndAutomation = DBTMAutomation.FindByOid(ADatabase.Dxs,
                                                                       DynamicSelectedData.EndingAutomationID);
                }
            }
            #endregion

            #region Tab 3
            HasAttachmentContact = DynamicSelectedData.HasLinkToContact;
            if (HasAttachmentContact)
            {
                AttachmentContact = DynamicSelectedData.LinkContact;
            }

            HasAttachmentCategory = DynamicSelectedData.HasLinkToCategory;
            if (HasAttachmentCategory)
            {
                AttachmentCategory = DynamicSelectedData.LinkCategory;
            }
            HasAttachmentFile = DynamicSelectedData.HasLinkToFile;
            if (HasAttachmentFile)
            {
                AttachmentFile = DynamicSelectedData.LinkFile;
                AttachmentFileName = DynamicSelectedData.LinkFileName;
            }
            #endregion

            #region Tab 4
            Creator = DynamicSelectedData.UserCreated;
            CreatedDate = HelperLocalize.DateTimeToString(DynamicSelectedData.DateCreated.Value,
                                                          HelperLocalize.ApplicationCalendar, HelperLocalize.ApplicationDateTimeFormat);
            SelectedResult = DynamicSelectedData.Result;
            ResultText = DynamicSelectedData.ResultNote;
            HasResultAutomation = DynamicSelectedData.HasResultAutomation;
            SelectedResultAutomation = DBTMAutomation.FindByOid(ADatabase.Dxs, DynamicSelectedData.ResultAutomationID);
            #endregion

            if (DynamicSelectedData.Result > 1)
                MainView.MakeReadOnly();
            GlobalResult = DynamicSelectedData.Result;
        }

        private int GlobalResult { get; set; }

        protected void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(Title) && GlobalResult <= 1);
            CommandAttCall = new RelayCommand(AttCall, () => false);
            CommandAttSMS = new RelayCommand(AttSMS, () => false);
            CommandAttEmail = new RelayCommand(AttEmail, () => false);
            CommandAttContact = new RelayCommand(AttContact, () => true);
            CommandAttContactGoto = new RelayCommand(AttContactGoto, () => true);
            CommandAttCategory = new RelayCommand(AttCategory, () => true);
            CommandAttFile = new RelayCommand(AttFile, () => true);
            CommandUnAttFile = new RelayCommand(UnAttFile, () => true);
            CommandOpenAttFile = new RelayCommand(OpenAttFile, () => AttachmentFile != null);
            CommandStartAutomationAddAutomation = new RelayCommand(AutomationAddAutomation, () => AMembership.HasPermission(PCOPermissions.Automations_Add));
            CommandEndAutomationAddAutomation = new RelayCommand(AutomationAddAutomation, () => AMembership.HasPermission(PCOPermissions.Automations_Add));
            CommandResultAutomationAddAutomation = new RelayCommand(AutomationAddAutomation, () => AMembership.HasPermission(PCOPermissions.Automations_Add));

            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp19 != "");
        }

        private void AttContactGoto()
        {
            if (AttachmentContact == null)
                return;
            var dbc = DBCTContact.FindByOid(ADatabase.Dxs, AttachmentContact.Oid);
            if (dbc == null) return;
            var apocContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            apocContactModule.GotoContactByCode(AttachmentContact.Code);
            var window = Window.GetWindow(DynamicOwner);
            if (window != null) window.Close();
        }

        private void AttCall()
        {
            throw new NotImplementedException();
        }
        private void AttSMS()
        {
            throw new NotImplementedException();
        }
        private void AttEmail()
        {
            throw new NotImplementedException();
        }
        private void AttContact()
        {
            var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var o = aPOCMainWindow.ShowSelectContact(DynamicOwner, null);
            if (o is DBCTContact)
            {
                AttachmentContact = o as DBCTContact;
            }
        }
        private void AttCategory()
        {
            var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var o = aPOCMainWindow.ShowSelectContactCat(DynamicOwner);
            if (o is DBCTContactCat)
            {
                AttachmentCategory = o as DBCTContactCat;
            }
        }
        private void AttFile()
        {
            var sf = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                FilterIndex = 0,
                RestoreDirectory = true,
            };
            if (sf.ShowDialog() != true) return;

            AttachmentFile = System.IO.File.ReadAllBytes(sf.FileName);
            AttachmentFileName = System.IO.Path.GetFileName(sf.FileName);

        }
        private void UnAttFile()
        {
            AttachmentFile = null;
            AttachmentFileName = string.Empty;
        }
        private void OpenAttFile()
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = AttachmentFileName,
            };
            if (sf.ShowDialog() != true) return;

            var f = System.IO.File.Open(sf.FileName, FileMode.CreateNew);
            f.Write(AttachmentFile, 0, AttachmentFile.Length);
            f.Flush();
            f.Close();

        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp19);
        }

        private void AutomationAddAutomation()
        {
            APOCMainWindow.ShowAddEditAutomation(MainView, null);
            PopulateAutomationList();
            RaisePropertyChanged("AutomationList");
        }
        private void OK()
        {
            if (Validate())
                if (Save())
                {
                    DynamicOwner.DialogResult = true;
                    DynamicOwner.Close();
                }
        }
        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                POLMessageBox.ShowWarning("عنوان معتبر نمی باشد.", MainView);
                return false;
            }
            #region Tab 1
            if (SendToType > 0 && SelectedSendTo == null)
            {
                POLMessageBox.ShowWarning("خطا در نحوه ارسال : نام كاربر و یا سطح دسترسی مشخص گردد.", MainView);
                return false;
            }
            #endregion

            #region Tab 2
            if (HasStartTime)
            {
                try
                {
                    var time = StartTime.Split(':');
                    sdate = StartDate.Value.Date;
                    sdate = sdate.AddHours(Convert.ToInt32(time[0])).AddMinutes(Convert.ToInt32(time[1]));
                }
                catch
                {
                    POLMessageBox.ShowWarning("خطا در زمانبندی : زمان شروع را اصلاح كنید.", MainView);
                    return false;
                }

                if (HasStartAutomation && SelectedStartAutomation == null)
                {
                    POLMessageBox.ShowWarning("خطا در زمانبندی : اتوماسیون زمان شروع را اصلاح كنید.", MainView);
                    return false;
                }
            }


            if (HasEndTime)
            {
                try
                {
                    var time = EndTime.Split(':');
                    edate = EndDate.Value.Date;
                    edate = edate.AddHours(Convert.ToInt32(time[0])).AddMinutes(Convert.ToInt32(time[1]));
                }
                catch
                {
                    POLMessageBox.ShowWarning("خطا در زمانبندی : مهلت اجرا را اصلاح كنید.", MainView);
                    return false;
                }

                if (HasStartAutomation && SelectedStartAutomation == null)
                {
                    POLMessageBox.ShowWarning("خطا در زمانبندی : اتوماسیون مهلت اجرا را اصلاح كنید.", MainView);
                    return false;
                }
            }

            if (HasStartTime && HasEndTime && edate < sdate)
            {
                POLMessageBox.ShowWarning("خطا در زمانبندی : مهلت اجرا قبل از زمان شروع می باشد.", MainView);
                return false;
            }
            #endregion

            #region Tab 3
            if (AttachmentContactEnable && HasAttachmentContact && AttachmentContact == null)
            {
                POLMessageBox.ShowWarning("خطا در ضمائم : پرونده تعیین نشده.", MainView);
                return false;
            }
            if (AttachmentCategoryEnable && HasAttachmentCategory && AttachmentCategory == null)
            {
                POLMessageBox.ShowWarning("خطا در ضمائم : دسته تعیین نشده.", MainView);
                return false;
            }
            #endregion

            #region Tab 4
            if (ResultAutomationEnable && HasResultAutomation && SelectedResultAutomation == null)
            {
                POLMessageBox.ShowWarning("خطا در نتیجه : اتوماسیون تعیین نشده.", MainView);
                return false;
            }
            #endregion

            return true;
        }
        private bool Save()
        {
            try
            {
                if (DynamicSelectedData == null)
                {
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        DynamicSelectedData = new DBTMCardTable2(uow)
                        {
                            Title = Title.Trim(),
                            CreatorName = AMembership.ActiveUser.Title,
                            SendToType = SendToType,
                            SendToData = SendToType < 1 ? Guid.Empty : ((XPGUIDObject)SelectedSendTo).Oid,
                            SendTo = SendToType < 1 ? "همه" : SelectedSendTo.ToString(),
                            Priority = Priority,
                            Note = Notes,

                            HasStartingDate = HasStartTime,
                            HasStartingAutomation = HasStartAutomation,
                            StartingDate = HasStartTime ? sdate : DateTime.MaxValue,
                            StartingAutomationID = HasStartAutomation ? SelectedStartAutomation.Oid : Guid.Empty,

                            HasEndingDate = HasEndTime,
                            HasEndingAutomation = HasEndAutomation,
                            EndingDate = HasEndTime ? edate : DateTime.MaxValue,
                            EndingAutomationID = HasEndAutomation ? SelectedEndAutomation.Oid : Guid.Empty,

                            HasLinkToCall = false,
                            HasLinkToEmail = false,
                            HasLinkToSMS = false,
                            HasLinkToContact = HasAttachmentContact,
                            HasLinkToCategory = HasAttachmentCategory,


                            LinkCall = null,
                            LinkEmail = null,
                            LinkSMS2 = null,
                            LinkContact = !HasAttachmentContact ? null : DBCTContact.FindByOid(uow, AttachmentContact.Oid),
                            LinkCategory = !HasAttachmentCategory ? null : DBCTContactCat.FindByOid(uow, AttachmentCategory.Oid),

                            Result = SelectedResult,
                            ResultNote = ResultText,
                            HasResultAutomation = HasResultAutomation,
                            ResultAutomationID = HasResultAutomation ? SelectedResultAutomation.Oid : Guid.Empty,
                            ResultDate = DateTime.MinValue,

                            HasLinkToFile = HasAttachmentFile,
                            LinkFile = AttachmentFile,
                            LinkFileName = _AttachmentFileName,

                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBTMCardTable2.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    if (SelectedResult > 1)
                    {
                        var dr = POLMessageBox.ShowQuestionYesNo("پس از ثبت نتیجه، دیگر قادر به تغییر كارتابل نمی باشید.\nنتیجه ثبت شود؟", MainView);
                        if (dr != MessageBoxResult.Yes)
                            return false;
                        if (SelectedResult == 4)
                        {
                            var APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                            var o = APOCMainWindow.ShowSelectUserRole(MainView, 0, Guid.Empty);
                            if (o == null) return false;

                            var res = o as Tuple<int, Guid, string>;
                            if (res != null)
                            {
                                var DynamicSelectedData2 = new DBTMCardTable2(ADatabase.Dxs)
                                {
                                    Title = Title.Trim(),
                                    CreatorName = AMembership.ActiveUser.Title,
                                    SendToType = res.Item1,
                                    SendToData = res.Item1 < 1 ? Guid.Empty : res.Item2,
                                    SendTo = res.Item1 < 1 ? "همه" : res.Item3,
                                    Priority = Priority,
                                    Note = Notes,

                                    HasStartingDate = HasStartTime,
                                    HasStartingAutomation = HasStartAutomation,
                                    StartingDate = HasStartTime ? sdate : DateTime.MaxValue,
                                    StartingAutomationID = HasStartAutomation ? SelectedStartAutomation.Oid : Guid.Empty,

                                    HasEndingDate = HasEndTime,
                                    HasEndingAutomation = HasEndAutomation,
                                    EndingDate = HasEndTime ? edate : DateTime.MaxValue,
                                    EndingAutomationID = HasEndAutomation ? SelectedEndAutomation.Oid : Guid.Empty,

                                    HasLinkToCall = false,
                                    HasLinkToEmail = false,
                                    HasLinkToSMS = false,
                                    HasLinkToContact = HasAttachmentContact,
                                    HasLinkToCategory = HasAttachmentCategory,

                                    LinkCall = null,
                                    LinkEmail = null,
                                    LinkSMS2 = null,
                                    LinkContact =
                                        !HasAttachmentContact
                                            ? null
                                            : DBCTContact.FindByOid(ADatabase.Dxs, AttachmentContact.Oid),
                                    LinkCategory =
                                        !HasAttachmentCategory
                                            ? null
                                            : DBCTContactCat.FindByOid(ADatabase.Dxs, AttachmentCategory.Oid),

                                    Result = 0,
                                    ResultNote = ResultText,
                                    HasResultAutomation = HasResultAutomation,
                                    ResultAutomationID = HasResultAutomation ? SelectedResultAutomation.Oid : Guid.Empty,
                                    ResultDate = DateTime.MinValue,

                                    HasLinkToFile = HasAttachmentFile,
                                    LinkFile = AttachmentFile,
                                    LinkFileName = AttachmentFileName,
                                };
                                DynamicSelectedData2.Save();
                            }

                        }
                        DynamicSelectedData.ResultDate = DateTime.Now.AddMinutes(1);
                    }

                    DynamicSelectedData.Title = Title.Trim();

                    DynamicSelectedData.SendToType = SendToType;
                    DynamicSelectedData.SendToData = SendToType < 1 ? Guid.Empty : ((XPGUIDObject)SelectedSendTo).Oid;
                    DynamicSelectedData.SendTo = SendToType < 1 ? "همه" : SelectedSendTo.ToString();
                    DynamicSelectedData.Priority = Priority;
                    DynamicSelectedData.Note = Notes;

                    DynamicSelectedData.HasStartingDate = HasStartTime;
                    DynamicSelectedData.HasStartingAutomation = HasStartAutomation;
                    DynamicSelectedData.StartingDate = HasStartTime ? sdate : DateTime.MaxValue;
                    DynamicSelectedData.StartingAutomationID = HasStartAutomation ? SelectedStartAutomation.Oid : Guid.Empty;

                    DynamicSelectedData.HasEndingDate = HasEndTime;
                    DynamicSelectedData.HasEndingAutomation = HasEndAutomation;
                    DynamicSelectedData.EndingDate = HasEndTime ? edate : DateTime.MaxValue;
                    DynamicSelectedData.EndingAutomationID = HasEndAutomation ? SelectedEndAutomation.Oid : Guid.Empty;

                    DynamicSelectedData.HasLinkToCall = false;
                    DynamicSelectedData.HasLinkToEmail = false;
                    DynamicSelectedData.HasLinkToSMS = false;
                    DynamicSelectedData.HasLinkToContact = HasAttachmentContact;
                    DynamicSelectedData.HasLinkToCategory = HasAttachmentCategory;

                    DynamicSelectedData.LinkCall = null;
                    DynamicSelectedData.LinkEmail = null;
                    DynamicSelectedData.LinkSMS2 = null;
                    DynamicSelectedData.LinkContact = !HasAttachmentContact ? null : AttachmentContact;
                    DynamicSelectedData.LinkCategory = !HasAttachmentCategory ? null : AttachmentCategory;

                    DynamicSelectedData.Result = SelectedResult;
                    DynamicSelectedData.ResultNote = ResultText;
                    DynamicSelectedData.HasResultAutomation = HasResultAutomation;
                    DynamicSelectedData.ResultAutomationID = HasResultAutomation ? SelectedResultAutomation.Oid : Guid.Empty;

                    DynamicSelectedData.HasLinkToFile = HasAttachmentFile;
                    DynamicSelectedData.LinkFile = AttachmentFile;
                    DynamicSelectedData.LinkFileName = AttachmentFileName;

                    DynamicSelectedData.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Microsoft.Practices.Prism.Logging.Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, MainView);
                return false;
            }
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }

        public RelayCommand CommandAttCall { get; set; }
        public RelayCommand CommandAttSMS { get; set; }
        public RelayCommand CommandAttEmail { get; set; }
        public RelayCommand CommandAttContact { get; set; }
        public RelayCommand CommandAttContactGoto { get; set; }
        public RelayCommand CommandAttCategory { get; set; }
        public RelayCommand CommandAttFile { get; set; }
        public RelayCommand CommandUnAttFile { get; set; }
        public RelayCommand CommandOpenAttFile { get; set; }

        public RelayCommand CommandStartAutomationAddAutomation { get; set; }
        public RelayCommand CommandEndAutomationAddAutomation { get; set; }
        public RelayCommand CommandResultAutomationAddAutomation { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
