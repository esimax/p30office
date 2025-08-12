using System;
using System.Windows;
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
    public class MProfileTValueAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCTProfileTValue DynamicSelectedData { get; set; }
        private DBCTProfileTable DynamicTable { get; set; }
        private DBCTProfileTValue DynamicParentValue { get; set; }

        #region CTOR
        public MProfileTValueAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "مقدار جدوال"; }
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
        #region Order
        private object _Order;
        public object Order
        {
            get { return _Order; }
            set
            {
                if (_Order == value) return;
                _Order = value;
                RaisePropertyChanged("Order");
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
                    DynamicTable.UpdateDepthValue();
                    DynamicOwner.DialogResult = true;
                    DynamicOwner.Close();
                    MainView.DynamicSelectedData = DynamicSelectedData;
                }
        }



        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            var db = DBCTProfileTValue.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicTable.Oid, DynamicSelectedData, HelperConvert.CorrectPersianBug(Title.Trim()), DynamicParentValue);
            if (db != null)
            {
                POLMessageBox.ShowError("عنوان تكراری می باشد. امكان ثبت وجود ندارد.", DynamicOwner);
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
                        DynamicSelectedData = new DBCTProfileTValue(uow)
                        {
                            Title = Title.Trim(),
                            Order = Convert.ToInt32(Order),
                            Parent = DynamicParentValue == null ? null : DBCTProfileTValue.FindByOid(uow, DynamicParentValue.Oid),
                        };

                        var main = DBCTProfileTable.FindByOid(uow, DynamicTable.Oid);
                        DynamicSelectedData.Table = main;
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTProfileTValue.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    DynamicSelectedData.Order = Convert.ToInt32(Order);
                    DynamicSelectedData.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, DynamicOwner);
                return false;
            }
        }

        private void PopulateData()
        {
            if (DynamicSelectedData == null) return;
            Title = DynamicSelectedData.Title;
            Order = DynamicSelectedData.Order;
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
            DynamicTable = MainView.DynamicTable;
            DynamicParentValue = MainView.DynamicParentValue;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion
    }
}
