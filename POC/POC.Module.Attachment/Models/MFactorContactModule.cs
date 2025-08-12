using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Attachment.Views;
using POL.DB.P30Office;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Attachment.Models
{

    public class MFactorContactModule : NotifyObjectBase, IDisposable, IRefrashable
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private UserControl DynamicView { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private DBCTContact CurrentContact { get; set; }

        #region CTOR
        public MFactorContactModule(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();

            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;

            InitCommands();
            GetDynamicData();
            DataRefresh();
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

        #region WindowTitle
        public string WindowTitle
        {
            get { return "فاكتور های " + ((DBCTContact)AContactModule.SelectedContact).Title; }
        }
        #endregion

        #region RootEnable
        public bool RootEnable { get { return AContactModule.SelectedContact != null; } }
        #endregion

        #region FactorList
        private XPCollection<DBACFactor> _FactorList;
        public XPCollection<DBACFactor> FactorList
        {
            get { return _FactorList; }
            set
            {
                _FactorList = value;
                RaisePropertyChanged("FactorList");
            }
        }
        #endregion
        #region FocusedFactor
        private DBACFactor _FocusedFactor;
        public DBACFactor FocusedFactor
        {
            get
            {
                return _FocusedFactor;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedFactor)) return;
                _FocusedFactor = value;
                RaisePropertyChanged("FocusedFactor");
            }
        }
        #endregion 
        


     


        #region [METHODS]

        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGridControl = MainView.DynamicGridControl;
            DynamicTableView = MainView.DynamicTableView;
            DynamicView = MainView as UserControl;
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
        private void PopulateFactorList()
        {
            CurrentContact = AContactModule.SelectedContact as DBCTContact;
            if (CurrentContact == null)
            {
                FactorList = null;
                return;
            }
            var xpc = DBACFactor.GetByContact(ADatabase.Dxs, CurrentContact.Oid);
            FactorList = xpc;
        }
        private void InitCommands()
        {
            CommandNew = new RelayCommand(FactorNew, () => AContactModule.SelectedContact != null && AMembership.HasPermission(PCOPermissions.Contact_Factor_Add));
            CommandEdit = new RelayCommand(FactorEdit, () => FocusedFactor != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.Contact_Factor_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => DynamicGridControl.SelectedItems.Count >= 1 && AMembership.HasPermission(PCOPermissions.Contact_Factor_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp11 != "");
        }



        private void FactorNew()
        {
            var w = new WFactorAddEdit(null, CurrentContact);
            w.Owner = Window.GetWindow(DynamicOwner);
            w.ShowDialog();
            if (w.DialogResult == true)
                PopulateFactorList();
        }
        private void FactorEdit()
        {
            var w = new WFactorAddEdit(FocusedFactor, CurrentContact);
            w.Owner = Window.GetWindow(DynamicOwner);
            w.ShowDialog();
            if (w.DialogResult == true)
                PopulateFactorList();
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
            PopulateFactorList();


            Task.Factory.StartNew(
                () =>
                {
                    System.Threading.Thread.Sleep(500);
                    HelperUtils.DoDispatcher(() => MainView.DynamicBestFitColumn());
                });
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp11);
        } 
        #endregion

      


        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            AContactModule.OnSelectedContactChanged -= AContactModule_OnSelectedContactChanged;
            FactorList = null;
        }
        #endregion

        #region IRefrashable
        public void DoRefresh()
        {
            PopulateFactorList();
            RaisePropertyChanged("RootEnable");
            FocusedFactor = null;
        }

        public bool RequiresRefresh { get; set; }
        #endregion
    }
}
