using System;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Call.Models
{
    public class MCallReportAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IMembership AMembership { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCLCallReport DynamicDBCallReport { get; set; }

        public MCallReportAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitDynamics();
            InitCommands();
        }


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
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion


        private void InitDynamics()
        {
            DynamicOwner = MainView as Window;
            DynamicDBCallReport = MainView.DynamicDBCallReport;
            if (DynamicDBCallReport != null)
                Title = DynamicDBCallReport.Title;
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
                            MainView.DynamicDBCallReport = DynamicDBCallReport;
                        }

                }, () => !string.IsNullOrWhiteSpace(Title));
        }

        private bool Validate()
        {
            var db = DBCLCallReport.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicDBCallReport, HelperConvert.CorrectPersianBug(Title.Trim()), AMembership.ActiveUser.UserName);
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
                if (DynamicDBCallReport == null)
                {
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        DynamicDBCallReport = new DBCLCallReport(uow)
                        {
                            Title = Title,
                        };
                        uow.CommitChanges();
                    }
                    DynamicDBCallReport = DBCLCallReport.FindByOid(ADatabase.Dxs, DynamicDBCallReport.Oid);
                }
                else
                {
                    DynamicDBCallReport.Title = Title;
                    DynamicDBCallReport.Save();
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
    }
}
