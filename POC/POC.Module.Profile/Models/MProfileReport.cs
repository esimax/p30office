using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using DevExpress.XtraPrinting;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Win32;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Profile.Views;
using System.Xml.Serialization;
using System.IO;
using DevExpress.Xpf.Grid;
using POL.WPF.DXControls;
using POL.Lib.Utils;
using System.Diagnostics;
using DevExpress.Data.Filtering;

namespace POC.Module.Profile.Models
{
    public class MProfileReport : NotifyObjectBase, IDisposable
    {
        #region Private Properties
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGrid { get; set; }

        private GroupOperator MainSearchCriteria { get; set; }
        #endregion

        private class HolderCol
        {
            public string FieldName { get; set; }
            public DBCTProfileItem ProfileItem { get; set; }
        }

        #region CTOR
        public MProfileReport(object mainView)
        {
            MainView = mainView;

            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            ADataFieldManager = ServiceLocator.Current.GetInstance<IDataFieldManager>();

            InitDynamics();
            InitCommands();
            PopulateContactCat();
        }
        #endregion


        #region SelectedReport
        private DBCTProfileReport _SelectedReport;
        public DBCTProfileReport SelectedReport
        {
            get { return _SelectedReport; }
            set
            {
                if (ReferenceEquals(value, _SelectedReport))
                    return;

                _SelectedReport = value;
                RaisePropertyChanged("SelectedReport");
                RaisePropertyChanged("ReportTitle");
            }
        }
        #endregion
        #region DataSource
        private List<object> _DataSource;
        public List<object> DataSource
        {
            get { return _DataSource; }
            set
            {
                if (value == _DataSource)
                    return;

                _DataSource = value;
                RaisePropertyChanged("DataSource");
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
                if (value == _FocusedData)
                    return;

                _FocusedData = value;
                RaisePropertyChanged("FocusedData");
            }
        }
        #endregion
        #region ReportTitle
        public string ReportTitle
        {
            get { return string.Format("گزارش : {0}", (SelectedReport == null ? string.Empty : SelectedReport.Title)); }

        }
        #endregion
        #region HolderColList
        private List<HolderCol> HolderColList { get; set; }
        #endregion


        #region ContactCatList
        private List<object> _ContactCatList;
        public List<object> ContactCatList
        {
            get
            {
                return _ContactCatList;
            }
            set
            {
                if (_ContactCatList == value)
                    return;
                _ContactCatList = value;
                RaisePropertyChanged("ContactCatList");
            }
        }
        #endregion
        #region SelectedContactCat
        private object _SelectedContactCat;
        public object SelectedContactCat
        {
            get
            {
                return _SelectedContactCat;
            }
            set
            {
                if (ReferenceEquals(_SelectedContactCat, value)) return;
                _SelectedContactCat = value;
                RaisePropertyChanged("SelectedContactCat");

                UpdateDatasource();
            }
        }
        #endregion
        private void PopulateContactCat()
        {
            if (!AMembership.IsAuthorized) return;
            var xpc = (from n in ACacheData.GetContactCatList() let cat = n.Tag as DBCTContactCat orderby cat.Title select cat).ToList();
            var dbc = SelectedContactCat as DBCTContactCat;
            ContactCatList = new List<object> { "(همه دسته ها)" };
            if (AMembership.ActiveUser.UserName.ToLower() == "admin")

                xpc.ToList().ForEach(n => ContactCatList.Add(n));
            else
                xpc.ToList().Where(n => n.Role != null && AMembership.ActiveUser.Roles.Select(r => r.ToLower()).Contains(n.Role.ToLower())).ToList().ForEach(n => ContactCatList.Add(n));

            var vv2 = ContactCatList.FirstOrDefault();
            var vv1 = dbc == null ? vv2 : ContactCatList.FirstOrDefault(n => (n is DBCTContactCat) && ((DBCTContactCat)n).Title == dbc.Title);
            SelectedContactCat = dbc != null ? vv1 : vv2;
        }
        private void CategoryRefresh()
        {
            PopulateContactCat();
        }




        #region [METHODS]
        private void InitDynamics()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGrid = MainView.DynamicGrid;
        }
        private void InitCommands()
        {
            CommandSelectReport = new RelayCommand(SelectReport, () => true);
            CommandRefresh = new RelayCommand(Refresh, () => SelectedReport != null);
            CommandExportXls = new RelayCommand(ExportXls, () => SelectedReport != null);
            CommandDeleteData = new RelayCommand(DeleteData, () => SelectedReport != null);
            CommandMoveToBasket = new RelayCommand(MoveToBasket, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp56 != "");

            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);
        }

        private void MoveToBasket()
        {

            var srh = DynamicGrid.GetSelectedRowHandles();
            if (srh.Count() > 0)
            {
                var type = DynamicGrid.GetRow(srh[0]).GetType();

                var oids = (from n in srh
                            let m = GetInstanceField(type, DynamicGrid.GetRow(n), "_values")
                            where m != null
                            let o = m as object[]
                            where o != null && o.Length > 0 && o[0] is Guid
                            select (Guid)(o[0])
                            ).ToList();

                APOCMainWindow.ShowAddToBasket(DynamicOwner, null, null, oids);
            }
        }
        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = type.GetField(fieldName, bindFlags);
            return field == null ? null : field.GetValue(instance);
        }
        private void DeleteData()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("موارد انتخاب شده حذف شود؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;
            var needRefresh = false;
            var tv = ((TableView)DynamicGrid.View);
            var scells = tv.GetSelectedCells();
            foreach (var gridCell in scells)
            {
                var row = DynamicGrid.GetRow(gridCell.RowHandle);
                var code = (int)DynamicGrid.GetCellValue(gridCell.RowHandle, DynamicGrid.Columns[0]);

                var q = (from n in HolderColList where n.FieldName == gridCell.Column.FieldName select n).FirstOrDefault();
                if (q != null && q.ProfileItem != null)
                {
                    var dbv = DBCTProfileValue.FindByContactCodeAndItem(ADatabase.Dxs, code, q.ProfileItem.Oid);
                    if (dbv != null)
                    {
                        dbv.Delete();
                        dbv.Save();
                        needRefresh = true;
                    }
                }

            }
            if (needRefresh)
                Refresh();
        }
        private void ExportXls()
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xls",
                Filter = "Excel File (*.xls)|*.xls",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "Data.xls"
            };
            if (sf.ShowDialog() != true) return;
            var options = new XlsExportOptions
                {
                    TextExportMode = TextExportMode.Text,
                    ExportMode = XlsExportMode.SingleFile,
                    Suppress256ColumnsWarning = true,
                    Suppress65536RowsWarning = true
                };
            try
            {
                ((TableView)DynamicGrid.View).ExportToXls(sf.FileName, options);
                HelperUtils.Try(() => Process.Start(sf.FileName));
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }
        private void Refresh()
        {
            UpdateDatasource();
        }
        private void SelectReport()
        {
            var w = new WProfileReportSelect
                        {
                            Owner = DynamicOwner
                        };
            if (w.ShowDialog() == true)
            {
                SelectedReport = w.DynamicSelectedData;
                UpdateDatasource();
            }
        }

        private void UpdateDatasource()
        {
            if (SelectedReport == null)
            {
                DataSource = null;
                return;
            }
            var reportPack = new MetaDataProfileReportPack { Items = null };

            try
            {
                var sz = new XmlSerializer(typeof(MetaDataProfileReportPack));
                reportPack = (MetaDataProfileReportPack)sz.Deserialize(new StringReader(SelectedReport.MetaData));
            }
            catch
            {
            }

            try
            {



                var sql = GenerateSQL(reportPack);
                var cs = ADatabase.Dxs.ConnectionString;
                var sqlcn = new SqlConnection(cs);
                sqlcn.Open();
                var myCommand = new SqlCommand(sql, sqlcn);
                var reader = myCommand.ExecuteReader();


                DynamicGrid.ItemsSource = reader;
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

            var collist = (from n in DynamicGrid.Columns select n).ToList();
            foreach (var col in collist)
            {
                if (!(col.Name == "colCode" || col.Name == "colTitle"))
                    DynamicGrid.Columns.Remove(col);
            }


            var rv = new System.Text.StringBuilder();
            rv.Append("SELECT ");
            rv.Append("[tContact].[Oid] , [tContact].[Code] as ContactCode, [tContact].[Title] as ContactTitle ");
            if (SelectedReport.CCreatedAt)
            {
                rv.Append(",[tContact].[DateCreated] as CCreatedAt");
                DynamicGrid.Columns.Add(new GridColumn() { FieldName = "CCreatedAt", Header = "ثبت در" });
                HolderColList.Add(new HolderCol { FieldName = "CCreatedAt", ProfileItem = null });
            }
            if (SelectedReport.CCreator)
            {
                rv.Append(",[tContact].[UserCreated] as CCreatedBy");
                DynamicGrid.Columns.Add(new GridColumn() { FieldName = "CCreatedBy", Header = "ثبت توسط" });
                HolderColList.Add(new HolderCol { FieldName = "CCreatedBy", ProfileItem = null });
            }
            if (SelectedReport.CEditedAt)
            {
                rv.Append(",[tContact].[DateModified] as CEditedAt");
                DynamicGrid.Columns.Add(new GridColumn() { FieldName = "CEditedAt", Header = "ویرایش در" });
                HolderColList.Add(new HolderCol { FieldName = "CEditedAt", ProfileItem = null });
            }
            if (SelectedReport.CEditor)
            {
                rv.Append(",[tContact].[UserModified] as CEditor");
                DynamicGrid.Columns.Add(new GridColumn() { FieldName = "CEditor", Header = "ویرایش توسط" });
                HolderColList.Add(new HolderCol { FieldName = "CEditedAt", ProfileItem = null });
            }
            if (SelectedReport.CCategory)
            {
                rv.Append(@",STUFF(
        (
            SELECT ' - ' + tContactCat.Title AS [text()]
                FROM [DBCTContactContacts_DBCTContactCatCategories] OE
                    Inner Join DBCTContactCat tContactCat 
                        ON tContactCat.Oid = OE.Categories
                WHERE tContact.Oid = OE.Contacts
            ORDER BY tContactCat.Title
            FOR XML PATH('')
        ), 1, 1, '') AS Categories ");
                DynamicGrid.Columns.Add(new GridColumn() { FieldName = "Categories", Header = "دسته ها" });
                HolderColList.Add(new HolderCol { FieldName = "Categories", ProfileItem = null });
            }
            if (SelectedReport.CPhone)
            {
                rv.Append(@",STUFF(
        (
            SELECT ' - ' + '(' +tPhone.CityCodeString+ ') ' + tPhone.PhoneNumber AS [text()]
                FROM DBCTPhoneBook tPhone 
                WHERE tContact.Oid = tPhone.Contact            
            FOR XML PATH('')
        ), 1, 1, '') AS Phones ");
                DynamicGrid.Columns.Add(new GridColumn() { FieldName = "Phones", Header = "شماره" });
                HolderColList.Add(new HolderCol { FieldName = "Phones", ProfileItem = null });
            }
            if (SelectedReport.CEmail)
            {
                rv.Append(@",STUFF(
        (
            SELECT ' - ' + tEmail.Address AS [text()]
                FROM DBCTEmail tEmail 
                WHERE tContact.Oid = tEmail.Contact            
            FOR XML PATH('')
        ), 1, 1, '') AS Emails ");
                DynamicGrid.Columns.Add(new GridColumn() { FieldName = "Emails", Header = "ایمیل" });
                HolderColList.Add(new HolderCol { FieldName = "Emails", ProfileItem = null });
            }
            if (SelectedReport.CAddress)
            {
                rv.Append(@",STUFF(
        (
            SELECT ' - ' + tAddress.Address AS [text()]
                FROM DBCTAddress tAddress 
                WHERE tContact.Oid = tAddress.Contact            
            FOR XML PATH('')
        ), 1, 1, '') AS Addresses ");
                DynamicGrid.Columns.Add(new GridColumn() { FieldName = "Addresses", Header = "آدرس" });
                HolderColList.Add(new HolderCol { FieldName = "Addresses", ProfileItem = null });
            }



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
                        DynamicGrid.Columns.Add(new GridColumn() { FieldName = "val" + i.ToString(), Header = rep.Title });
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
                        DynamicGrid.Columns.Add(new GridColumn() { FieldName = "val" + i.ToString(), Header = rep.Title });
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
                    if (item == null) continue;
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
            rv.Append(GenerateFilterForCategories());

            rv.Append(" WHERE [tContact].[GCRecord] is NULL ");

            return rv.ToString();
        }

        private string GenerateFilterForCategories()
        {
            var rv = string.Empty;

            if (AMembership.ActiveUser.UserName != "admin" && !(SelectedContactCat is DBCTContactCat))
            {
                foreach (var cat in ContactCatList)
                {
                    if (cat is DBCTContactCat)
                    {
                        if (!string.IsNullOrEmpty(rv))
                            rv += " OR ";
                        rv += " oe.Categories = '" + ((DBCTContactCat)cat).Oid + "' ";
                    }
                }
                if (!string.IsNullOrEmpty(rv))
                {
                    rv = " Right JOIN  (SELECT  Contacts,Categories from [DBCTContactContacts_DBCTContactCatCategories] OE WHERE " + rv + " ) as vv2 on vv2.Contacts = tContact.Oid ";
                }
                if (string.IsNullOrWhiteSpace(rv))
                    rv = " Right JOIN  (SELECT  Contacts,Categories from [DBCTContactContacts_DBCTContactCatCategories] OE WHERE False ) as vv2 on vv2.Contacts = tContact.Oid ";
            }
            if (SelectedContactCat is DBCTContactCat)
            {
                var selcat = (SelectedContactCat as DBCTContactCat).Oid;
                rv += " Right JOIN  (SELECT  Contacts,Categories from [DBCTContactContacts_DBCTContactCatCategories] OE WHERE oe.Categories = '" + selcat + "' ) as vv2 on vv2.Contacts = tContact.Oid ";
            }
            return rv;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp56);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandSelectReport { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandExportXls { get; set; }
        public RelayCommand CommandDeleteData { get; set; }
        public RelayCommand CommandMoveToBasket { get; set; }
        public RelayCommand CommandHelp { get; set; }

        public RelayCommand CommandCategoryRefresh { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion
    }
}
