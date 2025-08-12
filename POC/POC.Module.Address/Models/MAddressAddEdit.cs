using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.BT;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.DB.P30Office;

namespace POC.Module.Address.Models
{
    public class MAddressAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCTAddress DynamicSelectedData { get; set; }

        private static DBGLCity CurrentCity { get; set; }

        #region CTOR
        public MAddressAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            GetDynamicData();
            UpdateCountryList();
            UpdateTitleList();
            PopulateAddressTitle();
            PopulateData();
            InitCommands();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات آدرس"; }
        }
        #endregion


        #region TitleList
        private List<string> _TitleList;
        public List<string> TitleList
        {
            get { return _TitleList; }
            set
            {
                _TitleList = value;
                RaisePropertyChanged("TitleList");
            }
        }
        #endregion
        #region AddressTitle
        private string _AddressTitle;
        public string AddressTitle
        {
            get { return _AddressTitle; }
            set
            {
                if (_AddressTitle == value) return;
                _AddressTitle = value;
                RaisePropertyChanged("AddressTitle");
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


        #region Area
        private string _Area;
        public string Area
        {
            get { return _Area; }
            set
            {
                if (ReferenceEquals(_Area, value)) return;
                _Area = value;
                RaisePropertyChanged("Area");
            }
        }
        #endregion
        #region Address
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set
            {
                if (ReferenceEquals(_Address, value)) return;
                _Address = value;
                RaisePropertyChanged("Address");
            }
        }
        #endregion
        #region ZipCode
        private string _ZipCode;
        public string ZipCode
        {
            get { return _ZipCode; }
            set
            {
                if (ReferenceEquals(_ZipCode, value)) return;
                _ZipCode = value;
                RaisePropertyChanged("ZipCode");
            }
        }
        #endregion
        #region POBox
        private string _POBox;
        public string POBox
        {
            get { return _POBox; }
            set
            {
                if (ReferenceEquals(_POBox, value)) return;
                _POBox = value;
                RaisePropertyChanged("POBox");
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

        #region AddressTitleList
        private List<string> _AddressTitleList;
        public List<string> AddressTitleList
        {
            get { return _AddressTitleList; }
            set
            {
                _AddressTitleList = value;
                RaisePropertyChanged("AddressTitleList");
            }
        }
        #endregion

        #region [METHODS]
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
        }
        private void PopulateData()
        {
            if (CurrentCity == null)
                CurrentCity = DBGLCity.FindByOid(ADatabase.Dxs, APOCCore.STCI.CurrentCityGuid);

            SelectedCountry = CurrentCity.Country;
            SelectedCity = CurrentCity;

            if (DynamicSelectedData == null) return;
            var db = DynamicSelectedData;
            AddressTitle = db.Title;
            Area = db.Area;
            ZipCode = db.ZipCode;
            POBox = db.POBox;
            SelectedCity = db.City;
            if (SelectedCity != null)
                SelectedCountry = db.City.Country;
            Address = db.Address;
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
            CityList = DBGLCity.GetByCountryWithoutTeleCode(SelectedCountry.Session, SelectedCountry, null);
        }
        private void UpdateTitleList()
        {
            if (TitleList == null)
            {
                var xpc = DBBTAddressTitle2.GetAll(ADatabase.Dxs);
                TitleList = (from n in xpc select n.Title).ToList();
            }
        }
        private void PopulateAddressTitle()
        {
            AddressTitleList = (from n in DBBTAddressTitle2.GetAll(ADatabase.Dxs) select n.Title).ToList();
        }

        private void InitCommands()
        {
            CommandSaveClose = new RelayCommand(SaveClose, () => !string.IsNullOrWhiteSpace(Address));
            CommandSaveContinue = new RelayCommand(SaveContinue, () => !string.IsNullOrWhiteSpace(Address) && DynamicSelectedData == null);
            CommandManageCity = new RelayCommand(ManageCity, () => SelectedCountry != null);
            CommandManageAddressTitle = new RelayCommand(ManageAddressTitle, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp07 != "");
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp07);
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
                    POLMessageBox.ShowWarning("آدرس ثبت شد.", MainView);
                    DynamicSelectedData = null;
                    Area = string.Empty;
                    Address = string.Empty;
                    ZipCode = string.Empty;
                    POBox = string.Empty;
                    Note = string.Empty;
                    MainView.DynamicRefocus();
                }
        }
        private void ManageCity()
        {
            var poc = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            poc.ShowManageCity(DynamicOwner, SelectedCountry, false);
            UpdateCityList();
        }
        private void ManageAddressTitle()
        {
            var poc = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            poc.ShowManageAddressTitle(DynamicOwner);
            PopulateAddressTitle();
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Address))
            {
                POLMessageBox.ShowWarning("لطفا متن آدرس را وارد نمایید.", MainView);
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
                        DynamicSelectedData = new DBCTAddress(uow)
                        {
                            Title = AddressTitle,
                            City = SelectedCity == null ? null : DBGLCity.FindByOid(uow, SelectedCity.Oid),
                            Area = Area,
                            Address = Address,
                            ZipCode = ZipCode,
                            POBox = POBox,
                            Note = Note,
                            Contact = DBCTContact.FindByOid(uow, ((DBCTContact)APOCContactModule.SelectedContact).Oid),
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTAddress.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = AddressTitle;
                    DynamicSelectedData.City = SelectedCity;
                    DynamicSelectedData.Area = Area;
                    DynamicSelectedData.ZipCode = ZipCode;
                    DynamicSelectedData.POBox = POBox;
                    DynamicSelectedData.Note = Note;
                    DynamicSelectedData.Address = Address;
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
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandSaveContinue { get; set; }
        public RelayCommand CommandSaveClose { get; set; }
        public RelayCommand CommandManageCity { get; set; }
        public RelayCommand CommandManageAddressTitle { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
