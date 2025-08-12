using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Profile.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using System.Threading.Tasks;
using POL.Lib.Utils;

namespace POC.Module.Profile.Models
{
    public class MProfileTValueManage : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private TreeListControl DynamicTreeListControl { get; set; }
        private TreeListView DynamicTreeListView { get; set; }
        private bool DynamicIsSelectionMode { get; set; }
        private DBCTProfileTable DynamicTable { get; set; }


        #region CTOR
        public MProfileTValueManage(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitCommands();
            GetDynamicData();
            DataRefresh();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "فهرست مقادیر جدول"; }
        }
        #endregion

        #region SearchText
        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                if (_SearchText == value)
                    return;
                _SearchText = value;
                RaisePropertyChanged("SearchText");
                PopulateDataList();
            }
        }
        #endregion

        #region DataList
        private ObservableCollection<CacheItemProfileTValue> _DataList;
        public ObservableCollection<CacheItemProfileTValue> DataList
        {
            get { return _DataList; }
            set
            {
                if (_DataList == value)
                    return;
                _DataList = value;
                RaisePropertyChanged("DataList");
            }
        }
        #endregion


        #region SelectedData
        private CacheItemProfileTValue _SelectedData;
        public CacheItemProfileTValue SelectedData
        {
            get { return _SelectedData; }
            set
            {
                if (ReferenceEquals(_SelectedData, value))
                    return;
                _SelectedData = value;
                RaisePropertyChanged("SelectedData");
            }
        }
        #endregion


        public string RelMainTitle
        {
            get { return DynamicTable.Title; }
        }


        #region [METHODS]

        private void InitCommands()
        {
            CommandNew = new RelayCommand(DataNew, () => true);
            CommandNewChild = new RelayCommand(DataNewChild, () => SelectedData != null && ((DBCTProfileTValue)SelectedData.Tag).Parent == null);
            CommandEdit = new RelayCommand(DataEdit, () => SelectedData != null && DynamicTreeListControl.SelectedItems.Count == 1);
            CommandDelete = new RelayCommand(DataDelete, () => DynamicTreeListControl.SelectedItems.Count == 1);
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandOK = new RelayCommand(OK, () => DynamicIsSelectionMode);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp61 != "");
        }



        private void DataNew()
        {
            var w = new WProfileTValueAddEdit(DynamicTable, null, null) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            ACacheData.ForcePopulateCache(EnumCachDataType.ProfileTable, false, w.DynamicSelectedData);
            ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileTable);
        }
        private void DataNewChild()
        {
            var w = new WProfileTValueAddEdit(DynamicTable, null, ((DBCTProfileTValue)SelectedData.Tag)) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            ACacheData.ForcePopulateCache(EnumCachDataType.ProfileTable, false, w.DynamicSelectedData);
            ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileTable);
        }
        private void DataEdit()
        {
            var db = (DBCTProfileTValue)SelectedData.Tag;
            var w = new WProfileTValueAddEdit(DynamicTable, db, db.Parent) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            ACacheData.ForcePopulateCache(EnumCachDataType.ProfileTable, false, w.DynamicSelectedData);
            ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileTable);
        }
        private void DataDelete()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = new List<DBCTProfileTValue> { (DBCTProfileTValue)SelectedData.Tag };

            var failed = 0;
            var success = 0;
            POLProgressBox.Show("حذف اطلاعات", true, 0, 1, 1,
                w =>
                {
                    var dxs = ADatabase.GetNewSession();
                    foreach (var db in list)
                    {
                        try
                        {
                            if (w.NeedToCancel) return;
                            var db2 = DBCTProfileTValue.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, db2.Title);
                            db2.Delete();
                            db2.Save();
                            success++;
                            HelperUtils.DoDispatcher(() => ACacheData.ForcePopulateCache(EnumCachDataType.ProfileTable, false, db2));
                        }
                        catch
                        {
                            failed++;
                        }
                    }
                }, null, DynamicOwner);

            ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileTable);
            DynamicTable.UpdateDepthValue();
            POLMessageBox.ShowInformation(string.Format("گزارش حذف : {0}{0}موفقیت آمیز : {1}{0}بروز خطا : {2}", Environment.NewLine, success, failed), DynamicOwner);
        }
        private void DataRefresh()
        {
            PopulateDataList();
            Task.Factory.StartNew(
                () =>
                {
                    System.Threading.Thread.Sleep(500);
                    HelperUtils.DoDispatcher(
                        () =>
                        {
                            DynamicTreeListView.ExpandAllNodes();
                        });

                });
        }
        private void OK()
        {
            if (DynamicOwner == null) return;
            DynamicOwner.DialogResult = true;
            DynamicOwner.Close();
        }

        private void PopulateDataList()
        {

            DataList =
                (from n in ACacheData.GetProfileTableList()
                 where ((DBCTProfileTable)n.Tag).Oid == DynamicTable.Oid
                 select n.ChildList).FirstOrDefault();
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicTreeListControl = MainView.DynamicTreeListControl;
            DynamicTreeListView = DynamicTreeListControl.View as TreeListView;
            DynamicIsSelectionMode = MainView.DynamicIsSelectionMode;
            DynamicTable = MainView.DynamicTable;

            DynamicTreeListControl.MouseDoubleClick +=
                (s, e) =>
                {
                    var i = DynamicTreeListView.GetRowHandleByMouseEventArgs(e);

                    if (i < 0) return;
                    if (DynamicIsSelectionMode)
                    {
                        if (CommandOK.CanExecute(null))
                            CommandOK.Execute(null);
                    }
                    else
                    {
                        if (CommandEdit.CanExecute(null))
                            CommandEdit.Execute(null);
                    }
                    e.Handled = true;
                };
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp61);
        }
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandNewChild { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
