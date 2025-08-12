using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Profile.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.XOffice;

namespace POC.Module.Profile.Models
{
    public class MListManage : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private bool DynamicIsSelectionMode { get; set; }
        


        #region CTOR
        public MListManage(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateDataList();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "فهرست لیست ها"; }
        }
        #endregion

        #region DataList
        private XPCollection<DBCTList> _DataList;
        public XPCollection<DBCTList> DataList
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
        private DBCTList _SelectedData;
        public DBCTList SelectedData
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


        #region [METHODS]

        private void InitCommands()
        {
            CommandNew = new RelayCommand(DataNew, () => AMembership.HasPermission(PCOPermissions.BaseTable_List_Add));
            CommandEdit = new RelayCommand(DataEdit, () => SelectedData != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.BaseTable_List_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => DynamicGridControl.SelectedItems.Count != 0 && AMembership.HasPermission(PCOPermissions.BaseTable_List_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandOK = new RelayCommand(OK, () => DynamicIsSelectionMode);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp57 != "");
        }

        private void DataNew()
        {
            var w = new WListAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataEdit()
        {
            var w = new WListAddEdit(SelectedData) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTList).ToList();

            var failed = 0;
            var success = 0;
            POLProgressBox.Show("حذف اطلاعات", true, 0, srh.Count(), 1,
                w =>
                {
                    var dxs = ADatabase.GetNewSession();
                    foreach (var db in list)
                    {
                        try
                        {
                            if (w.NeedToCancel) return;
                            var db2 = DBCTList.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, db2.Title);
                            db2.Delete();
                            db2.Save();
                            success++;
                        }
                        catch
                        {
                            failed++;
                        }
                    }
                }, null, DynamicOwner);
            POLMessageBox.ShowInformation(string.Format("گزارش حذف : {0}{0}موفقیت آمیز : {1}{0}بروز خطا : {2}", Environment.NewLine, success, failed), DynamicOwner);
            DataRefresh();
        }
        private void DataRefresh()
        {
            PopulateDataList();
        }
        private void OK()
        {
            if (DynamicOwner == null) return;
            DynamicOwner.DialogResult = true;
            DynamicOwner.Close();
        }

        private void PopulateDataList()
        {
            var xpc = DBCTList.GetAll(ADatabase.Dxs);
            xpc.LoadAsync();
            DataList = xpc;
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGridControl = MainView.DynamicGridControl;
            DynamicTableView = MainView.DynamicTableView;
            DynamicIsSelectionMode = MainView.DynamicIsSelectionMode;

            DynamicGridControl.MouseDoubleClick +=
                (s, e) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);

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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp57);
        }
        
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
