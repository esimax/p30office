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
    public class MProfileTableAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCTProfileTable DynamicSelectedData { get; set; }

        #region CTOR
        public MProfileTableAddEdit(object mainView)
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
            get { return "اطلاعات جدول"; }
        }
        #endregion


  
        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (ReferenceEquals(_Title, value))return;
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion






        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(Title));
        }

        private  void OK()
        {
            if(Validate())
                if (Save())
                {
                    DynamicOwner.DialogResult = true;
                    DynamicOwner.Close();
                    MainView.DynamicSelectedData = DynamicSelectedData;
                }
        }

        

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.",DynamicOwner);
                return false;
            }

            var db = DBCTProfileTable.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicSelectedData, HelperConvert.CorrectPersianBug(Title.Trim()));
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
                        DynamicSelectedData = new DBCTProfileTable(uow)
                        {
                            Title = Title.Trim(),
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTProfileTable.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
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
            if(DynamicSelectedData==null)return;
            Title = DynamicSelectedData.Title;
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner; 
            DynamicSelectedData = MainView.DynamicSelectedData;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion
    }
}
