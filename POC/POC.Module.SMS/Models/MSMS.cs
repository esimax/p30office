using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using GemBox.Spreadsheet;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POL.DB.P30Office;
using POL.DB.P30Office.GL;
using POL.DB.Root;
using POL.Lib.Common;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.SMS.Models
{
    public class MSMS : NotifyObjectBase, IDisposable
    {
        #region Private Properties
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private POCCore APOCCore { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGrid { get; set; }
        private TableView DynamicTableView { get; set; }
        public TextEdit DynamicBody { get; set; }
        private bool HasLoadedLayout { get; set; }
        private const string ModuleID = "A45655E6-BFD3-4900-B527-FB45032DA162";

        private DispatcherTimer SmsUpdateTimer { get; set; }
        #endregion

        #region Constant
        private const int ColContactCode = 0;
        private const int ColContactTitle = 1;
        private const int ColTransDate = 2;
        private const int ColFrom = 3;
        private const int ColTo = 4;
        private const int ColSender = 5;
        private const int ColBody = 6;
        private const int ColSmsResult = 7;
        private const int ColDelivaryResult = 8;
        private const int ColResult = 9;
        #endregion

        #region CTOR
        public MSMS(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            AMembership.OnMembershipStatusChanged +=
               (s, e) =>
               {
                   if (e.Status == EnumMembershipStatus.AfterLogin)
                       PopulateContactCat();
               };


            InitDynamics();
            InitCommands();
            PopulateContactCat();
            UpdateSearch();
        }
        #endregion








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
        #region MainSearchCriteria
        public GroupOperator MainSearchCriteria { get; set; }
        #endregion

        #region QuickSearchText
        private string _QuickSearchText;
        public string QuickSearchText
        {
            get { return _QuickSearchText; }
            set
            {
                _QuickSearchText = value;
                RaisePropertyChanged("QuickSearchText");
                UpdateSearchWithDelay();
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
                Refresh();
            }
        }
        #endregion






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
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp62 != "");

        }
        private void UpdateSearch()
        {
            if (!AMembership.IsAuthorized) return;

            if (ReferenceEquals(MainSearchCriteria, null))
                MainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            MainSearchCriteria.Operands.Clear();


            if (IsShowSMSIn)
                MainSearchCriteria.Operands.Add(new BinaryOperator("SmsType", EnumSmsType.Receive));
            if (IsShowSMSOut)
                MainSearchCriteria.Operands.Add(new BinaryOperator("SmsType", EnumSmsType.SendDone));
            if (SelectedContactCat is DBCTContactCat)
                MainSearchCriteria.Operands.Add(CriteriaOperator.Parse("[Phone.Contact.Categories][Oid == {" + ((DBCTContactCat)SelectedContactCat).Oid + "}]"));

            if (!string.IsNullOrWhiteSpace(QuickSearchText))
            {
                QuickSearchText = QuickSearchText.Replace("%", "").Replace("*", "");
                var go = new GroupOperator(GroupOperatorType.Or);
                go.Operands.Add(new BinaryOperator("Body", "%" + QuickSearchText + "%", BinaryOperatorType.Like));
                go.Operands.Add(new BinaryOperator("To", "%" + QuickSearchText + "%", BinaryOperatorType.Like));
                go.Operands.Add(new BinaryOperator("From", "%" + QuickSearchText + "%", BinaryOperatorType.Like));
                MainSearchCriteria.Operands.Add(go);
            }

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBSMLog2))
            {
                FixedFilterCriteria = MainSearchCriteria,
            };
            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.Dxs;
            };
            SMSList = null;
            SMSList = xpi;
        }
        private void UpdateSearchWithDelay()
        {
            if (SmsUpdateTimer == null)
            {
                SmsUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                SmsUpdateTimer.Tick += (s, e) =>
                {
                    SmsUpdateTimer.Stop();
                    UpdateSearch();
                };
            }
            SmsUpdateTimer.Stop();
            SmsUpdateTimer.Start();
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
            CommandShowAll = new RelayCommand(() => { }, () => true);
            CommandShowSMSOut = new RelayCommand(() => { }, () => true);
            CommandShowSMSIn = new RelayCommand(() => { }, () => true);

            CommandSMSDeleteSingle = new RelayCommand(SMSDeleteSingle, () => FocusedSMS != null && AMembership.HasPermission(PCOPermissions.SMS_Delete));
            CommandSMSDeleteAll = new RelayCommand(SMSDeleteAll, () => FocusedSMS != null && AMembership.HasPermission(PCOPermissions.SMS_Delete));

            CommandRefresh = new RelayCommand(Refresh, () => true);
            CommandNew = new RelayCommand(SendSMS, () => AMembership.HasPermission(PCOPermissions.SMS_Send));
            CommandCopyText = new RelayCommand(CopyText, () => FocusedSMS != null);

            CommandSyncSingle = new RelayCommand(SyncSingle, () => FocusedSMS != null && AMembership.HasPermission(PCOPermissions.Call_AllowSync));
            CommandSyncAll = new RelayCommand(SyncAll, () => AMembership.HasPermission(PCOPermissions.Call_AllowSync));
            CommandGotoContact = new RelayCommand(GotoContact, () => FocusedSMS != null && FocusedSMS.Phone != null);

            CommandSMSResend = new RelayCommand(SMSResend, () => FocusedSMS != null && AMembership.HasPermission(PCOPermissions.SMS_Send));

            CommandQuickSearchClear = new RelayCommand(QuickSearchClear, () => true);

            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);
            CommandExportExcel = new RelayCommand(ExportExcel, () => AMembership.HasPermission(PCOPermissions.SMS_Export));
        }

        private void ExportExcel()
        {
            var criteria = new GroupOperator();
            if (!ReferenceEquals(DynamicGrid.FilterCriteria, null))
                criteria.Operands.Add(DynamicGrid.FilterCriteria);
            if (!ReferenceEquals(SMSList.FixedFilterCriteria, null))
                criteria.Operands.Add(SMSList.FixedFilterCriteria);
            XPGUIDObject.FixEmptyGroupOperand(criteria);

            var xpq = new XPQuery<DBSMLog2>(ADatabase.Dxs).OrderBy(n => n.Phone.Contact.Code);
            var xpq2 = xpq.AppendWhere(new CriteriaToExpressionConverter(), criteria) as XPQuery<DBSMLog2>;
            if (xpq2 == null)
                return;
            var count = xpq2.Count();
            if (count == 0) return;

            if (count > 65000)
            {
                POLMessageBox.ShowInformation("تعدا پیامك ها بیش از 65 هزار می باشد. امكان استخراج وجود ندارد.");
            }

            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xls",
                Filter = "Microsoft Excel 2003 files (*.xls)|*.xls",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "ExportSms.xls"
            };
            if (sf.ShowDialog() != true)
                return;

            var filename = sf.FileName;
            var eFile = new ExcelFile
            {
                DefaultFontName = HelperLocalize.ApplicationFontName,
                DefaultFontSize = (int)HelperLocalize.ApplicationFontSize
            };

            var ws = eFile.Worksheets.Add("پیامك");
            ws.ViewOptions.ShowColumnsFromRightToLeft = HelperLocalize.ApplicationFlowDirection == FlowDirection.RightToLeft;

            var row = 0;
            #region Columns Title
            ws.Cells[row, ColContactCode].Value = "كد پرونده";
            ws.Cells[row, ColContactCode].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColContactTitle].Value = "عنوان پرونده";
            ws.Cells[row, ColContactTitle].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColTransDate].Value = "تاریخ";
            ws.Cells[row, ColTransDate].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColFrom].Value = "از";
            ws.Cells[row, ColFrom].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColTo].Value = "به";
            ws.Cells[row, ColTo].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColSender].Value = "كاربر";
            ws.Cells[row, ColSender].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColBody].Value = "متن";
            ws.Cells[row, ColBody].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColSmsResult].Value = "نتیجه";
            ws.Cells[row, ColSmsResult].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColDelivaryResult].Value = "تاییدیه";
            ws.Cells[row, ColDelivaryResult].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColResult].Value = "خطا";
            ws.Cells[row, ColResult].Style.Font.Name = eFile.DefaultFontName;
            #endregion

            row++;

            POLProgressBox.Show("استخراج اطلاعات پیامك ها", true, 0, count, 2,
                pw =>
                {
                    foreach (var db in xpq2)
                    {
                        try
                        {
                            var cCode = string.Empty;
                            var cTitle = string.Empty;
                            if (db.Phone != null && db.Phone.Contact != null)
                            {
                                cCode = db.Phone.Contact.Code.ToString();
                                cTitle = db.Phone.Contact.Title;
                            }
                            ws.Cells[row, ColContactCode].Value = cCode;
                            ws.Cells[row, ColContactCode].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColContactTitle].Value = cTitle;
                            ws.Cells[row, ColContactTitle].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColTransDate].Value = HelperPersianCalendar.ToString(db.TransDate, "yyyy/MM/dd HH:mm:ss");
                            ws.Cells[row, ColTransDate].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColFrom].Value = db.From;
                            ws.Cells[row, ColFrom].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColTo].Value = db.To;
                            ws.Cells[row, ColTo].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColSender].Value = db.Sender;
                            ws.Cells[row, ColSender].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColBody].Value = db.Body;
                            ws.Cells[row, ColBody].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColSmsResult].Value = db.SmsResult;
                            ws.Cells[row, ColSmsResult].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColDelivaryResult].Value = db.DelivaryResult;
                            ws.Cells[row, ColDelivaryResult].Style.Font.Name = eFile.DefaultFontName;
                        }
                        catch (Exception ex)
                        {
                            ws.Cells[row, ColResult].Value = ex.Message;
                            ws.Cells[row, ColResult].Style.Font.Name = eFile.DefaultFontName;
                        }
                        pw.AsyncSetText(1, string.Format("{0} -> {1}", db.From, db.To));
                        pw.AsyncSetValue(row);
                        row++;

                        if (pw.NeedToCancel)
                        {
                            return;
                        }
                    }
                },
                pw =>
                {
                    eFile.SaveXls(filename);
                    TryToDisplayGeneratedFile(filename);
                }, DynamicOwner);

        }
        private void TryToDisplayGeneratedFile(string fileName)
        {
            try
            {
                var p = Process.Start(fileName);
                if (p != null)
                    SetForegroundWindow(p.MainWindowHandle);
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message);
            }
        }
        private void SMSResend()
        {
            var rows = DynamicGrid.GetSelectedRowHandles();
            if (rows.Count() != 1) return;
            var sms = DynamicTableView.Grid.GetRow(rows[0]) as DBSMLog2;
            if (sms == null) return;
            var dbsms = new DBSMLog2(ADatabase.Dxs)
            {
                Body = sms.Body,
                From = sms.From,
                Phone = sms.Phone,
                DelivaryResult = string.Empty,
                SmsType = EnumSmsType.RequestToSend,

                To = sms.To,
                TransDate = DateTime.Now,
                TransId = 0,
                IsRTL = sms.IsRTL,
            };
            dbsms.Save();
            Refresh();
        }

        private void GotoContact()
        {
            if (FocusedSMS == null) return;
            if (FocusedSMS.Phone == null) return;
            if (FocusedSMS.Phone.Contact == null) return;
            var roles = FocusedSMS.Phone.Contact.Categories.ToList().Select(n => n.Role).Where(r => r != null);
            if (AMembership.ActiveUser.Roles.Select(r => r.ToLower()).Intersect(roles.Select(r => r.ToLower())).Any() || AMembership.ActiveUser.UserName.ToLower() == "admin")
                APOCContactModule.GotoContactByCode(FocusedSMS.Phone.Contact.Code);
            else
            {
                POLMessageBox.ShowWarning("خطا : سطوح دسترسی كافی جهت مشاهده پرونده وجود ندارد.", APOCMainWindow.GetWindow());
            }
        }

        private void SyncAll()
        {
            SMSSync(true);
        }

        private void SyncSingle()
        {
            var CurrentCityOid = APOCCore.STCI.CurrentCityGuid;
            var dbcity = DBGLCity.FindByOid(ADatabase.Dxs, CurrentCityOid);
            var CurrentCityCode = dbcity.PhoneCode;
            var CurrentCountryOid = dbcity.Country.Oid;
            var CurrentCountryCode = dbcity.Country.TeleCode;

            var MobileStartingCode = Convert.ToInt32(APOCCore.STCI.MobileStartingCode);


            var dec = new PhoneDecoder3Code(null, ADatabase.Dxs,
                                CurrentCountryOid, CurrentCountryCode,
                                CurrentCityOid, CurrentCityCode,
                                APOCCore.STCI.MobileLength,
                                MobileStartingCode);

            var pn = FocusedSMS.SmsType == EnumSmsType.Receive ? FocusedSMS.From : FocusedSMS.To;
            var dres = dec.DecodeData(pn.Replace("+", "00"), EnumCallType.CallOut);
            var dbp = DBCTPhoneBook.FindByPhoneAndCode(ADatabase.Dxs, Convert.ToInt32(dres.CountryCode), string.IsNullOrEmpty(dres.CityCode) ? -1 : Convert.ToInt32(dres.CityCode), dres.Phone);
            FocusedSMS.Phone = dbp;
            FocusedSMS.Save();
        }

        private void CopyText()
        {
            if (FocusedSMS == null) return;
            Clipboard.SetText(FocusedSMS.Body);
        }

        private void SendSMS()
        {
            var phones = new List<string>();
            var contactList = DynamicGrid.GetSelectedRowHandles().Select(
                                rowHandle =>
                                {
                                    HelperUtils.AllowUIToUpdate();
                                    var sms = DynamicTableView.Grid.GetRow(rowHandle) as DBSMLog2;
                                    if (sms != null)
                                    {
                                        phones.Add(sms.From);
                                        phones.Add(sms.To);
                                        if (sms.Phone != null)
                                            return sms.Phone.Contact;
                                    }
                                    return null;
                                }).Select(dbc => dbc).ToList();
            contactList.RemoveAll(n => n == null);
            contactList = contactList.Distinct().ToList();
            phones.RemoveAll(n => n == "" || !n.StartsWith("0"));
            phones = phones.Distinct().ToList();


            var focusedContact = FocusedSMS == null ? null : (FocusedSMS.Phone == null ? null : FocusedSMS.Phone.Contact);

            APOCMainWindow.ShowSendSMS(APOCMainWindow.GetWindow(), EnumSelectionType.SelectedContact, null,
                                       focusedContact, contactList, null, null, phones, string.Empty);
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
        private void QuickSearchClear()
        {
            QuickSearchText = null;
            UpdateSearch();
        }
        private void SMSSync(bool ask)
        {
            if (FocusedSMS == null) return;

            POLProgressBox.Show(3, pw
                =>
            {



                var proceed = false;
                var fc = MainSearchCriteria;
                HelperUtils.DoDispatcher(
                    () =>
                    {
                        var opEmpty = MainSearchCriteria.Operands.Where(opGo => ReferenceEquals(opGo, null) || (opGo is GroupOperator && ((GroupOperator)opGo).Operands.Count == 0)).ToList();
                        opEmpty.ForEach(op => MainSearchCriteria.Operands.Remove(op));
                        if (!ReferenceEquals(DynamicGrid.FilterCriteria, null))
                            fc = new GroupOperator(MainSearchCriteria, DynamicGrid.FilterCriteria);
                    });

                var dxs = ADatabase.GetNewSession();


                var CurrentCityOid = APOCCore.STCI.CurrentCityGuid;
                var dbcity = DBGLCity.FindByOid(dxs, CurrentCityOid);
                var CurrentCityCode = dbcity.PhoneCode;
                var CurrentCountryOid = dbcity.Country.Oid;
                var CurrentCountryCode = dbcity.Country.TeleCode;

                var MobileStartingCode = Convert.ToInt32(APOCCore.STCI.MobileStartingCode);


                var dec = new PhoneDecoder3Code(null, dxs,
                                    CurrentCountryOid, CurrentCountryCode,
                                    CurrentCityOid, CurrentCityCode,
                                    APOCCore.STCI.MobileLength,
                                    MobileStartingCode);


                var xpq = new XPQuery<DBSMLog2>(dxs);
                var converter = new CriteriaToExpressionConverter();

                var xpq2 = fc.Operands.Count == 0 ? xpq : xpq.AppendWhere(converter, fc) as XPQuery<DBSMLog2>;
                var count = xpq2.Count();

                if (ask)
                {
                    var doo = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Func<object>)(() =>
                    {
                        var dr = POLMessageBox.ShowQuestionYesNo(String.Format("تطبیق برای {0} ركورد انجام شود؟", count));
                        return dr;
                    }));

                    while (doo.Status != DispatcherOperationStatus.Completed)
                    {
                        Thread.Sleep(1);
                    }

                    var v = (MessageBoxResult)doo.Result;
                    if (v == MessageBoxResult.Yes)
                        proceed = true;
                }
                else
                    proceed = true;

                if (proceed)
                {
                    pw.AsyncEnableCancel();
                    pw.AsyncSetMax(count);
                    pw.AsyncSetText(1, String.Format("تعداد : {0}", count));


                    for (var i = 0; i < count; i = i + 100)
                    {
                        var list = (from n in xpq2 orderby n.Oid select n).Skip(i).Take(100).ToList();
                        var counter = 0;
                        foreach (var dbsms in list)
                        {
                            if (pw.NeedToCancel) return;
                            counter++;
                            pw.AsyncSetText(2, String.Format("شمارش : {0}", i + counter));

                            var pn = dbsms.SmsType == EnumSmsType.Receive ? dbsms.From : dbsms.To;
                            var dres = dec.DecodeData(pn.Replace("+", "00"), EnumCallType.CallOut);
                            var dbp = DBCTPhoneBook.FindByPhoneAndCode(dxs, Convert.ToInt32(dres.CountryCode), string.IsNullOrEmpty(dres.CityCode) ? -1 : Convert.ToInt32(dres.CityCode), dres.Phone);
                            dbsms.Phone = dbp;
                            dbsms.Save();
                        }
                    }
                }
            }, pw =>
            {
                UpdateSearch();
            }, APOCMainWindow.GetWindow());


        }
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

            SelectedContactCat = dbc != null ? ContactCatList.FirstOrDefault(n => (n is DBCTContactCat) && ((DBCTContactCat)n).Title == dbc.Title) : ContactCatList.FirstOrDefault();
        }
        private void CategoryRefresh()
        {
            PopulateContactCat();
        }
        private void Help()
        {
            Process.Start(ConstantPOCHelpURL.POCHelp62);
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

        public RelayCommand CommandCopyText { get; set; }

        public RelayCommand CommandSyncSingle { get; set; }
        public RelayCommand CommandSyncAll { get; set; }
        public RelayCommand CommandGotoContact { get; set; }

        public RelayCommand CommandCategoryRefresh { get; set; }

        public RelayCommand CommandSMSResend { get; set; }

        public RelayCommand CommandQuickSearchClear { get; set; }
        public RelayCommand CommandHelp { get; set; }

        public RelayCommand CommandExportExcel { get; set; }
        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {

        }

        #endregion

        #region [DllImport]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion
    }
}
