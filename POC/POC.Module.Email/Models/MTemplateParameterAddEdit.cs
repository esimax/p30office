using System;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.DB.P30Office;

namespace POC.Module.Email.Models
{
    public class MTemplateParameterAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private POCCore APOCCore { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBEMTempParams DynamicSelectedData { get; set; }
        private DBEMTemplate DynamicParentTemp { get; set; }

        #region CTOR
        public MTemplateParameterAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            GetDynamicData();
            PopulateTagTypeList();
            PopulateModuleList();
            PopulateData();
            InitCommands();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات پارامتر قالب"; }
        }
        #endregion

        #region TagTitle
        private string _TagTitle;
        public string TagTitle
        {
            get { return _TagTitle; }
            set
            {
                if (ReferenceEquals(_TagTitle, value)) return;
                _TagTitle = value;
                RaisePropertyChanged("TagTitle");
            }
        }
        #endregion
        #region TagTypeList
        private List<string> _TagTypeList;
        public List<string> TagTypeList
        {
            get { return _TagTypeList; }
            set
            {
                _TagTypeList = value;
                RaisePropertyChanged("TagTypeList");
            }
        }
        #endregion
        #region TagTypeIndex
        private int _TagTypeIndex;
        public int TagTypeIndex
        {
            get { return _TagTypeIndex; }
            set
            {
                if (_TagTypeIndex == value) return;
                _TagTypeIndex = value;
                RaisePropertyChanged("TagTypeIndex");
                RaisePropertyChanged("CanSelectModule");
            }
        }
        #endregion
        #region CanSelectModule
        public bool CanSelectModule
        {
            get { return TagTypeIndex == 2; }
        }
        #endregion

        #region SelectedModule
        private DBCTProfileItem _SelectedModule;
        public DBCTProfileItem SelectedModule
        {
            get { return _SelectedModule; }
            set
            {
                if (value == _SelectedModule)
                    return;

                _SelectedModule = value;
                RaisePropertyChanged("SelectedModule");
            }
        }
        #endregion






        private void GetDynamicData()
        {
            DynamicOwner = (Window)MainView;
            DynamicSelectedData = MainView.DynamicSelectedData;
            DynamicParentTemp = MainView.DynamicParentTemp;
        }
        private void PopulateTagTypeList()
        {
            TagTypeList = new List<string>() { "متن تك خط", "متن چند خط", "فیلد" };
        }
        private void PopulateModuleList()
        {

        }
        private void PopulateData()
        {
            if (DynamicSelectedData == null) return;
            TagTitle = DynamicSelectedData.Tag;
            TagTypeIndex = (int)DynamicSelectedData.TagType;
            if (DynamicSelectedData.ProfileItem != null)
            {
                SelectedModule = DynamicSelectedData.ProfileItem;
            }
            else
            {
                SelectedModule = null;
            }
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
                }, () => !string.IsNullOrWhiteSpace(TagTitle));
            CommandSelectModule = new RelayCommand(SelectModule, () => true);
        }

        private void SelectModule()
        {
            var sm = APOCMainWindow.ShowSelectProfileItem(DynamicOwner, null);
            if (sm == null) return;
            SelectedModule = sm as DBCTProfileItem;
        }



        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(TagTitle)) TagTitle = string.Empty;
            if (string.IsNullOrWhiteSpace(TagTitle.Trim()))
            {
                POLMessageBox.ShowError("عنوان تگ معتبر نمی باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }

            var db = DBEMTempParams.FindDuplicateTagExcept(ADatabase.Dxs, DynamicParentTemp, DynamicSelectedData, HelperConvert.CorrectPersianBug(TagTitle.Trim()));
            if (db != null)
            {
                POLMessageBox.ShowError("عنوان تگ تكراری می باشد. امكان ثبت وجود ندارد.", DynamicOwner);
                return false;
            }


            if (CanSelectModule)
            {
                if (SelectedModule == null )
                {
                    POLMessageBox.ShowError("فیلد انتخاب شده معتبر نمی باشد.", DynamicOwner);
                    return false;
                }
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
                        DynamicSelectedData = new DBEMTempParams(uow)
                        {
                            Tag = TagTitle,
                            TagType = (EnumEmailTemplateTagType)TagTypeIndex,
                            ProfileItem = CanSelectModule ? DBCTProfileItem.FindByOid(uow, SelectedModule.Oid ) : null,
                            Template = DBEMTemplate.FindByOid(uow, DynamicParentTemp.Oid),
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBEMTempParams.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
                }
                else
                {
                    DynamicSelectedData.Tag = TagTitle.Trim();
                    DynamicSelectedData.TagType = (EnumEmailTemplateTagType)TagTypeIndex;
                    DynamicSelectedData.ProfileItem = CanSelectModule ? SelectedModule : null;
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

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandSelectModule { get; set; }
        #endregion
    }
}
