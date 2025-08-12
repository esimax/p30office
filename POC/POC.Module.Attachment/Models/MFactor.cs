using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Attachment.Views;
using POL.DB.P30Office;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Attachment.Models
{
    public class MFactor : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic DynamicMainView { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private Window DynamicOwner { get; set; }
        private GroupOperator MainSearchCriteria { get; set; }


        public MFactor(object mainView)
        {
            DynamicMainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
           
            InitDynamics();
            InitCommands();

            PopulateContactCat();
            PopulateData();
        }

        #region FactorItemSource
        private XPCollection<DBACFactor> _FactorItemSource;
        public XPCollection<DBACFactor> FactorItemSource
        {
            get { return _FactorItemSource; }
            set
            {
                if (value == _FactorItemSource)
                    return;

                _FactorItemSource = value;
                RaisePropertyChanged("FactorItemSource");
            }
        }
        #endregion
        #region FocusedFactor
        private DBACFactor _FocusedFactor;
        public DBACFactor FocusedFactor
        {
            get { return _FocusedFactor; }
            set
            {
                if (ReferenceEquals(value, _FocusedFactor))
                    return;

                _FocusedFactor = value;
                RaisePropertyChanged("FocusedFactor");
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
                
                PopulateData();
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
            DynamicGridControl = DynamicMainView.DynamicGridControl;
            DynamicTableView = DynamicGridControl.View as TableView;
            DynamicOwner = Window.GetWindow(DynamicMainView);

            DynamicGridControl.MouseDoubleClick +=
                (s, e) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);
                    if (i < 0) return;
                    if (CommandEdit.CanExecute(null))
                        CommandEdit.Execute(null);
                    e.Handled = true;
                };
        }
        private void PopulateData()
        {
            if (ReferenceEquals(MainSearchCriteria, null))
                MainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            MainSearchCriteria.Operands.Clear();

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


            var xpc = DBACFactor.GetAll(ADatabase.Dxs);
            xpc.Criteria = MainSearchCriteria;
            FactorItemSource = null;
            FactorItemSource = xpc;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        DynamicTableView.BestFitColumns();
                    }));
            });
        }
        private void InitCommands()
        {
            CommandNew = new RelayCommand(AddressNew, () => AMembership.HasPermission(PCOPermissions.Factor_Add));
            CommandEdit = new RelayCommand(AddressEdit, () => FocusedFactor != null && AMembership.HasPermission(PCOPermissions.Factor_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => FocusedFactor != null && AMembership.HasPermission(PCOPermissions.Factor_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp10 != "");

            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);
        }

        private void AddressNew()
        {
            var w = new WFactorAddEdit(null,null);
            w.Owner = Window.GetWindow(DynamicMainView);
            w.ShowDialog();
            if (w.DialogResult == true)
                PopulateData();
        }
        private void AddressEdit()
        {
            var w = new WFactorAddEdit(FocusedFactor,FocusedFactor.Contact);
            w.Owner = Window.GetWindow(DynamicMainView);
            w.ShowDialog();
            if (w.DialogResult == true)
                PopulateData();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("فاكتور(های) انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBACFactor).ToList();

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
                            var db2 = DBACFactor.FindByOid(dxs, db.Oid);
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
            PopulateData();
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp10);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandHelp { get; set; }

        public RelayCommand CommandCategoryRefresh { get; set; }
        #endregion
    }
}
