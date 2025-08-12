using System;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.CountryCity.Models
{
    public class MCityAddEdit : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private Window Owner { get; set; }
        private DBGLCity Data { get; set; }
        public bool AllowPhoneCode { get; set; }
        private dynamic MainView { get; set; }

        #region CTOR
        public MCityAddEdit(Window owner, DBGLCity data, object mainView)
        {
            Owner = owner;
            Data = data;
            MainView = mainView;


            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            if (Data != null)
            {
                TitleXX = Data.TitleXX;
                StateTitle = Data.StateTitle;
                PhoneCode = Data.PhoneCode;
                PhoneLen = Data.PhoneLen;
            }
            InitCommands();
            GetDynamicData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "ویرایش اطلاعات شهر"; }
        }
        #endregion

        #region TitleXX
        private string _TitleXX;
        public string TitleXX
        {
            get { return _TitleXX; }
            set
            {
                _TitleXX = value;
                RaisePropertyChanged("TitleXX");
            }
        }
        #endregion
        #region StateTitle
        private string _StateTitle;
        public string StateTitle
        {
            get { return _StateTitle; }
            set
            {
                _StateTitle = value;
                RaisePropertyChanged("StateTitle");
            }
        }
        #endregion
        #region PhoneCode
        private int _PhoneCode;
        public int PhoneCode
        {
            get { return _PhoneCode; }
            set
            {
                _PhoneCode = value;
                RaisePropertyChanged("PhoneCode");
            }
        }
        #endregion
        #region PhoneLen
        private int _PhoneLen;
        public int PhoneLen
        {
            get { return _PhoneLen; }
            set
            {
                _PhoneLen = value;
                RaisePropertyChanged("PhoneLen");
            }
        }
        #endregion

        public DBGLCountry Country { get; set; }
        public string CountryTitle { get; set; }


        #region [METHODS]
        private void GetDynamicData()
        {
            try
            {
                AllowPhoneCode = MainView.AllowPhoneCode;
                Country = MainView.Country;
                CountryTitle = Country.TitleXX;
            }
            catch
            {
            }
        }
        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                            RaiseRequestClose(true);
                }, () => !string.IsNullOrWhiteSpace(TitleXX));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp36 != "");
        }
        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(TitleXX))
                TitleXX = string.Empty;
            if (string.IsNullOrWhiteSpace(TitleXX.Trim()))
            {
                POLMessageBox.ShowError("نام شهر معتبر نمی باشد. امكان ثبت وجود ندارد.", MainView);
                return false;
            }

            if (AllowPhoneCode)
            {
                if (PhoneCode <= 0)
                {
                    POLMessageBox.ShowError("كد مخابراتی میبایست بزرگتر از صفر باشد.", MainView);
                    return false;
                }
                if (PhoneLen <= 3 || PhoneLen >= 13)
                {
                    POLMessageBox.ShowError("اندازه شماره میبایست بین چهار تا دوازده باشد.", MainView);
                    return false;
                }

                if (Data == null || Data.PhoneCode != PhoneCode)
                {
                    var db1 = DBGLCity.FindByCodes(ADatabase.Dxs, Country.TeleCode, PhoneCode);
                    if (db1 != null)
                    {
                        POLMessageBox.ShowError("كد مخابراتی تكراری می باشد.", MainView);
                        return false;
                    }
                }
            }
            var db = DBGLCity.FindDuplicateTitleXXExcept(ADatabase.Dxs, Data, TitleXX.Trim(), AllowPhoneCode);
            if (db != null)
            {
                POLMessageBox.ShowError("نام شهر تكراری می باشد. امكان ثبت وجود ندارد.", MainView);
                return false;
            }
            return true;
        }
        private bool Save()
        {
            try
            {
                if (Data == null)
                {
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        Data = new DBGLCity(uow)
                                   {
                                       TitleXX = TitleXX.Trim(),
                                       StateTitle = StateTitle.Trim(),
                                       Country = DBGLCountry.FindByOid(uow, Country.Oid),
                                       PhoneCode = AllowPhoneCode ? PhoneCode : -1,
                                       PhoneLen = PhoneLen,
                                   };
                        uow.CommitChanges();
                    }
                    Data = DBGLCity.FindByOid(ADatabase.Dxs, Data.Oid);
                }
                else
                {
                    Data.TitleXX = TitleXX.Trim();
                    Data.StateTitle = StateTitle.Trim();
                    Data.PhoneCode = AllowPhoneCode ? PhoneCode : -1;
                    Data.PhoneLen = AllowPhoneCode ? PhoneLen : -1; ;
                    Data.Save();
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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp36);
        } 
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IRequestCloseViewModel
        public event EventHandler<RequestCloseEventArgs> RequestClose;
        private void RaiseRequestClose(bool? dialogResult)
        {
            if (RequestClose != null)
                RequestClose.Invoke(this, new RequestCloseEventArgs { DialogResult = dialogResult });
        }
        #endregion
    }
}
