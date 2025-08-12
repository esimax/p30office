using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.Utils;
using DevExpress.Xpo;

namespace POC.Module.Profile.Models
{
    public class MListAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private DBCTList DynamicSelectedData { get; set; }


        #region CTOR
        public MListAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitCommands();
            GetDynamicData();
            PopulateRootList();
            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات لیست"; }
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

        #region SelectedProfileRoot
        private DBCTProfileRoot _SelectedProfileRoot;
        public DBCTProfileRoot SelectedProfileRoot
        {
            get { return _SelectedProfileRoot; }
            set
            {
                if (ReferenceEquals(_SelectedProfileRoot, value)) return;
                _SelectedProfileRoot = value;
                RaisePropertyChanged("SelectedProfileRoot");
                SelectedProfileGroup = null;
                PopulateGroupList();
            }
        }
        #endregion
        #region ProfileRootList
        private List<DBCTProfileRoot> _ProfileRootList;
        public List<DBCTProfileRoot> ProfileRootList
        {
            get { return _ProfileRootList; }
            set
            {
                if (ReferenceEquals(_ProfileRootList, value)) return;
                _ProfileRootList = value;
                RaisePropertyChanged("ProfileRootList");
            }
        }
        #endregion

        #region SelectedProfileGroup
        private DBCTProfileGroup _SelectedProfileGroup;
        public DBCTProfileGroup SelectedProfileGroup
        {
            get { return _SelectedProfileGroup; }
            set
            {
                if (ReferenceEquals(_SelectedProfileGroup, value)) return;
                _SelectedProfileGroup = value;
                RaisePropertyChanged("SelectedProfileGroup");
            }
        }
        #endregion
        #region ProfileGroupList
        private List<DBCTProfileGroup> _ProfileGroupList;
        public List<DBCTProfileGroup> ProfileGroupList
        {
            get { return _ProfileGroupList; }
            set
            {
                if (ReferenceEquals(_ProfileGroupList, value)) return;
                _ProfileGroupList = value;
                RaisePropertyChanged("ProfileGroupList");
            }
        }
        #endregion


        public bool CanChangeType
        {
            get { return DynamicSelectedData == null; }
        }
        private void PopulateRootList()
        {
            ProfileRootList = (from n in ACacheData.GetProfileItemList() select (DBCTProfileRoot)n.Tag).ToList();
        }
        private void PopulateGroupList()
        {
            if (SelectedProfileRoot == null)
            {
                ProfileGroupList = null;
                return;
            }
            ProfileGroupList =
                    (from n in ACacheData.GetProfileItemList()
                     where ((DBCTProfileRoot)n.Tag).Oid == SelectedProfileRoot.Oid
                     select n.ChildList.Select(m => (DBCTProfileGroup)m.Tag).ToList()).FirstOrDefault();
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => !string.IsNullOrWhiteSpace(Title));
        }

        private void OK()
        {
            if (Validate())
                if (Save())
                {
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

            var db = DBCTList.FindDuplicateTitleExcept(ADatabase.Dxs, DynamicSelectedData, HelperConvert.CorrectPersianBug(Title.Trim()));
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
                        DynamicSelectedData = new DBCTList(uow)
                        {
                            Title = Title.Trim(),
                            ProfileGroup = DBCTProfileGroup.FindByOid(uow, SelectedProfileGroup.Oid),
                        };
                        uow.CommitChanges();
                    }
                    DynamicSelectedData = DBCTList.FindByOid(ADatabase.Dxs, DynamicSelectedData.Oid);
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
                POLMessageBox.ShowWarning(ex.Message, MainView);
                return false;
            }
        }

        private void PopulateData()
        {
            SelectedProfileGroup = null;
            SelectedProfileRoot = null;
            if (DynamicSelectedData == null) return;
            Title = DynamicSelectedData.Title;
            SelectedProfileRoot = DynamicSelectedData.ProfileGroup.ProfileRoot;
            SelectedProfileGroup = DynamicSelectedData.ProfileGroup;
        }
        private void GetDynamicData()
        {
            DynamicSelectedData = MainView.DynamicSelectedData;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        #endregion
    }
}
