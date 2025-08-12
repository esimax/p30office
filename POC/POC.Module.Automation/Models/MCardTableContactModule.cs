using System;
using System.Linq;
using System.Windows;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POC.Module.Automation.Views;

namespace POC.Module.Automation.Models
{

    public class MCardTableContactModule : NotifyObjectBase, IDisposable, IRefrashable
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IPOCContactModule AContactModule { get; set; }

        private dynamic DynamicMainView { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCTContact CurrentContact { get; set; }

        #region CTOR
        public MCardTableContactModule(object mainView)
        {
            DynamicMainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();

            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;

            InitCommands();
            GetDynamicData();
            PopulateData();
        }
        #endregion

        void AContactModule_OnSelectedContactChanged(object sender, EventArgs e)
        {
            if (POCContactModuleItem.LasteSelectedVmType == null || POCContactModuleItem.LasteSelectedVmType != GetType())
            {
                RequiresRefresh = true;
                return;
            }
            if (ReferenceEquals(CurrentContact, (AContactModule.SelectedContact as DBCTContact)))
                return;

            DoRefresh();
        }





        #region CardTableList
        private XPServerCollectionSource _CardTableList;
        public XPServerCollectionSource CardTableList
        {
            get { return _CardTableList; }
            set
            {
                if (value == _CardTableList)
                    return;

                _CardTableList = value;
                RaisePropertyChanged("CardTableList");
            }
        }
        #endregion
        #region FocusedCardTable
        private DBTMCardTable2 _FocusedCardTable;
        public DBTMCardTable2 FocusedCardTable
        {
            get { return _FocusedCardTable; }
            set
            {
                if (ReferenceEquals(value, _FocusedCardTable))
                    return;

                _FocusedCardTable = value;
                RaisePropertyChanged("FocusedCardTable");
            }
        }
        #endregion

        #region IsShowAll
        private bool _IsShowAll = true;
        public bool IsShowAll
        {
            get { return _IsShowAll; }
            set
            {
                if (!_IsShowAll || value || IsShowSelf || IsShowOthers)
                {
                    _IsShowAll = value;
                    _IsShowSelf = !value;
                    _IsShowOthers = !value;
                    PopulateData();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowSelf");
                RaisePropertyChanged("IsShowOthers");
            }
        }
        #endregion
        #region IsShowOthers
        private bool _IsShowOthers;
        public bool IsShowOthers
        {
            get { return _IsShowOthers; }
            set
            {
                if (!_IsShowOthers || value || IsShowSelf || IsShowAll)
                {
                    _IsShowOthers = value;
                    _IsShowSelf = !value;
                    _IsShowAll = !value;
                    PopulateData();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowSelf");
                RaisePropertyChanged("IsShowOthers");
            }
        }
        #endregion
        #region IsShowSelf
        private bool _IsShowSelf;
        public bool IsShowSelf
        {
            get { return _IsShowSelf; }
            set
            {
                if (!_IsShowSelf || value || IsShowAll || IsShowOthers)
                {
                    _IsShowSelf = value;
                    _IsShowAll = !value;
                    _IsShowOthers = !value;
                    PopulateData();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowSelf");
                RaisePropertyChanged("IsShowOthers");
            }
        }
        #endregion

        #region IsShowAdmin
        private bool _IsShowAdmin;
        public bool IsShowAdmin
        {
            get { return _IsShowAdmin; }
            set
            {
                _IsShowAdmin = value;
                RaisePropertyChanged("IsShowAdmin");
            }
        }
        #endregion


        #region RootEnable
        public bool RootEnable { get { return AContactModule.SelectedContact != null; } }
        #endregion




        #region [METHODS]
        private void GetDynamicData()
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

            DynamicOwner = DynamicMainView.DynamicOwner;
        }
        private void InitCommands()
        {
            CommandNew = new RelayCommand(CardTableAdd, () => AMembership.HasPermission(PCOPermissions.CardTables_Add));
            CommandEdit = new RelayCommand(CardTableEdit, () => FocusedCardTable != null && AMembership.HasPermission(PCOPermissions.CardTables_Edit));
            CommandDelete = new RelayCommand(CardTableDelete, () => FocusedCardTable != null && AMembership.HasPermission(PCOPermissions.CardTables_Delete));
            CommandRefresh = new RelayCommand(CardTableRefresh, () => true);

            CommandShowAdmin = new RelayCommand(ShowAdmin, () => AMembership.ActiveUser != null && AMembership.ActiveUser.UserName.ToLower() == "admin");
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp17 != "");
        }
        private void ShowAdmin()
        {
            PopulateData();
        }
        private void PopulateData()
        {
            CurrentContact = AContactModule.SelectedContact as DBCTContact;
            if (CurrentContact == null)
            {
                CardTableList = null;
                return;
            }

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBTMCardTable2))
            {
            };


            var MainCriteria = new GroupOperator(GroupOperatorType.Or);

            MainCriteria.Operands.Add(new BinaryOperator("SendToType", 0));

            foreach (var rid in AMembership.ActiveUser.RolesOid)
            {
                var byrole = new GroupOperator();
                byrole.Operands.Add(new BinaryOperator("SendToType", 1));
                byrole.Operands.Add(new BinaryOperator("SendToData", rid));
                byrole.Operands.Add(new BinaryOperator("HasStartingDate", true));
                byrole.Operands.Add(new BinaryOperator("StartingDate", DateTime.Now.Date.AddDays(1), BinaryOperatorType.Less));
                MainCriteria.Operands.Add(byrole);

                byrole = new GroupOperator();
                byrole.Operands.Add(new BinaryOperator("SendToType", 1));
                byrole.Operands.Add(new BinaryOperator("SendToData", rid));
                byrole.Operands.Add(new BinaryOperator("HasStartingDate", false));
                MainCriteria.Operands.Add(byrole);
            }

            var MainCriteria2 = new GroupOperator(GroupOperatorType.And);

            if (!IsShowAdmin)
            {
                var byuser = new GroupOperator();
                byuser.Operands.Add(new BinaryOperator("SendToType", 2));
                byuser.Operands.Add(new BinaryOperator("SendToData", AMembership.ActiveUser.UserID));
                byuser.Operands.Add(new BinaryOperator("HasStartingDate", true));
                byuser.Operands.Add(new BinaryOperator("StartingDate", DateTime.Now.Date.AddDays(1),
                                                       BinaryOperatorType.Less));
                MainCriteria.Operands.Add(byuser);

                byuser = new GroupOperator();
                byuser.Operands.Add(new BinaryOperator("SendToType", 2));
                byuser.Operands.Add(new BinaryOperator("SendToData", AMembership.ActiveUser.UserID));
                byuser.Operands.Add(new BinaryOperator("HasStartingDate", false));
                MainCriteria.Operands.Add(byuser);

                MainCriteria.Operands.Add(new BinaryOperator("UserCreated", AMembership.ActiveUser.UserName));



                MainCriteria2.Operands.Add(MainCriteria);
                if (IsShowSelf)
                {
                    MainCriteria2.Operands.Add(new BinaryOperator("UserCreated", AMembership.ActiveUser.UserName));
                }
                if (IsShowOthers)
                {
                    MainCriteria2.Operands.Add(new BinaryOperator("UserCreated", AMembership.ActiveUser.UserName, BinaryOperatorType.NotEqual));
                }


            }

            MainCriteria2.Operands.Add(new BinaryOperator("LinkContact.Oid", CurrentContact.Oid, BinaryOperatorType.Equal));

            xpi.FixedFilterCriteria = MainCriteria2;
            CardTableList = null;
            DynamicMainView.UpdateCellTemplate();
            CardTableList = xpi;
        }
        private void CardTableAdd()
        {
            var w = new WCardTableAddEdit(null)
            {
                Owner = DynamicOwner,
                DynamicContact = CurrentContact
            };
            if (w.ShowDialog() == true)
                CardTableRefresh();
        }
        private void CardTableEdit()
        {
            var w = new WCardTableAddEdit(FocusedCardTable) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                CardTableRefresh();
        }
        private void CardTableDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("موارد انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBTMCardTable2).ToList();

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
                            var db2 = DBTMCardTable2.FindByOid(dxs, db.Oid);
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
            CardTableRefresh();
        }
        private void CardTableRefresh()
        {
            PopulateData();
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp17);
        }
        #endregion



        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }

        public RelayCommand CommandShowAll { get; set; }
        public RelayCommand CommandShowOthers { get; set; }
        public RelayCommand CommandShowSelf { get; set; }

        public RelayCommand CommandShowAdmin { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            AContactModule.OnSelectedContactChanged -= AContactModule_OnSelectedContactChanged;
            CardTableList = null;
        }
        #endregion

        #region IRefrashable
        public void DoRefresh()
        {
            PopulateData();
            FocusedCardTable = null;
        }

        public bool RequiresRefresh { get; set; }
        #endregion
    }
}
