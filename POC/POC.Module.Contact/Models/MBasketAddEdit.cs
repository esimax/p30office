using System;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using POL.Lib.Utils;

namespace POC.Module.Contact.Models
{
    public class MBasketAddEdit : NotifyObjectBase, IRequestCloseViewModel
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private Window Owner { get; set; }
        private DBCTContactSelection Data { get; set; }


        #region CTOR
        public MBasketAddEdit(Window owner, DBCTContactSelection data)
        {
            Owner = owner;
            Data = data;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            if (Data != null)
            {
                Title = Data.Title;
            }
            InitCommands();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return Data == null ? "ایجاد سبد انتخاب" : "ویرایش سبد انتخاب"; }
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
            var db = DBCTContactSelection.FindDuplicateTitleExcept(ADatabase.Dxs, Data, HelperConvert.CorrectPersianBug(Title.Trim()));
            if (db != null)
            {
                POLMessageBox.ShowError("عنوان سبد انتخاب تكراری می باشد. امكان ثبت وجود ندارد.", Owner);
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
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer) )
                    {
                        Data = new DBCTContactSelection(uow)
                                   {
                                       Title = Title,
                                       User2 = DBMSUser2.UserGetByOid(uow, AMembership.ActiveUser.UserID),
                                   };
                        uow.CommitChanges();
                    }
                    Data = DBCTContactSelection.FindByOid(ADatabase.Dxs, Data.Oid);
                }
                else
                {
                    Data.Title = Title;
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
