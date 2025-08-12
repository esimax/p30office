using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.DB.P30Office;

namespace POC.Module.Phone.Models
{
    public class MPhoneAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCTPhoneBook DynamicSelectedData { get; set; }
        private bool FirstAnalysis { get; set; }

        private static DBGLCity CurrentCity { get; set; }

        #region CTOR
        public MPhoneAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            GetDynamicData();
            UpdateCountryList();
            PopulatePhoneTitle();
            PopulateData();
            InitCommands();

            FirstAnalysis = true;
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات شماره تماس"; }
        }
        #endregion



        #region PhoneNumber
        private string _PhoneNumber;
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set
            {
                if (_PhoneNumber == value) return;
                _PhoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
                AnalysisPhone();
            }
        }



        #endregion
        #region IsPhoneFax
        private bool _IsPhoneFax;
        public bool IsPhoneFax
        {
            get { return _IsPhoneFax; }
            set
            {
                if (!value && !IsMobile && !IsSMS)
                {
                    RaisePropertyChanged("IsPhoneFax");
                    return;
                }

                if (_IsPhoneFax == value) return;
                _IsPhoneFax = value;

                if (value)
                {
                    IsMobile = false;
                    IsSMS = false;
                }
                RaisePropertyChanged("IsPhoneFax");
                RaisePropertyChanged("CountryEnable");
                RaisePropertyChanged("CityEnable");

            }
        }
        #endregion
        #region IsMobile
        private bool _IsMobile;
        public bool IsMobile
        {
            get { return _IsMobile; }
            set
            {
                if (!value && !IsPhoneFax && !IsSMS)
                {
                    RaisePropertyChanged("IsMobile");
                    return;
                }

                if (_IsMobile == value) return;
                _IsMobile = value;

                if (value)
                {
                    IsPhoneFax = false;
                    IsSMS = false;
                }
                RaisePropertyChanged("IsMobile");
                RaisePropertyChanged("CountryEnable");
                RaisePropertyChanged("CityEnable");
            }
        }
        #endregion
        #region IsSMS
        private bool _IsSMS;
        public bool IsSMS
        {
            get { return _IsSMS; }
            set
            {
                if (!value && !IsPhoneFax && !IsMobile)
                {
                    RaisePropertyChanged("IsSMS");
                    return;
                }

                if (_IsSMS == value) return;
                _IsSMS = value;
                if (value)
                {
                    IsMobile = false;
                    IsPhoneFax = false;
                }
                RaisePropertyChanged("IsSMS");
                RaisePropertyChanged("CountryEnable");
                RaisePropertyChanged("CityEnable");
            }
        }
        #endregion
        #region CountryList
        private object _CountryList;
        public object CountryList
        {
            get { return _CountryList; }
            set
            {
                _CountryList = value;
                RaisePropertyChanged("CountryList");
            }
        }
        #endregion
        #region CityList
        private object _CityList;
        public object CityList
        {
            get { return _CityList; }
            set
            {
                _CityList = value;
                RaisePropertyChanged("CityList");
            }
        }
        #endregion
        #region SelectedCountry
        private DBGLCountry _SelectedCountry;
        public DBGLCountry SelectedCountry
        {
            get { return _SelectedCountry; }
            set
            {
                _SelectedCountry = value;
                RaisePropertyChanged("SelectedCountry");
                UpdateCityList();
            }
        }
        #endregion
        #region SelectedCity
        private DBGLCity _SelectedCity;
        public DBGLCity SelectedCity
        {
            get { return _SelectedCity; }
            set
            {
                if (ReferenceEquals(_SelectedCity, value)) return;
                _SelectedCity = value;
                RaisePropertyChanged("SelectedCity");
            }
        }
        #endregion
        #region PhoneTitle
        private string _PhoneTitle;
        public string PhoneTitle
        {
            get { return _PhoneTitle; }
            set
            {
                if (ReferenceEquals(_PhoneTitle, value)) return;
                _PhoneTitle = value;
                RaisePropertyChanged("PhoneTitle");
            }
        }
        #endregion
        #region Note
        private string _Note;
        public string Note
        {
            get { return _Note; }
            set
            {
                if (ReferenceEquals(_Note, value)) return;
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }
        #endregion

        #region CountryEnable
        public bool CountryEnable
        {
            get { return IsPhoneFax || IsMobile; }
        }
        #endregion

        #region CityEnable
        public bool CityEnable
        {
            get { return IsPhoneFax; }
        }
        #endregion

        #region PhoneTitleList
        private List<string> _PhoneTitleList;
        public List<string> PhoneTitleList
        {
            get { return _PhoneTitleList; }
            set
            {
                _PhoneTitleList = value;
                RaisePropertyChanged("PhoneTitleList");
            }
        }
        #endregion



        #region [METHOD]
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
        }
        private void PopulateData()
        {
            IsPhoneFax = true;
            if (CurrentCity == null)
                CurrentCity = DBGLCity.FindByOid(ADatabase.Dxs, APOCCore.STCI.CurrentCityGuid);
            if (CurrentCity != null)
                SelectedCountry = CurrentCity.Country;
            SelectedCity = CurrentCity;

            PhoneTitle = APOCCore.STCI.PhoneNumberDefaultTitle;

            if (DynamicSelectedData == null) return;
            var db = DynamicSelectedData;
            PhoneNumber = db.PhoneNumber;
            IsPhoneFax = db.PhoneType == EnumPhoneType.PhoneFax;
            IsMobile = db.PhoneType == EnumPhoneType.Mobile;
            IsSMS = db.PhoneType == EnumPhoneType.SMS;
            SelectedCountry = db.Country;
            SelectedCity = db.City;
            if (SelectedCity != null)
                SelectedCountry = db.City.Country;
            PhoneTitle = db.Title;
            Note = db.Note;
        }
        private void UpdateCountryList()
        {
            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            CountryList = (from n in aCacheData.GetCountryList() select (DBGLCountry)n.Tag).ToList();
        }
        private void UpdateCityList()
        {
            if (CountryList == null)
            {
                CityList = null;
                return;
            }
            if (SelectedCountry == null)
            {
                CityList = null;
                return;
            }
            CityList = DBGLCity.GetByCountryWithTeleCode(SelectedCountry.Session, SelectedCountry, null);
        }
        private void PopulatePhoneTitle()
        {
            PhoneTitleList = (from n in POL.DB.P30Office.BT.DBBTPhoneTitle2.GetAll(ADatabase.Dxs) select n.Title).ToList();
        }
        private void AnalysisPhone()
        {
            if (DynamicSelectedData != null || !FirstAnalysis) return;
            var mobileStart = APOCCore.STCI.MobileStartingCode;
            if (!mobileStart.StartsWith("0"))
                mobileStart = "0" + mobileStart;
            if (PhoneNumber == null || !PhoneNumber.StartsWith(mobileStart)) return;
            IsMobile = true;
            PhoneTitle = APOCCore.STCI.MobileDefaultTitle;
            FirstAnalysis = false;
        }

        private void InitCommands()
        {
            CommandSaveClose = new RelayCommand(SaveClose, () => !string.IsNullOrWhiteSpace(PhoneNumber));
            CommandSaveContinue = new RelayCommand(SaveContinue, () => !string.IsNullOrWhiteSpace(PhoneNumber) && DynamicSelectedData == null);
            CommandManageCity = new RelayCommand(ManageCity, () => SelectedCountry != null);
            CommandManagePhoneTitle = new RelayCommand(ManagePhoneTitle, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp51 != "");
        }

        private void SaveClose()
        {
            if (Validate())
                if (Save())
                {
                    MainView.DialogResult = true;
                    MainView.Close();
                }
        }
        private void SaveContinue()
        {
            if (Validate())
                if (Save())
                {
                    POLMessageBox.ShowWarning("شماره ثبت شد.", MainView);
                    DynamicSelectedData = null;
                    PhoneNumber = string.Empty;
                    FirstAnalysis = true;
                    MainView.DynamicRefocus();
                }
        }
        private void ManageCity()
        {
            var poc = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            poc.ShowManageCity(DynamicOwner, SelectedCountry, true);
            UpdateCityList();
        }
        private void ManagePhoneTitle()
        {
            var poc = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            poc.ShowManagePhoneTitle(DynamicOwner);
            PopulatePhoneTitle();
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber) || !PhoneNumber.IsDigital())
            {
                POLMessageBox.ShowWarning("شماره وارو شده معتبر نمی باشد.", MainView);
                return false;
            }
            if (IsPhoneFax)
            {
                if (PhoneNumber.StartsWith("0"))
                {
                    POLMessageBox.ShowWarning("امكان ثبت شماره ثابت كه با صفر شروع شود وجود ندارد.", MainView);
                    return false;
                }
                if (SelectedCountry == null || SelectedCity == null)
                {
                    POLMessageBox.ShowWarning("لطفا كشور و شهر را بدرستی انتخاب كنید.", MainView);
                    return false;
                }
                if (PhoneNumber.Length > SelectedCity.PhoneLen)
                {
                    POLMessageBox.ShowWarning(string.Format("اندازه شماره بزرگتر از حد مجاز ({0}) می باشد.", SelectedCity.PhoneLen), MainView);
                    return false;
                }

                var db = DBCTPhoneBook.FindByPhoneAndCodeExcept(ADatabase.Dxs, SelectedCountry.TeleCode1,
                                                       SelectedCountry.TeleCode2, SelectedCity.PhoneCode, PhoneNumber,
                                                       DynamicSelectedData);
                if (db != null)
                {
                    POLMessageBox.ShowWarning(string.Format("این شماره قبلا ثبت شده است : {0}{0}مشخصات پرونده :{0}    كد : {1}{0}    عنوان : {2}", Environment.NewLine, db.Contact.Code, db.Contact.Title), MainView);
                    return false;
                }
            }
            else if (IsMobile)
            {
                var mobileStart = APOCCore.STCI.MobileStartingCode;
                if (!mobileStart.StartsWith("0")) mobileStart = string.Format("0{0}", mobileStart);
                if (!PhoneNumber.StartsWith(mobileStart))
                {
                    POLMessageBox.ShowWarning("شماره های موبایل با " + mobileStart + " شروع می شود، لطفا تصحیح نمایید.", MainView);
                    return false;
                }
                if (SelectedCountry == null)
                {
                    POLMessageBox.ShowWarning("لطفا كشور را بدرستی انتخاب كنید.", MainView);
                    return false;
                }
                if (PhoneNumber.TrimStart('0').Length != APOCCore.STCI.MobileLength)
                {
                    POLMessageBox.ShowWarning(string.Format("اندازه شماره كوچكتر/بزرگتر از {0} كاراكتر می باشد.", APOCCore.STCI.MobileLength), MainView);
                    return false;
                }

                var db = DBCTPhoneBook.FindByPhoneAndCodeExcept(ADatabase.Dxs, SelectedCountry.TeleCode1,
                                                       SelectedCountry.TeleCode2, -1, PhoneNumber,
                                                       DynamicSelectedData);
                if (db != null)
                {
                    POLMessageBox.ShowWarning(string.Format("این شماره قبلا ثبت شده است : {0}{0}مشخصات پرونده :{0}    كد : {1}{0}    عنوان : {2}", Environment.NewLine, db.Contact.Code, db.Contact.Title), MainView);
                    return false;
                }
            }
            else
            {
                var db = DBCTPhoneBook.FindByPhoneAndCodeExcept(ADatabase.Dxs, -1,
                                                       -1, -1, PhoneNumber,
                                                       DynamicSelectedData);
                if (db != null)
                {
                    POLMessageBox.ShowWarning(string.Format("این شماره قبلا ثبت شده است : {0}{0}مشخصات پرونده :{0}    كد : {1}{0}    عنوان : {2}", Environment.NewLine, db.Contact.Code, db.Contact.Title), MainView);
                    return false;
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
                        DynamicSelectedData = new DBCTPhoneBook(uow)
                        {
                            PhoneNumber = PhoneNumber,
                            PhoneType = IsPhoneFax ? EnumPhoneType.PhoneFax : (IsMobile ? EnumPhoneType.Mobile : EnumPhoneType.SMS),
                            Country = (IsPhoneFax || IsMobile) ? DBGLCountry.FindByOid(uow, SelectedCountry.Oid) : null,
                            City = IsPhoneFax ? DBGLCity.FindByOid(uow, SelectedCity.Oid) : null,
                            Title = PhoneTitle,
                            Note = Note,

                            Contact = DBCTContact.FindByOid(uow, ((DBCTContact)APOCContactModule.SelectedContact).Oid),
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTPhoneBook.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.PhoneNumber = PhoneNumber;
                    DynamicSelectedData.PhoneType = IsPhoneFax
                                                        ? EnumPhoneType.PhoneFax
                                                        : (IsMobile ? EnumPhoneType.Mobile : EnumPhoneType.SMS);
                    DynamicSelectedData.Country = (IsPhoneFax || IsMobile) ? SelectedCountry : null;
                    DynamicSelectedData.City = IsPhoneFax ? SelectedCity : null;
                    DynamicSelectedData.Title = PhoneTitle;
                    DynamicSelectedData.Note = Note;
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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp51);
        }
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandSaveContinue { get; set; }
        public RelayCommand CommandSaveClose { get; set; }
        public RelayCommand CommandManageCity { get; set; }
        public RelayCommand CommandManagePhoneTitle { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
