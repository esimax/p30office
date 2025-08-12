using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Automation.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Automation.Models
{
    public class MAutomation : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic DynamicMainView { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private Window DynamicOwner { get; set; }

        public MAutomation(object mainView)
        {
            DynamicMainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                };

            InitDynamics();
            InitCommands();
            PopulateData();
        }


        #region AutomationList
        private XPServerCollectionSource _AutomationList;
        public XPServerCollectionSource AutomationList
        {
            get { return _AutomationList; }
            set
            {
                if (value == _AutomationList)
                    return;

                _AutomationList = value;
                RaisePropertyChanged("AutomationList");
            }
        }
        #endregion
        #region FocusedAutomation
        private DBTMAutomation _FocusedAutomation;
        public DBTMAutomation FocusedAutomation
        {
            get { return _FocusedAutomation; }
            set
            {
                if (ReferenceEquals(value, _FocusedAutomation))
                    return;

                _FocusedAutomation = value;
                RaisePropertyChanged("FocusedAutomation");
            }
        }
        #endregion



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

        private void InitCommands()
        {
            CommandNew = new RelayCommand(AutomationAdd, () => AMembership.HasPermission(PCOPermissions.Automations_Add));
            CommandEdit = new RelayCommand(AutomationEdit, () => FocusedAutomation != null && AMembership.HasPermission(PCOPermissions.Automations_Edit));
            CommandDelete = new RelayCommand(AutomationDelete, () => FocusedAutomation != null && AMembership.HasPermission(PCOPermissions.Automations_Delete));
            CommandRefresh = new RelayCommand(AutomationRefresh, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp15 != "");

        }

        private void PopulateData()
        {
            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBTMAutomation))
            {
                DisplayableProperties = "Oid;Title;PopupEnable;EmailEnable;SMSEnable"
            };
            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.GetNewSession();
            };
            AutomationList = null;
            AutomationList = xpi;
        }


        private void AutomationAdd()
        {
            var w = new WAutomationAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                AutomationRefresh();
        }

        private void AutomationEdit()
        {
            var w = new WAutomationAddEdit(FocusedAutomation) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                AutomationRefresh();
        }

        private void AutomationDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("موارد انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBTMAutomation).ToList();

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
                            var db2 = DBTMAutomation.FindByOid(dxs, db.Oid);
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
            AutomationRefresh();
        }

        private void AutomationRefresh()
        {
            PopulateData();
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp15);
        } 
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

    }
}
