using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using System.Windows;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Call.Models
{
    public class MCallContactModule : NotifyObjectBase, IDisposable, IRefrashable
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
        private bool HasLoadedLayout { get; set; }
        private const string ModuleID = "3FB8E280-7B56-42D0-81E8-CBCBD697F322";
        private DBCTContact CurrentContact { get; set; }


        #region CTOR
        public MCallContactModule(object mainView)
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


        #region IsShowAll
        private bool _IsShowAll;
        public bool IsShowAll
        {
            get { return _IsShowAll; }
            set
            {
                if (!_IsShowAll || value || IsShowCallIn || IsShowCallOut)
                {
                    _IsShowAll = value;
                    _IsShowCallIn = !value;
                    _IsShowCallOut = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowCallIn");
                RaisePropertyChanged("IsShowCallOut");
            }
        }
        #endregion
        #region IsShowCallOut
        private bool _IsShowCallOut;
        public bool IsShowCallOut
        {
            get { return _IsShowCallOut; }
            set
            {
                if (!_IsShowCallOut || value || IsShowCallIn || IsShowAll)
                {
                    _IsShowCallOut = value;
                    _IsShowCallIn = !value;
                    _IsShowAll = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowCallIn");
                RaisePropertyChanged("IsShowCallOut");
            }
        }
        #endregion
        #region IsShowCallIn
        private bool _IsShowCallIn;
        public bool IsShowCallIn
        {
            get { return _IsShowCallIn; }
            set
            {
                if (!_IsShowCallIn || value || IsShowAll || IsShowCallOut)
                {
                    _IsShowCallIn = value;
                    _IsShowAll = !value;
                    _IsShowCallOut = !value;
                    UpdateSearch();
                }
                RaisePropertyChanged("IsShowAll");
                RaisePropertyChanged("IsShowCallIn");
                RaisePropertyChanged("IsShowCallOut");
            }
        }
        #endregion

        #region CallList
        private XPServerCollectionSource _CallList;
        public XPServerCollectionSource CallList
        {
            get { return _CallList; }
            set
            {
                _CallList = value;
                RaisePropertyChanged("CallList");
            }
        }
        #endregion
        #region FocusedCall
        private DBCLCall _FocusedCall;
        public DBCLCall FocusedCall
        {
            get
            {
                return _FocusedCall;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedCall)) return;
                _FocusedCall = value;
                RaisePropertyChanged("FocusedCall");
            }
        }
        #endregion FocusedCall


        #region SelectedRowCount
        public int SelectedRowCount
        {
            get
            {
                return DynamicGrid.GetSelectedRowHandles().Length;
            }
        }
        #endregion
        #region DeviceHasInternal
        public bool DeviceHasInternal
        {
            get { return APOCCore.STCI.Device == EnumDeviceUsed.Panasonic; }
        }
        #endregion
        #region DeviceHasRecord
        public bool DeviceHasRecord
        {
            get { return APOCCore.STCI.DeviceHasRecord; }
        }
        #endregion
        #region DeviceHasVoiceMessage
        public bool DeviceHasVoiceMessage
        {
            get { return APOCCore.STCI.DeviceHasVoiceMessage; }
        }
        #endregion

        public GroupOperator MainSearchCriteria { get; set; }


        #region [METHODS]
        private void InitDynamics()
        {
            DynamicGrid = MainView.DynamicGrid;
            DynamicTableView = DynamicGrid.View as TableView;
            InitGrid();

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
        }
        private void InitGrid()
        {
            if (!DeviceHasInternal)
            {
                var q = from n in DynamicGrid.Columns
                        where n.FieldName == "InternalString" || n.FieldName == "LastExt"
                        select n;
                if (q.Any())
                {
                    q.ToList().ForEach(n => DynamicGrid.Columns.Remove(n));
                }
            }
            if (!APOCCore.STCI.DeviceHasRecord)
            {
                DynamicGrid.Columns.Remove(DynamicGrid.Columns["RecordEnable"]);
                DynamicGrid.Columns.Remove(DynamicGrid.Columns["RecordRole"]);
            }
        }
        private void UpdateSearch()
        {
            if (!AMembership.IsAuthorized) return;
            CurrentContact = AContactModule.SelectedContact as DBCTContact;
            if (CurrentContact == null)
            {
                CallList = null;
                return;
            }

            if (ReferenceEquals(MainSearchCriteria, null))
                MainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            MainSearchCriteria.Operands.Clear();

            MainSearchCriteria.Operands.Add(new BinaryOperator("Del", false));
            MainSearchCriteria.Operands.Add(new BinaryOperator("Contact.Oid", CurrentContact.Oid));

            if (!AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_1))
                MainSearchCriteria.Operands.Add(new BinaryOperator("LineNumber", 1, BinaryOperatorType.NotEqual));
            if (!AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_2))
                MainSearchCriteria.Operands.Add(new BinaryOperator("LineNumber", 2, BinaryOperatorType.NotEqual));
            if (!AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_3))
                MainSearchCriteria.Operands.Add(new BinaryOperator("LineNumber", 3, BinaryOperatorType.NotEqual));
            if (!AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_4))
                MainSearchCriteria.Operands.Add(new BinaryOperator("LineNumber", 4, BinaryOperatorType.NotEqual));

            if (IsShowCallIn)
                MainSearchCriteria.Operands.Add(new BinaryOperator("CallType", EnumCallType.CallIn));
            if (IsShowCallOut)
                MainSearchCriteria.Operands.Add(new BinaryOperator("CallType", EnumCallType.CallOut));

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBCLCall)) { FixedFilterCriteria = MainSearchCriteria };
            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.Dxs;
            };
            CallList = null;
            CallList = xpi;
        }
        private void RestoreCallGridLayout()
        {
            HelperUtils.Try(
                () =>
                {
                    var fn = Path.Combine(APOCCore.LayoutPath, string.Format("{0}_CallLog.XML", ModuleID));
                    DynamicGrid.RestoreLayoutFromXml(fn);
                });
        }
        private void SaveCallGridLayout()
        {
            HelperUtils.Try(
                () =>
                {
                    var fn = Path.Combine(APOCCore.LayoutPath, string.Format("{0}_CallLog.XML", ModuleID));
                    DynamicGrid.SaveLayoutToXml(fn);
                });
        }

        private void InitCommands()
        {
            CommandShowAll = new RelayCommand(() => { }, () => AContactModule.SelectedContact != null);
            CommandShowCallOut = new RelayCommand(() => { }, () => AContactModule.SelectedContact != null);
            CommandShowCallIn = new RelayCommand(() => { }, () => AContactModule.SelectedContact != null);

            CommandCallDeleteSingle = new RelayCommand(CallDeleteSingle, () => FocusedCall != null && AMembership.HasPermission(PCOPermissions.Call_AllowDelete));
            CommandCallDeleteAll = new RelayCommand(CallDeleteAll, () => FocusedCall != null && AMembership.HasPermission(PCOPermissions.Call_AllowDelete));

            CommandRefresh = new RelayCommand(Refresh, () => true);
            CommandSetCallNote = new RelayCommand(SetCallNote, () => FocusedCall != null && AMembership.HasPermission(PCOPermissions.Contact_Calls_AddTag));


            CommandVoicePlay = new RelayCommand(VoicePlay, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Contact_Calls_RecordPlay));
            CommandVoiceDelete = new RelayCommand(VoiceDelete, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Contact_Calls_RecordDelete));
            CommandVoiceSave = new RelayCommand(VoiceSave, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Contact_Calls_RecordSave));
            CommandVoiceRole1 = new RelayCommand(VoiceRole1, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Contact_Calls_RecordChangeRole));
            CommandVoiceRole2 = new RelayCommand(VoiceRole2, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Contact_Calls_RecordChangeRole));
            CommandVoiceRole3 = new RelayCommand(VoiceRole3, () => DeviceHasRecord && FocusedCall != null && FocusedCall.RecordEnable && AMembership.HasPermission(PCOPermissions.Contact_Calls_RecordChangeRole));

            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp23 != "");
        }
        private void CallDeleteSingle()
        {
            if (SelectedRowCount <= 0) return;
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("تعداد {0} تماس حذف شود؟", SelectedRowCount), DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;
            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("حذف تماس", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCLCall> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCLCall).ToList();
                        }));

                    w.AsyncSetText(1, "در حال حذف");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.PhoneNumber);
                            v.Del = true;
                            v.DelUser = AMembership.IsAuthorized ? AMembership.ActiveUser.UserName : string.Empty;
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
                    POLMessageBox.ShowInformation(String.Format("تعداد {0} تماس با موفقیت حذف شد.{1}تعداد خطا ها : {2}", successCount, Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, DynamicOwner);
        }
        private void CallDeleteAll()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("تمام تماس های نمایش داده شده حذف شود؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;
            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("حذف تماس", true, 0, SelectedRowCount, 3,
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
                            uow.Update<DBCLCall>(
                                  () => new DBCLCall(uow)
                                  {
                                      Del = true,
                                      DelUser = AMembership.ActiveUser.UserName,
                                  },
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
        private void SetCallNote()
        {
            APOCMainWindow.ShowSetCallNote(APOCMainWindow.GetWindow(), FocusedCall);
        }

        private void VoicePlay()
        {
            var serverport = HelperSettingsClient.ServerPort;
            var servername = HelperSettingsClient.ServerName;

            var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                            servername,
                                                            Convert.ToInt32(serverport) + ConstantGeneral.ProtocolVoiceRecordPortOffset,
                                                            ConstantGeneral.ProtocolVoiceRecordServiceName));
            var binding = new NetTcpBinding
            {
                TransferMode = TransferMode.Streamed,
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                MaxReceivedMessageSize = long.MaxValue,
                Security = { Mode = SecurityMode.None }
            };

            var channel = new ChannelFactory<Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord>(binding, address);

            var proxy = channel.CreateChannel();

            var fileName = string.Format("{0}{1}.wav", Path.GetTempPath(), Guid.NewGuid());
            POLProgressBox.Show("ذخیره صدا", false, 0, 100, 1,
                pb =>
                {
                    try
                    {
                        var v2 = proxy.DownloadFile(new Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter { RecordTag = FocusedCall.RecordTag });
                        using (Stream file = File.Create(fileName))
                        {
                            CopyStream(v2.FileByteStream, file, v2.Length, pb);
                        }
                    }
                    catch (Exception ex)
                    {
                        HelperUtils.DoDispatcher(
                            () => POLMessageBox.ShowError(ex.Message, pb));
                    }
                },
                pb =>
                {
                    var fi = new FileInfo(fileName);
                    if (fi.Exists)
                    {
                        System.Diagnostics.Process.Start(fileName);
                    }
                }, APOCMainWindow.GetWindow());
        }
        private void VoiceDelete()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات صوتی برای تماس های انتخاب شده حذف شود؟", APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;

            var serverport = HelperSettingsClient.ServerPort;
            var servername = HelperSettingsClient.ServerName;

            var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                            servername,
                                                            Convert.ToInt32(serverport) + ConstantGeneral.ProtocolVoiceRecordPortOffset,
                                                            ConstantGeneral.ProtocolVoiceRecordServiceName));
            var binding = new NetTcpBinding
            {
                TransferMode = TransferMode.Streamed,
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                MaxReceivedMessageSize = long.MaxValue,
                Security = { Mode = SecurityMode.None }
            };

            var channel = new ChannelFactory<Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord>(binding, address);

            var proxy = channel.CreateChannel();

            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("حذف", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCLCall> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCLCall).ToList();
                        }));

                    w.AsyncSetText(1, "در حال حذف");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.PhoneNumber);


                            if (!v.RecordEnable && v.RecordRole == 1 && string.IsNullOrEmpty(v.RecordTag)) continue;

                            var v2 = proxy.DeleteMapItem(new Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter { RecordTag = v.RecordTag });
                            if (v2.Succeed)
                                successCount++;
                            else
                                failedCount++;
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
                    POLMessageBox.ShowInformation(String.Format("تعداد {0} مكالمه با موفقیت حذف شد.{1}تعداد خطا ها : {2}", successCount, Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, APOCMainWindow.GetWindow());
        }
        private void VoiceSave()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK) return;
            var path = dialog.SelectedPath;

            var serverport = HelperSettingsClient.ServerPort;
            var servername = HelperSettingsClient.ServerName;

            var address = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}",
                                                            servername,
                                                            Convert.ToInt32(serverport) + ConstantGeneral.ProtocolVoiceRecordPortOffset,
                                                            ConstantGeneral.ProtocolVoiceRecordServiceName));
            var binding = new NetTcpBinding
            {
                TransferMode = TransferMode.Streamed,
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                MaxReceivedMessageSize = long.MaxValue,
                Security = { Mode = SecurityMode.None }
            };

            var channel = new ChannelFactory<Library.WCF.ProxyVoiceRecord.IProtocolVoiceRecord>(binding, address);

            var proxy = channel.CreateChannel();

            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("ذخیره", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCLCall> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCLCall).ToList();
                        }));

                    w.AsyncSetText(1, "در حال ذخیره");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.PhoneNumber);
                            if (!v.RecordEnable) continue;
                            var fn = string.Format("{0}\\{1}-{2}-{3}.wav",
                                path,
                                HelperLocalize.DateTimeToString(v.CallDate.Value, HelperLocalize.ApplicationCalendar, "yyMMdd"),
                                string.Format("{0:HHmmss}", v.CallDate.Value),
                                v.LineNumber);

                            var v2 = proxy.DownloadFile(new Library.WCF.ProxyVoiceRecord.VoiceRecordDownloadParameter { RecordTag = v.RecordTag });
                            using (Stream file = File.Create(fn))
                            {
                                CopyStream(v2.FileByteStream, file, v2.Length, w);
                            }
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
                    POLMessageBox.ShowInformation(String.Format("تعداد {0} مكالمه با موفقیت ذخیره شد.{1}تعداد خطا ها : {2}", successCount, Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                    HelperUtils.Try(() => System.Diagnostics.Process.Start(path));
                }, APOCMainWindow.GetWindow());
        }
        private void VoiceRole1()
        {
            SetVoiceRole(1);
        }
        private void VoiceRole2()
        {
            SetVoiceRole(2);
        }
        private void VoiceRole3()
        {
            SetVoiceRole(3);
        }

        private void SetVoiceRole(int role)
        {
            if (SelectedRowCount <= 0) return;
            if (SelectedRowCount > 1)
            {
                var dr = POLMessageBox.ShowQuestionYesNo(string.Format("تعداد {0} تماس از قانون شماره 3 پیروی كنند؟", SelectedRowCount),
                                                         APOCMainWindow.GetWindow());
                if (dr != MessageBoxResult.Yes) return;
            }
            var successCount = 0;
            var failedCount = 0;
            POLProgressBox.Show("تغییر قانون حذف", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCLCall> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                        {
                            list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCLCall).ToList();
                        }));

                    w.AsyncSetText(1, "در حال تغییر");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.PhoneNumber);
                            if (!v.RecordEnable) continue;
                            if (v.RecordRole == role) continue;
                            v.RecordRole = role;
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
                    if (SelectedRowCount == 1) return;
                    POLMessageBox.ShowInformation(
                        String.Format("تعداد {0} تماس با موفقیت تغییر كرد.{1}تعداد خطا ها : {2}", successCount,
                                      Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, APOCMainWindow.GetWindow());
        }
        private void CopyStream(Stream input, Stream output, long size, POLProgressBox pb)
        {
            long current = 0;
            var buffer = new byte[10 * 1024];
            int len;
            var lastp = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
                current += len;
                var p = (int)((current * 100) / size);
                if (p == lastp) continue;
                pb.AsyncSetValue(p);
                lastp = p;
            }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp23);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandShowAll { get; set; }
        public RelayCommand CommandShowCallOut { get; set; }
        public RelayCommand CommandShowCallIn { get; set; }

        public RelayCommand CommandCallDeleteSingle { get; set; }
        public RelayCommand CommandCallDeleteAll { get; set; }

        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandSetCallNote { get; set; }

        public RelayCommand CommandPrint { get; set; }

        public RelayCommand CommandVoicePlay { get; set; }
        public RelayCommand CommandVoiceDelete { get; set; }
        public RelayCommand CommandVoiceSave { get; set; }
        public RelayCommand CommandVoiceRole1 { get; set; }
        public RelayCommand CommandVoiceRole2 { get; set; }
        public RelayCommand CommandVoiceRole3 { get; set; }
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
