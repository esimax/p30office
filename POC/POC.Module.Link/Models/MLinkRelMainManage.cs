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
using POC.Module.Link.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;

namespace POC.Module.Link.Models
{
    public class MLinkRelMainManage : NotifyObjectBase
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
        public MLinkRelMainManage(object mainView)
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
            get { return "فهرست عناوین ارتباطات"; }
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
        private XPCollection<DBCTContactRelMain> _DataList;
        public XPCollection<DBCTContactRelMain> DataList
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
        private DBCTContactRelMain _SelectedData;
        public DBCTContactRelMain SelectedData
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
            CommandNew = new RelayCommand(DataNew, () => AMembership.HasPermission(PCOPermissions.BaseTable_Link_Add));
            CommandEdit = new RelayCommand(DataEdit, () => SelectedData != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.BaseTable_Link_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => DynamicGridControl.SelectedItems.Count != 0 && AMembership.HasPermission(PCOPermissions.BaseTable_Link_Edit));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandOK = new RelayCommand(OK, () => DynamicIsSelectionMode);

            CommandRelSubManage = new RelayCommand(RelSubManage, () => SelectedData != null && AMembership.HasPermission(PCOPermissions.BaseTable_Link_ManageSub));

            CommandExport = new RelayCommand(Export, () => AMembership.HasPermission(PCOPermissions.BaseTable_Link_Export));
            CommandImport = new RelayCommand(Import, () => AMembership.HasPermission(PCOPermissions.BaseTable_Link_Import));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp47 != "");
        }

        private void DataNew()
        {
            var w = new WLinkRelMainAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataEdit()
        {
            var w = new WLinkRelMainAddEdit(SelectedData) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTContactRelMain).ToList();

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
                            var db2 = DBCTContactRelMain.FindByOid(dxs, db.Oid);
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
        private void RelSubManage()
        {
            var w = new WLinkRelSubManager(false, SelectedData) { Owner = DynamicOwner };
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
                FileName = "ContactLink.xml"
            };
            if (sf.ShowDialog() != true) return;

            try
            {
                var io = new LinkRelMainIO
                {
                    Subs = (from rm in DBCTContactRelMain.GetAll(ADatabase.Dxs)
                            select new LinkRelSubIO
                                {
                                    MainTitle = rm.Title,
                                    Titles = (from r in DBCTContactRelSub.GetByMainOid(ADatabase.Dxs, rm.Oid) select r.Title).ToArray()
                                }
                           ).ToArray()
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
                var serializer = new XmlSerializer(typeof(LinkRelMainIO));
                using (var f = new StreamReader(sf.FileName))
                {
                    var io = serializer.Deserialize(f) as LinkRelMainIO;
                    if (io == null)
                        throw new Exception("محتوای فایل معتبر نمی باشد.");
                    foreach (var v in io.Subs)
                    {
                        var dbrm = DBCTContactRelMain.FindDuplicateTitleExcept(ADatabase.Dxs, null, v.MainTitle);
                        if (dbrm == null)
                        {
                            dbrm = new DBCTContactRelMain(ADatabase.Dxs)
                                       {
                                           Title = v.MainTitle,
                                       };
                            dbrm.Save();
                        }
                        foreach (var sub in v.Titles)
                        {
                            var dbrs = DBCTContactRelSub.FindDuplicateTitleExcept(ADatabase.Dxs, dbrm.Oid, null, sub);
                            if (dbrs != null) continue;
                            dbrs = new DBCTContactRelSub(ADatabase.Dxs)
                            {
                                Title = sub,
                                RelMain = dbrm,
                            };
                            dbrs.Save();
                        }
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
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp47);
        }
        #endregion

        private void PopulateDataList()
        {
            var xpc = DBCTContactRelMain.GetAll(ADatabase.Dxs);
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
