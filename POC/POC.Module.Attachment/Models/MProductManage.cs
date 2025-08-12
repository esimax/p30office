using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using DevExpress.Data;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using GemBox.Spreadsheet;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POC.Module.Attachment.Views;
using POL.DB.P30Office.AC;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.XOffice;

namespace POC.Module.Attachment.Models
{
    public class MProductManage : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private bool DynamicIsSelectionMode { get; set; }

        #region CTOR
        public MProductManage(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            InitCommands();
            GetDynamicData();
            PopulateDataList();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "فهرست كالا / محصولات"; }
        }
        #endregion

        #region SearchText
        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                if (_SearchText == value)
                    return;
                _SearchText = value;
                RaisePropertyChanged("SearchText");
                PopulateDataList();
            }
        }
        #endregion

        #region DataList
        private XPCollection<DBACProduct> _DataList;
        public XPCollection<DBACProduct> DataList
        {
            get { return _DataList; }
            set
            {
                if (_DataList == value)
                    return;
                _DataList = value;
                RaisePropertyChanged("DataList");
            }
        }
        #endregion
        #region SelectedData
        private DBACProduct _SelectedData;
        public DBACProduct SelectedData
        {
            get { return _SelectedData; }
            set
            {
                if (ReferenceEquals(_SelectedData, value))
                    return;
                _SelectedData = value;
                RaisePropertyChanged("SelectedData");
            }
        }
        #endregion





        #region [METHODS]

        private void InitCommands()
        {
            CommandNew = new RelayCommand(DataNew, () => AMembership.HasPermission(PCOPermissions.Product_Add));
            CommandEdit = new RelayCommand(DataEdit, () => SelectedData != null && DynamicGridControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.Product_Edit));
            CommandDelete = new RelayCommand(DataDelete, () => SelectedData != null && AMembership.HasPermission(PCOPermissions.Product_Delete));
            CommandRefresh = new RelayCommand(DataRefresh, () => true);
            CommandOK = new RelayCommand(OK, () => DynamicIsSelectionMode);

            CommandExport = new RelayCommand(Export, () => AMembership.HasPermission(PCOPermissions.Product_Export));
            CommandImport = new RelayCommand(Import, () => AMembership.HasPermission(PCOPermissions.Product_Import));
            CommandStartImport = new RelayCommand(StartImport, () => ImportList.Count != 0);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp13 != "");
        }

        private void DataNew()
        {
            var w = new WProductAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataEdit()
        {
            var w = new WProductAddEdit(SelectedData) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
                DataRefresh();
        }
        private void DataDelete()
        {
            var srh = DynamicGridControl.GetSelectedRowHandles();
            var dr = POLMessageBox.ShowQuestionYesNo("اطلاعات انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBACProduct).ToList();

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
                            var db2 = DBACProduct.FindByOid(dxs, db.Oid);
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
            PopulateDataList();
        }
        private void OK()
        {
            if (DynamicOwner == null) return;
            DynamicOwner.DialogResult = true;
            DynamicOwner.Close();
        }


        private void Export()
        {
            var xpq = new XPQuery<DBACProduct>(ADatabase.Dxs);
            var count = xpq.Count();
            if (count == 0) return;
            if (count >= 64 * 1024)
            {
                POLMessageBox.ShowError("حداكثر ركورد های قابل استخراج 65 هزارتا می باشد.", Window.GetWindow(DynamicGridControl));
                return;
            }

            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xls",
                Filter = "Microsoft Excel 2003 files (*.xls)|*.xls",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "ExportProduct.xls"
            };
            if (sf.ShowDialog() != true)
                return;

            var filename = sf.FileName;
            var eFile = new ExcelFile
            {
                DefaultFontName = HelperLocalize.ApplicationFontName,
                DefaultFontSize = (int)HelperLocalize.ApplicationFontSize
            };

            var ws = eFile.Worksheets.Add("تماس ها");
            ws.ViewOptions.ShowColumnsFromRightToLeft = HelperLocalize.ApplicationFlowDirection == FlowDirection.RightToLeft;

            var row = 0;
            #region Columns Title
            var colIndex = 0;
            foreach (var col in DynamicGridControl.Columns.OrderBy(n => n.VisibleIndex))
            {
                if (col.Visible)
                {
                    ws.Cells[row, colIndex].Value = col.HeaderCaption;
                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                    colIndex++;
                }
            }
            #endregion

            var colfields = (from n in DynamicGridControl.Columns where n.Visible orderby n.VisibleIndex select n.FieldName).ToList();
            var colOrders = (from n in DynamicGridControl.Columns where n.Visible && n.SortOrder != ColumnSortOrder.None orderby n.SortIndex select new { n.FieldName, n.SortOrder }).ToList();
            row++;

            POLProgressBox.Show("استخراج اطلاعات تماس ها", true, 0, count, 2,
                pw =>
                {
                    var q = from n in xpq select n;
                    var oCount = 0;
                    foreach (var colOrder in colOrders)
                    {
                        #region Order First Time

                        if (colOrder.FieldName == "Code")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Code)
                                : q.OrderByDescending(n => n.Code);
                            oCount++;
                        }
                        if (colOrder.FieldName == "Title")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Title)
                                : q.OrderByDescending(n => n.Title);
                            oCount++;
                        }
                        if (colOrder.FieldName == "Price")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Price)
                                : q.OrderByDescending(n => n.Price);
                            oCount++;
                        }
                        if (colOrder.FieldName == "Contact.Cats")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.Unit.Title)
                                : q.OrderByDescending(n => n.Unit.Title);
                            oCount++;
                        }
                        #endregion
                    }


                    foreach (var db in q)
                    {

                        colIndex = 0;
                        foreach (var field in colfields)
                        {
                            try
                            {
                                #region Code
                                if (field == "Code")
                                {
                                    ws.Cells[row, colIndex].Value = db.Code;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Title
                                if (field == "Title")
                                {
                                    ws.Cells[row, colIndex].Value = db.Title;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Price
                                if (field == "Price")
                                {
                                    ws.Cells[row, colIndex].Value = db.Price;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region Unit.Title
                                if (field == "Unit.Title")
                                {
                                    ws.Cells[row, colIndex].Value = db.Unit != null ? db.Unit.Title : "";
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                ws.Cells[row, colIndex].Value = ex.Message;
                                ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                colIndex++;
                            }
                        }

                        pw.AsyncSetText(1, db.Title ?? string.Empty);
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
                }, Window.GetWindow(DynamicGridControl));
        }
        

        public List<ImportProductStructure> ImportList { get; set; }
        private void Import()
        {
            POLMessageBox.ShowInformation("لطفا فایل اكسل را به نحوی انتخاب كنید كه دارای شرایط زیر باشد:" + Environment.NewLine + Environment.NewLine +
            "ستون اول : کد محصول" + Environment.NewLine +
            "ستون دوم : عنوان محصول" + Environment.NewLine +
            "ستون سوم : قیمت" + Environment.NewLine +
            "ستون چهارم : واحد" + Environment.NewLine +
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

            ImportList = new List<ImportProductStructure>();

            POLProgressBox.Show("بررسی اطلاعات", true, 0, 0, 2,
                pb =>
                {
                    var dxs = ADatabase.GetNewSession();
                    var poc = ServiceLocator.Current.GetInstance<POCCore>();


                    foreach (ExcelWorksheet sheet in eFile.Worksheets)
                    {
                        var irow = 0;
                        foreach (ExcelRow row in sheet.Rows)
                        {
                            irow++;
                            if (irow == 1) continue;

                            if (pb.NeedToCancel)
                                break;

                            var ips = new ImportProductStructure(irow);
                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColICode];
                                    ips.Code = Convert.ToInt32(ec.Value);
                                });
                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColITitle];
                                    ips.Title = Convert.ToString(ec.Value);
                                    ips.Title = ips.Title.Trim();
                                    if (ips.Title.Length > 64)
                                        ips.Title = ips.Title.Substring(0, 64);
                                });
                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIUnit];
                                    ips.Unit = Convert.ToString(ec.Value);
                                    ips.Unit = ips.Unit.Trim();
                                    if (ips.Unit.Length > 16)
                                        ips.Unit = ips.Unit.Substring(0, 16);
                                });

                            HelperUtils.Try(
                                () =>
                                {
                                    var ec = row.Cells[ColIPrice];
                                    ips.Price = Convert.ToDecimal(ec.Value);
                                });
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


                    var w = new WProductImportPreview
                    {
                        Owner = DynamicOwner,
                        DataContext = this,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    };
                    w.ShowDialog();
                },
                DynamicOwner);
        }
        private void StartImport()
        {
            var failed = 0;
            var count = 0;

            POLProgressBox.Show("ورود اطلاعات", true, 0, ImportList.Count, 1,
                pb =>
                {
                    var dxs = ADatabase.GetNewSession();
                    var rows = (from n in ImportList where n.ErrorType == EnumImportErrorType.None orderby n.Row select n.Row).Distinct();
                    foreach (var row in rows)
                    {
                        var q = from n in ImportList where n.ErrorType == EnumImportErrorType.None && n.Row == row select n;



                        foreach (var ips in q)
                        {
                            pb.AsyncSetValue(count);
                            pb.AsyncSetText(1, ips.Title);
                            count++;

                            if (pb.NeedToCancel)
                                return;

                            try
                            {
                                var dbc = ips.Code != 0
                                              ? DBACProduct.FindByCode(dxs, ips.Code)
                                              : new DBACProduct(dxs);
                                if (dbc.Oid == Guid.Empty)
                                {
                                    dbc.Code = DBACProduct.GetNextCode(dxs);
                                    dbc.Title = ips.Title;
                                    dbc.Unit = DBBTUnit.FindDuplicateTitleExcept(dxs, null, ips.Unit);
                                    dbc.Price = ips.Price;
                                }
                                dbc.Save();
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

        private void PopulateDataList()
        {
            var ff = SearchText;
            if (string.IsNullOrWhiteSpace(ff))
                ff = string.Empty;
            ff = ff.Replace("*", "").Replace("%", "").Trim();
            HelperConvert.CorrectPersianBug(ref ff);
            XPCollection<DBACProduct> xpc = null;

            xpc = DBACProduct.GetAll(ADatabase.Dxs, ff);

            xpc.LoadAsync();
            DataList = xpc;
        }
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGridControl = MainView.DynamicGridControl;
            DynamicTableView = MainView.DynamicTableView;
            DynamicIsSelectionMode = MainView.DynamicIsSelectionMode;

            DynamicGridControl.MouseDoubleClick +=
                (s, e) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e);

                    if (i < 0) return;
                    if (DynamicIsSelectionMode)
                    {
                        if (CommandOK.CanExecute(null))
                            CommandOK.Execute(null);
                    }
                    else
                    {
                        if (CommandEdit.CanExecute(null))
                            CommandEdit.Execute(null);
                    }
                    e.Handled = true;
                };
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp13);
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
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandOK { get; set; }

        public RelayCommand CommandExport { get; set; }
        public RelayCommand CommandImport { get; set; }
        public RelayCommand CommandStartImport { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region [DllImport]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        private const int ColICode = 0;
        private const int ColITitle = 1;
        private const int ColIPrice = 2;
        private const int ColIUnit = 3;

        public int ImportErrorCount { get; set; }
    }
}
