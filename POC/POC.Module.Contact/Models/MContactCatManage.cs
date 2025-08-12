using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo.Exceptions;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POC.Module.Contact.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.XOffice;

namespace POC.Module.Contact.Models
{
    public class MContactCatManage : NotifyObjectBase
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
        public MContactCatManage(object mainView)
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
            get { return "فهرست عناوین دسته بندی پرونده"; }
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
        private ObservableCollection<CacheItemContactCat> _DataList;
        public ObservableCollection<CacheItemContactCat> DataList
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
        private CacheItemContactCat _SelectedData;
        public CacheItemContactCat SelectedData
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
            CommandNew = new RelayCommand(DataNew, () => AMembership.HasPermission(PCOPermissions.BaseTable_Category_Add));
            CommandEdit = new RelayCommand(DataEdit, () => SelectedData != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.BaseTable_Category_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => SelectedData != null && AMembership.HasPermission(PCOPermissions.BaseTable_Category_Delete));
            CommandRefresh = new RelayCommand(() => DataRefresh(null), () => true);
            CommandOK = new RelayCommand(OK, () => DynamicIsSelectionMode);

            CommandExport = new RelayCommand(Export, () => AMembership.HasPermission(PCOPermissions.BaseTable_Category_Export));
            CommandImport = new RelayCommand(Import, () => AMembership.HasPermission(PCOPermissions.BaseTable_Category_Import));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp30 != "");
        }

        private void DataNew()
        {
            var w = new WContactCatAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            DataRefresh(w.DynamicSelectedData);
        }
        private void DataEdit()
        {
            try
            {
                var w = new WContactCatAddEdit(SelectedData.Tag as DBCTContactCat) {Owner = DynamicOwner};
                if (w.ShowDialog() != true) return;
                DataRefresh(w.DynamicSelectedData);
            }
            catch (ExceptionBundleException)
            {
            }
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => ((CacheItemContactCat)DynamicGridControl.GetRow(n)).Tag as DBCTContactCat).ToList();

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
                            var db2 = DBCTContactCat.FindByOid(dxs, db.Oid);
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
            DataRefresh(null);
            POLMessageBox.ShowInformation(string.Format("گزارش حذف : {0}{0}موفقیت آمیز : {1}{0}بروز خطا : {2}", Environment.NewLine, success, failed), DynamicOwner);
        }
        private void DataRefresh(object db)
        {
            ACacheData.ForcePopulateCache(EnumCachDataType.ContactCat, false, db);
            ACacheData.RaiseCacheChanged(EnumCachDataType.ContactCat);
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
                FileName = "ContactCategory.xml"
            };
            if (sf.ShowDialog() != true) return;

            try
            {
                var io = new PackIOContactCatArray()
                {
                    ContactCats = (from n in DBCTContactCat.GetAll(ADatabase.Dxs) select new PackIOContactCat { Title = n.Title, Role = n.Role }).ToArray()
                };
                var serializer = new XmlSerializer(io.GetType());
                using (var f = new StreamWriter(sf.FileName))
                {
                    serializer.Serialize(f, io);
                }
                POLMessageBox.ShowInformation(string.Format("عملیات با موفقیت انجام شد.{0}{0}نوع استخراج : تمامی دسته ها.", Environment.NewLine), DynamicOwner);
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
                var serializer = new XmlSerializer(typeof(PackIOContactCatArray));
                using (var f = new StreamReader(sf.FileName))
                {
                    var io = serializer.Deserialize(f) as PackIOContactCatArray;
                    if (io == null)
                        throw new Exception("محتوای فایل معتبر نمی باشد.");
                    foreach (var v in io.ContactCats)
                    {
                        var dbcc = DBCTContactCat.FindDuplicateTitleExcept(ADatabase.Dxs, null, v.Title);
                        if (dbcc != null) continue;
                        var dbr = (from n in ACacheData.GetRoleList() where n.Title == v.Role select n.Title).FirstOrDefault();
                        dbcc = new DBCTContactCat(ADatabase.Dxs)
                        {
                            Title = v.Title,
                            Role = dbr,
                        };
                        dbcc.Save();
                    }
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", DynamicOwner);
                DataRefresh(null);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }

        private void PopulateDataList()
        {
            DataList = ACacheData.GetContactCatList();
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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp30);
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
