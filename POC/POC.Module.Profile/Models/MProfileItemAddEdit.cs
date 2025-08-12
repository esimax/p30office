using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class MProfileItemAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }

        private dynamic MainView { get; set; }
        private DBCTProfileItem DynamicSelectedData { get; set; }
        private DBCTProfileGroup DynamicProfileGroup { get; set; }


        #region CTOR
        public MProfileItemAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            ADataFieldManager = ServiceLocator.Current.GetInstance<IDataFieldManager>();


            InitCommands();
            GetDynamicData();
            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات فیلد"; }
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


        #region ModuleTypeList
        public ObservableCollection<IDataField> ModuleTypeList
        {
            get { return ADataFieldManager.GetRegisterFields(); }
        }
        #endregion
        #region SelectedModuleType
        private IDataField _SelectedModuleType;
        public IDataField SelectedModuleType
        {
            get { return _SelectedModuleType; }
            set
            {
                if (_SelectedModuleType == value) return;
                _SelectedModuleType = value;
                RaisePropertyChanged("SelectedModuleType");
                if (DynamicSelectedData == null)
                    POL.Lib.XOffice.HelperSettingsClient.ModuleLastItemType = (int)value.ItemType;
            }
        }
        #endregion



        #region CanChangeType
        public bool CanChangeType
        {
            get { return DynamicSelectedData == null; }
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
                    MainView.DynamicSelectedData = DynamicSelectedData;
                    MainView.DialogResult = true;
                    MainView.Close();
                }
        }



        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.", MainView);
                return false;
            }

            var db = DBCTProfileItem.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicProfileGroup.Oid, DynamicSelectedData, HelperConvert.CorrectPersianBug(Title.Trim()));
            if (db != null)
            {
                POLMessageBox.ShowError("عنوان تكراری می باشد. امكان ثبت وجود ندارد.", MainView);
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
                        DynamicSelectedData = new DBCTProfileItem(uow)
                        {
                            Title = Title.Trim(),
                            ProfileGroup = DBCTProfileGroup.FindByOid(uow, DynamicProfileGroup.Oid),
                            ItemType = SelectedModuleType.ItemType,
                            Order = DBCTProfileItem.GetLastOrder(uow, DynamicProfileGroup.Oid) + 1,
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTProfileItem.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Title = Title.Trim();
                    DynamicSelectedData.ItemType = SelectedModuleType.ItemType;
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

        private void PopulateData()
        {
            SelectedModuleType = (from n in ModuleTypeList where ((int)n.ItemType) == POL.Lib.XOffice.HelperSettingsClient.ModuleLastItemType select n).FirstOrDefault();
            if (DynamicSelectedData == null) return;
            Title = DynamicSelectedData.Title;
            SelectedModuleType = (from n in ModuleTypeList where n.ItemType == DynamicSelectedData.ItemType select n).FirstOrDefault(); 
        }
        private void GetDynamicData()
        {
            DynamicSelectedData = MainView.DynamicSelectedData;
            DynamicProfileGroup = MainView.DynamicProfileGroup;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion
    }
}
