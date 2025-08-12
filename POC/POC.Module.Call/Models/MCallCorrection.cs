using System.Globalization;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.P30Office.GL;
using POL.Lib.Common;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using System;
using System.Windows;

namespace POC.Module.Call.Models
{
    public class MCallCorrection : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private POCCore APOCCore { get; set; }
        private IMembership AMembership { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCLCall DynamicDBCall { get; set; }

        public MCallCorrection(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitDynamics();
            InitCommands();
        }


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اصلاح شماره"; }
        }
        #endregion

        #region OriginalNumber
        private string _OriginalNumber;
        public string OriginalNumber
        {
            get { return _OriginalNumber; }
            set
            {
                _OriginalNumber = value;
                RaisePropertyChanged("OriginalNumber");
            }
        }
        #endregion
        #region CountryCode
        private string _CountryCode;
        public string CountryCode
        {
            get { return _CountryCode; }
            set
            {
                _CountryCode = value;
                RaisePropertyChanged("CountryCode");
            }
        }
        #endregion
        #region CityCode
        private string _CityCode;
        public string CityCode
        {
            get { return _CityCode; }
            set
            {
                _CityCode = value;
                RaisePropertyChanged("CityCode");
            }
        }
        #endregion
        #region PhoneNumber
        private string _PhoneNumber;
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set
            {
                _PhoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
            }
        }
        #endregion
        #region Extra
        private string _Extra;
        public string Extra
        {
            get { return _Extra; }
            set
            {
                _Extra = value;
                RaisePropertyChanged("Extra");
            }
        }
        #endregion


        #region [METHODS]
        private void InitDynamics()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicDBCall = MainView.DynamicDBCall;
            if (DynamicDBCall != null)
            {
                OriginalNumber = DynamicDBCall.OriginalCallerID;
                PhoneNumber = DynamicDBCall.PhoneNumber;
                if (DynamicDBCall.Country != null)
                {
                    CountryCode = DynamicDBCall.Country.TeleCode.ToString(CultureInfo.InvariantCulture);
                    CityCode = string.Empty;
                }
                if (DynamicDBCall.City != null)
                {
                    CountryCode = DynamicDBCall.City.Country.TeleCode.ToString(CultureInfo.InvariantCulture);
                    CityCode = DynamicDBCall.City.PhoneCode.ToString(CultureInfo.InvariantCulture);
                }
                Extra = DynamicDBCall.ExtraDialed;
            }
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                        {
                            DynamicOwner.DialogResult = true;
                            DynamicOwner.Close();
                        }

                }, () => !string.IsNullOrWhiteSpace(PhoneNumber));

            CommandCorrect = new RelayCommand(Correct, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp24 != "");
        }

        private void Correct()
        {
            var cityOid = APOCCore.STCI.CurrentCityGuid;
            var dbcity = DBGLCity.FindByOid(ADatabase.Dxs, cityOid);
            if (dbcity == null || dbcity.Country == null)
            {
                POLMessageBox.ShowError("كشور و شهر جاری بدرستی تنظیم نشده است.", DynamicOwner);
                return;
            }

            var pd = new PhoneDecoder3Code(
                ALogger,
                ADatabase.Dxs,

                dbcity.Country.Oid,
                dbcity.Country.TeleCode,

                cityOid,
                dbcity.PhoneCode,

                APOCCore.STCI.MobileLength,
                Convert.ToInt32(APOCCore.STCI.MobileStartingCode));

            var dd = pd.DecodeData(OriginalNumber, EnumCallType.CallOut);
            if (dd.HasError)
            {
                PhoneNumber = string.Empty;
                CountryCode = string.Empty;
                CityCode = string.Empty;
                Extra = string.Empty;
            }
            else
            {
                PhoneNumber = dd.Phone;
                CountryCode = dd.CountryCode;
                CityCode = dd.CityCode;
                Extra = dd.ExtraDialed;
            }
        }
        private bool Validate()
        {
            try
            {
                var code = Convert.ToInt32(CountryCode);
                if (code <= 0)
                    throw new Exception();
                var dbc = DBGLCountry.FindByTeleCode(ADatabase.Dxs, code);
                if (dbc == null)
                    throw new Exception();
            }
            catch
            {
                POLMessageBox.ShowError("كد كشور صحیح نمی باشد.", DynamicOwner);
                return false;
            }


            try
            {
                if (!string.IsNullOrWhiteSpace(CityCode))
                {
                    var code = Convert.ToInt32(CityCode);
                    if (code <= 0)
                        throw new Exception();
                    var dbc = DBGLCity.FindByCodes(ADatabase.Dxs, Convert.ToInt32(CountryCode), code);
                    if (dbc == null)
                        throw new Exception();
                }
            }
            catch
            {
                POLMessageBox.ShowError("كد شهر صحیح نمی باشد.", DynamicOwner);
                return false;
            }

            if (!PhoneNumber.IsDigital())
            {
                POLMessageBox.ShowError("شماره تلفن صحیح نمی باشد.", DynamicOwner);
                return false;
            }
            if (!string.IsNullOrWhiteSpace(Extra) && !Extra.IsDigital())
            {
                POLMessageBox.ShowError("شماره مازاد صحیح نمی باشد.", DynamicOwner);
                return false;
            }
            return true;
        }
        private bool Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CityCode))
                {
                    var code = Convert.ToInt32(CountryCode);
                    DynamicDBCall.Country = DBGLCountry.FindByTeleCode(ADatabase.Dxs, code);
                }
                else
                {
                    var countryCode = Convert.ToInt32(CountryCode);
                    var cityCode = Convert.ToInt32(CityCode);
                    DynamicDBCall.City = DBGLCity.FindByCodes(ADatabase.Dxs, countryCode, cityCode);
                }

                DynamicDBCall.PhoneNumber = PhoneNumber;
                DynamicDBCall.ExtraDialed = Extra;

                if (AMembership.IsAuthorized)
                    DynamicDBCall.ModifierUser = AMembership.ActiveUser.UserName;
                DynamicDBCall.Save();

                return true;
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
                return false;
            }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp24);
        } 
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCorrect { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
