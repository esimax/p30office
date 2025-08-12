using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.Utils;

namespace POC.Module.Contact.Models
{
    public class MContactCatAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCTContactCat DynamicSelectedData { get; set; }

        #region CTOR
        public MContactCatAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitCommands();
            GetDynamicData();
            PopulateRoleList();
            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات عنوان دسته بندی پرونده ها"; }
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
        #region RoleList
        public List<string> RoleList { get; set; }
        #endregion
        #region SelectedRole
        private string _SelectedRole;
        public string SelectedRole
        {
            get { return _SelectedRole; }
            set
            {
                if (ReferenceEquals(_SelectedRole, value)) return;
                _SelectedRole = value;
                RaisePropertyChanged("SelectedRole");
            }
        }
        #endregion

        

        #region [METHODS]

        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(Title));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp29 != "");
        }

        private void OK()
        {
            if (Validate())
                if (Save())
                {
                    DynamicOwner.DialogResult = true;
                    DynamicOwner.Close();
                }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp29);
        }


        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            var db = DBCTContactCat.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicSelectedData, HelperConvert.CorrectPersianBug(Title.Trim()));
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
                        DynamicSelectedData = new DBCTContactCat(uow)
                        {
                            Title = Title.Trim(),
                            Role = SelectedRole,
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTContactCat.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    DynamicSelectedData.Role = SelectedRole;
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
            SelectedRole = DynamicSelectedData.Role;
        }
        private void PopulateRoleList()
        {
            RoleList = new List<string> { "" };
            var v = (from n in ACacheData.GetRoleList() orderby n.Title select n.Title).ToArray();
            RoleList.AddRange(v );
        }

        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicSelectedData = MainView.DynamicSelectedData;
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
