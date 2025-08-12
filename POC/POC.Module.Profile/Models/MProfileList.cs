using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpo;
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
using DevExpress.Xpf.Grid;
using POL.WPF.DXControls;
using POL.Lib.Utils;
using System.Diagnostics;

namespace POC.Module.Profile.Models
{
    public class MProfileList : NotifyObjectBase, IDisposable
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
        #endregion

        private Assembly DynamicDBListAssembly;
        private Type DynamicDBListType;

        #region CTOR
        public MProfileList(object mainView)
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


           
            PopulateProfileListList();
            InitDynamics();
            InitCommands();

            PopulateContactCat();
        }
        #endregion

        #region SelectedProfilrList
        private DBCTList _SelectedProfilrList;
        public DBCTList SelectedProfilrList
        {
            get { return _SelectedProfilrList; }
            set
            {
                if (ReferenceEquals(value, _SelectedProfilrList))
                    return;

                _SelectedProfilrList = value;
                RaisePropertyChanged("SelectedProfilrList");

                UpdateList();
            }
        }
        #endregion

        #region ProfileListList
        private List<DBCTList> _ProfileListList;
        public List<DBCTList> ProfileListList
        {
            get { return _ProfileListList; }
            set
            {
                if (value == _ProfileListList)
                    return;

                _ProfileListList = value;
                RaisePropertyChanged("ProfileListList");
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
                UpdateList();
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
            {
                xpc.ToList().ForEach(n => ContactCatList.Add(n));
            }
            else
            {
                xpc.ToList().Where(n => n.Role != null && AMembership.ActiveUser.Roles.Select(r => r.ToLower()).Contains(n.Role.ToLower())).ToList().ForEach(n => ContactCatList.Add(n));
            }

            var vv2 = ContactCatList.FirstOrDefault();
            var vv1 = dbc == null ? vv2 : ContactCatList.FirstOrDefault(n => (n is DBCTContactCat) && ((DBCTContactCat)n).Title == dbc.Title);
            SelectedContactCat = dbc != null ? vv1 : vv2;
        }
        private void CategoryRefresh()
        {
            PopulateContactCat();
        }


























        private GroupOperator MainSearchCriteria { get; set; }
        private void UpdateList()
        {
            if (SelectedProfilrList == null)
            {
                DataList = null;
                return;
            }
            DynamicDBListAssembly = SelectedProfilrList.GetAssemblyOfListObject();
            DynamicDBListType = DynamicDBListAssembly.GetTypes()[0];

            MainView.DynamicDBList = SelectedProfilrList;
            MainView.GenerateGridColumns();

            if (!AMembership.IsAuthorized) return;

            if (ReferenceEquals(MainSearchCriteria, null))
                MainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            MainSearchCriteria.Operands.Clear();

            try
            {
                var xpi = new XPServerCollectionSource(ADatabase.Dxs, DynamicDBListType)
                {
                };

                if (AMembership.ActiveUser.UserName != "admin" && !(SelectedContactCat is DBCTContactCat))
                {
                    var goCat = new GroupOperator(GroupOperatorType.Or);
                    foreach (var cat in ContactCatList)
                    {
                        if (cat is DBCTContactCat)
                            goCat.Operands.Add(CriteriaOperator.Parse("[Contact.Categories][Oid == {" + ((DBCTContactCat)cat).Oid + "}]"));
                    }
                    if (goCat.Operands.Count > 0)
                        MainSearchCriteria.Operands.Add(goCat);
                }
                if (SelectedContactCat is DBCTContactCat)
                    MainSearchCriteria.Operands.Add(CriteriaOperator.Parse("[Contact.Categories][Oid == {" + ((DBCTContactCat)SelectedContactCat).Oid + "}]"));
                xpi.FixedFilterCriteria = MainSearchCriteria;


                
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
        private void PopulateProfileListList()
        {
            ProfileListList = DBCTList.GetAll(ADatabase.Dxs).ToList();
        }









        #region [METHODS]
        private void InitDynamics()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGrid = MainView.DynamicGrid;
        }
        private void InitCommands()
        {
            CommandDataNew = new RelayCommand(DataNew, () => SelectedProfilrList != null && AMembership.HasPermission(PCOPermissions.ProfileList_Add));
            CommandDataEdit = new RelayCommand(DataEdit, () => SelectedProfilrList != null && FocusedData != null && AMembership.HasPermission(PCOPermissions.ProfileList_Edit));
            CommandDataDelete = new RelayCommand(DataDelete, () => SelectedProfilrList != null && FocusedData != null && AMembership.HasPermission(PCOPermissions.ProfileList_Delete));
            CommandBestFitColumn = new RelayCommand(BestFitColumn, () => SelectedProfilrList != null);
            CommandRefresh = new RelayCommand(DataRefresh, () => SelectedProfilrList != null);
            CommandExportXls = new RelayCommand(ExportXls, () => SelectedProfilrList != null && AMembership.HasPermission(PCOPermissions.ProfileList_Export));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp55 != "");

            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);
        }


        private void DataNew()
        {
            var dbct = APOCMainWindow.ShowSelectContact(Window.GetWindow(MainView), null);
            if (!(dbct is DBCTContact)) return;
            var dbc = dbct as DBCTContact;
            var itemsOid =
                (from n in dbc.ProfileValues
                 where n.ProfileItem.ItemType == EnumProfileItemType.List &&
                       n.ProfileItem.Guid1 == SelectedProfilrList.Oid
                 select n).ToList();

            if (!itemsOid.Any())
            {
                POLMessageBox.ShowWarning("پرونده انتخاب شده دارای فیلد (" + SelectedProfilrList.Title + ") نمی باشد.", Window.GetWindow(MainView));
                return;
            }

            var w = new WListEditor(dbc, null, DynamicDBListType, DynamicDBListAssembly, SelectedProfilrList) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataEdit()
        {
            dynamic db = FocusedData;
            var dbc = db.Contact;

            var w = new WListEditor(dbc, FocusedData, DynamicDBListType, DynamicDBListAssembly, SelectedProfilrList) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGrid.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(i => DynamicGrid.GetRow(i)).Cast<dynamic>().ToList();
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
            UpdateList();
        }
        private void BestFitColumn()
        {
            var view = DynamicGrid.View as TableView;
            if (view != null)
            {
                view.BestFitArea = BestFitArea.All;
                view.BestFitMode = BestFitMode.VisibleRows;
                view.BestFitColumns();
            }
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

        private void Help()
        {
            Process.Start(ConstantPOCHelpURL.POCHelp55);
        }









        #endregion

        #region [COMMANDS]
        public RelayCommand CommandDataNew { get; set; }
        public RelayCommand CommandDataEdit { get; set; }
        public RelayCommand CommandDataDelete { get; set; }

        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandExportXls { get; set; }
        public RelayCommand CommandBestFitColumn { get; set; }
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
