using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.DB.P30Office;
using POL.DB.Membership;

namespace POC.Module.Automation.Models
{
    public class MAutomationAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBTMAutomation DynamicSelectedData { get; set; }


        #region CTOR
        public MAutomationAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            GetDynamicData();

            PopulateUserList();
            PopulateRoleList();
            PopulateEmailTemplateList();
            PopulateEmailAccountList();
            PopulateData();

            InitCommands();
        }

        
        #endregion


        public List<DBMSUser2> UserList { get; set; }
        public List<DBMSRole2> RoleList { get; set; }
        public List<DBEMTemplate> EmailTemplateList { get; set; }
        public List<DBEMEmailApp> EmailAccountList { get; set; }

        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات عملكرد اتوماتیك"; }
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




        #region Popup
        #region PopupEnable
        private bool _PopupEnable;
        public bool PopupEnable
        {
            get { return _PopupEnable; }
            set
            {
                _PopupEnable = value;
                RaisePropertyChanged("PopupEnable");
                RaisePropertyChanged("PopupSendToEnable");
            }
        }
        #endregion
        #region PopupText
        private string _PopupText;
        public string PopupText
        {
            get { return _PopupText; }
            set
            {
                _PopupText = value;
                RaisePropertyChanged("PopupText");
            }
        }
        #endregion

        #region PopupSendToEnable
        public bool PopupSendToEnable
        {
            get { return PopupEnable ; }
        }
        #endregion

        public List<object> PopupSendToList { get; set; }
        public string PopupSendToData { get; set; }
        #endregion

        #region SMS
        #region SMSEnable
        private bool _SMSEnable;
        public bool SMSEnable
        {
            get { return _SMSEnable; }
            set
            {
                _SMSEnable = value;
                RaisePropertyChanged("SMSEnable");
                RaisePropertyChanged("SMSCanEditNumbers");
            }
        }
        #endregion
        #region SMSText
        private string _SMSText;
        public string SMSText
        {
            get { return _SMSText; }
            set
            {
                _SMSText = value;
                RaisePropertyChanged("SMSText");
            }
        }
        #endregion
        #region SMSType
        private int _SMSType;
        public int SMSType
        {
            get { return _SMSType; }
            set
            {
                _SMSType = value;
                RaisePropertyChanged("SMSType");
            }
        }
        #endregion
        #region SMSData
        private string _SMSData;
        public string SMSData
        {
            get { return _SMSData; }
            set
            {
                _SMSData = value;
                RaisePropertyChanged("SMSData");
            }
        }
        #endregion

        #region SMSSendToAll
        private bool _SMSSendToAll;
        public bool SMSSendToAll
        {
            get { return _SMSSendToAll; }
            set
            {
                _SMSSendToAll = value;
                RaisePropertyChanged("SMSSendToAll");
                if (value)
                {
                    SMSType = 0;
                    SMSData = null;
                    SMSNumbers = null;
                    SMSContact = null;
                    SMSCategory = null;
                }
            }
        }
        #endregion
        #region SMSSendToNumbers
        private bool _SMSSendToNumbers;
        public bool SMSSendToNumbers
        {
            get { return _SMSSendToNumbers; }
            set
            {
                _SMSSendToNumbers = value;
                RaisePropertyChanged("SMSSendToNumbers");
                RaisePropertyChanged("SMSCanEditNumbers");
                if (value)
                {
                    SMSType = 1;
                    SMSData = null;
                    SMSNumbers = null;
                    SMSContact = null;
                    SMSCategory = null;
                }
            }
        }
        #endregion
        #region SMSSendToContact
        private bool _SMSSendToContact;
        public bool SMSSendToContact
        {
            get { return _SMSSendToContact; }
            set
            {
                _SMSSendToContact = value;
                RaisePropertyChanged("SMSSendToContact");
                if (value)
                {
                    SMSType = 2;
                    SMSData = null;
                    SMSNumbers = null;
                    SMSContact = null;
                    SMSCategory = null;
                }
            }
        }
        #endregion
        #region SMSSendToCategory
        private bool _SMSSendToCategory;
        public bool SMSSendToCategory
        {
            get { return _SMSSendToCategory; }
            set
            {
                _SMSSendToCategory = value;
                RaisePropertyChanged("SMSSendToCategory");
                if (value)
                {
                    SMSType = 3;
                    SMSData = null;
                    SMSNumbers = null;
                    SMSContact = null;
                    SMSCategory = null;
                }
            }
        }
        #endregion
        #region SMSSendToRelatedProfileItem
        private bool _SMSSendToRelatedProfileItem;
        public bool SMSSendToRelatedProfileItem
        {
            get { return _SMSSendToRelatedProfileItem; }
            set
            {
                _SMSSendToRelatedProfileItem = value;
                RaisePropertyChanged("SMSSendToRelatedProfileItem");
                SMSType = 4;
            }
        }
        #endregion

        #region SMSSendToAnyOne
        private bool _SMSSendToAnyOne;
        public bool SMSSendToAnyOne
        {
            get { return _SMSSendToAnyOne; }
            set
            {
                _SMSSendToAnyOne = value;
                RaisePropertyChanged("SMSSendToAnyOne");
            }
        }
        #endregion
        #region SMSNumbersAnyone
        private string _SMSNumbersAnyone;
        public string SMSNumbersAnyone
        {
            get { return _SMSNumbersAnyone; }
            set
            {
                _SMSNumbersAnyone = value;
                RaisePropertyChanged("SMSNumbersAnyone");
            }
        }
        #endregion

        

        public bool SMSCanEditNumbers
        {
            get { return SMSEnable && SMSSendToNumbers; }
        }

        #region SMSNumbers
        private string _SMSNumbers;
        public string SMSNumbers
        {
            get { return _SMSNumbers; }
            set
            {
                _SMSNumbers = value;
                RaisePropertyChanged("SMSNumbers");
            }
        }
        #endregion
        #region SMSContact
        private string _SMSContact;
        public string SMSContact
        {
            get { return _SMSContact; }
            set
            {
                _SMSContact = value;
                RaisePropertyChanged("SMSContact");
            }
        }
        #endregion
        #region SMSCategory
        private string _SMSCategory;
        public string SMSCategory
        {
            get { return _SMSCategory; }
            set
            {
                _SMSCategory = value;
                RaisePropertyChanged("SMSCategory");
            }
        }
        #endregion
        #region SMSBasket
        private string _SMSBasket;
        public string SMSBasket
        {
            get { return _SMSBasket; }
            set
            {
                _SMSBasket = value;
                RaisePropertyChanged("SMSBasket");
            }
        }
        #endregion
        #endregion

        #region Email
        #region EmailEnable
        private bool _EmailEnable;
        public bool EmailEnable
        {
            get { return _EmailEnable; }
            set
            {
                _EmailEnable = value;
                RaisePropertyChanged("EmailEnable");
                RaisePropertyChanged("EmailCanEditNumbers");
            }
        }
        #endregion
        #region EmailText
        private string _EmailText;
        public string EmailText
        {
            get { return _EmailText; }
            set
            {
                _EmailText = value;
                RaisePropertyChanged("EmailText");
            }
        }
        #endregion
        #region EmailType
        private int _EmailType;
        public int EmailType
        {
            get { return _EmailType; }
            set
            {
                _EmailType = value;
                RaisePropertyChanged("EmailType");
            }
        }
        #endregion
        #region EmailData
        private string _EmailData;
        public string EmailData
        {
            get { return _EmailData; }
            set
            {
                _EmailData = value;
                RaisePropertyChanged("EmailData");
            }
        }
        #endregion

        #region EmailSendToAll
        private bool _EmailSendToAll;
        public bool EmailSendToAll
        {
            get { return _EmailSendToAll; }
            set
            {
                _EmailSendToAll = value;
                RaisePropertyChanged("EmailSendToAll");
                if (value)
                {
                    EmailType = 0;
                    EmailData = null;
                    EmailNumbers = null;
                    EmailContact = null;
                    EmailCategory = null;
                }
            }
        }
        #endregion
        #region EmailSendToNumbers
        private bool _EmailSendToNumbers;
        public bool EmailSendToNumbers
        {
            get { return _EmailSendToNumbers; }
            set
            {
                _EmailSendToNumbers = value;
                RaisePropertyChanged("EmailSendToNumbers");
                RaisePropertyChanged("EmailCanEditNumbers");
                if (value)
                {
                    EmailType = 1;
                    EmailData = null;
                    EmailNumbers = null;
                    EmailContact = null;
                    EmailCategory = null;
                }
            }
        }
        #endregion
        #region EmailSendToContact
        private bool _EmailSendToContact;
        public bool EmailSendToContact
        {
            get { return _EmailSendToContact; }
            set
            {
                _EmailSendToContact = value;
                RaisePropertyChanged("EmailSendToContact");
                if (value)
                {
                    EmailType = 2;
                    EmailData = null;
                    EmailNumbers = null;
                    EmailContact = null;
                    EmailCategory = null;
                }
            }
        }
        #endregion
        #region EmailSendToCategory
        private bool _EmailSendToCategory;
        public bool EmailSendToCategory
        {
            get { return _EmailSendToCategory; }
            set
            {
                _EmailSendToCategory = value;
                RaisePropertyChanged("EmailSendToCategory");
                if (value)
                {
                    EmailType = 3;
                    EmailData = null;
                    EmailNumbers = null;
                    EmailContact = null;
                    EmailCategory = null;
                }
            }
        }
        #endregion
        #region EmailSendToRelatedProfileItem
        private bool _EmailSendToRelatedProfileItem;
        public bool EmailSendToRelatedProfileItem
        {
            get { return _EmailSendToRelatedProfileItem; }
            set
            {
                _EmailSendToRelatedProfileItem = value;
                RaisePropertyChanged("EmailSendToRelatedProfileItem");
                EmailType = 4;
            }
        }
        #endregion

        public bool EmailCanEditNumbers
        {
            get { return EmailEnable && EmailSendToNumbers; }
        }

        #region EmailNumbers
        private string _EmailNumbers;
        public string EmailNumbers
        {
            get { return _EmailNumbers; }
            set
            {
                _EmailNumbers = value;
                RaisePropertyChanged("EmailNumbers");
            }
        }
        #endregion
        #region EmailContact
        private string _EmailContact;
        public string EmailContact
        {
            get { return _EmailContact; }
            set
            {
                _EmailContact = value;
                RaisePropertyChanged("EmailContact");
            }
        }
        #endregion
        #region EmailCategory
        private string _EmailCategory;
        public string EmailCategory
        {
            get { return _EmailCategory; }
            set
            {
                _EmailCategory = value;
                RaisePropertyChanged("EmailCategory");
            }
        }
        #endregion
        #region EmailBasket
        private string _EmailBasket;
        public string EmailBasket
        {
            get { return _EmailBasket; }
            set
            {
                _EmailBasket = value;
                RaisePropertyChanged("EmailBasket");
            }
        }
        #endregion

        #region EmailSelectedTemplate
        private DBEMTemplate _EmailSelectedTemplate;
        public DBEMTemplate EmailSelectedTemplate
        {
            get { return _EmailSelectedTemplate; }
            set
            {
                _EmailSelectedTemplate = value;
                RaisePropertyChanged("EmailSelectedTemplate");
            }
        }
        #endregion
        #region EmailSelectedAccount
        private DBEMEmailApp _EmailSelectedAccount;
        public DBEMEmailApp EmailSelectedAccount
        {
            get { return _EmailSelectedAccount; }
            set
            {
                _EmailSelectedAccount = value;
                RaisePropertyChanged("EmailSelectedAccount");
            }
        }
        #endregion
        #endregion




        #region [METHODS]

        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
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
        private void PopulateEmailTemplateList()
        {
            var v = DBEMTemplate.GetAll(ADatabase.Dxs);
            EmailTemplateList = (from u in v select u).ToList();
        }
        private void PopulateEmailAccountList()
        {
            var xpc = DBEMEmailApp.GetAll(ADatabase.Dxs);
            EmailAccountList = (from n in xpc select n).Where(n=>n.IsEnable)
                .Where(IsAllowedToSeeEmail).ToList();
        }

        private bool IsAllowedToSeeEmail(DBEMEmailApp app)
        {
            bool allow = false;
            if (app.ViewPermissionType == 3)
                allow = true;
            else if (app.ViewPermissionType == 2 && AMembership.ActiveUser.RolesOid.Contains(app.ViewOid))
                allow = true;
            else if (app.ViewPermissionType == 1 && AMembership.ActiveUser.UserID == app.ViewOid)
                allow = true;
            else
            {
                if (AMembership.ActiveUser.UserName.ToLower() == "admin")
                    allow = true;
            }
            return allow;
        }

        private void PopulateData()
        {
            SMSSendToAll = true;
            EmailSendToAll = true;
            if (DynamicSelectedData == null) return;
            var db = DynamicSelectedData;
            Title = db.Title;

            #region Popup Tab Populate
            PopupEnable = db.PopupEnable;
            if (PopupEnable)
            {
                PopupText = db.PopupText;
            }
            #endregion

            #region SMS Tab Populate
            SMSEnable = db.SMSEnable;
            if (SMSEnable)
            {
                SMSText = db.SMSText;
                SMSType = db.SMSType;
                switch (SMSType)
                {
                    case 1:
                        SMSSendToNumbers = true;
                        SMSNumbers = db.SMSData;
                        break;
                    case 2:
                        SMSSendToContact = true;
                        try
                        {
                            SMSContact = DBCTContact.FindByOid(ADatabase.Dxs, Guid.Parse(db.SMSData)).Title;
                        }
                        catch
                        {
                        }
                        break;
                    case 3:
                        SMSSendToCategory = true;
                        try
                        {
                            SMSCategory = DBCTContactCat.FindByOid(ADatabase.Dxs, Guid.Parse((db.SMSData))).Title;
                        }
                        catch
                        {
                        }
                        break;
                    case 4:
                        SMSSendToRelatedProfileItem = true;
                        break;
                    default:
                        SMSSendToAll = true;
                        break;
                }
                SMSData = db.SMSData;

                SMSSendToAnyOne = db.SMSSendToAnyOne;
                SMSNumbersAnyone = db.SMSNumbersAnyone;

            }
            #endregion

            #region Email Tab Populate
            EmailEnable = db.EmailEnable;
            if (EmailEnable)
            {
                EmailSelectedAccount = db.EmailAccount;
                EmailSelectedTemplate = db.EmailTemplate;
                EmailType = db.EmailType;
                switch (EmailType)
                {
                    case 0:
                        EmailSendToAll = true;
                        break;
                    case 1:
                        EmailSendToNumbers = true;
                        EmailNumbers = db.EmailData;
                        break;
                    case 2:
                        EmailSendToContact = true;
                        try
                        {
                            EmailContact = DBCTContact.FindByOid(ADatabase.Dxs, Guid.Parse(db.EmailData)).Title;
                        }
                        catch
                        {
                        }
                        break;
                    case 3:
                        EmailSendToCategory = true;
                        try
                        {
                            EmailCategory = DBCTContactCat.FindByOid(ADatabase.Dxs, Guid.Parse((db.EmailData))).Title;
                        }
                        catch
                        {
                        }
                        break;
                    case 4:
                        EmailSendToRelatedProfileItem = true;
                        break;
                }
                EmailData = db.EmailData;
            }
            #endregion
        }


        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(Title));
            CommandSMSSelectContact = new RelayCommand(SMSSelectContact, () => SMSSendToContact);
            CommandSMSSelectCategory = new RelayCommand(SMSSelectCategory, () => SMSSendToCategory);

            CommandEmailSelectContact = new RelayCommand(EmailSelectContact, () => EmailSendToContact);
            CommandEmailSelectCategory = new RelayCommand(EmailSelectCategory, () => EmailSendToCategory);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp18 != "");
        }



        private void SMSSelectContact()
        {
            var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var o = aPOCMainWindow.ShowSelectContact(DynamicOwner, null);
            if (o is DBCTContact)
            {
                SMSData = ((DBCTContact)o).Oid.ToString();
                SMSContact = ((DBCTContact)o).Title;
            }
        }
        private void SMSSelectCategory()
        {
            var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var o = aPOCMainWindow.ShowSelectContactCat(DynamicOwner);
            if (o is DBCTContactCat)
            {
                SMSData = ((DBCTContactCat)o).Oid.ToString();
                SMSCategory = ((DBCTContactCat)o).Title;
            }
        }
        private void SMSSelectBasket()
        {
        }

        private void EmailSelectContact()
        {
            var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var o = aPOCMainWindow.ShowSelectContact(DynamicOwner, null);
            if (o is DBCTContact)
            {
                EmailData = ((DBCTContact)o).Oid.ToString();
                EmailContact = ((DBCTContact)o).Title;
            }
        }
        private void EmailSelectCategory()
        {
            var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var o = aPOCMainWindow.ShowSelectContactCat(DynamicOwner);
            if (o is DBCTContactCat)
            {
                EmailData = ((DBCTContactCat)o).Oid.ToString();
                EmailCategory = ((DBCTContactCat)o).Title;
            }
        }
        private void EmailSelectBasket()
        {
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
            var db = DBTMAutomation.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicSelectedData, Title.Trim());
            if (db != null)
            {
                POLMessageBox.ShowWarning("عنوان تكراری می باشد.", MainView);
                return false;
            }
            if (!(PopupEnable || SMSEnable || EmailEnable))
            {
                POLMessageBox.ShowWarning("هیچ یك از موارد پیامك، پاپ اپ و یا ایمیل انتخاب نشده است.", MainView);
                return false;
            }

            if (PopupEnable)
            {
                if (string.IsNullOrWhiteSpace(PopupText))
                {
                    POLMessageBox.ShowWarning("خطای پاپ آپ : متن خالی می باشد.", MainView);
                    return false;
                }
            }

            if (SMSEnable)
            {
                if (string.IsNullOrWhiteSpace(SMSText))
                {
                    POLMessageBox.ShowWarning("خطای پیامك : متن خالی می باشد.", MainView);
                    return false;
                }
                if (SMSType == 1) 
                {
                    SMSData = SMSNumbers;
                    if (string.IsNullOrWhiteSpace(SMSData))
                    {
                        POLMessageBox.ShowWarning("خطای پیامك : شماره معتبر نمی باشد.", MainView);
                        return false;
                    }
                }
                if (SMSType == 2) 
                {
                    if (string.IsNullOrWhiteSpace(SMSData))
                    {
                        POLMessageBox.ShowWarning("خطای پیامك : پرونده معتبر نمی باشد.", MainView);
                        return false;
                    }
                }
                if (SMSType == 3) 
                {
                    if (string.IsNullOrWhiteSpace(SMSData))
                    {
                        POLMessageBox.ShowWarning("خطای پیامك : دسته معتبر نمی باشد.", MainView);
                        return false;
                    }
                }

                if (SMSSendToAnyOne) 
                {
                    if (string.IsNullOrWhiteSpace(SMSNumbersAnyone))
                    {
                        POLMessageBox.ShowWarning("خطای پیامك : شماره(ها) در بخش اختیاری خالی می باشد.", MainView);
                        return false;
                    }
                }
            }
            if (EmailEnable)
            {
                if (EmailSelectedTemplate == null)
                {
                    POLMessageBox.ShowWarning("خطای ایمیل : قالب ایمیل را انتخاب كنید.", MainView);
                    return false;
                }
                if (EmailSelectedAccount == null)
                {
                    POLMessageBox.ShowWarning("خطای ایمیل : حساب ایمیل را انتخاب كنید.", MainView);
                    return false;
                }
                if (EmailType == 1) 
                {
                    EmailData = EmailNumbers;
                    if (string.IsNullOrWhiteSpace(EmailData))
                    {
                        POLMessageBox.ShowWarning("خطای ایمیل : آدرس معتبر نمی باشد.", MainView);
                        return false;
                    }
                }
                if (EmailType == 2) 
                {
                    if (string.IsNullOrWhiteSpace(EmailData))
                    {
                        POLMessageBox.ShowWarning("خطای ایمیل : پرونده معتبر نمی باشد.", MainView);
                        return false;
                    }
                }
                if (EmailType == 3) 
                {
                    if (string.IsNullOrWhiteSpace(EmailData))
                    {
                        POLMessageBox.ShowWarning("خطای ایمیل : دسته معتبر نمی باشد.", MainView);
                        return false;
                    }
                }
            }
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
                        DynamicSelectedData = new DBTMAutomation(uow)
                        {
                            Title = Title.Trim(),
                            PopupEnable = PopupEnable,
                            PopupText = PopupText,

                            SMSEnable = SMSEnable,
                            SMSType = SMSType,
                            SMSText = SMSText,
                            SMSData = SMSData,
                            SMSSendToAnyOne = SMSSendToAnyOne,
                            SMSNumbersAnyone = SMSNumbersAnyone,

                            EmailAccount = EmailSelectedAccount == null ? null : DBEMEmailApp.FindByOid(uow, EmailSelectedAccount.Oid),
                            EmailEnable = EmailEnable,
                            EmailTemplate = EmailSelectedTemplate == null ? null : DBEMTemplate.FindByOid(uow, EmailSelectedTemplate.Oid),
                            EmailData = EmailData,
                            EmailType = EmailType,
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBTMAutomation.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    DynamicSelectedData.PopupEnable = PopupEnable;
                    DynamicSelectedData.PopupText = PopupText;

                    DynamicSelectedData.SMSData = SMSData;
                    DynamicSelectedData.SMSEnable = SMSEnable;
                    DynamicSelectedData.SMSText = SMSText;
                    DynamicSelectedData.SMSType = SMSType;

                    DynamicSelectedData.SMSSendToAnyOne = SMSSendToAnyOne;
                    DynamicSelectedData.SMSNumbersAnyone = SMSNumbersAnyone;

                    DynamicSelectedData.EmailAccount = EmailSelectedAccount;
                    DynamicSelectedData.EmailData = EmailData;
                    DynamicSelectedData.EmailEnable = EmailEnable;
                    DynamicSelectedData.EmailTemplate = EmailSelectedTemplate;
                    DynamicSelectedData.EmailType = EmailType;
                    DynamicSelectedData.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, MainView);
                return false;
            }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp18);
        } 
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }

        public RelayCommand CommandSMSSelectContact { get; set; }
        public RelayCommand CommandSMSSelectCategory { get; set; }

        public RelayCommand CommandEmailSelectContact { get; set; }
        public RelayCommand CommandEmailSelectCategory { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
