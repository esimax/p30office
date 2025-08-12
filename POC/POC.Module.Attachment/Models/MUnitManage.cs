using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POC.Module.Attachment.Views;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.XOffice;

namespace POC.Module.Attachment.Models
{
    public class MUnitManage : NotifyObjectBase
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
        public MUnitManage(object mainView)
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
            get { return "فهرست واحد"; }
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
        private XPCollection<DBBTUnit> _DataList;
        public XPCollection<DBBTUnit> DataList
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
        private DBBTUnit _SelectedData;
        public DBBTUnit SelectedData
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
            CommandNew = new RelayCommand(DataNew, () => AMembership.HasPermission(PCOPermissions.BaseTable_CallTitle_Add));
            CommandEdit = new RelayCommand(DataEdit, () => SelectedData != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.BaseTable_CallTitle_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => SelectedData != null && AMembership.HasPermission(PCOPermissions.BaseTable_CallTitle_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandOK = new RelayCommand(OK, () => DynamicIsSelectionMode);

            CommandExport = new RelayCommand(Export, () => AMembership.HasPermission(PCOPermissions.BaseTable_CallTitle_Export));
            CommandImport = new RelayCommand(Import, () => AMembership.HasPermission(PCOPermissions.BaseTable_CallTitle_Import));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp14 != "");
        }

        private void DataNew()
        {
            var w = new WUnitAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataEdit()
        {
            var w = new WUnitAddEdit(SelectedData) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBBTUnit).ToList();

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
                            var db2 = DBBTUnit.FindByOid(dxs, db.Oid);
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
                FileName = "Unit.xml"
            };
            if (sf.ShowDialog() != true) return;

            try
            {
                var io = new UnitIO()
                {
                    Titles = (from n in DBBTUnit.GetAll(ADatabase.Dxs) select n.Title).ToArray()
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
                var serializer = new XmlSerializer(typeof(UnitIO));
                using (var f = new StreamReader(sf.FileName))
                {
                    var io = serializer.Deserialize(f) as UnitIO;
                    if (io == null)
                        throw new Exception("محتوای فایل معتبر نمی باشد.");
                    foreach (var v in io.Titles)
                    {
                        var dbcc = DBBTUnit.FindDuplicateTitleExcept(ADatabase.Dxs, null, v);
                        if (dbcc != null) continue;
                        dbcc = new DBBTUnit(ADatabase.Dxs) { Title = v };
                        dbcc.Save();
                    }
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", DynamicOwner);
                DataRefresh();
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }

        private void PopulateDataList()
        {
            var ff = SearchText;
            if (string.IsNullOrWhiteSpace(ff))
                ff = string.Empty;
            ff = ff.Replace("*", "").Replace("%", "").Trim();
            HelperConvert.CorrectPersianBug(ref ff);
            XPCollection<DBBTUnit> xpc = null;

            xpc = DBBTUnit.GetAll(ADatabase.Dxs, ff);

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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp14);
        } 
        #endregion



        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandOK { get; set; }

        public RelayCommand CommandExport { get; set; }
        public RelayCommand CommandImport { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
