using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using GemBox.Spreadsheet;
using POC.Module.Call.Models;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.DXControls;
using Microsoft.Win32;

namespace POC.Module.Call.Views
{
    public partial class WReport6 : DXWindow
    {
        private ICacheData ACacheData { get; set; }
        private IMembership AMembership { get; set; }
        private IDatabase ADatabase { get; set; }
        private GroupOperator MainSearchCriteria { get; set; }
        private object SelectedContactCat { get; set; }
        private List<object> ContactCatList { get; set; }

        public WReport6()
        {
            InitializeComponent();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();

            PopulateContactCat();
            cbeContactCat.ItemsSource = ContactCatList;
            #region cbeContactCat.SelectedIndexChanged
            cbeContactCat.SelectedIndexChanged +=
                    (s, e) =>
                    {
                        SelectedContactCat = cbeContactCat.SelectedItem;
                    }; 
            #endregion
            #region biRefresh.Click
            biRefresh.Click += (s, e) =>
                {
                    PopulateContactCat();
                    cbeContactCat.SelectedIndex = 0;
                }; 
            #endregion
            #region bCalculate.Click
            bCalculate.Click += (s, e) =>
                {
                    UpdateSearch();
                }; 
            #endregion
            #region Loaded
            Loaded += (s, e) =>
                {
                    cbeContactCat.SelectedIndex = 0;
                    deDate.DateEditValue = DateTime.Today;
                }; 
            #endregion
            #region bExport.Click
            bExport.Click += (s, e) =>
            {
                var datasource = gcMain.ItemsSource as List<Report6Holder>;
                if (datasource == null || datasource.Count == 0)
                {
                    POLMessageBox.Show("هیچ اطلاعاتی برای استخراج وجود ندارد.");
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
                    FileName = "ExportCall.xls"
                };
                if (sf.ShowDialog() != true)
                    return;

                var filename = sf.FileName;
                var eFile = new ExcelFile
                {
                    DefaultFontName = HelperLocalize.ApplicationFontName,
                    DefaultFontSize = (int)HelperLocalize.ApplicationFontSize
                };

                var ws = eFile.Worksheets.Add(Title);
                ws.ViewOptions.ShowColumnsFromRightToLeft = HelperLocalize.ApplicationFlowDirection == FlowDirection.RightToLeft;

                var row = 0;
                #region Columns Title
                var colIndex = 0;
                foreach (var col in gcMain.Columns.OrderBy(n => n.VisibleIndex))
                {
                    if (col.Visible)
                    {
                        ws.Cells[row, colIndex].Value = col.HeaderCaption;
                        ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                        colIndex++;
                    }
                }
                #endregion

                var colfields = (from n in gcMain.Columns where n.Visible orderby n.VisibleIndex select n.FieldName).ToList();
                var colOrders = (from n in gcMain.Columns where n.Visible && n.SortOrder != ColumnSortOrder.None orderby n.SortIndex select new { n.FieldName, n.SortOrder }).ToList();
                row++;



                POLProgressBox.Show("استخراج اطلاعات تماس ها", true, 0, datasource.Count, 2,
                pw =>
                {
                    var q = from n in datasource select n;
                    var oCount = 0;
                    foreach (var colOrder in colOrders)
                    {
                        #region Order First Time

                        if (colOrder.FieldName == "ContactCode")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.ContactCode)
                                : q.OrderByDescending(n => n.ContactCode);
                            oCount++;
                        }
                        if (colOrder.FieldName == "ContactTitle")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.ContactTitle)
                                : q.OrderByDescending(n => n.ContactTitle);
                            oCount++;
                        }
                        if (colOrder.FieldName == "PhoneNumber")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.PhoneNumber)
                                : q.OrderByDescending(n => n.PhoneNumber);
                            oCount++;
                        }
                        if (colOrder.FieldName == "LastCallDateInOutContact")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.LastCallDateInOutContact)
                                : q.OrderByDescending(n => n.LastCallDateInOutContact);
                            oCount++;
                        }
                        if (colOrder.FieldName == "LastCallAsDays")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.LastCallAsDays)
                                : q.OrderByDescending(n => n.LastCallAsDays);
                            oCount++;
                        }
                        if (colOrder.FieldName == "LastCallType")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.LastCallType)
                                : q.OrderByDescending(n => n.LastCallType);
                            oCount++;
                        }
                        if (colOrder.FieldName == "LastLine")
                        {
                            q = colOrder.SortOrder == ColumnSortOrder.Ascending
                                ? q.OrderBy(n => n.LastLine)
                                : q.OrderByDescending(n => n.LastLine);
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
                                #region ContactCode
                                if (field == "ContactCode")
                                {
                                    ws.Cells[row, colIndex].Value = db.ContactCode;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region CallDate
                                if (field == "ContactTitle")
                                {
                                    ws.Cells[row, colIndex].Value = db.ContactTitle;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region PhoneNumber
                                if (field == "PhoneNumber")
                                {
                                    ws.Cells[row, colIndex].Value = db.PhoneNumber;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region LastCallDateInOutContact
                                if (field == "LastCallDateInOutContact")
                                {
                                    ws.Cells[row, colIndex].Value = db.LastCallDateInOutContact;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region LastCallAsDays
                                if (field == "LastCallAsDays")
                                {
                                    ws.Cells[row, colIndex].Value = db.LastCallAsDays;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region LastCallType
                                if (field == "LastCallType")
                                {
                                    ws.Cells[row, colIndex].Value = db.LastCallType;
                                    ws.Cells[row, colIndex].Style.Font.Name = eFile.DefaultFontName;
                                    colIndex++;
                                }
                                #endregion
                                #region LastLine
                                if (field == "LastLine")
                                {
                                    ws.Cells[row, colIndex].Value = db.LastLine;
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

                        pw.AsyncSetText(1, db.PhoneNumber ?? string.Empty);
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
                }, this);
            };
            #endregion
            #region bCancel.Click
            bCancel.Click += (s, e) =>
                {
                    Close();
                }; 
            #endregion
        }

        private void PopulateContactCat()
        {
            var xpc = (from n in ACacheData.GetContactCatList() let cat = n.Tag as DBCTContactCat orderby cat.Title select cat).ToList();
            ContactCatList = new List<object> { "(همه دسته ها)" };
            if (AMembership.ActiveUser.UserName.ToLower() == "admin")
            {
                xpc.ToList().ForEach(n => ContactCatList.Add(n));
            }
            else
            {
                xpc.ToList().Where(n => n.Role != null && AMembership.ActiveUser.Roles.Select(r => r.ToLower()).Contains(n.Role.ToLower())).ToList().ForEach(n => ContactCatList.Add(n));
            }
        }

        private void UpdateSearch()
        {
            if (!deDate.DateEditValue.HasValue)
                return;
            var date = deDate.DateEditValue.Value;

            MainSearchCriteria = new GroupOperator(GroupOperatorType.And);
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
            MainSearchCriteria.Operands.Add(new BinaryOperator("CallDate",date.Date,BinaryOperatorType.GreaterOrEqual));

            var xpc = new XPCollection<DBCLCall>(ADatabase.Dxs, MainSearchCriteria);

            var phones =  xpc.Select(n=>n.FullPhoneString).Distinct().ToList();
            var datasource = new List<Report6Holder>();

            POLProgressBox.Show("محاسبه", true, 0,phones.Count(), 1,
                pw =>
                {
                    if(phones.Count==0)
                        return;
                    var internals = new List<int>();
                    if (!string.IsNullOrWhiteSpace(AMembership.ActiveUser.InternalPhone))
                    {
                        internals = (from n in AMembership.ActiveUser.InternalPhone.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                     select Convert.ToInt32(n)).ToList();

                    }
                    int index = 0;
                    foreach (var phone in phones)
                    {
                        index++;
                        pw.AsyncSetText(1, phone);
                        pw.AsyncSetValue(index);

                        var dbcall = xpc.Where(n => n.FullPhoneString == phone).ToList();
                        var hasCallIn = dbcall.Count(n => n.CallType == EnumCallType.CallIn);
                        var hasCallOut = dbcall.Count(n => n.CallType == EnumCallType.CallOut);

                        if (!(hasCallIn > 0 && hasCallOut == 0))continue;
                        var lastCall = dbcall.OrderByDescending(n=>n.CallDate).First();

                        var holder = new Report6Holder();
                        if (lastCall.Contact != null)
                        {

                            holder.ContactCode = Convert.ToInt32(lastCall.Contact.Code);
                            holder.ContactTitle = lastCall.Contact.Title;
                        }

                        holder.PhoneNumber = phone;
                        holder.LastCallDateInOutContact = HelperPersianCalendar.ToString(lastCall.CallDate.Value,
                            "yyyy/MM/dd HH:mm:ss");
                        holder.LastCallAsDays = (DateTime.Today - lastCall.CallDate.Value.Date).Days;
                        holder.LastCallType = "دریافتی";
                        if (lastCall.DurationSeconds < 0 && lastCall.CallType == EnumCallType.CallIn)
                            holder.LastCallType = ConstantGeneral.MissCallTitle;
                        holder.LastLine = lastCall.LineNumber;

                        datasource.Add(holder);
                    }
                },
                pw =>
                {
                    gcMain.ItemsSource = datasource;
                    Task.Factory.StartNew(
                        () =>
                        {
                            System.Threading.Thread.Sleep(500);
                            HelperUtils.DoDispatcher(() => tvMain.BestFitColumns());
                        });
                }, this);

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
        #region [DllImport]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion
    }
}
