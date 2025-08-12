using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.DB.P30Office;

namespace POC.Module.Email.Models
{
    public class MEmailAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCTEmail DynamicSelectedData { get; set; }

        #region CTOR
        public MEmailAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            GetDynamicData();
            PopulateEmailTitle();
            PopulateData();
            InitCommands();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات ایمیل"; }
        }
        #endregion


        #region EmailTitleList
        private List<string> _EmailTitleList;
        public List<string> EmailTitleList
        {
            get { return _EmailTitleList; }
            set
            {
                _EmailTitleList = value;
                RaisePropertyChanged("EmailTitleList");
            }
        }
        #endregion
        #region EmailTitle
        private string _EmailTitle;
        public string EmailTitle
        {
            get { return _EmailTitle; }
            set
            {
                if (_EmailTitle == value) return;
                _EmailTitle = value;
                RaisePropertyChanged("EmailTitle");
            }
        }



        #endregion

        #region EmailAddress
        private string _EmailAddress;
        public string EmailAddress
        {
            get { return _EmailAddress; }
            set
            {
                if (ReferenceEquals(_EmailAddress, value)) return;
                _EmailAddress = value;
                RaisePropertyChanged("EmailAddress");
            }
        }
        #endregion



        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
        }
        private void PopulateData()
        {
            if (DynamicSelectedData == null) return;
            var db = DynamicSelectedData;
            EmailTitle = db.Title;
            EmailAddress = db.Address;
        }

        private void PopulateEmailTitle()
        {
            EmailTitleList = (from n in DBBTEmailTitle2.GetAll(ADatabase.Dxs) select n.Title).ToList();
        }

        private void InitCommands()
        {
            CommandSaveClose = new RelayCommand(SaveClose, () => !string.IsNullOrWhiteSpace(EmailTitle) && !string.IsNullOrWhiteSpace(EmailAddress));
            CommandSaveContinue = new RelayCommand(SaveContinue, () => !string.IsNullOrWhiteSpace(EmailTitle) && !string.IsNullOrWhiteSpace(EmailAddress) && DynamicSelectedData == null);
            CommandManageEmailTitle = new RelayCommand(ManageEmailTitle, () => true);
        }

        private void SaveClose()
        {
            if (Validate())
                if (Save())
                {
                    MainView.DialogResult = true;
                    MainView.Close();
                    MainView.DynamicSelectedData = DynamicSelectedData;
                }
        }
        private void SaveContinue()
        {
            if (Validate())
                if (Save())
                {
                    POLMessageBox.ShowWarning("ایمیل ثبت شد.", MainView);
                    DynamicSelectedData = null;
                    EmailAddress = string.Empty;
                    EmailTitle = string.Empty;
                    MainView.DynamicRefocus();
                }
        }

        private void ManageEmailTitle()
        {
        }

        private bool Validate()
        {
            if (!EmailAddress.IsValidEmailAddress())
            {
                POLMessageBox.ShowWarning("آدرس ایمیل معتبر نمی باشد.", MainView);
                return false;
            }
            var dbe = DBCTEmail.FindByAddressExcept(ADatabase.Dxs, DynamicSelectedData, EmailAddress);
            if(dbe!=null)
            {
                POLMessageBox.ShowWarning("آدرس ایمیل قبلا ثبت شده، امكان ثبت مجدد نمی باشد.", MainView);
                return false;
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
                        DynamicSelectedData = new DBCTEmail(uow)
                        {
                            Title = EmailTitle,
                            Address = EmailAddress,
                            Contact = DBCTContact.FindByOid(uow, ((DBCTContact)APOCContactModule.SelectedContact).Oid),
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTEmail.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = EmailTitle.Trim();
                    DynamicSelectedData.Address = EmailAddress.Trim();
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

        #region [COMMANDS]
        public RelayCommand CommandSaveContinue { get; set; }
        public RelayCommand CommandSaveClose { get; set; }
        public RelayCommand CommandManageEmailTitle { get; set; }
        #endregion
    }
}
