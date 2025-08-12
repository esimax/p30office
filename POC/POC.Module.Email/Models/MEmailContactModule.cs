using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Limilabs.Mail;
using Microsoft.Practices.Prism.Logging;
using POC.Module.Email.Views;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Utils;
using POL.DB.P30Office;
using POL.WPF.DXControls.POLControls;

namespace POC.Module.Email.Models
{

    public class MEmailContactModule : NotifyObjectBase, IDisposable
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private WebBrowser DynamicWebBrowserReceive { get; set; }

        private GridControl DynamicReceiveGrid { get; set; }
        private TableView DynamicReceiveTableView { get; set; }




        public MEmailContactModule(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();

            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;

            IsReceive = true;
            InitCommands();
            GetDynamicData();

            PopulateEmailAccounts();
        }
        void AContactModule_OnSelectedContactChanged(object sender, EventArgs e)
        {
            PopulateEmailAccounts();
            RaisePropertyChanged("RootEnable");
            if (EmailAccountList != null)
                FocusedEmailAccount = EmailAccountList.FirstOrDefault();
        }

        #region WindowTitle
        public string WindowTitle
        {
            get { return "ایمیل های " + ((DBCTContact)AContactModule.SelectedContact).Title; }
        }
        #endregion



        #region IsReceive
        private bool _IsReceive;
        public bool IsReceive
        {
            get { return _IsReceive; }
            set
            {
                if (value == _IsReceive)
                    return;

                _IsReceive = value;

                if (value)
                {
                    IsSend = false;
                    IsWait = false;
                    Refresh();
                    UpdateGrid();
                }
                else
                {
                    if (!IsWait && !IsSend)
                    {
                        _IsReceive = true;
                    }
                }

                RaisePropertyChanged("IsReceive");
            }
        }
        #endregion
        #region IsSend
        private bool _IsSend;
        public bool IsSend
        {
            get { return _IsSend; }
            set
            {
                if (value == _IsSend)
                    return;

                _IsSend = value;

                if (value)
                {
                    IsReceive = false;
                    IsWait = false;
                    Refresh();
                    UpdateGrid();
                }
                else
                {
                    if (!IsWait && !IsReceive)
                    {
                        _IsSend = true;
                    }
                }

                RaisePropertyChanged("IsSend");
            }
        }
        #endregion
        #region IsWait
        private bool _IsWait;
        public bool IsWait
        {
            get { return _IsWait; }
            set
            {
                if (value == _IsWait)
                    return;

                _IsWait = value;

                if (value)
                {
                    IsSend = false;
                    IsReceive = false;
                    Refresh();
                    UpdateGrid();
                }
                else
                {
                    if (!IsReceive && !IsSend)
                    {
                        _IsWait = true;
                    }
                }

                RaisePropertyChanged("IsWait");
            }
        }
        #endregion





        #region RootEnable
        public bool RootEnable { get { return AContactModule.SelectedContact != null; } }
        #endregion

        #region EmailAccountList
        private XPCollection<DBCTEmail> _EmailAccountList;
        public XPCollection<DBCTEmail> EmailAccountList
        {
            get { return _EmailAccountList; }
            set
            {
                _EmailAccountList = value;
                RaisePropertyChanged("EmailAccountList");
            }
        }
        #endregion
        #region FocusedEmailAccount
        private DBCTEmail _FocusedEmailAccount;
        public DBCTEmail FocusedEmailAccount
        {
            get
            {
                return _FocusedEmailAccount;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedEmailAccount)) return;
                _FocusedEmailAccount = value;
                RaisePropertyChanged("FocusedEmailAccount");
                PopulateReceiveList();
            }
        }
        #endregion


        #region ReceiveList
        private XPServerCollectionSource _ReceiveList;
        public XPServerCollectionSource ReceiveList
        {
            get { return _ReceiveList; }
            set
            {
                _ReceiveList = value;
                RaisePropertyChanged("ReceiveList");
            }
        }
        #endregion
        #region ReceiveFocusedEmail
        private DBEMEmailInbox _ReceiveFocusedEmail;
        public DBEMEmailInbox ReceiveFocusedEmail
        {
            get
            {
                return _ReceiveFocusedEmail;
            }
            set
            {
                if (ReferenceEquals(value, _ReceiveFocusedEmail)) return;
                _ReceiveFocusedEmail = value;
                if (value != null)
                {
                    var un = "|" + AMembership.ActiveUser.UserName + "|";
                    if (value.ReadBy != null)
                    {
                        if (!value.ReadBy.Contains(un))
                        {
                            HelperUtils.Try(() =>
                            {
                                value.ReadBy += un;
                                value.Save();
                            });
                        }
                    }
                    else
                    {
                        HelperUtils.Try(() =>
                        {
                            value.ReadBy = un;
                            value.Save();
                        });
                    }
                }
                RaisePropertyChanged("ReceiveFocusedEmail");
                RaisePropertyChanged("ReceiveHasAttachments");
                RaisePropertyChanged("ReceiveAttachmentList");

                if (_ReceiveFocusedEmail != null)
                {
                    DynamicWebBrowserReceive.NavigateToString(HelperEmail.RenderBodyCache(_ReceiveFocusedEmail.BodyCache));


                }


            }
        }
        #endregion ReceiveFocusedEmail

        #region ReceiveHasAttachments
        public bool ReceiveHasAttachments
        {
            get { return ReceiveFocusedEmail != null && ReceiveFocusedEmail.AttachmentCount > 0; }
        }
        #endregion
        #region ReceiveAttachmentList
        public List<DBEMEmailAttachment> ReceiveAttachmentList
        {
            get { return ReceiveFocusedEmail == null ? null : ReceiveFocusedEmail.Attachments.ToList(); }
        }
        #endregion
        #region ReceiveFocusedAttachment
        private DBEMEmailAttachment _ReceiveFocusedAttachment;
        public DBEMEmailAttachment ReceiveFocusedAttachment
        {
            get { return _ReceiveFocusedAttachment; }
            set
            {
                if (ReferenceEquals(value, _ReceiveFocusedAttachment))
                    return;

                _ReceiveFocusedAttachment = value;
                RaisePropertyChanged("ReceiveFocusedAttachment");
            }
        }
        #endregion

        #region ReceiveSelectedRowCount
        public int ReceiveSelectedRowCount
        {
            get
            {
                return DynamicReceiveGrid.GetSelectedRowHandles().Length;
            }
        }
        #endregion








        #region [METHODS]
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicWebBrowserReceive = MainView.DynamicWebBrowserReceive;
            DynamicReceiveGrid = MainView.DynamicReceiveGrid;
            DynamicReceiveTableView = MainView.DynamicReceiveTableView;

        }
        private void UpdateGrid()
        {
            if (DynamicReceiveGrid == null) return;
            if (IsReceive)
            {
                DynamicReceiveGrid.Columns.First(n => n.Name == "colTitle").Header = "از";
                DynamicReceiveGrid.Columns.First(n => n.Name == "colAddress").Header = "از";
            }
            if (IsSend || IsWait)
            {
                DynamicReceiveGrid.Columns.First(n => n.Name == "colTitle").Header = "به";
                DynamicReceiveGrid.Columns.First(n => n.Name == "colAddress").Header = "به";
            }
        }
        private void InitCommands()
        {
            CommandNewAccount = new RelayCommand(NewAccount, () => AContactModule.SelectedContact != null);
            CommandEditAccount = new RelayCommand(EditAccount, () => FocusedEmailAccount != null && AContactModule.SelectedContact != null);
            CommandDeleteAccount = new RelayCommand(DeleteAccount, () => FocusedEmailAccount != null && AContactModule.SelectedContact != null);

            CommandNew = new RelayCommand(NewEmail, () => AContactModule.SelectedContact != null && FocusedEmailAccount != null);
            CommandSendEmailNow = new RelayCommand(SendEmailNow, () => IsWait &&
                AContactModule.SelectedContact != null &&
                FocusedEmailAccount != null &&
                ReceiveFocusedEmail != null &&
                AMembership.HasPermission(PCOPermissions.Email_SendNow));
            CommandReply = new RelayCommand(() => { }, () => false);
            CommandReplyAll = new RelayCommand(() => { }, () => false);
            CommandForward = new RelayCommand(() => { }, () => false);


            CommandDelete = new RelayCommand(Delete,
                () =>
                {
                    if (AContactModule.SelectedContact == null) return false;
                    if (FocusedEmailAccount == null) return false;
                    if (ReceiveFocusedEmail == null) return false;
                    return true;
                });
            CommandRefresh = new RelayCommand(Refresh,
                () =>
                {
                    if (AContactModule.SelectedContact == null) return false;
                    if (FocusedEmailAccount == null) return false;
                    return true;
                });


            CommandReceiveAttachmentSave = new RelayCommand(ReceiveAttachmentSave);
            CommandReceiveAttachmentSaveAll = new RelayCommand(ReceiveAttachmentSaveAll);
            CommandReceiveAttachmentDelete = new RelayCommand(ReceiveAttachmentDelete);
            CommandReceiveAttachmentDeleteAll = new RelayCommand(ReceiveAttachmentDeleteAll);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp40 != "");
        }

        private void NewEmail()
        {
            APOCMainWindow.ShowEmailSend(
                DynamicOwner,
                null,
                null,
                FocusedEmailAccount.AddressLower,
                FocusedEmailAccount.Contact,
                null,
                null);
            PopulateReceiveList();
        }
        private void SendEmailNow()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("اولویت ارسال ایمیل های انتخاب شده ارتقاع یابد؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var successCount = 0;
            var failedCount = 0;
            var rows = DynamicReceiveGrid.GetSelectedRowHandles().ToList();
            var oids = rows.Select(n => ((DBEMEmailInbox)DynamicReceiveGrid.GetRow(n)).Oid).ToList();
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
                    DynamicReceiveGrid.UnselectAll();
                    ReceiveRefresh();
                },
                APOCMainWindow.GetWindow())
                ;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp40);
        }




        private void NewAccount()
        {
            var w = new WEmailAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
            {
                PopulateEmailAccounts();
                if (w.DynamicSelectedData != null)
                    FocusedEmailAccount = EmailAccountList.FirstOrDefault(n => n.Address == w.DynamicSelectedData.Address);
            }
        }
        private void EditAccount()
        {
            if (FocusedEmailAccount == null) return;
            var w = new WEmailAddEdit(FocusedEmailAccount) { Owner = DynamicOwner };
            if (w.ShowDialog() == true)
            {
                EmailAccountList.Reload();
                FocusedEmailAccount.Reload();
            }
        }
        private void DeleteAccount()
        {
            if (FocusedEmailAccount == null) return;
            var dr =
                POLMessageBox.ShowQuestionYesNo(
                    string.Format("ایمیل زیر بطور كامل حذف شود؟{0}{0}{1}", Environment.NewLine,
                                  FocusedEmailAccount.ToString()), DynamicOwner);
            if (dr == MessageBoxResult.Yes)
            {
                try
                {
                    var dbe = DBCTEmail.FindByOid(ADatabase.GetNewSession(), FocusedEmailAccount.Oid);
                    dbe.Delete();
                    dbe.Save();
                    PopulateEmailAccounts();
                    if (EmailAccountList != null)
                        FocusedEmailAccount = EmailAccountList.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    POLMessageBox.ShowError(ex.Message, DynamicOwner);
                }
            }
        }

        private void Delete()
        {
            ReceiveDelete();
        }
        private void ReceiveDelete()
        {
            var dr = POLMessageBox.ShowQuestionYesNo("ایمیلهای انتخاب شده حذف شوند؟", DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            var successCount = 0;
            var failedCount = 0;
            var rows = DynamicReceiveGrid.GetSelectedRowHandles().ToList();
            var oids = rows.Select(n => ((DBEMEmailInbox)DynamicReceiveGrid.GetRow(n)).Oid).ToList();

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
                                        n.Delete();
                                    });
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
                    DynamicReceiveGrid.UnselectAll();
                    PopulateReceiveList();
                },
                APOCMainWindow.GetWindow())
                ;
        }
        private void Refresh()
        {
            ReceiveRefresh();
        }
        private void ReceiveRefresh()
        {
            PopulateReceiveList();
            ReceiveFocusedAttachment = null;
            ReceiveFocusedEmail = null;
        }



        private void PopulateEmailAccounts()
        {
            ReceiveFocusedEmail = null;
            FocusedEmailAccount = null;
            if (AContactModule.SelectedContact == null || !(AContactModule.SelectedContact is DBCTContact))
            {
                EmailAccountList = null;
                PopulateReceiveList();
                return;
            }

            var dbc = AContactModule.SelectedContact as DBCTContact;
            var xpc = DBCTEmail.GetByContact(ADatabase.Dxs, dbc.Oid);
            EmailAccountList = xpc;
        }
        private void PopulateReceiveList()
        {
            if (AContactModule.SelectedContact == null || !(AContactModule.SelectedContact is DBCTContact) || FocusedEmailAccount == null)
            {
                ReceiveList = null;
                RaisePropertyChanged("ReceiveList");
                return;
            }
            var dbc = AContactModule.SelectedContact as DBCTContact;

            var ft = EnumEmailFolderType.Inbox;
            if (IsSend)
                ft = EnumEmailFolderType.Sent;
            if (IsWait)
                ft = EnumEmailFolderType.WaitForSend;

            var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            mainSearchCriteria.Operands.Add(new BinaryOperator("Contact.Oid", dbc.Oid));
            mainSearchCriteria.Operands.Add(new BinaryOperator("ParentFolder.FolderType", (int)ft));
            mainSearchCriteria.Operands.Add(new BinaryOperator("FromAddress", FocusedEmailAccount.AddressLower));

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBEMEmailInbox))
            {
                DisplayableProperties =
                    "Oid;EmailApp.Title;Contact.Oid;Contact.Title;Date;Subject;FromAddress;FromName;Size;Star;Read;Priority;AttachmentCount",
                FixedFilterCriteria = mainSearchCriteria
            };
            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.Dxs;
            };
            ReceiveList = null;
            ReceiveList = xpi;
        }


        private void ReceiveAttachmentSave()
        {
            ReceiveSaveAttachment(false);
        }
        private void ReceiveAttachmentSaveAll()
        {
            ReceiveSaveAttachment(true);
        }
        private void ReceiveSaveAttachment(bool all)
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
            var fullpath = System.IO.Path.Combine(dirpath, ReceiveFocusedAttachment.FileName);
            if (System.IO.File.Exists(fullpath) && !all)
            {
                var dr = POLMessageBox.ShowQuestionYesNo(
                     string.Format("فایل دیگری با همین نام در مسیر انتخاب شده وجود دارد.{0}فایل قبلی حذف شود؟", Environment.NewLine), DynamicOwner);
                if (dr != MessageBoxResult.Yes)
                {
                    HelperLocalize.SetLanguage(lang);
                    return;
                }
            }

            POLProgressBox.Show("ذخیره فایل", false, 0, all ? ReceiveFocusedEmail.AttachmentCount : 0, 1,
                pb =>
                {
                    try
                    {
                        var dxs = ADatabase.GetNewSession();
                        if (!all)
                        {
                            var db = DBEMEmailInbox.FindByOid(dxs, ReceiveFocusedEmail.Oid);
                            var builder = new MailBuilder();
                            var email = builder.CreateFromEml((!string.IsNullOrWhiteSpace(db.Header) ? db.Header + Environment.NewLine : string.Empty) + db.Body);

                            var att = email.Attachments.Where(n => HelperConvert.CorrectPersianBug(n.FileName) == HelperConvert.CorrectPersianBug(ReceiveFocusedAttachment.FileName));
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
                            var db = DBEMEmailInbox.FindByOid(dxs, ReceiveFocusedEmail.Oid);
                            var builder = new MailBuilder();
                            var email = builder.CreateFromEml((!string.IsNullOrWhiteSpace(db.Header) ? db.Header + Environment.NewLine : string.Empty) + db.Body);

                            foreach (var att in ReceiveAttachmentList)
                            {
                                var cont = email.Attachments.Where(n => HelperConvert.CorrectPersianBug(n.FileName) == HelperConvert.CorrectPersianBug(att.FileName));
                                if (!cont.Any()) continue;
                                var at = cont.First();
                                pb.AsyncSetText(1, att.FileName);
                                fullpath = System.IO.Path.Combine(dirpath, att.FileName);

                                using (var f = System.IO.File.Create(fullpath))
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
                        System.Diagnostics.Process.Start(dirpath);
                    }

                    HelperLocalize.SetLanguage(lang);
                }, DynamicOwner);
        }

        private void ReceiveAttachmentDelete()
        {
            ReceiveDeleteAttachment(false);
        }
        private void ReceiveAttachmentDeleteAll()
        {
            ReceiveDeleteAttachment(true);
        }
        private void ReceiveDeleteAttachment(bool all)
        {
        }

        #endregion


        #region [COMMANDS]
        public RelayCommand CommandNewAccount { get; set; }
        public RelayCommand CommandEditAccount { get; set; }
        public RelayCommand CommandDeleteAccount { get; set; }

        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandSendEmailNow { get; set; }
        public RelayCommand CommandReply { get; set; }
        public RelayCommand CommandReplyAll { get; set; }
        public RelayCommand CommandForward { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }

        public RelayCommand CommandReceiveAttachmentSave { get; set; }
        public RelayCommand CommandReceiveAttachmentSaveAll { get; set; }
        public RelayCommand CommandReceiveAttachmentDelete { get; set; }
        public RelayCommand CommandReceiveAttachmentDeleteAll { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            AContactModule.OnSelectedContactChanged -= AContactModule_OnSelectedContactChanged;
        }
        #endregion
    }
}
