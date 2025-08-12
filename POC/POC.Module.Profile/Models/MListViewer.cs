using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POC.Module.Profile.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using System.Reflection;
using DevExpress.XtraPrinting;

namespace POC.Module.Profile.Models
{
    public class MListViewer : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule AContactModule { get; set; }

        private DispatcherTimer DataUpdateTimer { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private DBCTList DynamicDBList { get; set; }
        private DBCTContact DynamicDBContact { get; set; }
        private DBCTProfileValue DynamicDBProfileValue { get; set; }
        private Assembly DynamicDBListAssembly { get; set; }
        private Type DynamicDBListType { get; set; }

        #region CTOR
        public MListViewer(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;

            InitCommands();
            GetDynamicData();
            UpdateSearch();
        }
        #endregion

        void AContactModule_OnSelectedContactChanged(object sender, EventArgs e)
        {
            if (!IsSyncWithContact) return;

            DataList = null;
            IsSyncActive = false;

            var dbc = AContactModule.SelectedContact as DBCTContact;
            if (dbc == null)
            {
                DataList = null;
                return;
            }

            var xpcVal = DBCTProfileValue.GetAll(ADatabase.Dxs, dbc.Oid);
            var q1 = from n in xpcVal
                     where n.ProfileItem.ItemType == EnumProfileItemType.List &&
                           n.ProfileItem.Guid1 == DynamicDBList.Oid
                     select n;
            if (!q1.Any()) return;

            var dbpv = q1.First();
            var dbl = DBCTList.FindByOid(ADatabase.Dxs, dbpv.ProfileItem.Guid1);

            if (DynamicDBList == null) return;

            DynamicDBProfileValue = dbpv;
            DynamicDBList = dbl;
            DynamicDBContact = dbc;

            MainView.DynamicDBProfileValue = dbpv;
            MainView.DynamicDBList = dbl;
            MainView.DynamicDBContact = dbc;


            IsSyncActive = true;
            RaisePropertyChanged("WindowTitle");
            RaisePropertyChanged("ContactTitle");
            DataRefresh();
        }

        #region WindowTitle
        public string WindowTitle
        {
            get
            {
                var title = DynamicDBList.Title;
                var contact = DynamicDBProfileValue.Contact.Title;
                return string.Format("{0} - {1}", title, contact);
            }
        }
        #endregion

        #region ContactTitle
        public string ContactTitle
        {
            get
            {
                if (IsSyncWithContact)
                {
                    if (IsSyncActive)
                    {
                        return DynamicDBContact.Title;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return DynamicDBContact.Title;
                }
            }
        }
        #endregion

        public FlowDirection AppFlowDirection { get { return HelperLocalize.ApplicationFlowDirection; } }

        #region DataList
        private XPServerCollectionSource _DataList;
        public XPServerCollectionSource DataList
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
        #region FocusedData
        private object _FocusedData;
        public object FocusedData
        {
            get { return _FocusedData; }
            set
            {
                if (ReferenceEquals(_FocusedData, value))
                    return;
                _FocusedData = value;
                RaisePropertyChanged("FocusedData");
            }
        }
        #endregion

        #region IsSyncWithContact
        private bool _IsSyncWithContact;
        public bool IsSyncWithContact
        {
            get { return _IsSyncWithContact; }
            set
            {
                if (_IsSyncWithContact == value)
                    return;
                _IsSyncWithContact = value;
                RaisePropertyChanged("IsSyncWithContact");
            }
        }
        #endregion

        #region IsSyncActive
        private bool _IsSyncActive;
        public bool IsSyncActive
        {
            get { return _IsSyncActive; }
            set
            {
                if (_IsSyncActive == value)
                    return;
                _IsSyncActive = value;
                RaisePropertyChanged("IsSyncActive");
            }
        }
        #endregion







        #region [METHODS]

        private void InitCommands()
        {
            CommandDataNew = new RelayCommand(DataNew, () => true);
            CommandDataEdit = new RelayCommand(DataEdit, () => FocusedData != null && CanEdit );
            CommandDataDelete = new RelayCommand(DataDelete, () => FocusedData != null && CanEdit );
            CommandDataRefresh = new RelayCommand(DataRefresh, () => true);

            CommandBestFitColumn = new RelayCommand(BestFitColumn, () => true);

            CommandExportExcel = new RelayCommand(ExportExcel, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp58 != "");
        }

        private void ExportExcel()
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,                
                Filter = "Excel 2007  File (*.xls)|*.xls",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = string.Empty,
            };
            if (sf.ShowDialog() != true) return;

            try
            {
                var options = new XlsExportOptions
                {
                    TextExportMode = TextExportMode.Text,
                    ExportMode = XlsExportMode.SingleFile,
                    Suppress256ColumnsWarning = true,
                    Suppress65536RowsWarning = true
                };

                ((TableView)DynamicTableView).ExportToXls(sf.FileName, options);
                var dr = POLMessageBox.ShowQuestionYesNo("آیا می خواهید فایل را باز كنید؟", DynamicOwner);
                if (dr != MessageBoxResult.Yes) return;
                HelperUtils.Try(() => Process.Start(sf.FileName));
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }






        private void DataNew()
        {
            var w = new WListEditor(DynamicDBContact, null, DynamicDBListType, DynamicDBListAssembly, DynamicDBList) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataEdit()
        {
            var w = new WListEditor(DynamicDBContact, FocusedData, DynamicDBListType, DynamicDBListAssembly, DynamicDBList) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(i => DynamicGridControl.GetRow(i)).Cast<dynamic>().ToList();


            var failed = 0;
            var success = 0;
            POLProgressBox.Show("حذف اطلاعات", true, 0, srh.Count(), 1,
                w =>
                {
                    var dxs = ADatabase.GetNewSession();
                    foreach (var db in list)
                    {
                        var i = 0;
                        try
                        {
                            if (w.NeedToCancel) return;
                            var db2 = db.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, (i++).ToString());
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
            UpdateSearch();
        }
        private void BestFitColumn()
        {
            DynamicTableView.BestFitArea = BestFitArea.All;
            DynamicTableView.BestFitMode = BestFitMode.VisibleRows;
            DynamicTableView.BestFitColumns();
        }


        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGridControl = MainView.DynamicGridControl;
            DynamicTableView = MainView.DynamicTableView;
            DynamicDBList = MainView.DynamicDBList;
            DynamicDBProfileValue = MainView.DynamicDBProfileValue;



            DynamicGridControl.MouseDoubleClick +=
                (s, e) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);
                    if (i < 0) return;
                    if (CommandDataEdit.CanExecute(null))
                        CommandDataEdit.Execute(null);
                    e.Handled = true;
                };

            DynamicDBListAssembly = DynamicDBList.GetAssemblyOfListObject();
            DynamicDBListType = DynamicDBListAssembly.GetTypes()[0];
            DynamicDBContact = MainView.DynamicDBContact;
        }
        private void UpdateSearchWithDelay()
        {
            if (DataUpdateTimer == null)
            {
                DataUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                DataUpdateTimer.Tick += (s, e) =>
                {
                    DataUpdateTimer.Stop();
                    UpdateSearch();
                };
            }
            DataUpdateTimer.Stop();
            DataUpdateTimer.Start();
        }
        private void UpdateSearch()
        {
            if (!AMembership.IsAuthorized) return;

            var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);

            mainSearchCriteria.Operands.Add(new BinaryOperator("Contact.Oid", DynamicDBContact.Oid));









            try
            {
                var xpi = new XPServerCollectionSource(ADatabase.Dxs, DynamicDBListType)
                              {
                                  FixedFilterCriteria = mainSearchCriteria
                              };
                xpi.ResolveSession +=
                    (s, e) =>
                    {
                        e.Session = ADatabase.Dxs;
                    };
                DataList = null;
                DataList = xpi;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.High);
                POLMessageBox.ShowError(ex.Message);
            }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp58);
        }

        #endregion


        #region [COMMANDS]
        public RelayCommand CommandDataNew { get; set; }
        public RelayCommand CommandDataEdit { get; set; }
        public RelayCommand CommandDataDelete { get; set; }
        public RelayCommand CommandDataRefresh { get; set; }

        public RelayCommand CommandBestFitColumn { get; set; }

        public RelayCommand CommandExportExcel { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        public bool CanEdit { get; set; }
    }
}
