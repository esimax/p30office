using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using GemBox.Spreadsheet;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POC.Module.Phone.Views;
using POL.DB.P30Office;
using POL.Lib.Common;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using POL.DB.P30Office.GL;
using POL.DB.Root;

namespace POC.Module.Phone.Models
{
    public class MPhone : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }
        private POCCore APOCCore { get; set; }

        private dynamic DynamicMainView { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private Window DynamicOwner { get; set; }

        public MPhone(object mainView)
        {
            DynamicMainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogin)
                        PopulateContactCat();
                };

            PopulateContactCat();
            InitDynamics();
            InitCommands();
            PopulateData();
        }









        #region PhoneList
        private XPServerCollectionSource _PhoneList;
        public XPServerCollectionSource PhoneList
        {
            get { return _PhoneList; }
            set
            {
                if (value == _PhoneList)
                    return;

                _PhoneList = value;
                RaisePropertyChanged("PhoneList");
            }
        }
        #endregion

        #region FocusedPhone
        private DBCTPhoneBook _FocusedPhone;
        public DBCTPhoneBook FocusedPhone
        {
            get { return _FocusedPhone; }
            set
            {
                if (ReferenceEquals(value, _FocusedPhone))
                    return;

                _FocusedPhone = value;
                RaisePropertyChanged("FocusedPhone");
            }
        }
        #endregion

        private const int ColContactCode = 0;
        private const int ColContactTitle = 1;
        private const int ColPhoneNumber = 2;
        private const int ColPhoneCityCode = 3;
        private const int ColPhoneCountryCode = 4;
        private const int ColPhoneTitle = 5;
        private const int ColPhoneNote = 6;
        private const int ColResult = 7;

        private const int ColIContactCode = 0;
        private const int ColIContactTitle = 1;
        private const int ColIPhoneNumbers = 2;
        private const int ColIPhoneTitle = 3;
        private const int ColIPhoneNote = 4;

        public List<ImportPhoneStructure> ImportList { get; set; }
        public int ImportErrorCount { get; set; }
        public int ImportPhoneAdded { get; set; }
        public int ImportContactAdded { get; set; }

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
            CommandEdit = new RelayCommand(PhoneEdit, () => FocusedPhone != null && AMembership.HasPermission(PCOPermissions.Phones_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => FocusedPhone != null && AMembership.HasPermission(PCOPermissions.Phones_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);

            CommandImportExcel = new RelayCommand(ImportExcel, () => AMembership.HasPermission(PCOPermissions.Phones_Import));
            CommandExportExcel = new RelayCommand(ExportExcel, () => AMembership.HasPermission(PCOPermissions.Phones_Export));
            CommandStartImport = new RelayCommand(StartImport, () => ImportPhoneAdded != 0);
            CommandPrint1 = new RelayCommand(Print1, () => AMembership.HasPermission(PCOPermissions.Phones_PrintDesigner));
            CommandPrintOpen = new RelayCommand(PrintOpen, () => AMembership.HasPermission(PCOPermissions.Phones_PrintDesigner));

            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);
            CommandMoveToBasket = new RelayCommand(MoveToBasket, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp49 != "");

            CommandCalculateStat = new RelayCommand(CalculateStat, () => AMembership.HasPermission(PCOPermissions.Phones_CalculateStat));

        }

        private void CalculateStat()
        {
            var count = new XPQuery<DBCTPhoneBook>(ADatabase.Dxs).Count();
            POLProgressBox.Show("محاسبه", true, 0, count, 2,
                pw =>
                {
                    #region Phone
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        var index = 0;
                        pw.AsyncSetText(1, "مرحله اول");
                        var xpcPhones = new XPCollection<DBCTPhoneBook>(uow).ToList();
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
                    DataRefresh();
                }, DynamicOwner);
        }

        private void MoveToBasket()
        {

            var srh = DynamicGridControl.GetSelectedRowHandles();
            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTPhoneBook).ToList();
            var oids = list.Select(n => n.Contact.Oid).Distinct().ToList();

            APOCMainWindow.ShowAddToBasket(DynamicOwner, null, null, oids);

        }
        private void PrintOpen()
        {
            try
            {
                var od = new OpenFileDialog { CheckFileExists = true, DefaultExt = "*.repx" };
                var dr = od.ShowDialog();
                if (dr == true)
                {
                    var report = new POC.Module.Phone.Reports.xrDefault();
                    report.LoadLayout(od.FileName);
                    report.DataSource = DynamicGridControl.ItemsSource;
                    var printTool = new DevExpress.XtraReports.UI.ReportPrintTool(report);
                    printTool.ShowPreviewDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Print1()
        {

            try
            {
                var report = new POC.Module.Phone.Reports.xrDefault();
                report.Name = "ReportAddressLabel";
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "POC.Module.Phone.Resources.ReportPhone.repx";
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                report.LoadLayout(stream);
                report.DataSource = DynamicGridControl.ItemsSource;
                var printTool = new DevExpress.XtraReports.UI.ReportPrintTool(report);
                printTool.ShowPreviewDialog();
            }
            catch (Exception ex)
            {
                using (var f = System.IO.File.CreateText(@"c:\log.exception"))
                {
                    f.Write(ex);
                }
                MessageBox.Show(ex.Message);
            }


        }

        private void StartImport()
        {
            var failed = 0;
            var count = 0;

            POLProgressBox.Show("ورود اطلاعات", true, 0, ImportPhoneAdded, 1,
                pb =>
                {
                    var dxs = ADatabase.GetNewSession();
                    var rows = (from n in ImportList where n.ErrorType == EnumImportErrorType.None orderby n.Row select n.Row).Distinct();
                    foreach (var row in rows)
                    {
                        var q = from n in ImportList where n.ErrorType == EnumImportErrorType.None && n.Row == row select n;

                        var dbcsample = q.First();
                        var dbc = dbcsample.ContactCode != 0
                                              ? DBCTContact.FindByCodeAndOrTitle(dxs, dbcsample.ContactCode, null)
                                              : new DBCTContact(dxs);
                        if (dbc.Oid == Guid.Empty)
                        {
                            dbc.Code = DBCTContact.GetNextCode(dxs);
                            dbc.Title = dbcsample.ContactTitle;
                        }
                        dbc.Save();

                        foreach (var ips in q)
                        {
                            pb.AsyncSetValue(count);
                            pb.AsyncSetText(1, ips.ContactTitle);
                            count++;

                            if (pb.NeedToCancel)
                                return;

                            try
                            {
                                var dbp = new DBCTPhoneBook(dxs)
                                              {
                                                  PhoneNumber = ips.PhoneNumber,
                                                  Note = ips.PhoneNote,
                                                  Title = ips.PhoneTitle
                                              };
                                if (dbp.PhoneNumber.StartsWith("0" + APOCCore.STCI.MobileStartingCode))
                                {
                                    dbp.PhoneType = EnumPhoneType.Mobile;
                                }
                                if (ips.CityOid != Guid.Empty)
                                    dbp.City = DBGLCity.FindByOid(dxs, ips.CityOid);
                                else
                                    dbp.Country = DBGLCountry.FindByOid(dxs, ips.CountryOid);

                                dbp.Contact = dbc;
                                dbp.Save();
                            }
                            catch 
                            {
                                failed++;
                                pb.AsyncSetText(2, string.Format("خطا : {0}", failed));
                            }
                        }
                    }
                },
                pb =>
                {
                    if (failed > 0)
                    {
                        POLMessageBox.ShowWarning(failed + " مورد خطا رخ داد.", DynamicOwner);
                    }
                    else
                    {
                        POLMessageBox.ShowInformation(count + " مورد ثبت شد.", DynamicOwner);
                    }
                },
            DynamicOwner);

        }

        private void ExportExcel()
        {
            var criteria = new GroupOperator();
            if (!ReferenceEquals(DynamicGridControl.FilterCriteria , null))
                criteria.Operands.Add(DynamicGridControl.FilterCriteria);
            if (!ReferenceEquals(PhoneList.FixedFilterCriteria ,null))
                criteria.Operands.Add(PhoneList.FixedFilterCriteria);
            XPGUIDObject.FixEmptyGroupOperand(criteria);
            
            var xpq = new XPQuery<DBCTPhoneBook>(ADatabase.Dxs).OrderBy(n => n.Contact.Code);
            var xpq2 = xpq.AppendWhere(new CriteriaToExpressionConverter(), criteria) as XPQuery<DBCTPhoneBook>;
            if (xpq2 == null) return;
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
                FileName = "ExportPhone.xls"
            };
            if (sf.ShowDialog() != true)
                return;

            var filename = sf.FileName;
            var eFile = new ExcelFile
                            {
                                DefaultFontName = HelperLocalize.ApplicationFontName,
                                DefaultFontSize = (int)HelperLocalize.ApplicationFontSize
                            };

            var ws = eFile.Worksheets.Add("شماره های تلفن");
            ws.ViewOptions.ShowColumnsFromRightToLeft = HelperLocalize.ApplicationFlowDirection == FlowDirection.RightToLeft;

            var row = 0;
            #region Columns Title
            ws.Cells[row, ColContactCode].Value = "كد پرونده";
            ws.Cells[row, ColContactCode].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColContactTitle].Value = "عنوان پرونده";
            ws.Cells[row, ColContactTitle].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColPhoneNumber].Value = "شماره";
            ws.Cells[row, ColPhoneNumber].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColPhoneCityCode].Value = "كد شهر";
            ws.Cells[row, ColPhoneCityCode].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColPhoneCountryCode].Value = "كد كشور";
            ws.Cells[row, ColPhoneCountryCode].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColPhoneTitle].Value = "عنوان شماره";
            ws.Cells[row, ColPhoneTitle].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColPhoneNote].Value = "نكته";
            ws.Cells[row, ColPhoneNote].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColResult].Value = "نتیجه";
            ws.Cells[row, ColResult].Style.Font.Name = eFile.DefaultFontName;
            #endregion

            row++;

            POLProgressBox.Show("استخراج اطلاعات شماره ها", true, 0, count, 2,
                pw =>
                {
                    foreach (var db in xpq2)
                    {
                        try
                        {
                            ws.Cells[row, ColContactCode].Value = db.Contact.Code;
                            ws.Cells[row, ColContactCode].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColContactTitle].Value = db.Contact.Title;
                            ws.Cells[row, ColContactTitle].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColPhoneNumber].Value = db.PhoneNumber;
                            ws.Cells[row, ColPhoneNumber].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColPhoneCityCode].Value = db.CityCode;
                            ws.Cells[row, ColPhoneCityCode].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColPhoneCountryCode].Value = db.CountryCode;
                            ws.Cells[row, ColPhoneCountryCode].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColPhoneTitle].Value = db.Title;
                            ws.Cells[row, ColPhoneTitle].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColPhoneNote].Value = db.Note;
                            ws.Cells[row, ColPhoneNote].Style.Font.Name = eFile.DefaultFontName;
                        }
                        catch (Exception ex)
                        {
                            ws.Cells[row, ColResult].Value = ex.Message;
                            ws.Cells[row, ColResult].Style.Font.Name = eFile.DefaultFontName;
                        }
                        pw.AsyncSetText(1, db.Contact.Title);
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

        private void ImportExcel()
        {
            POLMessageBox.ShowInformation("لطفا فایل اكسل را به نحوی انتخاب كنید كه دارای شرایط زیر باشد:" + Environment.NewLine + Environment.NewLine +
            "ستون اول : كد پرونده (اگر خالی باشد پرونده جدید ساخته می شود)" + Environment.NewLine +
            "ستون دوم : عنوان پرونده (می تواند خالی باشد اگر كد پرونده خالی نباشد)" + Environment.NewLine +
            "ستون سوم : شماره های تماس (برای ثبت چند شماره آنها را با ';' جدا كنید)" + Environment.NewLine +
            "ستون چهارم : عنوان شماره (برای ثبت چند عنوان آنها را با ';' جدا كنید)" + Environment.NewLine +
            "ستون پنجم : توضیحات (برای ثبت چند توضیح آنها را با ';' جدا كنید)" + Environment.NewLine + Environment.NewLine +
            "توجه : ردیف اول فایل اكسل پردازش نمی شود.", DynamicOwner);

            var of = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "xls",
                Filter = "Microsoft Excel 2003 files (*.xls)|*.xls",
                FilterIndex = 0,
                RestoreDirectory = true,
            };

            if (of.ShowDialog() != true)
                return;

            var filename = of.FileName;
            var eFile = new ExcelFile();
            try
            {
                eFile = new ExcelFile();
                eFile.LoadXls(filename);
            }
            catch
            {
                POLMessageBox.ShowError("خطا در بازخوانی فایل اكسل.");
                return;
            }

            ImportList = new List<ImportPhoneStructure>();

            POLProgressBox.Show("بررسی اطلاعات", true, 0, 0, 2,
                pb =>
                {
                    var dxs = ADatabase.GetNewSession();
                    var poc = ServiceLocator.Current.GetInstance<POCCore>();
                    var curCity = poc.STCI.CurrentCityGuid;
                    var dbcity = DBGLCity.FindByOid(dxs, curCity);
                    if (dbcity == null || dbcity.Country == null)
                    {
                        POLMessageBox.ShowError("كد شهر جاری پیدا نشد.", pb);
                        return;
                    }

                    var pdec = new PhoneDecoder3Code(
                        ServiceLocator.Current.GetInstance<ILoggerFacade>(),
                        dxs,
                        dbcity.Country.Oid,
                        dbcity.Country.TeleCode,
                        poc.STCI.CurrentCityGuid,
                        dbcity.PhoneCode,
                        poc.STCI.MobileLength,
                        Convert.ToInt32(poc.STCI.MobileStartingCode)
                        );

                    foreach (ExcelWorksheet sheet in eFile.Worksheets)
                    {
                        var irow = 0;
                        foreach (ExcelRow row in sheet.Rows)
                        {
                            irow++;
                            if (irow == 1) continue;

                            if (pb.NeedToCancel)
                                break;

                            var ips = new ImportPhoneStructure(irow);
                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIContactCode];
                                    ips.ContactCode = Convert.ToInt32(ec.Value);
                                });
                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIContactTitle];
                                    ips.ContactTitle = Convert.ToString(ec.Value);
                                    ips.ContactTitle = ips.ContactTitle.Trim();
                                    if (ips.ContactTitle.Length > 128)
                                        ips.ContactTitle = ips.ContactTitle.Substring(0, 128);
                                });
                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIPhoneTitle];
                                    ips.PhoneTitle = Convert.ToString(ec.Value);
                                    ips.PhoneTitle = ips.PhoneTitle.Trim();
                                    if (ips.PhoneTitle.Length > 32)
                                        ips.PhoneTitle = ips.PhoneTitle.Substring(0, 32);
                                });
                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIPhoneNote];
                                    ips.PhoneNote = Convert.ToString(ec.Value);
                                    ips.PhoneNote = ips.PhoneNote.Trim();
                                    if (ips.PhoneNote.Length > 256)
                                        ips.PhoneNote = ips.PhoneNote.Substring(0, 256);
                                });
                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIPhoneNumbers];
                                    ips.PhoneNumber = Convert.ToString(ec.Value);
                                });

                            if (ips.ContactCode == 0 && ips.ContactTitle == string.Empty)
                            {
                                ips.ErrorType = EnumImportErrorType.ErrorInContactTitle;
                                ips.Column = ColIContactTitle;
                                ImportList.Add(ips);
                                continue;
                            }
                            if (ips.ContactCode != 0)
                            {
                                var dbc = DBCTContact.FindByCodeAndOrTitle(dxs, ips.ContactCode, null);
                                if (dbc == null)
                                {
                                    ips.ErrorType = EnumImportErrorType.ErrorInContactCode;
                                    ips.Column = ColIContactCode;
                                    ImportList.Add(ips);
                                    continue;
                                }
                            }
                            if (string.IsNullOrWhiteSpace(ips.PhoneNumber))
                            {
                                ips.ErrorType = EnumImportErrorType.ErrorInvalidNumber;
                                ips.Column = ColIPhoneNumbers;
                                ImportList.Add(ips);
                                continue;
                            }


                            var phones = ips.PhoneNumber.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            var phonetitles = ips.PhoneTitle.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            var phonenotes = ips.PhoneNote.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            var phone_index = 0;
                            foreach (var phone1 in phones)
                            {
                                var phone = phone1.Trim().Replace("\n", "").Replace("\r", "");
                                if (phone.Length > 16)
                                    phone = phone.Substring(0, 16);

                                var ips2 = new ImportPhoneStructure(irow)
                                               {
                                                   ContactCode = ips.ContactCode,
                                                   ContactTitle = ips.ContactTitle,
                                                   Row = irow,
                                               };


                                var v = pdec.DecodeData(phone, EnumCallType.CallOut);
                                if (v.HasError)
                                {
                                    ips2.ErrorType = EnumImportErrorType.ErrorInvalidNumber;
                                    ips2.Column = ColIPhoneNumbers;
                                    ImportList.Add(ips2);
                                    continue;
                                }
                                ips2.CountryCode = v.CountryCode;
                                ips2.CountryOid = v.CountryOid;
                                ips2.CityCode = v.CityCode;
                                ips2.CityOid = v.CityOid;

                                ips2.PhoneNumber = v.Phone;

                                HelperUtils.Try(
                                () =>
                                {
                                    ips2.PhoneTitle = phonetitles[phone_index].Trim();
                                });
                                HelperUtils.Try(
                                () =>
                                {
                                    ips2.PhoneNote = phonenotes[phone_index].Trim();
                                });


                                if (!ips2.PhoneNumber.IsDigital())
                                {
                                    ips2.ErrorType = EnumImportErrorType.ErrorInvalidNumber;
                                    ips2.Column = ColIPhoneNumbers;
                                    ImportList.Add(ips2);
                                    continue;
                                }

                                if (ips2.CityOid != Guid.Empty)
                                {
                                    var dbp = DBCTPhoneBook.FindByPhoneAndCity(dxs, ips2.PhoneNumber, ips2.CityOid);
                                    if (dbp != null)
                                    {
                                        ips2.ErrorType = EnumImportErrorType.ErrorDuplicateNumber;
                                        ips2.Column = ColIPhoneNumbers;
                                        ImportList.Add(ips2);
                                        continue;
                                    }
                                }
                                else
                                {
                                    var dbp = DBCTPhoneBook.FindByPhoneAndCountry(dxs, ips2.PhoneNumber, ips2.CountryOid);
                                    if (dbp != null)
                                    {
                                        ips2.ErrorType = EnumImportErrorType.ErrorDuplicateNumber;
                                        ips2.Column = ColIPhoneNumbers;
                                        ImportList.Add(ips2);
                                        continue;
                                    }
                                }

                                var q = from n in ImportList
                                        where
                                            n.PhoneNumber == ips2.PhoneNumber &&
                                            n.CountryCode == ips2.CountryCode &&
                                            n.CityCode == ips2.CityCode
                                        select n;
                                if (q.Any())
                                {
                                    ips2.ErrorType = EnumImportErrorType.ErrorDuplicateNumberInExcel;
                                    ips2.Column = ColIPhoneNumbers;
                                    ImportList.Add(ips2);
                                    continue;
                                }

                                ips2.ErrorType = EnumImportErrorType.None;
                                ips2.Column = -1;
                                ImportList.Add(ips2);
                                phone_index++;
                            }
                        }

                        break;
                    }
                },
                pb =>
                {
                    pb.AsyncClose();

                    ImportErrorCount = (from n in ImportList
                                        where n.ErrorType != EnumImportErrorType.None
                                        select n).Count();
                    ImportPhoneAdded = (from n in ImportList
                                        where n.ErrorType == EnumImportErrorType.None
                                        select n).Count();
                    ImportContactAdded = (from n in ImportList
                                          where n.ErrorType == EnumImportErrorType.None && n.ContactCode == 0
                                          select n.Row).Distinct().Count();

                    var w = new WPhoneImportPreview
                                {
                                    Owner = DynamicOwner,
                                    DataContext = this,
                                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                                };
                    w.ShowDialog();
                },
                DynamicOwner);
        }
        private void TryToDisplayGeneratedFile(string fileName)
        {
            try
            {
                var p = System.Diagnostics.Process.Start(fileName);
                if (p != null)
                    SetForegroundWindow(p.MainWindowHandle);
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message);
            }
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
            PopulateData();
            Task.Factory.StartNew(
                () =>
                {
                    System.Threading.Thread.Sleep(500);
                    HelperUtils.DoDispatcher(() => DynamicTableView.BestFitColumns());
                });
        }

        private GroupOperator MainSearchCriteria { get; set; }
        private void PopulateData()
        {
            if (ReferenceEquals(MainSearchCriteria, null))
                MainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            MainSearchCriteria.Operands.Clear();

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBCTPhoneBook))
            {
                DisplayableProperties = "Oid;PhoneNumber;PhoneType;Country;City;Title;Note"
            };
            xpi.DisplayableProperties += ";Contact.Oid;Contact.Code;Contact.Title";
            xpi.DefaultSorting = "PhoneNumber DESC";

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


            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.GetNewSession();
            };
            PhoneList = null;
            PhoneList = xpi;
        }

        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp49);
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

        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }

        public RelayCommand CommandImportExcel { get; set; }
        public RelayCommand CommandExportExcel { get; set; }

        public RelayCommand CommandStartImport { get; set; }
        public RelayCommand CommandPrint1 { get; set; }
        public RelayCommand CommandPrintOpen { get; set; }

        public RelayCommand CommandCategoryRefresh { get; set; }
        public RelayCommand CommandMoveToBasket { get; set; }
        public RelayCommand CommandHelp { get; set; }

        public RelayCommand CommandCalculateStat { get; set; }
        #endregion

        #region [DllImport]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion

    }
}
