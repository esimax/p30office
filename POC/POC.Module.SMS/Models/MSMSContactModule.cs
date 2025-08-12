using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.SMS.Models
{
    public class MSMSContactModule : NotifyObjectBase, IDisposable, IRefrashable
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private POCCore APOCCore { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }


        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGrid { get; set; }
        private TableView DynamicTableView { get; set; }
        public TextEdit DynamicBody { get; set; }
        private bool HasLoadedLayout { get; set; }
        private const string ModuleID = "F79BC47B-1293-427C-8802-985BB87F5205";
        private DBCTContact CurrentContact { get; set; }


        #region CTOR
        public MSMSContactModule(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;

            InitDynamics();
            InitCommands();

            UpdateSearch();
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

        #region SMSList
        private XPServerCollectionSource _SMSList;
        public XPServerCollectionSource SMSList
        {
            get { return _SMSList; }
            set
            {
                _SMSList = value;
                RaisePropertyChanged("SMSList");
            }
        }
        #endregion

        #region FocusedSMS
        private DBSMLog2 _FocusedSMS;
        public DBSMLog2 FocusedSMS
        {
            get
            {
                return _FocusedSMS;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedSMS)) return;
                _FocusedSMS = value;
                RaisePropertyChanged("FocusedSMS");
                if (_FocusedSMS != null)
                    DynamicBody.FlowDirection = _FocusedSMS.IsRTL
                                                    ? FlowDirection.RightToLeft
                                                    : FlowDirection.LeftToRight;
            }
        }
        #endregion
        #region IsShowAll
        private bool _IsShowAll;
        public bool IsShowAll
        {
            get { return _IsShowAll; }
            set
            {
                if (!_IsShowAll || value || IsShowSMSIn || IsShowSMSOut)
                {
                    _IsShowAll = value;
                    _IsShowSMSIn = !value;
                    _IsShowSMSOut = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowSMSIn");
                RaisePropertyChanged("IsShowSMSOut");
            }
        }
        #endregion
        #region IsShowSMSOut
        private bool _IsShowSMSOut;
        public bool IsShowSMSOut
        {
            get { return _IsShowSMSOut; }
            set
            {
                if (!_IsShowSMSOut || value || IsShowSMSIn || IsShowAll)
                {
                    _IsShowSMSOut = value;
                    _IsShowSMSIn = !value;
                    _IsShowAll = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowSMSIn");
                RaisePropertyChanged("IsShowSMSOut");
            }
        }
        #endregion
        #region IsShowSMSIn
        private bool _IsShowSMSIn;
        public bool IsShowSMSIn
        {
            get { return _IsShowSMSIn; }
            set
            {
                if (!_IsShowSMSIn || value || IsShowAll || IsShowSMSOut)
                {
                    _IsShowSMSIn = value;
                    _IsShowAll = !value;
                    _IsShowSMSOut = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowSMSIn");
                RaisePropertyChanged("IsShowSMSOut");
            }
        }
        #endregion

        #region SelectedRowCount
        public int SelectedRowCount
        {
            get
            {
                return DynamicGrid.GetSelectedRowHandles().Length;
            }
        }
        #endregion
        public GroupOperator MainSearchCriteria { get; set; }


        #region [METHODS]
        private void InitDynamics()
        {
            DynamicGrid = MainView.DynamicGrid;
            DynamicTableView = DynamicGrid.View as TableView;
            DynamicBody = MainView.DynamicBody;

            DynamicOwner = Window.GetWindow((FrameworkElement)MainView);

            APOCMainWindow.GetWindow().Closing +=
                (s, e) => SaveCallGridLayout();
            if (DynamicTableView != null)
                DynamicTableView.Loaded +=
                    (s, e) =>
                    {
                        if (HasLoadedLayout) return;
                        RestoreCallGridLayout();
                        HasLoadedLayout = true;
                    };
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp63 != "");

        }
        private void UpdateSearch()
        {
            if (!AMembership.IsAuthorized) return;
            CurrentContact = AContactModule.SelectedContact as DBCTContact;
            if (CurrentContact == null)
            {
                SMSList = null;
                return;
            }

            if (ReferenceEquals(MainSearchCriteria, null))
                MainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            MainSearchCriteria.Operands.Clear();

            MainSearchCriteria.Operands.Add(new BinaryOperator("Phone.Contact.Oid", CurrentContact.Oid));

            if (IsShowSMSIn)
                MainSearchCriteria.Operands.Add(new BinaryOperator("SmsType", EnumSmsType.Receive));
            if (IsShowSMSOut)
                MainSearchCriteria.Operands.Add(new BinaryOperator("SmsType", EnumSmsType.SendDone));

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBSMLog2)) { FixedFilterCriteria = MainSearchCriteria };
            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.Dxs;
            };
            SMSList = null;
            SMSList = xpi;
        }
        private void RestoreCallGridLayout()
        {
            HelperUtils.Try(
                () =>
                {
                    var fn = Path.Combine(APOCCore.LayoutPath, string.Format("{0}_SMSLog.XML", ModuleID));
                    DynamicGrid.RestoreLayoutFromXml(fn);
                });
        }
        private void SaveCallGridLayout()
        {
            HelperUtils.Try(
                () =>
                {
                    var fn = Path.Combine(APOCCore.LayoutPath, string.Format("{0}_SMSLog.XML", ModuleID));
                    DynamicGrid.SaveLayoutToXml(fn);
                });
        }

        private void InitCommands()
        {
            CommandShowAll = new RelayCommand(() => { }, () => AContactModule.SelectedContact != null);
            CommandShowSMSOut = new RelayCommand(() => { }, () => AContactModule.SelectedContact != null);
            CommandShowSMSIn = new RelayCommand(() => { }, () => AContactModule.SelectedContact != null);

            CommandSMSDeleteSingle = new RelayCommand(SMSDeleteSingle, () => FocusedSMS != null && AMembership.HasPermission(PCOPermissions.SMS_Delete));
            CommandSMSDeleteAll = new RelayCommand(SMSDeleteAll, () => FocusedSMS != null && AMembership.HasPermission(PCOPermissions.SMS_Delete));

            CommandRefresh = new RelayCommand(Refresh, () => true);
            CommandNew = new RelayCommand(SendSMS, () => AMembership.HasPermission(PCOPermissions.SMS_Send));
        }

        private void SendSMS()
        {
            var phones = new List<string>();

            var contactList = new List<DBCTContact>();

            if (AContactModule.SelectedContact != null)
            {
                var contact = AContactModule.SelectedContact as DBCTContact;
                if (contact != null)
                {
                    contactList.Add(contact);
                    contact.Phones.ToList().ForEach(
                        p =>
                        {
                            if (p.PhoneNumber.StartsWith("0"))
                                phones.Add(p.PhoneNumber);
                        });
                }
            }
            APOCMainWindow.ShowSendSMS(APOCMainWindow.GetWindow(), EnumSelectionType.SelectedContact, null,
                                       AContactModule.SelectedContact as DBCTContact, contactList, null, null, phones, string.Empty);
        }

        private void SMSDeleteSingle()
        {
            if (SelectedRowCount <= 0) return;
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("تعداد {0} پیامك حذف شود؟", SelectedRowCount), DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;
            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("حذف پیامك", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBSMLog2> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBSMLog2).ToList();
                        }));

                    w.AsyncSetText(1, "در حال حذف");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.To);
                            v.Delete();
                            v.Save();
                            successCount++;
                        }
                        catch
                        {
                            failedCount++;
                        }
                        w.AsyncSetText(3, string.Format("موفقیت : {0}  - خطا : {1}", successCount, failedCount));
                    }
                },
                w =>
                {
                    POLMessageBox.ShowInformation(String.Format("تعداد {0} پیامك با موفقیت حذف شد.{1}تعداد خطا ها : {2}", successCount, Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, DynamicOwner);
        }
        private void SMSDeleteAll()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("تمام پیامك های نمایش داده شده حذف شود؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;
            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("حذف پیامك", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال حذف");

                    try
                    {
                        var fc = MainSearchCriteria;
                        HelperUtils.DoDispatcher(
                            () =>
                            {
                                var opEmpty = MainSearchCriteria.Operands.Where(opGo => ReferenceEquals(opGo, null) || (opGo is GroupOperator && ((GroupOperator)opGo).Operands.Count == 0)).ToList();
                                opEmpty.ForEach(op => MainSearchCriteria.Operands.Remove(op));
                                if (!ReferenceEquals(DynamicGrid.FilterCriteria, null))
                                    fc = new GroupOperator(MainSearchCriteria, DynamicGrid.FilterCriteria);
                            });

                        using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                        {
                            uow.Delete<DBSMLog2>(
                                  fc);
                            uow.CommitChanges();
                        }
                        successCount = 1;
                    }
                    catch
                    {
                        failedCount = 1;

                    }
                },
                w =>
                {
                    if (successCount > 0)
                        POLMessageBox.ShowInformation("عملیات انجام شد.", w);
                    if (failedCount > 0)
                        POLMessageBox.ShowError("بروز خطا در حذف اطلاعات.", w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, DynamicOwner);
        }

        private void Refresh()
        {
            APOCMainWindow.ShowBusyIndicator();
            var i = DynamicTableView.TopRowIndex;
            var srhs = DynamicGrid.GetSelectedRowHandles();
            UpdateSearch();
            DynamicGrid.UnselectAll();

            srhs.ToList().ForEach(r => DynamicGrid.SelectItem(r));
            DynamicTableView.TopRowIndex = i;
            APOCMainWindow.HideBusyIndicator();
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp63);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandShowAll { get; set; }
        public RelayCommand CommandShowSMSOut { get; set; }
        public RelayCommand CommandShowSMSIn { get; set; }

        public RelayCommand CommandSMSDeleteSingle { get; set; }
        public RelayCommand CommandSMSDeleteAll { get; set; }

        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            AContactModule.OnSelectedContactChanged -= AContactModule_OnSelectedContactChanged;
        }
        #endregion

        #region IRefrashable
        public void DoRefresh()
        {
            UpdateSearch();
        }

        public bool RequiresRefresh { get; set; }
        #endregion
    }
}
