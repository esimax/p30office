using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Attachment.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Attachment.Models
{
    public class MEventUnit : NotifyObjectBase
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

        public MEventUnit(object mainView)
        {
            DynamicMainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitDynamics();
            InitCommands();
            PopulateData();
        }



        #region EventUnitItemSource
        private XPCollection<DBCTEvent> _EventUnitItemSource;
        public XPCollection<DBCTEvent> EventUnitItemSource
        {
            get { return _EventUnitItemSource; }
            set
            {
                if (value == _EventUnitItemSource)
                    return;

                _EventUnitItemSource = value;
                RaisePropertyChanged("EventUnitItemSource");
            }
        }
        #endregion

        #region FocusedEvent
        private DBCTEvent _FocusedEvent;
        public DBCTEvent FocusedEvent
        {
            get { return _FocusedEvent; }
            set
            {
                if (ReferenceEquals(value, _FocusedEvent))
                    return;

                _FocusedEvent = value;
                RaisePropertyChanged("FocusedEvent");
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
        private void PopulateData()
        {
            var xpc = DBCTEvent.GetAll(ADatabase.Dxs);
            EventUnitItemSource = null;
            EventUnitItemSource = xpc;
        }
        private void InitCommands()
        {
            CommandNew = new RelayCommand(AddressNew, () => AMembership.HasPermission(PCOPermissions.EventUnit_Add));
            CommandEdit = new RelayCommand(AddressEdit, () => FocusedEvent != null && AMembership.HasPermission(PCOPermissions.EventUnit_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => FocusedEvent != null && AMembership.HasPermission(PCOPermissions.EventUnit_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp09 != "");
        }

        private void AddressNew()
        {
            var w = new WEventUnitAddEdit(null);
            w.Owner = Window.GetWindow(DynamicMainView);
            w.ShowDialog();
            if (w.DialogResult == true)
                PopulateData();
        }

        private void AddressEdit()
        {
            var w = new WEventUnitAddEdit(FocusedEvent);
            w.Owner = Window.GetWindow(DynamicMainView);
            w.ShowDialog();
            if (w.DialogResult == true)
                PopulateData();
        }

        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("برنامه(های) انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTEvent).ToList();

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
                            var db2 = DBCTEvent.FindByOid(dxs, db.Oid);
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
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp09);
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
