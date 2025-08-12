using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POC.Module.Profile.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.XOffice;

namespace POC.Module.Profile.Models
{
    public class MProfileTableManage : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private bool DynamicIsSelectionMode { get; set; }

        #region CTOR
        public MProfileTableManage(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitCommands();
            GetDynamicData();
            PopulateDataList();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "فهرست جداول ساده (فرم)"; }
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
        private ObservableCollection<CacheItemProfileTable> _DataList;

        public ObservableCollection<CacheItemProfileTable> DataList
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
        private CacheItemProfileTable _SelectedData;
        public CacheItemProfileTable SelectedData
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
            CommandNew = new RelayCommand(DataNew, () => AMembership.HasPermission(PCOPermissions.BaseTable_ProfileTable_Add));
            CommandEdit = new RelayCommand(DataEdit, () => SelectedData != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.BaseTable_ProfileTable_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => DynamicGridControl.SelectedItems.Count != 0 && AMembership.HasPermission(PCOPermissions.BaseTable_ProfileTable_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandOK = new RelayCommand(OK, () => DynamicIsSelectionMode);

            CommandRelSubManage = new RelayCommand(RelSubManage, () => SelectedData != null && AMembership.HasPermission(PCOPermissions.BaseTable_ProfileTable_ManageValues));

            CommandExport = new RelayCommand(Export, () => SelectedData != null && AMembership.HasPermission(PCOPermissions.BaseTable_ProfileTable_Export));
            CommandImport = new RelayCommand(Import, () => AMembership.HasPermission(PCOPermissions.BaseTable_ProfileTable_Import));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp60 != "");
        }

        private void DataNew()
        {
            var w = new WProfileTableAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            ACacheData.ForcePopulateCache(EnumCachDataType.ProfileTable, false, w.DynamicSelectedData);
            ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileTable);
        }
        private void DataEdit()
        {
            var w = new WProfileTableAddEdit(SelectedData.Tag as DBCTProfileTable) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            ACacheData.ForcePopulateCache(EnumCachDataType.ProfileTable, false, w.DynamicSelectedData);
            ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileTable);
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => ((CacheItemProfileTable)DynamicGridControl.GetRow(n)).Tag as DBCTProfileTable).ToList();

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
                            var db2 = DBCTProfileTable.FindByOid(dxs, db.Oid);
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
            POLMessageBox.ShowInformation(string.Format("گزارش حذف : {0}{0}موفقیت آمیز : {1}{0}بروز خطا : {2}", Environment.NewLine, success, failed), DynamicOwner);

        }
        private void DataRefresh()
        {
            ACacheData.ForcePopulateCache(EnumCachDataType.ProfileTable, true, null);

            DataList = null;
            DataList = ACacheData.GetProfileTableList();

            RaisePropertyChanged("DataList");
        }
        private void RelSubManage()
        {
            var w = new WProfileTValueManager(false, SelectedData.Tag as DBCTProfileTable) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void OK()
        {
            if (DynamicOwner == null) return;
            DynamicOwner.DialogResult = true;
            DynamicOwner.Close();
        }

        private void Export()
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "ProfileTable.xml"
            };
            if (sf.ShowDialog() != true) return;

            try
            {
                var sd = (DBCTProfileTable)(SelectedData.Tag);
                var io = new ProfileTableNameIO
                             {
                                 TableTitle = SelectedData.Title,
                                 ValueDepth = sd.ValueDepth,
                                 Values = GetValues(DBCTProfileTValue.GetAll(sd.Session, sd.Oid)),
                             };
                var serializer = new XmlSerializer(io.GetType());
                using (var f = new StreamWriter(sf.FileName))
                {
                    serializer.Serialize(f, io);
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", DynamicOwner);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }
        private ProfileTableValueIO[] GetValues(XPCollection<DBCTProfileTValue> xpc)
        {
            return xpc.Select(v => new ProfileTableValueIO { Title = v.Title, Values = GetValues(v.Children) }).ToArray();
        }
        private void Import()
        {
            var sf = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
            };
            if (sf.ShowDialog() != true) return;
            try
            {
                var serializer = new XmlSerializer(typeof(ProfileTableNameIO));
                using (var f = new StreamReader(sf.FileName))
                {
                    var io = serializer.Deserialize(f) as ProfileTableNameIO;
                    if (io == null)
                        throw new Exception("محتوای فایل معتبر نمی باشد.");

                    var dbt = DBCTProfileTable.FindDuplicateTitleExcept(ADatabase.Dxs, null, io.TableTitle);
                    if (dbt == null)
                    {
                        dbt = new DBCTProfileTable(ADatabase.Dxs)
                        {
                            Title = io.TableTitle,
                            ValueDepth = io.ValueDepth,
                        };
                        dbt.Save();
                    }

                    foreach (var sub in io.Values)
                    {
                        var dbv = DBCTProfileTValue.FindDuplicateTitleExcept(ADatabase.Dxs, dbt.Oid, null, sub.Title, null);
                        if (dbv == null)
                        {
                            dbv = new DBCTProfileTValue(ADatabase.Dxs) { Title = sub.Title, Table = dbt };
                            dbv.Save();
                        }

                        foreach (var sub2 in sub.Values)
                        {
                            var dbv2 = DBCTProfileTValue.FindDuplicateTitleExcept(ADatabase.Dxs, dbt.Oid, null, sub2.Title, dbv);
                            if (dbv2 == null)
                            {
                                dbv2 = new DBCTProfileTValue(ADatabase.Dxs) { Title = sub2.Title, Table = dbt, Parent = dbv };
                                dbv2.Save();
                            }
                        }
                    }
                }
                DataRefresh();
                ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileTable);
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", DynamicOwner);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }

        private void PopulateDataList()
        {
            DataList = ACacheData.GetProfileTableList();
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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp60);
        }
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandRelSubManage { get; set; }

        public RelayCommand CommandOK { get; set; }

        public RelayCommand CommandExport { get; set; }
        public RelayCommand CommandImport { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
