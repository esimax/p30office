using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using GemBox.Spreadsheet;
using Limilabs.Mail;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POC.Module.Email.Views;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls.POLControls;

namespace POC.Module.Email.Models
{
    public class MEmail : NotifyObjectBase, IDisposable
    {
        #region Private Properties
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private DispatcherTimer EmailUpdateTimer { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private GridControl DynamicGridControl { get; set; }
        private TableView DynamicTableView { get; set; }
        private WebBrowser DynamicWebBrowser { get; set; }
        #endregion
        private const string ModuleID = "5D7C2B61-3934-424F-91F2-16AB3FC2B94D";

        #region CTOR
        public MEmail(object mainView)
        {
            MainView = mainView;

            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

            InitDynamics();

            PopulateTree();
            InitCommands();
            UpdateSearch();

            _NavIsExpanded = HelperSettingsClient.EmailNavIsExpanded;

            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogout)
                    {
                        SelectedItem = null;
                        EmailTreeList = null;

                        FocusedEmail = null;
                        EmailList = null;
                        FocusedEmail = null;
                    }
                    else if (e.Status == EnumMembershipStatus.AfterLogin)
                    {
                        PopulateTree();
                        UpdateSearch();
                    }
                };
        }
        #endregion




        #region NavIsExpanded
        private bool _NavIsExpanded;
        public bool NavIsExpanded
        {
            get
            {
                return _NavIsExpanded;
            }
            set
            {
                if (value == _NavIsExpanded) return;
                HelperSettingsClient.EmailNavIsExpanded = value;
                _NavIsExpanded = value;
                RaisePropertyChanged("NavIsExpanded");
            }
        }
        #endregion



        #region EmailTreeList
        public ObservableCollection<EmailTreeItem> EmailTreeList { get; set; }
        #endregion
        #region SelectedItem
        private EmailTreeItem _SelectedItem;
        public EmailTreeItem SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                RaisePropertyChanged("SelectedItem");
                UpdateSearch();
                UpdateGrid();
                FocusedEmail = null;
            }
        }
        #endregion

        #region EmailList
        private XPServerCollectionSource _EmailList;
        public XPServerCollectionSource EmailList
        {
            get { return _EmailList; }
            set
            {
                _EmailList = value;
                RaisePropertyChanged("EmailList");
            }
        }
        #endregion
        #region FocusedEmail
        private DBEMEmailInbox _FocusedEmail;
        public DBEMEmailInbox FocusedEmail
        {
            get
            {
                return _FocusedEmail;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedEmail)) return;
                _FocusedEmail = value;
                if (value != null)
                {
                    var un = "|" + AMembership.ActiveUser.UserName + "|";
                    if (value.ReadBy != null)
                    {
                        if (!value.ReadBy.Contains(un))
                        {
                            HelperUtils.Try(
                                () =>
                                {
                                    value.ReadBy += un;
                                    value.Save();
                                });
                        }
                    }
                    else
                    {
                        HelperUtils.Try(
                            () =>
                            {
                                value.ReadBy = un;
                                value.Save();
                            });
                    }
                }
                RaisePropertyChanged("FocusedEmail");
                RaisePropertyChanged("HasAttachments");
                RaisePropertyChanged("AttachmentList");

                if (_FocusedEmail != null)
                {
                    if (_FocusedEmail.BodyCache == null)
                        DynamicWebBrowser.NavigateToString("<html></html>");
                    else
                        DynamicWebBrowser.NavigateToString(HelperEmail.RenderBodyCache(_FocusedEmail.BodyCache));

                }


            }
        }
        #endregion FocusedEmail


        #region HasAttachments
        public bool HasAttachments
        {
            get { return FocusedEmail != null && FocusedEmail.AttachmentCount > 0; }
        }
        #endregion
        #region AttachmentList
        public List<DBEMEmailAttachment> AttachmentList
        {
            get { return FocusedEmail == null ? null : FocusedEmail.Attachments.ToList(); }
        }
        #endregion

        #region FocusedAttachment
        private DBEMEmailAttachment _FocusedAttachment;
        public DBEMEmailAttachment FocusedAttachment
        {
            get { return _FocusedAttachment; }
            set
            {
                if (ReferenceEquals(value, _FocusedAttachment))
                    return;

                _FocusedAttachment = value;
                RaisePropertyChanged("FocusedAttachment");
            }
        }
        #endregion





        #region [METHODS]
        private void InitDynamics()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicGridControl = MainView.DynamicGridControl;
            DynamicTableView = DynamicGridControl.View as TableView;
            DynamicWebBrowser = MainView.DynamicWebBrowser;


            ((UserControl)MainView).Loaded +=
                (s, e) =>
                {
                    HelperUtils.Try(
                    () =>
                    {
                        var fn = Path.Combine(APOCCore.LayoutPath, ModuleID + "_Email.XML");
                        DynamicGridControl.RestoreLayoutFromXml(fn);
                    });
                };

            ((UserControl)MainView).Unloaded +=
                (s, e) =>
                {
                    HelperUtils.Try(
                    () =>
                    {
                        var fn = Path.Combine(APOCCore.LayoutPath, ModuleID + "_Email.XML");
                        DynamicGridControl.SaveLayoutToXml(fn);
                    });
                };
            if (DynamicTableView != null)
                DynamicTableView.MouseDoubleClick += (s1, e1) =>
                {
                    POLMessageBox.ShowError("puff");
                };



        }
        private void PopulateTree()
        {
            EmailTreeList = new ObservableCollection<EmailTreeItem>();

            var xpcApp = DBEMEmailApp.GetAll(ADatabase.Dxs).Where(n => n.IsEnable).ToList();
            xpcApp.ForEach(
                app =>
                {

                    bool allow = false;
                    if (app.ViewPermissionType == 3)
                        allow = true;
                    else if (app.ViewPermissionType == 2 && AMembership.ActiveUser.RolesOid.Contains(app.ViewOid))
                        allow = true;
                    else if (app.ViewPermissionType == 1 && AMembership.ActiveUser.UserID == app.ViewOid)
                        allow = true;
                    else
                    {
                        if (AMembership.ActiveUser.UserName.ToLower() == "admin")
                            allow = true;
                    }

                    if (allow)
                    {
                        var etv = new EmailTreeItem
                                      {
                                          Name = app.Title,
                                          Icon = HelperImage.GetStandardImage16("_16_email.png"),
                                          SubFolders = new ObservableCollection<EmailTreeItem>(),
                                          DBFolder = null,
                                      };
                        EmailTreeList.Add(etv);
                        var folders = app.Folders.ToList();
                        var inbox = folders.FirstOrDefault(f => f.FolderType == EnumEmailFolderType.Inbox);
                        if (inbox != null)
                            etv.SubFolders.Add(new EmailTreeItem
                                                   {
                                                       Name = inbox.Title,
                                                       Icon = HelperImage.GetStandardImage16("_16_EmailInbox.png"),
                                                       DBFolder = inbox
                                                   });

                        var sent = folders.FirstOrDefault(f => f.FolderType == EnumEmailFolderType.Sent);
                        if (sent != null)
                            etv.SubFolders.Add(new EmailTreeItem
                                                   {
                                                       Name = sent.Title,
                                                       Icon = HelperImage.GetStandardImage16("_16_EmailSent.png"),
                                                       DBFolder = sent
                                                   });

                        var sendwait = folders.FirstOrDefault(f => f.FolderType == EnumEmailFolderType.WaitForSend);
                        if (sendwait != null)
                            etv.SubFolders.Add(new EmailTreeItem
                                                   {
                                                       Name = sendwait.Title,
                                                       Icon = HelperImage.GetStandardImage16("_16_Clock.png"),
                                                       DBFolder = sendwait
                                                   });



                        var fol =
                            folders.Where(f => f.FolderType == EnumEmailFolderType.Folder).OrderBy(f => f.Title).
                                ToList();
                        fol.ForEach(
                            f =>
                            {
                                var f1 = new EmailTreeItem
                                             {
                                                 Name = f.Title,
                                                 Icon = HelperImage.GetStandardImage16("_16_EmailFolder.png"),
                                                 DBFolder = f,
                                             };
                                etv.SubFolders.Add(f1);
                            });
                    }
                });
            RaisePropertyChanged("EmailTreeList");
        }
        private void UpdateSearch()
        {
            if (!AMembership.IsAuthorized) return;

            var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);

            if (SelectedItem == null || SelectedItem.DBFolder == null)
            {
                EmailList = null;
                RaisePropertyChanged("EmailList");
                return;
            }

            mainSearchCriteria.Operands.Add(new BinaryOperator("ParentFolder.Oid", SelectedItem.DBFolder.Oid));
            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBEMEmailInbox))
                          {
                              DisplayableProperties =
                                  "Oid;ParentFolder.Oid;Contact.Title;Date;Subject;FromAddress;FromName;Size;Star;Read;Priority;AttachmentCount",
                              FixedFilterCriteria = mainSearchCriteria
                          };
            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.Dxs;
            };
            EmailList = null;
            EmailList = xpi;
        }
        private void UpdateGrid()
        {
            if (SelectedItem == null) return;
            if (SelectedItem.DBFolder == null) return;
            if (SelectedItem.DBFolder.FolderType == EnumEmailFolderType.Inbox)
            {
                DynamicGridControl.Columns["FromAddress"].Header = "از";
                DynamicGridControl.Columns["FromName"].Header = "از";
            }
            if (SelectedItem.DBFolder.FolderType == EnumEmailFolderType.WaitForSend ||
                SelectedItem.DBFolder.FolderType == EnumEmailFolderType.Sent)
            {
                DynamicGridControl.Columns["FromAddress"].Header = "به";
                DynamicGridControl.Columns["FromName"].Header = "به";
            }
        }
        private void InitCommands()
        {
            CommandNew = new RelayCommand(EmailNew, () => SelectedItem != null && AMembership.HasPermission(PCOPermissions.Email_Send));
            CommandReply = new RelayCommand(() => { }, () => false);
            CommandReplyAll = new RelayCommand(() => { }, () => false);
            CommandForward = new RelayCommand(() => { }, () => false);

            CommandDelete = new RelayCommand(EmailDelete, () => FocusedEmail != null && AMembership.HasPermission(PCOPermissions.Email_Trash));
            CommandRefresh = new RelayCommand(EmailRefresh, () => true);

            CommandSyncSingle = new RelayCommand(SyncSingle, () => FocusedEmail != null && AMembership.HasPermission(PCOPermissions.Email_Sync));
            CommandSyncAll = new RelayCommand(SyncAll, () => AMembership.HasPermission(PCOPermissions.Email_Sync));
            CommandGotoContact = new RelayCommand(GotoContact, () => FocusedEmail != null && FocusedEmail.Contact != null);


            CommandAttachmentSave = new RelayCommand(AttachmentSave);
            CommandAttachmentSaveAll = new RelayCommand(AttachmentSaveAll);
            CommandAttachmentDelete = new RelayCommand(AttachmentDelete);
            CommandAttachmentDeleteAll = new RelayCommand(AttachmentDeleteAll);

            CommandSendEmailNow = new RelayCommand(SendEmailNow, () => SelectedItem != null && FocusedEmail != null && AMembership.HasPermission(PCOPermissions.Email_SendNow) && SelectedItem.DBFolder != null && SelectedItem.DBFolder.FolderType == EnumEmailFolderType.WaitForSend);

            CommandImportExcel = new RelayCommand(ImportExcel, () => AMembership.HasPermission(PCOPermissions.Email_Import));
            CommandExportExcel = new RelayCommand(ExportExcel, () => AMembership.HasPermission(PCOPermissions.Email_Export));
            CommandStartImport = new RelayCommand(StartImport, () => ImportEmailAdded != 0);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp39 != "");
        }

        private void Help()
        {
            Process.Start(ConstantPOCHelpURL.POCHelp39);
        }

        #region Import
        private const int ColIContactCode = 0;
        private const int ColIContactTitle = 1;
        private const int ColIAddressTitle = 2;
        private const int ColIAddressAddress = 3;
        public List<ImportEmailStructure> ImportList { get; set; }
        public int ImportErrorCount { get; set; }
        public int ImportEmailAdded { get; set; }
        public int ImportContactAdded { get; set; }
        private void ImportExcel()
        {
            POLMessageBox.ShowInformation("لطفا فایل اكسل را به نحوی انتخاب كنید كه دارای شرایط زیر باشد:" + Environment.NewLine + Environment.NewLine +
           "ستون اول : كد پرونده (اگر خالی باشد پرونده جدید ساخته می شود)" + Environment.NewLine +
           "ستون دوم : عنوان پرونده (می تواند خالی باشد اگر كد پرونده خالی نباشد)" + Environment.NewLine +
           "ستون سوم : عنوان آدرس" + Environment.NewLine +
           "ستون چهارم : آدرس ایمیل" + Environment.NewLine +
           Environment.NewLine +
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

            ImportList = new List<ImportEmailStructure>();

            POLProgressBox.Show("بررسی اطلاعات", true, 0, 0, 2,
                pb =>
                {
                    var dxs = ADatabase.GetNewSession();
                    foreach (ExcelWorksheet sheet in eFile.Worksheets)
                    {
                        var irow = 0;
                        foreach (ExcelRow row in sheet.Rows)
                        {
                            irow++;
                            if (irow == 1) continue;

                            if (pb.NeedToCancel)
                                break;

                            var ips = new ImportEmailStructure(irow);
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
                                   var ec = row.Cells[ColIAddressAddress];
                                   ips.Address = Convert.ToString(ec.Value);
                                   ips.Address = ips.Address.Trim();
                                   if (ips.Address.Length > 72)
                                       ips.Address = ips.Address.Substring(0, 72);
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
                    ImportEmailAdded = (from n in ImportList
                                        where n.ErrorType == EnumImportErrorType.None
                                        select n).Count();
                    ImportContactAdded = (from n in ImportList
                                          where n.ErrorType == EnumImportErrorType.None && n.ContactCode == 0
                                          select n.Row).Distinct().Count();

                    var w = new WEmailImportPreview
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

            POLProgressBox.Show("ورود اطلاعات", true, 0, ImportEmailAdded, 1,
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
                                var dbp = new DBCTEmail(dxs)
                                {
                                    Title = ips.Title,
                                    Address = ips.Address,
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
        #endregion


        #region Export
        private const int ColContactCode = 0;
        private const int ColContactTitle = 1;
        private const int ColEmailTitle = 2;
        private const int ColEmailAddress = 3;
        private const int ColResult = 4;

        private void ExportExcel()
        {
            var criteria = DynamicGridControl.FilterCriteria;

            var xpq = new XPQuery<DBCTEmail>(ADatabase.Dxs).OrderBy(n => n.Contact.Code);
            var count = xpq.Count();
            if (count == 0) return;


            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xls",
                Filter = "Microsoft Excel 2003 files (*.xls)|*.xls",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "ExportEmail.xls"
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

            ws.Cells[row, ColEmailTitle].Value = "عنوان ایمیل";
            ws.Cells[row, ColEmailTitle].Style.Font.Name = eFile.DefaultFontName;

            ws.Cells[row, ColEmailAddress].Value = "آدرس ایمیل";
            ws.Cells[row, ColEmailAddress].Style.Font.Name = eFile.DefaultFontName;
            #endregion

            row++;

            POLProgressBox.Show("استخراج اطلاعات ایمیل ها", true, 0, count, 2,
                pw =>
                {
                    foreach (var db in xpq)
                    {
                        try
                        {
                            ws.Cells[row, ColContactCode].Value = db.Contact.Code;
                            ws.Cells[row, ColContactCode].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColContactTitle].Value = db.Contact.Title;
                            ws.Cells[row, ColContactTitle].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColEmailTitle].Value = db.Title;
                            ws.Cells[row, ColEmailTitle].Style.Font.Name = eFile.DefaultFontName;

                            ws.Cells[row, ColEmailAddress].Value = db.Address;
                            ws.Cells[row, ColEmailAddress].Style.Font.Name = eFile.DefaultFontName;
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

        #endregion

        private void MoveToBasket()
        {

            var srh = DynamicGridControl.GetSelectedRowHandles();
            var list = srh.Select(n => DynamicGridControl.GetRow(n) as DBCTEmail).ToList();
            var oids = list.Select(n => n.Contact.Oid).Distinct().ToList();

            APOCMainWindow.ShowAddToBasket(DynamicOwner, null, null, oids);

        }

        private void AttachmentSave()
        {
            SaveAttachment(false);
        }
        private void AttachmentSaveAll()
        {
            SaveAttachment(true);
        }
        private void SaveAttachment(bool all)
        {
            var lang = HelperLocalize.GetCurrentLanguage();
            HelperLocalize.SetLanguageToLTR();

            var dirpath = FolderDialogEx.ShowDialog("", true,
                                                    () =>
                                                    HelperSettingsClient.
                                                        EmailLastAttachePath,
                                                    lp =>
                                                    HelperSettingsClient.
                                                        EmailLastAttachePath = lp,
                                                    DynamicOwner);
            if (string.IsNullOrWhiteSpace(dirpath))
            {
                HelperLocalize.SetLanguage(lang);
                return;
            }

            Exception failedEx = null;
            var fullpath = Path.Combine(dirpath, FocusedAttachment.FileName);
            if (File.Exists(fullpath) && !all)
            {
                var dr = POLMessageBox.ShowQuestionYesNo(
                     string.Format("فایل دیگری با همین نام در مسیر انتخاب شده وجود دارد.{0}فایل قبلی حذف شود؟", Environment.NewLine), DynamicOwner);
                if (dr != MessageBoxResult.Yes)
                {
                    HelperLocalize.SetLanguage(lang);
                    return;
                }
            }

            POLProgressBox.Show("ذخیره فایل", false, 0, all ? FocusedEmail.AttachmentCount : 0, 1,
                pb =>
                {
                    try
                    {
                        var dxs = ADatabase.GetNewSession();
                        if (!all)
                        {
                            var db = DBEMEmailInbox.FindByOid(dxs, FocusedEmail.Oid);
                            var builder = new MailBuilder();
                            var email = builder.CreateFromEml((!string.IsNullOrWhiteSpace(db.Header) ? db.Header + Environment.NewLine : string.Empty) + db.Body);

                            var att = email.Attachments.Where(n => HelperConvert.CorrectPersianBug(n.FileName) == HelperConvert.CorrectPersianBug(FocusedAttachment.FileName));
                            if (!att.Any()) return;
                            using (var f = File.Create(fullpath))
                            {
                                var at = att.First();
                                f.Write(at.Data, 0, at.Data.Length);
                            }
                        }
                        else
                        {
                            var c = 0;
                            var db = DBEMEmailInbox.FindByOid(dxs, FocusedEmail.Oid);
                            var builder = new MailBuilder();
                            var email = builder.CreateFromEml((!string.IsNullOrWhiteSpace(db.Header) ? db.Header + Environment.NewLine : string.Empty) + db.Body);

                            foreach (var att in AttachmentList)
                            {
                                var cont = email.Attachments.Where(n => HelperConvert.CorrectPersianBug(n.FileName) == HelperConvert.CorrectPersianBug(att.FileName));
                                if (!cont.Any()) continue;
                                var at = cont.First();
                                pb.AsyncSetText(1, att.FileName);
                                fullpath = Path.Combine(dirpath, att.FileName);

                                using (var f = File.Create(fullpath))
                                {
                                    f.Write(at.Data, 0, at.Data.Length);
                                }
                                c++;
                                pb.AsyncSetValue(c);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        failedEx = ex;
                    }
                },
                pb =>
                {
                    if (failedEx != null)
                    {
                        POLMessageBox.ShowError((failedEx.Message));
                        return;
                    }
                    else
                    {
                        Process.Start(dirpath);
                    }

                    HelperLocalize.SetLanguage(lang);
                }, DynamicOwner);
        }




        private void AttachmentDelete()
        {
            DeleteAttachment(false);
        }
        private void AttachmentDeleteAll()
        {
            DeleteAttachment(true);
        }
        private void DeleteAttachment(bool all)
        {
        }



        private void EmailNew()
        {
            try
            {
                if (SelectedItem == null) return;
                var ea = SelectedItem.DBFolder == null
                    ? SelectedItem.SubFolders[0].DBFolder.EmailApp
                    : SelectedItem.DBFolder.EmailApp;
                APOCMainWindow.ShowEmailSend(DynamicOwner, ea,
                    null, FocusedEmail == null ? null : FocusedEmail.FromAddress, null, null, null);
                EmailRefresh();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowWarning("حساب ایمیل شما ناقص می باشد. لطفا از برنامه خارج شوید و سرویس اصلی را مجددا راه اندازی كنید.", Window.GetWindow(DynamicOwner));
            }
        }
        private void SendEmailNow()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("اولویت ارسال ایمیل های انتخاب شده ارتقاع یابد؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var successCount = 0;
            var failedCount = 0;
            var rows = DynamicGridControl.GetSelectedRowHandles().ToList();
            var oids = rows.Select(n => ((DBEMEmailInbox)DynamicGridControl.GetRow(n)).Oid).ToList();
            if (rows.Count == 0) return;

            POLProgressBox.Show("ارتقاع اولویت", true, 0, rows.Count(), 3,
                w =>
                {
                    w.AsyncSetText(1, string.Format("در حال ارتقاع اولویت ارسال {0} ایمیل", rows.Count));
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        var i = 0;
                        foreach (var oid in oids)
                        {
                            i++;
                            try
                            {
                                if (w.NeedToCancel)
                                    return;
                                var dbi = DBEMEmailInbox.FindByOid(uow, oid);
                                if (dbi == null) continue;
                                if (dbi.ParentFolder.FolderType != EnumEmailFolderType.WaitForSend) continue;
                                dbi.Date = DateTime.Now;

                                uow.CommitChanges();
                                successCount++;
                                w.AsyncSetValue(i);
                                w.AsyncSetText(2, "موفقیت : " + successCount);
                            }
                            catch
                            {
                                uow.RollbackTransaction();
                                failedCount++;
                                w.AsyncSetText(3, "خطا : " + failedCount);
                            }
                        }
                    }
                },
                w =>
                {
                    if (failedCount > 0)
                        POLMessageBox.ShowError("بروز خطا در تغییر اولویت.", w);
                    DynamicGridControl.UnselectAll();
                    UpdateSearch();
                },
                APOCMainWindow.GetWindow())
                ;
        }
        private void EmailReply()
        {
        }

        private void EmailReplyAll()
        {
        }

        private void EmailForward()
        {
        }

        private void EmailDelete()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("ایمیلهای انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var successCount = 0;
            var failedCount = 0;
            var rows = DynamicGridControl.GetSelectedRowHandles().ToList();
            var oids = rows.Select(n => ((DBEMEmailInbox)DynamicGridControl.GetRow(n)).Oid).ToList();

            POLProgressBox.Show("حذف ایمیل", true, 0, rows.Count(), 3,
                w =>
                {
                    w.AsyncSetText(1, string.Format("در حال حذف {0} ایمیل", rows.Count));
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        var i = 0;
                        foreach (var oid in oids)
                        {
                            i++;
                            try
                            {
                                if (w.NeedToCancel)
                                    return;
                                var dbi = DBEMEmailInbox.FindByOid(uow, oid);
                                if (dbi != null)
                                {
                                    var alist = dbi.Attachments.ToList();
                                    alist.ForEach(n =>
                                    {
                                        dbi.Attachments.Remove(n);
                                        if (n.Inboxes.Count == 0)
                                            n.Delete();
                                    });
                                    dbi.Contact = null;
                                    dbi.Save();
                                    dbi.Delete();
                                    uow.CommitChanges();

                                    successCount++;
                                    w.AsyncSetValue(i);
                                    w.AsyncSetText(2, "حذف با  موفقیت : " + successCount);
                                }
                            }
                            catch
                            {
                                uow.RollbackTransaction();
                                failedCount++;
                                w.AsyncSetText(3, "حذف با  خطا : " + failedCount);
                            }
                        }
                    }
                },
                w =>
                {
                    if (successCount > 0)
                        POLMessageBox.ShowInformation("عملیات انجام شد.", w);
                    if (failedCount > 0)
                        POLMessageBox.ShowError("بروز خطا در حذف اطلاعات.", w);
                    DynamicGridControl.UnselectAll();
                    UpdateSearch();
                },
                APOCMainWindow.GetWindow());
        }

        private void EmailRefresh()
        {
            UpdateSearch();
        }

        private void SyncSingle()
        {
            EmailSync(false, false);
        }

        private void SyncAll()
        {
            EmailSync(true, true);
        }

        private void GotoContact()
        {
            if (FocusedEmail == null) return;
            if (FocusedEmail.Contact == null) return;
            APOCContactModule.GotoContactByCode(FocusedEmail.Contact.Code);
        }

        private void EmailSync(bool ask, bool fromSearchResult)
        {
            if (FocusedEmail == null) return;
            var selectedcalls = DynamicGridControl.GetSelectedRowHandles();

            POLProgressBox.Show(3, pw
                =>
            {
                if (!fromSearchResult)
                {
                    var dxs = ADatabase.GetNewSession();
                    if (selectedcalls == null || selectedcalls.Length == 0)
                        return;
                    var items = (from n in selectedcalls select DBEMEmailInbox.FindByOid(dxs, ((DBEMEmailInbox)DynamicGridControl.GetRow(n)).Oid));
                    if (items.Count() == 1)
                        SyncSingle(items.First());
                    else if (items.Count() > 1)
                        SyncMulti(pw, items.ToList());
                }
                else
                {
                    var proceed = false;

                    var dxs = ADatabase.GetNewSession();

                    var xpq = new XPQuery<DBEMEmailInbox>(dxs);

                    var count = xpq.Count();

                    if (ask)
                    {
                        var doo = Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Func<object>)(() =>
                        {
                            var dr = POLMessageBox.ShowQuestionYesNo(String.Format("تطبیق برای {0} ركورد ایمیل انجام شود؟", count));
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

                        var synced = 0;

                        for (var i = 0; i < count; i = i + 100)
                        {
                            var list = (from n in xpq orderby n.Oid select n).Skip(i).Take(100).ToList();
                            var counter = 0;
                            foreach (var dbcall in list)
                            {
                                if (pw.NeedToCancel) return;
                                counter++;
                                pw.AsyncSetText(2, String.Format("ایمیل : {0}", i + counter));
                                if (dbcall.Contact == null)
                                {
                                    if (FindProperContact(dbcall))
                                        synced++;
                                }
                                else
                                {
                                    if (dbcall.Contact.IsDeleted)
                                    {
                                        if (RefindContact(dbcall))
                                            synced++;
                                    }
                                }
                                pw.AsyncSetText(3, string.Format("تطبیق : {0}", synced));
                                pw.AsyncSetValue(counter + i - 1);
                            }
                        }
                    }
                }
            }, pw => UpdateSearch(), APOCMainWindow.GetWindow());


        }

        private bool RefindContact(DBEMEmailInbox dbcall)
        {
            try
            {
                dbcall.Contact = null;
                dbcall.Save();
                return FindProperContact(dbcall);
            }
            catch { }
            return false;
        }
        private bool FindProperContact(DBEMEmailInbox dbei)
        {
            if (dbei == null) return false;
            var dbp = DBCTEmail.FindByAddressExcept(dbei.Session, null, dbei.FromAddress);
            if (dbp == null) return false;
            try
            {
                dbei.Contact = dbp.Contact;
                dbei.Save();
                return true;
            }
            catch { }
            return false;
        }
        private void SyncSingle(DBEMEmailInbox dbc)
        {
            if (dbc.Contact == null)
            {
                var foundContact = false;
                var dbp = DBCTEmail.FindByAddressExcept(dbc.Session, null, dbc.FromAddress);
                if (dbp != null)
                {
                    if (dbp.Contact != null)
                        foundContact = true;
                }

                if (foundContact)
                {
                    dbc.Contact = dbp.Contact;
                    dbc.Save();
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)(() =>
                    {
                        APOCMainWindow.ShowEmailSync(APOCMainWindow.GetWindow(), dbc);
                        UpdateSearch();
                    }));
                }
            }
            else
            {
                if (dbc.Contact.IsDeleted)
                {
                    HelperUtils.DoDispatcher(
                        () =>
                        {
                            var tdr = POLMessageBox.ShowQuestionYesNo("اطلاعات قبلی ایمیل انتخاب شده، حذف شده اند" + "\n" + "اطلاعات سوخته از نو بررسی شود؟");
                            if (tdr != MessageBoxResult.Yes) return;
                            dbc.Contact = null;
                            dbc.Save();
                            SyncSingle(dbc);
                        });
                }

            }
        }
        private void SyncMulti(POLProgressBox e2, List<DBEMEmailInbox> calllist)
        {
            if (calllist == null) return;
            if (calllist.Count == 0) return;
            e2.AsyncEnableCancel();
            e2.AsyncSetMax(calllist.Count);

            var synced = 0;
            var counter = 0;
            e2.AsyncSetText(1, String.Format("تعداد : {0}", calllist.Count));
            foreach (var dbcall in calllist)
            {
                counter++;
                e2.AsyncSetText(2, String.Format("ایمیل : {0}", counter));
                if (dbcall.Contact == null)
                {
                    if (FindProperContact(dbcall))
                        synced++;
                }
                else
                {
                    if (dbcall.Contact.IsDeleted)
                    {
                        if (RefindContact(dbcall))
                            synced++;
                    }
                }
                e2.AsyncSetText(3, string.Format("تطبیق : {0}", synced));
                e2.AsyncSetValue(counter - 1);
            }
        }

        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandSendEmailNow { get; set; }
        public RelayCommand CommandReply { get; set; }
        public RelayCommand CommandReplyAll { get; set; }
        public RelayCommand CommandForward { get; set; }

        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }

        public RelayCommand CommandSyncSingle { get; set; }
        public RelayCommand CommandSyncAll { get; set; }
        public RelayCommand CommandGotoContact { get; set; }

        public RelayCommand CommandAttachmentSave { get; set; }
        public RelayCommand CommandAttachmentSaveAll { get; set; }
        public RelayCommand CommandAttachmentDelete { get; set; }
        public RelayCommand CommandAttachmentDeleteAll { get; set; }
        public RelayCommand CommandMoveToBasket { get; set; }


        public RelayCommand CommandImportExcel { get; set; }
        public RelayCommand CommandExportExcel { get; set; }
        public RelayCommand CommandStartImport { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }

}
