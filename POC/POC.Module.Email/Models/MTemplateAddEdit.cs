using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.DB.P30Office;
using POC.Module.Email.Views;

namespace POC.Module.Email.Models
{
    public class MTemplateAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBEMTemplate DynamicSelectedData { get; set; }

        #region CTOR
        public MTemplateAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            GetDynamicData();
            PopulateData();
            InitCommands();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات قالب"; }
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
        #region TemplateBody
        private string _TemplateBody;
        public string TemplateBody
        {
            get { return _TemplateBody; }
            set
            {
                if (_TemplateBody == value) return;
                _TemplateBody = value;
                RaisePropertyChanged("TemplateBody");
            }
        }



        #endregion



        #region ParamList
        private List<DBEMTempParams> _ParamList;
        public List<DBEMTempParams> ParamList
        {
            get { return _ParamList; }
            set
            {
                _ParamList = value;
                RaisePropertyChanged("ParamList");
            }
        }
        #endregion

        #region SelectedParam
        private DBEMTempParams _SelectedParam;
        public DBEMTempParams SelectedParam
        {
            get { return _SelectedParam; }
            set
            {
                if (_SelectedParam == value) return;
                _SelectedParam = value;
                RaisePropertyChanged("SelectedParam");
            }
        }



        #endregion




        #region [METHODS]

        private void GetDynamicData()
        {
            DynamicOwner = (Window)MainView;
            DynamicSelectedData = MainView.DynamicSelectedData;
        }
        private void PopulateData()
        {
            if (DynamicSelectedData == null) return;
            Title = DynamicSelectedData.Title;
            TemplateBody = DynamicSelectedData.HTMLBody;
            ParamList = DynamicSelectedData.Parameters.OrderBy(n => n.Tag).ToList();
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
                }, () => !string.IsNullOrWhiteSpace(Title));

            CommandParameterAdd = new RelayCommand(ParameterAdd, () => DynamicSelectedData != null);
            CommandParameterEdit = new RelayCommand(ParameterEdit, () => SelectedParam != null);
            CommandParameterDelete = new RelayCommand(ParameterDelete, () => SelectedParam != null);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp44 != "");
        }

        private void ParameterAdd()
        {
            var w = new WTemplateParameterAddEdit(null, DynamicSelectedData) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            DynamicSelectedData.Parameters.Reload();
            ParamList = DynamicSelectedData.Parameters.OrderBy(n => n.Tag).ToList();
            RaisePropertyChanged("ParamList");
        }

        private void ParameterEdit()
        {
            var w = new WTemplateParameterAddEdit(SelectedParam, DynamicSelectedData) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            DynamicSelectedData.Parameters.Reload();
            ParamList = DynamicSelectedData.Parameters.OrderBy(n => n.Tag).ToList();
            RaisePropertyChanged("ParamList");
        }

        private void ParameterDelete()
        {
            if (SelectedParam == null) return;
            var dr = POLMessageBox.ShowQuestionYesNo("متغییر انتخاب شده حذف شود؟");
            if (dr != MessageBoxResult.Yes) return;
            SelectedParam.Delete();
            SelectedParam.Save();

            DynamicSelectedData.Parameters.Reload();
            ParamList = DynamicSelectedData.Parameters.OrderBy(n => n.Tag).ToList();
            RaisePropertyChanged("ParamList");
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            var db = DBEMTemplate.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicSelectedData, HelperConvert.CorrectPersianBug(Title.Trim()));
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
                        DynamicSelectedData = new DBEMTemplate(uow)
                        {
                            Title = Title,
                            HTMLBody = TemplateBody,
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBEMTemplate.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    DynamicSelectedData.HTMLBody = TemplateBody.Trim();
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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp44);
        } 
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandParameterAdd { get; set; }
        public RelayCommand CommandParameterEdit { get; set; }
        public RelayCommand CommandParameterDelete { get; set; }
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
