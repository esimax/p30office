using System;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.CountryCity.Models
{
    public class MCountryAddEdit : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private Window Owner { get; set; }
        private DBGLCountry Data { get; set; }


        #region CTOR
        public MCountryAddEdit(Window owner, DBGLCountry data)
        {
            Owner = owner;
            Data = data;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            if (Data != null)
            {
                TitleXX = Data.TitleXX;
                TitleEn = Data.TitleEn;
                CurrencyName = Data.CurrencyName;
                CurrencyCode = Data.CurrencyCode;
                CurrencySymbol = Data.CurrencySymbol;
                TeleCode1 = Data.TeleCode1;
                TeleCode2 = Data.TeleCode2;
                TimeZoneString = Data.TimeZoneString;
            }
            InitCommands();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "ویرایش اطلاعات كشور"; }
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
        #region TitleEn
        private string _TitleEn;
        public string TitleEn
        {
            get { return _TitleEn; }
            set
            {
                _TitleEn = value;
                RaisePropertyChanged("TitleEn");
            }
        }
        #endregion
        #region CurrencyName
        private string _CurrencyName;
        public string CurrencyName
        {
            get { return _CurrencyName; }
            set
            {
                _CurrencyName = value;
                RaisePropertyChanged("CurrencyName");
            }
        }
        #endregion
        #region CurrencyCode
        private string _CurrencyCode;
        public string CurrencyCode
        {
            get { return _CurrencyCode; }
            set
            {
                _CurrencyCode = value;
                RaisePropertyChanged("CurrencyCode");
            }
        }
        #endregion
        #region CurrencySymbol
        private string _CurrencySymbol;
        public string CurrencySymbol
        {
            get { return _CurrencySymbol; }
            set
            {
                _CurrencySymbol = value;
                RaisePropertyChanged("CurrencySymbol");
            }
        }
        #endregion
        #region TeleCode1
        private int _TeleCode1;
        public int TeleCode1
        {
            get { return _TeleCode1; }
            set
            {
                _TeleCode1 = value;
                RaisePropertyChanged("TeleCode1");
            }
        }
        #endregion
        #region TeleCode2
        private int _TeleCode2;
        public int TeleCode2
        {
            get { return _TeleCode2; }
            set
            {
                _TeleCode2 = value;
                RaisePropertyChanged("TeleCode2");
            }
        }
        #endregion
        #region TimeZoneString
        private string _TimeZoneString;
        public string TimeZoneString
        {
            get { return _TimeZoneString; }
            set
            {
                _TimeZoneString = value;
                RaisePropertyChanged("TimeZoneString");
            }
        }
        #endregion



        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                            RaiseRequestClose(true);
                }, () => !string.IsNullOrWhiteSpace(TitleXX));
        }
        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(TitleXX))
                TitleXX = string.Empty;
            if (string.IsNullOrWhiteSpace(TitleXX.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            var db = DBGLCountry.FindDuplicateTitleXXExcept(ADatabase.Dxs, Data, TitleXX.Trim());
            if (db != null)
            {
                POLMessageBox.ShowError("عنوان تكراری می باشد. امكان ثبت وجود ندارد.", Owner);
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
                        Data = new DBGLCountry(uow)
                                   {
                                       TitleXX = TitleXX.Trim(),
                                       CurrencyCode = CurrencyCode.Trim(),
                                       CurrencyName = CurrencyName.Trim(),
                                       CurrencySymbol = CurrencySymbol.Trim(),
                                       TeleCode1 = TeleCode1,
                                       TeleCode2 = TeleCode2,
                                       TimeZoneString = TimeZoneString,
                                   };
                        uow.CommitChanges();
                    }
                    Data = DBGLCountry.FindByOid(ADatabase.Dxs, Data.Oid);
                }
                else
                {
                    Data.TitleXX = TitleXX.Trim();
                    Data.CurrencyCode = CurrencyCode.Trim();
                    Data.CurrencyName = CurrencyName.Trim();
                    Data.CurrencySymbol = CurrencySymbol.Trim();
                    Data.TeleCode1 = TeleCode1;
                    Data.TeleCode2 = TeleCode2;
                    Data.TimeZoneString = TimeZoneString;
                    Data.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, Owner);
                return false;
            }
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
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
