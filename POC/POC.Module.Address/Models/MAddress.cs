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
using POC.Module.Address.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using POL.DB.P30Office.GL;
using POL.DB.Root;

namespace POC.Module.Address.Models
{
    public class MAddress : NotifyObjectBase
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

        public MAddress(object mainView)
        {
            DynamicMainView = mainView;

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            PopulateContactCat();
            InitDynamics();
            InitCommands();
            PopulateData();
        }









        #region AddressList
        private XPServerCollectionSource _AddressList;
        public XPServerCollectionSource AddressList
        {
            get { return _AddressList; }
            set
            {
                if (value == _AddressList)
                    return;

                _AddressList = value;
                RaisePropertyChanged("AddressList");
            }
        }
        #endregion

        #region FocusedAddress
        private DBCTAddress _FocusedAddress;
        public DBCTAddress FocusedAddress
        {
            get { return _FocusedAddress; }
            set
            {
                if (ReferenceEquals(value, _FocusedAddress))
                    return;

                _FocusedAddress = value;
                RaisePropertyChanged("FocusedAddress");
            }
        }
        #endregion

        private const int ColContactCode = 0;
        private const int ColContactTitle = 1;
        private const int ColAddressTitle = 2;
        private const int ColAddressCity = 3;
        private const int ColAddressArea = 4;
        private const int ColAddressAddress = 5;
        private const int ColAddressPOBox = 6;
        private const int ColAddressZipCode = 7;
        private const int ColAddressNote = 8;
        private const int ColAddressLat = 9;
        private const int ColAddressLong = 10;
        private const int ColResult = 11;

        private const int ColIContactCode = 0;
        private const int ColIContactTitle = 1;
        private const int ColIAddressTitle = 2;
        private const int ColIAddressCity = 3;
        private const int ColIAddressArea = 4;
        private const int ColIAddressAddress = 5;
        private const int ColIAddressPOBox = 6;
        private const int ColIAddressZipCode = 7;
        private const int ColIAddressNote = 8;

        public List<ImportAddressStructure> ImportList { get; set; }
        public int ImportErrorCount { get; set; }
        public int ImportAddressAdded { get; set; }
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
            CommandEdit = new RelayCommand(AddressEdit, () => FocusedAddress != null && AMembership.HasPermission(PCOPermissions.Addresses_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => FocusedAddress != null && AMembership.HasPermission(PCOPermissions.Addresses_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);

            CommandImportExcel = new RelayCommand(ImportExcel, () => AMembership.HasPermission(PCOPermissions.Addresses_Import));
            CommandExportExcel = new RelayCommand(ExportExcel, () => AMembership.HasPermission(PCOPermissions.Addresses_Export));
            CommandStartImport = new RelayCommand(StartImport, () => ImportAddressAdded != 0);
            CommandPrint1 = new RelayCommand(Print1,()=>AMembership.HasPermission(PCOPermissions.Addresses_PrintDesigner));
            CommandPrintOpen = new RelayCommand(PrintOpen, () => AMembership.HasPermission(PCOPermissions.Addresses_PrintDesigner));
            CommandMoveToBasket = new RelayCommand(MoveToBasket, () => true

            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp05 != "");
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp05);
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

        private void MoveToBasket()
        {
            
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTAddress).ToList();
            var oids = list.Select(n => n.Contact.Oid).Distinct().ToList();

            APOCMainWindow.ShowAddToBasket(DynamicOwner, null, null, oids);

        }

        private void PrintOpen()
        {
            try
            {
                var od = new OpenFileDialog {CheckFileExists = true, DefaultExt = "*.repx"};
                var dr = od.ShowDialog();
                if (dr == true)
                {
                    var report = new Reports.XtraReport1();
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
                var report = new Reports.XtraReport1 {Name = "ReportAddressLabel"};
                var assembly = Assembly.GetExecutingAssembly();
                const string resourceName = "POC.Module.Address.Resources.ReportAddressLetterBox.repx";
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                report.LoadLayout(stream);
                report.DataSource = DynamicGridControl.ItemsSource;
                var printTool = new DevExpress.XtraReports.UI.ReportPrintTool(report);
                printTool.ShowPreviewDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void StartImport()
        {
            var failed = 0;
            var count = 0;

            POLProgressBox.Show("ورود اطلاعات", true, 0, ImportAddressAdded, 1,
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
                            if (ips.ErrorType != EnumImportErrorType.None)
                                continue;
                            pb.AsyncSetValue(count);
                            pb.AsyncSetText(1, ips.ContactTitle);
                            count++;

                            if (pb.NeedToCancel)
                                return;

                            try
                            {
                                var dbp = new DBCTAddress(dxs)
                                              {
                                                  Title = ips.Title,
                                                  City = ips.CityOid==Guid.Empty?null:DBGLCity.FindByOid(dxs,ips.CityOid),
                                                  Area = ips.Area,
                                                  Address = ips.Address,
                                                  POBox = ips.POBox,
                                                  ZipCode = ips.ZipCode,
                                                  Note = ips.Note,
                                                  Contact = dbc,
                                              };
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
            if (!ReferenceEquals(DynamicGridControl.FilterCriteria, null))
                criteria.Operands.Add(DynamicGridControl.FilterCriteria);
            if (!ReferenceEquals(AddressList.FixedFilterCriteria, null))
                criteria.Operands.Add(AddressList.FixedFilterCriteria);
            XPGUIDObject.FixEmptyGroupOperand(criteria);

            var xpq = new XPQuery<DBCTAddress>(ADatabase.Dxs).OrderBy(n => n.Contact.Code);
            var xpq2 = xpq.AppendWhere(new CriteriaToExpressionConverter(), criteria) as XPQuery<DBCTAddress>;
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
                FileName = "ExportAddress.xls"
            };
            if (sf.ShowDialog() != true)
                return;

            var filename = sf.FileName;
            var eFile = new ExcelFile
                            {
                                DefaultFontName = HelperLocalize.ApplicationFontName,
                                DefaultFontSize = (int)HelperLocalize.ApplicationFontSize
                            };

            var ws = eFile.Worksheets.Add("آدرس");
            ws.ViewOptions.ShowColumnsFromRightToLeft = HelperLocalize.ApplicationFlowDirection == FlowDirection.RightToLeft;

            var row = 0;
            #region Columns Title
            ws.Cells[row, ColContactCode].Value = "كد پرونده";
            ws.Cells[row, ColContactCode].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColContactTitle].Value = "عنوان پرونده";
            ws.Cells[row, ColContactTitle].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressTitle].Value = "عنوان آدرس";
            ws.Cells[row, ColAddressTitle].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressCity].Value = "شهر";
            ws.Cells[row, ColAddressCity].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressArea].Value = "محله";
            ws.Cells[row, ColAddressArea].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressAddress].Value = "آدرس";
            ws.Cells[row, ColAddressAddress].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressPOBox].Value = "صندق پستی";
            ws.Cells[row, ColAddressPOBox].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressZipCode].Value = "كد پستی";
            ws.Cells[row, ColAddressZipCode].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressNote].Value = "نكته";
            ws.Cells[row, ColAddressNote].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressLat].Value = "عرض جغرافیایی";
            ws.Cells[row, ColAddressLat].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColAddressLong].Value = "طول جغرافیایی";
            ws.Cells[row, ColAddressLong].Style.Font.Name = eFile.DefaultFontName;

            #endregion

            row++;

            POLProgressBox.Show("استخراج اطلاعات آدرس ها", true, 0, count, 2,
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

                            ws.Cells[row, ColAddressTitle].Value = db.Title;
                            ws.Cells[row, ColAddressTitle].Style.Font.Name = eFile.DefaultFontName;

                            if (db.City != null)
                            {
                                ws.Cells[row, ColAddressCity].Value = db.City.TitleXX;
                                ws.Cells[row, ColAddressCity].Style.Font.Name = eFile.DefaultFontName;
                            }
                            else
                            {
                                ws.Cells[row, ColAddressCity].Value = "";
                                ws.Cells[row, ColAddressCity].Style.Font.Name = eFile.DefaultFontName;
                            }

                            ws.Cells[row, ColAddressArea].Value = db.Area;
                            ws.Cells[row, ColAddressArea].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColAddressAddress].Value = db.Address;
                            ws.Cells[row, ColAddressAddress].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColAddressPOBox].Value = db.POBox;
                            ws.Cells[row, ColAddressPOBox].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColAddressZipCode].Value = db.ZipCode;
                            ws.Cells[row, ColAddressZipCode].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColAddressNote].Value = db.Note;
                            ws.Cells[row, ColAddressNote].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColAddressLat].Value = db.Latitude;
                            ws.Cells[row, ColAddressLat].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColAddressLong].Value = db.Longitude;
                            ws.Cells[row, ColAddressLong].Style.Font.Name = eFile.DefaultFontName;
                        }
                        catch (Exception ex)
                        {
                            ws.Cells[row, ColResult].Value = ex.Message;
                            ws.Cells[row, ColResult].Style.Font.Name = eFile.DefaultFontName;
                        }
                        pw.AsyncSetText(1, db.Contact.Title);
                        pw.AsyncSetValue(row);
                        row++;
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
            "ستون سوم : عنوان آدرس" + Environment.NewLine +
            "ستون چهارم : نام شهر" + Environment.NewLine +
            "ستون پنجم : محله" + Environment.NewLine + 
            "ستون ششم : آدرس" + Environment.NewLine + 
            "ستون هفتم : صندوق پستی" + Environment.NewLine + 
            "ستون هشتم : كد پستی" + Environment.NewLine + 
            "ستون نهم : نكته" + Environment.NewLine + Environment.NewLine +
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

            ImportList = new List<ImportAddressStructure>();

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

                    foreach (ExcelWorksheet sheet in eFile.Worksheets)
                    {
                        var irow = 0;
                        foreach (ExcelRow row in sheet.Rows)
                        {
                            irow++;
                            if (irow == 1) continue;

                            if (pb.NeedToCancel)
                                break;

                            var ips = new ImportAddressStructure(irow);
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
                                    var ec = row.Cells[ColIAddressTitle];
                                    ips.Title = Convert.ToString(ec.Value);
                                    ips.Title = ips.Title.Trim();
                                    if (ips.Title.Length > 32)
                                        ips.Title = ips.Title.Substring(0, 32);
                                });

                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIAddressCity];
                                    ips.City = Convert.ToString(ec.Value);
                                    var dbc = DBGLCity.FindDuplicateTitleXXExcept(dxs,null,ips.City,false);
                                    ips.CityOid = dbc != null ? dbc.Oid : Guid.Empty;
                                });
                            HelperUtils.Try(
                               () =>
                               {
                                   var ec = row.Cells[ColIAddressArea];
                                   ips.Area = Convert.ToString(ec.Value);
                                   ips.Area = ips.Area.Trim();
                                   if (ips.Area.Length > 32)
                                       ips.Area = ips.Area.Substring(0, 32);
                               });
                            HelperUtils.Try(
                               () =>
                               {
                                   var ec = row.Cells[ColIAddressAddress];
                                   ips.Address = Convert.ToString(ec.Value);
                                   ips.Address = ips.Address.Trim();
                                   if (ips.Address.Length > 1024)
                                       ips.Address = ips.Address.Substring(0, 1024);
                               });
                            HelperUtils.Try(
                               () =>
                               {
                                   var ec = row.Cells[ColIAddressPOBox];
                                   ips.POBox = Convert.ToString(ec.Value);
                                   ips.POBox = ips.POBox.Trim();
                                   if (ips.POBox.Length > 32)
                                       ips.POBox = ips.POBox.Substring(0, 32);
                               });
                            HelperUtils.Try(
                               () =>
                               {
                                   var ec = row.Cells[ColIAddressZipCode];
                                   ips.ZipCode = Convert.ToString(ec.Value);
                                   ips.ZipCode = ips.ZipCode.Trim();
                                   if (ips.ZipCode.Length > 32)
                                       ips.ZipCode = ips.ZipCode.Substring(0, 32);
                               });

                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIAddressNote];
                                    ips.Note = Convert.ToString(ec.Value);
                                    ips.Note = ips.Note.Trim();
                                    if (ips.Note.Length > 1024)
                                        ips.Note = ips.Note.Substring(0, 1024);
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
                            if (string.IsNullOrWhiteSpace(ips.Address))
                            {
                                ips.ErrorType = EnumImportErrorType.ErrorInvalidAddress;
                                ips.Column = ColIAddressAddress;
                                ImportList.Add(ips);
                                continue;
                            }

                            ImportList.Add(ips);
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
                    ImportAddressAdded = (from n in ImportList
                                          where n.ErrorType == EnumImportErrorType.None
                                          select n).Count();
                    ImportContactAdded = (from n in ImportList
                                          where n.ErrorType == EnumImportErrorType.None && n.ContactCode == 0
                                          select n.Row).Distinct().Count();

                    var w = new WAddressImportPreview
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

        private void AddressEdit()
        {
            var w = new WAddressAddEdit(FocusedAddress) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("آدرس(های) انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTAddress).ToList();

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
                            var db2 = DBCTAddress.FindByOid(dxs, db.Oid);
                            w.AsyncSetText(1, db2.Address);
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

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBCTAddress))
            {
                DisplayableProperties = "Oid;Title;City.TitleXX;Area;Address;POBox;ZipCode;Note;Latitude;Longitude;ZoomLevel"
            };
            xpi.DisplayableProperties += ";Contact.Oid;Contact.Code;Contact.Title";
            xpi.DefaultSorting = "Address";

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
            AddressList = null;
            AddressList = xpi;
        }



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

        public RelayCommand CommandMoveToBasket { get; set; }
        public RelayCommand CommandCategoryRefresh { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);



    }
}
