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
    public class MWExtraCodeAddEdit : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private Window Owner { get; set; }
        private DBGLExtraCodes Data { get; set; }
        public bool AllowPhoneCode { get; set; }
        private dynamic MainView { get; set; }

        #region CTOR
        public MWExtraCodeAddEdit(Window owner, DBGLExtraCodes data, object mainView)
        {
            Owner = owner;
            Data = data;
            MainView = mainView;


            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            if (Data != null)
            {
                Title = Data.Title;
                CountryCode = Data.CountryCode;
                ExtraCode = Data.AreaCode;
            }
            InitCommands();
            GetDynamicData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "ویرایش اطلاعات "; }
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
        
        #region CountryCode
        private int _CountryCode;
        public int CountryCode
        {
            get { return _CountryCode; }
            set
            {
                _CountryCode = value;
                RaisePropertyChanged("CountryCode");
            }
        }
        #endregion
        #region ExtraCode
        private int _ExtraCode;
        public int ExtraCode
        {
            get { return _ExtraCode; }
            set
            {
                _ExtraCode = value;
                RaisePropertyChanged("ExtraCode");
            }
        }
        #endregion

        


        private void GetDynamicData()
        {
        }
        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                            RaiseRequestClose(true);
                }, () => !string.IsNullOrWhiteSpace(Title));
        }
        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
                Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("نام شهر معتبر نمی باشد. امكان ثبت وجود ندارد.", MainView);
                return false;
            }

            if (AllowPhoneCode)
            {
                if (CountryCode <= 0)
                {
                    POLMessageBox.ShowError("كد كشور میبایست بزرگتر از صفر باشد.", MainView);
                    return false;
                }
            }
            var db = DBGLExtraCodes.FindDuplicateTitleXXExcept(ADatabase.Dxs, Data, Title.Trim());
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
                        Data = new DBGLExtraCodes(uow)
                                   {
                                       Title = Title.Trim(),
                                       CountryCode = CountryCode,
                                       AreaCode = ExtraCode,
                                   };
                        uow.CommitChanges();
                    }
                    Data = DBGLExtraCodes.FindByOid(ADatabase.Dxs, Data.Oid);
                }
                else
                {
                    Data.Title = Title.Trim();
                    Data.CountryCode = CountryCode;
                    Data.AreaCode = ExtraCode;
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
