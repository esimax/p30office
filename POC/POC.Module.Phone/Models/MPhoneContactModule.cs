using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using POC.Module.Phone.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Utils;
using POL.DB.P30Office;

namespace POC.Module.Phone.Models
{

    public class MPhoneContactModule : NotifyObjectBase, IDisposable, IRefrashable
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule AContactModule { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private UserControl DynamicView { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private DBCTContact CurrentContact { get; set; }

        #region CTOR
        public MPhoneContactModule(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();

            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;

            InitCommands();
            GetDynamicData();

            IsViewFull = HelperSettingsClient.PhoneViewIsFull;
            IsViewSimple = !IsViewFull;
            MainView.DynamicReorderColumns();
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
            get { return "شماره های " + ((DBCTContact)AContactModule.SelectedContact).Title; }
        }
        #endregion

        #region RootEnable
        public bool RootEnable { get { return AContactModule.SelectedContact != null; } }
        #endregion

        #region PhoneList
        private XPCollection<DBCTPhoneBook> _PhoneList;
        public XPCollection<DBCTPhoneBook> PhoneList
        {
            get { return _PhoneList; }
            set
            {
                _PhoneList = value;
                RaisePropertyChanged("PhoneList");
            }
        }
        #endregion
        #region FocusedPhone
        private DBCTPhoneBook _FocusedPhone;
        public DBCTPhoneBook FocusedPhone
        {
            get
            {
                return _FocusedPhone;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedPhone)) return;
                _FocusedPhone = value;
                RaisePropertyChanged("FocusedPhone");
            }
        }
        #endregion FocusedPhone

        public bool IsViewFull { get; set; }
        public bool IsViewSimple { get; set; }

        private Guid CopyCutDataOid { get; set; }
        private bool IsCut { get; set; }






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
        private void PopulatePhoneList()
        {
            CurrentContact = AContactModule.SelectedContact as DBCTContact;
            if (CurrentContact == null)
            {
                PhoneList = null;
                return;
            }
            var xpc = DBCTPhoneBook.GetByContact(ADatabase.Dxs, CurrentContact.Oid);
            PhoneList = xpc;
        }
        private void InitCommands()
        {
            CommandNew = new RelayCommand(PhoneNew, () => AContactModule.SelectedContact != null && AMembership.HasPermission(PCOPermissions.Contact_Phones_New));
            CommandEdit = new RelayCommand(PhoneEdit, () => FocusedPhone != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.Contact_Phones_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => DynamicGridControl.SelectedItems.Count >= 1 && AMembership.HasPermission(PCOPermissions.Contact_Phones_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);

            CommandCut = new RelayCommand(() =>
            {
                CopyCutDataOid = FocusedPhone.Oid;
                IsCut = true;
            }, () => FocusedPhone != null && AContactModule.SelectedContact != null && DynamicGridControl.VisibleRowCount > 0 && AMembership.HasPermission(PCOPermissions.Contact_Phones_CopyCut));
            CommandPaste = new RelayCommand(PastePhone, () => CopyCutDataOid != Guid.Empty && AContactModule.SelectedContact != null && AMembership.HasPermission(PCOPermissions.Contact_Phones_CopyCut));

            CommandCalculate = new RelayCommand(Calculate, () => AMembership.HasPermission(PCOPermissions.Contact_Phones_Calculate));

            CommandViewFull = new RelayCommand(ViewFull, () => AMembership.HasPermission(PCOPermissions.Contact_Phones_FullView));
            CommandViewSimple = new RelayCommand(ViewSimple);

            CommandPrint = new RelayCommand(PhonePrint, () => !DynamicGridControl.Columns["This"].Visible && AMembership.HasPermission(PCOPermissions.Contact_Phones_Print));
            CommandCopyPhoneNumber = new RelayCommand(CopyPhoneNumber, () => true);

            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp50 != "");
        }

        private void Calculate()
        {
            if (AContactModule.SelectedContact == null) return;
            var dbc = AContactModule.SelectedContact as DBCTContact;
            if (dbc == null) return;
            if(dbc.Phones.Count == 0)return;

            POLProgressBox.Show("محاسبه", true, 0, dbc.Phones.Count, 2,
                pw =>
                {
                    #region Phone
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        var index = 0;
                        pw.AsyncSetText(1, "مرحله اول");
                        var xpcPhones = new XPCollection<DBCTPhoneBook>(uow).Where(n=>n.Contact.Oid==dbc.Oid) .ToList();
                        foreach (var db in xpcPhones)
                        {
                            var dbq = new XPQuery<DBCLCall>(uow);
                            if (db.City != null)
                            {
                                #region Calc
                                db.CallOutCount = dbq.Count(n => n.CallType == EnumCallType.CallOut &&
                                                                                         n.PhoneNumber == db.PhoneNumber
                                                                                         && n.City.Oid == db.City.Oid);
                                db.CallInCount = dbq.Count(n => n.CallType == EnumCallType.CallIn &&
                                                                 n.PhoneNumber == db.PhoneNumber
                                                                 && n.City.Oid == db.City.Oid);
                                db.CallTotalCount = db.CallOutCount + db.CallInCount;


                                db.DurationOut = dbq.Where(n => n.CallType == EnumCallType.CallOut
                                                                 && n.DurationSeconds > 0
                                                                 && n.PhoneNumber == db.PhoneNumber
                                                                 && n.City.Oid == db.City.Oid)
                                                    .Select(n => n.DurationSeconds).Sum();
                                db.DurationIn = dbq.Where(n => n.CallType == EnumCallType.CallIn &&
                                                                 n.PhoneNumber == db.PhoneNumber
                                                                 && n.City.Oid == db.City.Oid
                                                                 && n.DurationSeconds > 0)
                                                    .Select(n => n.DurationSeconds).Sum();
                                db.DurationTotal = db.DurationOut + db.DurationIn;

                                db.LastCallOutDate = dbq.Where(n => n.CallType == EnumCallType.CallOut &&
                                                                    n.PhoneNumber == db.PhoneNumber &&
                                                                    n.City.Oid == db.City.Oid &&
                                                                    n.CallDate != null)
                                    .OrderByDescending(n => n.CallDate)
                                    .Select(n => n.CallDate).FirstOrDefault();

                                db.LastCallInDate = dbq.Where(n => n.CallType == EnumCallType.CallIn &&
                                                                    n.PhoneNumber == db.PhoneNumber
                                                                    && n.City.Oid == db.City.Oid
                                                                    && n.CallDate != null)
                                    .OrderByDescending(n => n.CallDate)
                                    .Select(n => n.CallDate).FirstOrDefault();

                                #endregion
                            }
                            else if (db.Country != null)
                            {
                                #region Calc
                                db.CallOutCount = dbq.Count(n => n.CallType == EnumCallType.CallOut &&
                                                                                         n.PhoneNumber == db.PhoneNumber
                                                                                         && n.Country.Oid == db.Country.Oid);
                                db.CallInCount = dbq.Count(n => n.CallType == EnumCallType.CallIn &&
                                                                 n.PhoneNumber == db.PhoneNumber
                                                                 && n.Country.Oid == db.Country.Oid);
                                db.CallTotalCount = db.CallOutCount + db.CallInCount;


                                db.DurationOut = dbq.Where(n => n.CallType == EnumCallType.CallOut &&
                                                                 n.PhoneNumber == db.PhoneNumber
                                                                 && n.Country.Oid == db.Country.Oid
                                                                 && n.DurationSeconds > 0)
                                                    .Select(n => n.DurationSeconds).Sum();
                                db.DurationIn = dbq.Where(n => n.CallType == EnumCallType.CallIn &&
                                                                 n.PhoneNumber == db.PhoneNumber
                                                                 && n.Country.Oid == db.Country.Oid
                                                                 && n.DurationSeconds > 0)
                                                    .Select(n => n.DurationSeconds).Sum();
                                db.DurationTotal = db.DurationOut + db.DurationIn;

                                db.LastCallOutDate = dbq.Where(n => n.CallType == EnumCallType.CallOut &&
                                                                    n.PhoneNumber == db.PhoneNumber
                                                                    && n.Country.Oid == db.Country.Oid
                                                                    && n.CallDate != null)
                                    .OrderByDescending(n => n.CallDate)
                                    .Select(n => n.CallDate).FirstOrDefault();

                                db.LastCallInDate = dbq.Where(n => n.CallType == EnumCallType.CallIn &&
                                                                    n.PhoneNumber == db.PhoneNumber
                                                                    && n.Country.Oid == db.Country.Oid
                                                                    && n.CallDate != null)
                                    .OrderByDescending(n => n.CallDate)
                                    .Select(n => n.CallDate).FirstOrDefault();
                                #endregion
                            }
                            db.Save();
                            index++;
                            pw.AsyncSetText(2, db.PhoneNumber);
                            pw.AsyncSetValue(index);
                            if (pw.NeedToCancel)
                            {
                                uow.CommitChanges();
                                return;
                            }
                            if (index % 100 == 0)
                                uow.CommitChanges();
                        }
                        pw.AsyncDisableCancel();
                        pw.AsyncSetText(2, "لطفا صبر كنید ...");
                        pw.AsyncDisableCancel();
                        uow.CommitChanges();

                    }
                    #endregion
                    
                },
                pw =>
                {
                    dbc.Phones.ToList().ForEach(p=>p.Reload());
                }, DynamicOwner);
        }

        private void CopyPhoneNumber()
        {
            if (FocusedPhone == null) return;
            System.Windows.Clipboard.SetText(FocusedPhone.PhoneNumber);
        }

        private void PhoneNew()
        {
            var w = new WPhoneAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void PhoneEdit()
        {
            var w = new WPhoneAddEdit(FocusedPhone) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("شماره(های) تماس انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTPhoneBook).ToList();

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
                            var db2 = DBCTPhoneBook.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, db2.PhoneNumber);
                            db2.Delete();
                            db2.Save();

                            DBCLCall.RemoveContactLink(db2.Session, db.Contact.Oid, db.PhoneNumber,
                                                       db.Country == null ? Guid.Empty : db.Country.Oid,
                                                       db.City == null ? Guid.Empty : db.City.Oid);
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
            PopulatePhoneList();
            if (IsViewFull)
                Task.Factory.StartNew(
                    () =>
                    {
                        System.Threading.Thread.Sleep(500);
                        HelperUtils.DoDispatcher(() => MainView.DynamicBestFitColumn());
                    });
        }

        private void ViewFull()
        {
            IsViewFull = true;
            IsViewSimple = !IsViewFull;

            RaisePropertyChanged("IsViewFull");
            RaisePropertyChanged("IsViewSimple");

            HelperSettingsClient.PhoneViewIsFull = IsViewFull;

            MainView.DynamicReorderColumns();
        }
        private void ViewSimple()
        {
            IsViewFull = false;
            IsViewSimple = !IsViewFull;

            RaisePropertyChanged("IsViewFull");
            RaisePropertyChanged("IsViewSimple");

            HelperSettingsClient.PhoneViewIsFull = IsViewFull;
        }
        private void PhonePrint()
        {
            var link = new PrintableControlLink(DynamicTableView);

            var preview = new DocumentPreview { Model = new LinkPreviewModel(link) };
            var v = (DataTemplate)DynamicView.FindResource("toolbarCustomization");
            var barManagerCustomizer = new TemplatedBarManagerController { Template = v };
            preview.BarManager.Controllers.Add(barManagerCustomizer);
            var previewWindow = new DocumentPreviewWindow
            {
                Owner = DynamicOwner,
                Content = preview,
                FlowDirection = HelperLocalize.ApplicationFlowDirection,
                FontFamily = new FontFamily(HelperLocalize.ApplicationFontName),
                FontSize = HelperLocalize.ApplicationFontSize,
                Title = "پیش نمایش",
            };
            preview.FlowDirection = FlowDirection.LeftToRight;


            link.ReportHeaderData = this;
            link.ReportHeaderTemplate = (DataTemplate)DynamicView.FindResource("reportHeaderTemplate");
            link.CreateDocument(true);
            previewWindow.ShowDialog();
        }
        private void PastePhone()
        {
            if (!IsCut) 
            {
                #region Copy
                #endregion
            }
            else 
            {
                var dba1 = DBCTPhoneBook.FindByOid(ADatabase.Dxs, CopyCutDataOid);
                if (dba1 == null)
                {
                    CopyCutDataOid = Guid.Empty;
                    return;
                }
                if (!ReferenceEquals(dba1.Contact, AContactModule.SelectedContact))
                {
                    try
                    {
                        dba1.Contact = (DBCTContact)AContactModule.SelectedContact;
                        dba1.Save();
                        CopyCutDataOid = Guid.Empty;
                    }
                    catch (Exception ex)
                    {
                        POLMessageBox.ShowError(ex.Message);
                        ALogger.Log("Exception : " + ex, Category.Exception, Priority.Medium);
                    }
                }
            }
            DataRefresh();
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp50);
        }

        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandNewMulti { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }

        public RelayCommand CommandCut { get; set; }
        public RelayCommand CommandPaste { get; set; }

        public RelayCommand CommandCalculate { get; set; }

        public RelayCommand CommandViewSimple { get; set; }
        public RelayCommand CommandViewFull { get; set; }

        public RelayCommand CommandPrint { get; set; }
        public RelayCommand CommandCopyPhoneNumber { get; set; }
        public RelayCommand CommandHelp { get; set; }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            AContactModule.OnSelectedContactChanged -= AContactModule_OnSelectedContactChanged;
            PhoneList = null;
        }
        #endregion

        #region IRefrashable
        public void DoRefresh()
        {
            PopulatePhoneList();
            RaisePropertyChanged("RootEnable");
        }
        public bool RequiresRefresh { get; set; }
        #endregion
    }
}
