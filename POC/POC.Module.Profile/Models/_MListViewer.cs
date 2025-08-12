using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Printing;
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
using System.Reflection;

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
            UpdateDatasource();
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








        private void InitCommands()
        {
            CommandDataNew = new RelayCommand(DataNew, () => true);
            CommandDataEdit = new RelayCommand(DataEdit, () => FocusedData != null && CanEdit /*&& DynamicTableView.SelectedRows.Count == 1*/);
            CommandDataDelete = new RelayCommand(DataDelete, () => FocusedData != null /*DynamicTableView.SelectedRows.Count != 0*/);
            CommandDataRefresh = new RelayCommand(DataRefresh, () => true);

            CommandBestFitColumn = new RelayCommand(BestFitColumn, () => true);

            CommandPrintPreview = new RelayCommand(PrintPreview, () => true);
            CommandExportExcel = new RelayCommand(ExportExcel, () => true);
        }

        private void ExportExcel()
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                Filter = "Excel 2007  File (*.xlsx)|*.xlsx",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = string.Empty,
            };
            if (sf.ShowDialog() != true) return;

            try
            {
                var link = new PrintableControlLink(DynamicTableView);
                link.ExportToXlsx(sf.FileName);
                var dr = POLMessageBox.ShowQuestionYesNo("آیا می خواهید فایل را باز کنید؟", DynamicOwner);
                if (dr != MessageBoxResult.Yes) return;
                Process.Start(sf.FileName);
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }

        private void PrintPreview()
        {




            var report = new POC.Module.Profile.Reports.XRMain();
            report.DataSource = DynamicGridControl.ItemsSource;
            var tool = new DevExpress.XtraReports.UI.ReportDesignTool(report);
            tool.Report.DesignerLoaded += (s, e) =>
                                              {
                                                  dynamic s2 = s;
                                                  s2.DataSource = DynamicGridControl.ItemsSource;
                                              };
            tool.ShowDesignerDialog();
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
            var srh = DynamicTableView.GetSelectedRowHandles();
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
            UpdateDatasource();
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

            DynamicDBContact = MainView.DynamicDBContact;
        }












        #region Command
        public RelayCommand CommandDataNew { get; set; }
        public RelayCommand CommandDataEdit { get; set; }
        public RelayCommand CommandDataDelete { get; set; }
        public RelayCommand CommandDataRefresh { get; set; }

        public RelayCommand CommandBestFitColumn { get; set; }

        public RelayCommand CommandPrintPreview { get; set; }
        public RelayCommand CommandExportExcel { get; set; }
        #endregion

        public bool CanEdit { get; set; }

        
        
        
        
        
        
        private class HolderCol
        {
            public string FieldName { get; set; }
            public DBCTProfileItem ProfileItem { get; set; }
        }


        #region HolderColList
        private List<HolderCol> HolderColList { get; set; }
        #endregion
        private void UpdateDatasource()
        {
            var reportPack = new MetaDataProfileReportPack { Items = null };

            var dbl = DBCTList.FindByOid(ADatabase.Dxs, DynamicDBProfileValue.ProfileItem.Guid1);
            var dbg = DBCTProfileGroup.FindByOid(ADatabase.Dxs, dbl.ProfileGroup.Oid);
            var items = DBCTProfileItem.GetAll(ADatabase.Dxs, dbg.Oid).ToList();
            reportPack.Items = new MetaDataProfileReportItem[items.Count];
            for (var i = 0; i < items.Count; i++)
                reportPack.Items[i] = new MetaDataProfileReportItem
                                          {
                                              Order = i,
                                              ProfileItem = items[i],
                                              ProfileItemOid = items[i].Oid,
                                              Title = items[i].Title
                                          };

            try
            {
                var sql = GenerateSQL(reportPack);
                var cs = ADatabase.Dxs.ConnectionString;
                var sqlcn = new SqlConnection(cs);
                sqlcn.Open();
                var myCommand = new SqlCommand(sql, sqlcn);
                var reader = myCommand.ExecuteReader();
                DynamicGridControl.ItemsSource = reader;
                sqlcn.Close();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message);
            }
        }
        private string GenerateSQL(MetaDataProfileReportPack reportPack)
        {
            HolderColList = new List<HolderCol>();

            var collist = (from n in DynamicGridControl.Columns select n).ToList();
            foreach (var col in collist)
            {
                if (!(col.Name == "colCode" || col.Name == "colTitle"))
                    DynamicGridControl.Columns.Remove(col);
            }


            var rv = new System.Text.StringBuilder();
            rv.Append("SELECT ");
            rv.Append("[tContact].[Oid] , [tContact].[Code] as ContactCode, [tContact].[Title] as ContactTitle ");

            var i = 0;
            if (reportPack != null && reportPack.Items != null)
            {
                foreach (var rep in reportPack.Items)
                {
                    i++;
                    var data_col = "String1";
                    var data_col2 = "";
                    var item = DBCTProfileItem.FindByOid(ADatabase.Dxs, rep.ProfileItemOid);
                    rep.ProfileItem = item;
                    if (item.ItemType == EnumProfileItemType.Bool)
                        data_col = "Int1";
                    if (item.ItemType == EnumProfileItemType.Double)
                        data_col = "Double1";
                    if (item.ItemType == EnumProfileItemType.City)
                        data_col = "String1";
                    if (item.ItemType == EnumProfileItemType.Location)
                    {
                        data_col = "Double1";
                        data_col2 = "Double2";
                    }
                    if (item.ItemType == EnumProfileItemType.Color)
                    {
                        data_col = "Double1";
                    }
                    if (item.ItemType == EnumProfileItemType.Image)
                    {
                        data_col = "Guid1";
                    }
                    if (item.ItemType == EnumProfileItemType.Date)
                    {
                        data_col = "DateTime1";
                    }
                    if (item.ItemType == EnumProfileItemType.DateTime)
                    {
                        data_col = "DateTime1";
                    }
                    if (item.ItemType == EnumProfileItemType.Time)
                    {
                        data_col = "Int1";
                    }
                    if (data_col2 == "")
                    {
                        DynamicGridControl.Columns.Add(new GridColumn() { FieldName = "val" + i.ToString(), Header = rep.Title });
                        if (item.ItemType == EnumProfileItemType.Color)
                        {
                            rv.Append(", [tValue" + i.ToString() + "].[" + data_col + "] as val" + i.ToString());
                        }
                        else if (item.ItemType == EnumProfileItemType.Time)
                        {
                            rv.Append(", ( STR([tValue" + i.ToString() + "].[" + data_col + "],2,0) + ':' + STR([tValue" +
                                      i.ToString() + "].[Int2],2,0) ) as val" + i.ToString());
                        }
                        else
                        {
                            rv.Append(", [tValue" + i.ToString() + "].[" + data_col + "] as val" + i.ToString());
                        }
                    }
                    else
                    {
                        DynamicGridControl.Columns.Add(new GridColumn() { FieldName = "val" + i.ToString(), Header = rep.Title });
                        rv.Append(", ( STR([tValue" + i.ToString() + "].[" + data_col +
                                  "],19,19) + ' , ' + STR ([tValue" + i.ToString() + "].[" + data_col2 +
                                  "],19,19))  as val" + i.ToString());
                    }
                    HolderColList.Add(new HolderCol { FieldName = "val" + i.ToString(), ProfileItem = item });
                }
            }
            rv.Append(" FROM [DBCTContact] as tContact");

            i = 0;
            if (reportPack != null && reportPack.Items != null)
            {
                foreach (var rep in reportPack.Items)
                {
                    i++;
                    var data_col = "String1";
                    var item = rep.ProfileItem as DBCTProfileItem;
                    if (item.ItemType == EnumProfileItemType.Bool)
                        data_col = "Int1";
                    if (item.ItemType == EnumProfileItemType.Double)
                        data_col = "Double1";
                    if (item.ItemType == EnumProfileItemType.City)
                        data_col = "String1";
                    if (item.ItemType == EnumProfileItemType.Location)
                        data_col = "Double1, Double2";
                    if (item.ItemType == EnumProfileItemType.Color)
                        data_col = "Double1";
                    if (item.ItemType == EnumProfileItemType.Image)
                        data_col = "Guid1";
                    if (item.ItemType == EnumProfileItemType.Date)
                        data_col = "DateTime1";
                    if (item.ItemType == EnumProfileItemType.DateTime)
                        data_col = "DateTime1";
                    if (item.ItemType == EnumProfileItemType.Time)
                        data_col = "Int1,Int2";
                    rv.Append(" FULL JOIN  (");
                    rv.Append(" SELECT Contact," + data_col + " ");
                    rv.Append(" FROM [DBCTProfileValue] ");
                    rv.Append(" WHERE [GCRecord] is NULL AND [ProfileItem] = '" + rep.ProfileItemOid.ToString() +
                              "') as tValue" + i.ToString() + " ON tValue" + i.ToString() + ".Contact =  tContact.Oid ");
                }
            }
            rv.Append(" WHERE [tContact].[Code] = "+DynamicDBContact.Code);
            var rv1 = rv.ToString();
            return rv1;
        }
    }
}
