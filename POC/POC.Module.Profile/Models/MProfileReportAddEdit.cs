using System;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.Utils;

namespace POC.Module.Profile.Models
{
    public class MProfileReportAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private DBCTProfileReport DynamicSelectedData { get; set; }

        #region CTOR
        public MProfileReportAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitCommands();
            GetDynamicData();
            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات گزارش"; }
        }
        #endregion



        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (ReferenceEquals(_Title, value)) return;
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion
        #region CCreator
        private bool _CCreator;
        public bool CCreator
        {
            get { return _CCreator; }
            set
            {
                if (value == _CCreator)
                    return;

                _CCreator = value;
                RaisePropertyChanged("CCreator");
            }
        }
        #endregion
        #region CCreatedAt
        private bool _CCreatedAt;
        public bool CCreatedAt
        {
            get { return _CCreatedAt; }
            set
            {
                if (value == _CCreatedAt)
                    return;

                _CCreatedAt = value;
                RaisePropertyChanged("CCreatedAt");
            }
        }
        #endregion
        #region CEditor
        private bool _CEditor;
        public bool CEditor
        {
            get { return _CEditor; }
            set
            {
                if (value == _CEditor)
                    return;

                _CEditor = value;
                RaisePropertyChanged("CEditor");
            }
        }
        #endregion
        #region CEditedAt
        private bool _CEditedAt;
        public bool CEditedAt
        {
            get { return _CEditedAt; }
            set
            {
                if (value == _CEditedAt)
                    return;

                _CEditedAt = value;
                RaisePropertyChanged("CEditedAt");
            }
        }
        #endregion

        #region CCategory
        private bool _CCategory;
        public bool CCategory
        {
            get { return _CCategory; }
            set
            {
                if (value == _CCategory)
                    return;

                _CCategory = value;
                RaisePropertyChanged("CCategory");
            }
        }
        #endregion
        #region CPhone
        private bool _CPhone;
        public bool CPhone
        {
            get { return _CPhone; }
            set
            {
                if (value == _CPhone)
                    return;

                _CPhone = value;
                RaisePropertyChanged("CPhone");
            }
        }
        #endregion
        #region CEmail
        private bool _CEmail;
        public bool CEmail
        {
            get { return _CEmail; }
            set
            {
                if (value == _CEmail)
                    return;

                _CEmail = value;
                RaisePropertyChanged("CEmail");
            }
        }
        #endregion
        #region CAddress
        private bool _CAddress;
        public bool CAddress
        {
            get { return _CAddress; }
            set
            {
                if (value == _CAddress)
                    return;

                _CAddress = value;
                RaisePropertyChanged("CAddress");
            }
        }
        #endregion






        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(Title));
        }

        private void OK()
        {
            if (Validate())
                if (Save())
                {
                    MainView.DynamicSelectedData = DynamicSelectedData;
                    MainView.DialogResult = true;
                    MainView.Close();
                }
        }



        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.", MainView);
                return false;
            }

            var db = DBCTProfileReport.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicSelectedData, HelperConvert.CorrectPersianBug(Title.Trim()));
            if (db != null)
            {
                POLMessageBox.ShowError("عنوان تكراری می باشد. امكان ثبت وجود ندارد.", MainView);
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
                        DynamicSelectedData = new DBCTProfileReport(uow)
                        {
                            Title = Title.Trim(),
                            CCreator = CCreator,
                            CCreatedAt = CCreatedAt,
                            CEditedAt = CEditedAt,
                            CEditor = CEditor,
                            CCategory = CCategory,
                            CPhone = CPhone,
                            CEmail = CEmail,
                            CAddress = CAddress,
                        };
                        DynamicSelectedData.Save();
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTProfileReport.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    DynamicSelectedData.CCreatedAt = CCreatedAt;
                    DynamicSelectedData.CCreator = CCreator;
                    DynamicSelectedData.CEditedAt = CEditedAt;
                    DynamicSelectedData.CEditor = CEditor;

                    DynamicSelectedData.CCategory = CCategory;
                    DynamicSelectedData.CPhone = CPhone;
                    DynamicSelectedData.CEmail = CEmail;
                    DynamicSelectedData.CAddress = CAddress;
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

        private void PopulateData()
        {
            if (DynamicSelectedData == null) return;
            Title = DynamicSelectedData.Title;
            CCreatedAt = DynamicSelectedData.CCreatedAt;
            CCreator = DynamicSelectedData.CCreator;
            CEditedAt = DynamicSelectedData.CEditedAt;
            CEditor = DynamicSelectedData.CEditor;

            CCategory = DynamicSelectedData.CCategory;
            CPhone = DynamicSelectedData.CPhone;
            CEmail = DynamicSelectedData.CEmail;
            CAddress = DynamicSelectedData.CAddress;
        }
        private void GetDynamicData()
        {
            DynamicSelectedData = MainView.DynamicSelectedData;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion
    }
}
