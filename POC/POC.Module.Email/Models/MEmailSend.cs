using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Limilabs.Mail;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office.BT;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.DB.P30Office;
using System.Collections.ObjectModel;
using System.IO;
using POC.Module.Email.Views;
using Microsoft.Win32;

namespace POC.Module.Email.Models
{
    public class MEmailSend : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private ICacheData ACacheData { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private POCCore APOCCore { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }

        public DBEMEmailApp DynamicFrom { get; set; }
        private DBEMEmailInbox DynamicInbox { get; set; }
        private string DynamicEmailAddresses { get; set; }
        private DBCTContact DynamicContact { get; set; }
        private DBCTContactCat DynamicCategory { get; set; }
        private DBCTContactSelection DynamicBasket { get; set; }
        public RichTextBox DynamicRichTextBox { get; set; }

        #region CTOR
        public MEmailSend(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ADataFieldManager = ServiceLocator.Current.GetInstance<IDataFieldManager>();

            InitDynamicData();
            InitCommands();

            PopulateCategoryList();
            PopulateBasketList();
            PopulateEmailTitleList();
            PopulatePriorityList();

            PopulateTemplateList();
            PopulateParameterList();

            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "ارسال ایمیل"; }
        }
        #endregion

        #region TabSelectedIndex
        private int _TabSelectedIndex;
        public int TabSelectedIndex
        {
            get { return _TabSelectedIndex; }
            set
            {
                _TabSelectedIndex = value;
                RaisePropertyChanged("TabSelectedIndex");
                if (value == 3)
                    RaisePropertyChanged("PreviewData");
            }
        }
        #endregion


        #region EmailAppList
        private List<DBEMEmailApp> _EmailAppList;
        public List<DBEMEmailApp> EmailAppList
        {
            get { return _EmailAppList; }
            set
            {
                if (value == _EmailAppList)
                    return;

                _EmailAppList = value;
                RaisePropertyChanged("EmailAppList");
            }
        }
        #endregion
        #region SelectedEmailApp
        private DBEMEmailApp _SelectedEmailApp;
        public DBEMEmailApp SelectedEmailApp
        {
            get { return _SelectedEmailApp; }
            set
            {
                if (value == _SelectedEmailApp)
                    return;

                _SelectedEmailApp = value;
                RaisePropertyChanged("SelectedEmailApp");
            }
        }
        #endregion


        #region CategoryList
        private List<DBCTContactCat> _CategoryList;
        public List<DBCTContactCat> CategoryList
        {
            get { return _CategoryList; }
            set
            {
                _CategoryList = value;
                RaisePropertyChanged("CategoryList");
            }
        }
        #endregion
        #region BasketList
        private XPCollection<DBCTContactSelection> _BasketList;
        public XPCollection<DBCTContactSelection> BasketList
        {
            get { return _BasketList; }
            set
            {
                _BasketList = value;
                RaisePropertyChanged("BasketList");
            }
        }
        #endregion
        #region EmailTitleList
        private List<string> _EmailTitleList;
        public List<string> EmailTitleList
        {
            get { return _EmailTitleList; }
            set
            {
                _EmailTitleList = value;
                RaisePropertyChanged("EmailTitleList");
            }
        }
        #endregion
        #region PriorityList
        private List<string> _PriorityList;
        public List<string> PriorityList
        {
            get { return _PriorityList; }
            set
            {
                _PriorityList = value;
                RaisePropertyChanged("PriorityList");
            }
        }
        #endregion


        #region SendToEmailAdresses
        private bool _SendToEmailAdresses;
        public bool SendToEmailAdresses
        {
            get { return _SendToEmailAdresses; }
            set
            {
                _SendToEmailAdresses = value;
                RaisePropertyChanged("SendToEmailAdresses");
                RaisePropertyChanged("FilterEnabled");
            }
        }
        #endregion
        #region SendToContact
        private bool _SendToContact;
        public bool SendToContact
        {
            get { return _SendToContact; }
            set
            {
                _SendToContact = value;
                RaisePropertyChanged("SendToContact");
                RaisePropertyChanged("FilterEnabled");
            }
        }
        #endregion
        #region SendToCategory
        private bool _SendToCategory;
        public bool SendToCategory
        {
            get { return _SendToCategory; }
            set
            {
                _SendToCategory = value;
                RaisePropertyChanged("SendToCategory");
                RaisePropertyChanged("FilterEnabled");
            }
        }
        #endregion
        #region SendToBasket
        private bool _SendToBasket;
        public bool SendToBasket
        {
            get { return _SendToBasket; }
            set
            {
                _SendToBasket = value;
                RaisePropertyChanged("SendToBasket");
                RaisePropertyChanged("FilterEnabled");
            }
        }
        #endregion
        #region SendToFile
        private bool _SendToFile;
        public bool SendToFile
        {
            get { return _SendToFile; }
            set
            {
                _SendToFile = value;
                RaisePropertyChanged("SendToFile");
                RaisePropertyChanged("FilterEnabled");
            }
        }
        #endregion
        #region SendToAll
        private bool _SendToAll;
        public bool SendToAll
        {
            get { return _SendToAll; }
            set
            {
                _SendToAll = value;
                RaisePropertyChanged("SendToAll");
                RaisePropertyChanged("FilterEnabled");
            }
        }
        #endregion
        #region FilterByEmailTitle
        private bool _FilterByEmailTitle;
        public bool FilterByEmailTitle
        {
            get { return _FilterByEmailTitle; }
            set
            {
                _FilterByEmailTitle = value;
                RaisePropertyChanged("FilterByEmailTitle");
            }
        }
        #endregion
        #region FilterEnabled
        public bool FilterEnabled
        {
            get { return SendToBasket || SendToCategory; }
        }
        #endregion


        #region EmailSubject
        private string _EmailSubject;
        public string EmailSubject
        {
            get { return _EmailSubject; }
            set
            {
                _EmailSubject = value;
                RaisePropertyChanged("EmailSubject");
            }
        }
        #endregion
        #region EmailAddresses
        private string _EmailAddresses;
        public string EmailAddresses
        {
            get { return _EmailAddresses; }
            set
            {
                _EmailAddresses = value;
                RaisePropertyChanged("EmailAddresses");
            }
        }
        #endregion
        #region SelectedContact
        private DBCTContact _SelectedContact;
        public DBCTContact SelectedContact
        {
            get { return _SelectedContact; }
            set
            {
                _SelectedContact = value;
                RaisePropertyChanged("SelectedContact");
            }
        }
        #endregion
        #region SelectedFile
        private string _SelectedFile;
        public string SelectedFile
        {
            get { return _SelectedFile; }
            set
            {
                _SelectedFile = value;
                RaisePropertyChanged("SelectedFile");
            }
        }
        #endregion
        #region SelectedCategory
        private DBCTContactCat _SelectedCategory;
        public DBCTContactCat SelectedCategory
        {
            get { return _SelectedCategory; }
            set
            {
                _SelectedCategory = value;
                RaisePropertyChanged("SelectedCategory");
            }
        }
        #endregion
        #region SelectedBasket
        private DBCTContactSelection _SelectedBasket;
        public DBCTContactSelection SelectedBasket
        {
            get { return _SelectedBasket; }
            set
            {
                _SelectedBasket = value;
                RaisePropertyChanged("SelectedBasket");
            }
        }
        #endregion
        #region SelectedEmailTitle
        private string _SelectedEmailTitle;
        public string SelectedEmailTitle
        {
            get { return _SelectedEmailTitle; }
            set
            {
                _SelectedEmailTitle = value;
                RaisePropertyChanged("SelectedEmailTitle");
            }
        }
        #endregion
        #region SelectedPriority
        private int _SelectedPriority;
        public int SelectedPriority
        {
            get { return _SelectedPriority; }
            set
            {
                _SelectedPriority = value;
                RaisePropertyChanged("SelectedPriority");
            }
        }
        #endregion
        #region SelectedTemplate
        private DBEMTemplate _SelectedTemplate;
        public DBEMTemplate SelectedTemplate
        {
            get { return _SelectedTemplate; }
            set
            {
                SaveCurrentParamValueToCach();
                _SelectedTemplate = value;
                RaisePropertyChanged("SelectedTemplate");
                PopulateParameterList();
                if (string.IsNullOrEmpty(EmailSubject) && _SelectedTemplate != null)
                    EmailSubject = _SelectedTemplate.Title;
            }
        }
        #endregion

        #region AttachmentCount
        private int _AttachmentCount;
        public int AttachmentCount
        {
            get { return _AttachmentCount; }
            set
            {
                _AttachmentCount = value;
                RaisePropertyChanged("AttachmentCount");
            }
        }
        #endregion

        #region AttachmentSize
        private string _AttachmentSize;
        public string AttachmentSize
        {
            get { return _AttachmentSize; }
            set
            {
                _AttachmentSize = value;
                RaisePropertyChanged("AttachmentSize");
            }
        }
        #endregion

        #region SendCount
        private int _SendCount;
        public int SendCount
        {
            get { return _SendCount; }
            set
            {
                _SendCount = value;
                RaisePropertyChanged("SendCount");
            }
        }
        #endregion


        #region ParamList
        public ObservableCollection<ParameterHolder> ParamList { get; set; }
        #endregion

        #region AttachedFileList
        public ObservableCollection<AttachedFileHolder> AttachedFileList { get; set; }
        #endregion
        #region AttachedFileSelected
        private AttachedFileHolder _AttachedFileSelected;
        public AttachedFileHolder AttachedFileSelected
        {
            get { return _AttachedFileSelected; }
            set
            {
                if (value == _AttachedFileSelected)
                    return;

                _AttachedFileSelected = value;
                RaisePropertyChanged("AttachedFileSelected");
            }
        }
        #endregion

        #region AttachedFieldList
        public ObservableCollection<DBCTProfileItem> AttachedFieldList { get; set; }
        #endregion
        #region AttachedFieldSelected
        private DBCTProfileItem _AttachedFieldSelected;
        public DBCTProfileItem AttachedFieldSelected
        {
            get { return _AttachedFieldSelected; }
            set
            {
                if (value == _AttachedFieldSelected)
                    return;

                _AttachedFieldSelected = value;
                RaisePropertyChanged("AttachedFieldSelected");
            }
        }
        #endregion


        #region TemplateList
        public XPCollection<DBEMTemplate> TemplateList { get; set; }
        #endregion

        #region PreviewData
        public string PreviewData
        {
            get
            {
                var rv = string.Format("{0}{1}{2}", SelectedEmailApp.ContentSendHeader, GetTemplateData(), SelectedEmailApp.ContentSendFooter);

                if (SelectedEmailApp != null)
                {
                    try
                    {
                        if (SelectedEmailApp.EmbedImage1 != null)
                        {
                            const string id = "[IMG01]";
                            var location = rv.IndexOf(id, System.StringComparison.Ordinal);
                            if (location >= 0)
                            {
                                var rep = string.Format("data:{0};base64,{1}", "image/png", Convert.ToBase64String(SelectedEmailApp.EmbedImage1));
                                rv = rv.Replace(id, rep);
                            }
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (SelectedEmailApp.EmbedImage2 != null)
                        {
                            const string id = "[IMG02]";
                            var location = rv.IndexOf(id, System.StringComparison.Ordinal);
                            if (location >= 0)
                            {
                                var rep = string.Format("data:{0};base64,{1}", "image/png", Convert.ToBase64String(SelectedEmailApp.EmbedImage2));
                                rv = rv.Replace(id, rep);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                return rv;
            }
        }
        #endregion


        #region SendDate
        private DateTime? _SendDate;
        public DateTime? SendDate
        {
            get { return _SendDate; }
            set
            {
                if (value == _SendDate)
                    return;

                _SendDate = value;
                RaisePropertyChanged("SendDate");
            }
        }
        #endregion

        #region SendTime
        private string _SendTime;
        public string SendTime
        {
            get { return _SendTime; }
            set
            {
                if (value == _SendTime)
                    return;

                _SendTime = value;
                RaisePropertyChanged("SendTime");
            }
        }
        #endregion





        #region [METHODS]

        private void InitDynamicData()
        {
            DynamicOwner = (Window)MainView;
            DynamicFrom = MainView.DynamicFrom;

            DynamicInbox = MainView.DynamicInbox;
            DynamicEmailAddresses = MainView.DynamicEmailAddresses;
            DynamicContact = MainView.DynamicContact;
            DynamicCategory = MainView.DynamicCategory;
            DynamicBasket = MainView.DynamicBasket;

            DynamicRichTextBox = MainView.DynamicRichTextBox;
            DynamicOwner.Closing += (s, e) => SaveCurrentParamValueToCach();
        }
        private void PopulateCategoryList()
        {
            CategoryList = (from n in ACacheData.GetContactCatList() orderby n.Title select (DBCTContactCat)n.Tag).ToList();
        }
        private void PopulateBasketList()
        {
            BasketList = DBCTContactSelection.GetByUser(ADatabase.Dxs, AMembership.ActiveUser.UserID, false, null);
            BasketList.Sorting = new SortingCollection(new SortProperty("Title", SortingDirection.Ascending));
        }
        private void PopulateEmailTitleList()
        {
            EmailTitleList = (from n in DBBTEmailTitle2.GetAll(ADatabase.Dxs) select n.Title).ToList();
        }
        private void PopulatePriorityList()
        {
            PriorityList = new List<string> { "معمولی", "بالا", "پایین" };
        }
        private void PopulateParameterList()
        {
            ParamList = new ObservableCollection<ParameterHolder>();
            RaisePropertyChanged("ParamList");
            if (SelectedTemplate == null) return;
            var q = from n in SelectedTemplate.Parameters
                    where n.TagType != EnumEmailTemplateTagType.ProfileItem
                    orderby n.Tag
                    select n;
            if (!q.Any()) return;
            q.ToList().ForEach(
                p => ParamList.Add(
                    new ParameterHolder
                    {
                        TagTitle = p.Tag,
                        IsMultiLine = p.TagType == EnumEmailTemplateTagType.Memo,
                        HasScrollBar = p.TagType == EnumEmailTemplateTagType.Memo ? ScrollBarVisibility.Visible : ScrollBarVisibility.Disabled,
                        Height = p.TagType == EnumEmailTemplateTagType.Memo ? 120 : double.NaN,
                    }));

            LoadCurrentParamValueFromCach();
        }
        private void PopulateData()
        {
            AttachedFileList = new ObservableCollection<AttachedFileHolder>();
            AttachedFieldList = new ObservableCollection<DBCTProfileItem>();
            AttachmentCount = 0;
            AttachmentSize = "0";

            SendDate = DateTime.Now.Date;
            var d = DateTime.Now;
            SendTime = string.Format("{0}:{1}", d.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    d.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));

            EmailAppList = DBEMEmailApp.GetAll(ADatabase.Dxs).ToList();
            if (DynamicFrom != null)
                SelectedEmailApp = EmailAppList.FirstOrDefault(n => n.Oid == DynamicFrom.Oid);
            if (SelectedEmailApp == null) SelectedEmailApp = EmailAppList.First();

            if (DynamicBasket != null)
            {
                SendToBasket = true;
                SelectedBasket = DynamicBasket;
            }
            if (DynamicCategory != null)
            {
                SendToCategory = true;
                SelectedCategory = DynamicCategory;
            }
            if (DynamicContact != null)
            {
                SendToContact = true;
                SelectedContact = DynamicContact;
            }
            if (DynamicEmailAddresses != null)
            {
                SendToEmailAdresses = true;
                EmailAddresses = DynamicEmailAddresses;
            }
            if (DynamicInbox != null)
            {
                EmailSubject = "پاسخ به : " + DynamicInbox.Subject;
                EmailAddresses = DynamicInbox.FromAddress;
                SendToEmailAdresses = true;
            }

            SelectedPriority = 0;
        }
        private void PopulateTemplateList()
        {
            TemplateList = DBEMTemplate.GetAll(ADatabase.Dxs);
            RaisePropertyChanged("TemplateList");
        }
        private void InitCommands()
        {
            CommandSend = new RelayCommand(Send, () => true);
            CommandSelectContact = new RelayCommand(SelectContact, () => true);
            CommandSelectFile = new RelayCommand(SelectFile, () => true);
            CommandCalculateSendCount = new RelayCommand(CalculateSendCount, () => true);

            CommandAttacheFileAdd = new RelayCommand(AttacheFileAdd, () => true);
            CommandAttacheFileDelete = new RelayCommand(AttacheFileDelete, () => AttachedFileSelected != null);
            CommandAttacheFieldAdd = new RelayCommand(AttachedFieldAdd, () => true);
            CommandAttacheFieldDelete = new RelayCommand(AttachedFieldDelete, () => true);

            CommandTemplateAdd = new RelayCommand(TemplateAdd, () => true);
            CommandTemplateEdit = new RelayCommand(TemplateEdit, () => SelectedTemplate != null);
            CommandTemplateDelete = new RelayCommand(TemplateDelete, () => SelectedTemplate != null);
            CommandTemplateExport = new RelayCommand(TemplateExport, () => SelectedTemplate != null);
            CommandTemplateImport = new RelayCommand(TemplateImport, () => true);

            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp41 != "");
        }

        private void AttacheFileAdd()
        {
            var sf = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Any File (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true,
            };
            if (sf.ShowDialog() != true) return;
            var fi = new FileInfo(sf.FileName);
            AttachedFileList.Add(new AttachedFileHolder
                                     {
                                         FileName = fi.Name,
                                         FileLen = fi.Length,
                                         FilePath = fi.FullName,
                                     });
        }
        private void AttacheFileDelete()
        {
            var index = AttachedFileList.IndexOf(AttachedFileSelected);
            AttachedFileList.Remove(AttachedFileSelected);
            try
            {
                AttachedFileSelected = AttachedFileList[index];
            }
            catch
            {
                AttachedFileSelected = AttachedFileList.FirstOrDefault();
            }
        }
        private void AttachedFieldAdd()
        {
            var dt = APOCMainWindow.ShowSelectProfileItem(DynamicOwner, EnumProfileItemType.File);
            var pi = dt as DBCTProfileItem;
            if (pi != null)
            {
                if (!AttachedFieldList.Contains(pi))
                    AttachedFieldList.Add(pi);
            }
        }
        private void AttachedFieldDelete()
        {
            var index = AttachedFieldList.IndexOf(AttachedFieldSelected);
            AttachedFieldList.Remove(AttachedFieldSelected);
            try
            {
                AttachedFileSelected = AttachedFileList[index];
            }
            catch
            {
                AttachedFieldSelected = AttachedFieldList.FirstOrDefault();
            }
        }



        private void TemplateAdd()
        {
            SaveCurrentParamValueToCach();
            var w = new WTemplateAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            PopulateTemplateList();
            RaisePropertyChanged("TemplateList");
            PopulateParameterList();
        }
        private void TemplateEdit()
        {
            SaveCurrentParamValueToCach();
            if (SelectedTemplate == null) return;
            var w = new WTemplateAddEdit(SelectedTemplate) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            PopulateTemplateList();
            RaisePropertyChanged("TemplateList");
            RaisePropertyChanged("SelectedTemplate");
            PopulateParameterList();
        }
        private void TemplateDelete()
        {
            if (SelectedTemplate == null) return;
            var dr = POLMessageBox.ShowQuestionYesNo("قالب انتخاب شده حذف شود؟");
            if (dr != MessageBoxResult.Yes) return;
            var t = DBEMTemplate.FindByOid(ADatabase.Dxs, SelectedTemplate.Oid);
            t.Delete();
            t.Save();
            DeleteTemplateCach(t);
            PopulateTemplateList();
            SelectedTemplate = null;
        }

        private string GetFileFromByteImage(byte[] img)
        {
            var tPath = Path.GetTempPath();
            var tFileName = Path.GetRandomFileName() + ".png";
            var fn = Path.Combine(tPath, tFileName);
            using (var f = new FileStream(fn, FileMode.Create))
            {
                var bs = HelperImage.ConvertImageByteToBitmapImage(img);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bs));
                encoder.Save(f);
            }
            return fn;
        }
        private void Send()
        {
            if (Validate())
            {
                var allEmailAddress = CollectAllEmailAddress();
                var attCount = 0;
                var emailCount = 0;
                var failedCount = 0;
                POLProgressBox.Show("ارسال ایمیل", true, 0, 0, 3,
                                    pb =>
                                    {
                                        var listFileAtt = new List<DBEMEmailAttachment>();

                                        using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                                        {
                                            pb.AsyncSetText(1, "ارسال :");
                                            pb.AsyncSetMax(allEmailAddress.Count);
                                            var app = DBEMEmailApp.FindByOid(uow, SelectedEmailApp.Oid);
                                            var folder = app.Folders.First(n => n.FolderType == EnumEmailFolderType.WaitForSend);
                                            var ss = SendTime.Split(':');
                                            var hour = Convert.ToInt32(ss[0]);
                                            var min = Convert.ToInt32(ss[1]);
                                            var senddate = new DateTime(SendDate.Value.Year, SendDate.Value.Month, SendDate.Value.Day, hour, min, 0);

                                            var i = 0;
                                            try
                                            {
                                                foreach (var email in allEmailAddress)
                                                {
                                                    if (pb.NeedToCancel)
                                                        return;
                                                    i++;
                                                    pb.AsyncSetText(2, email);
                                                    var dbe = DBCTEmail.FindByAddressExcept(uow, null, email);
                                                    var dbc = dbe == null ? null : dbe.Contact;

                                                    var body = GenerateBody(uow, dbc);
                                                    LinkedResource img1 = null;
                                                    if (app.EmbedImage1 != null)
                                                    {
                                                        var cid = Guid.NewGuid().ToString();
                                                        body = body.Replace("[IMG01]", string.Format("cid:{0}", cid));
                                                        var fn = GetFileFromByteImage(app.EmbedImage1);
                                                        img1 = new System.Net.Mail.LinkedResource(fn)
                                                        {
                                                            ContentId = cid,
                                                            TransferEncoding = System.Net.Mime.TransferEncoding.Base64,
                                                            ContentType = new System.Net.Mime.ContentType("image/png"),
                                                        };
                                                        HelperUtils.Try(() => File.Delete(fn));
                                                    }
                                                    LinkedResource img2 = null;
                                                    if (app.EmbedImage2 != null)
                                                    {
                                                        var cid = Guid.NewGuid().ToString();
                                                        body = body.Replace("[IMG02]", string.Format("cid:{0}", cid));
                                                        var fn = GetFileFromByteImage(app.EmbedImage2);
                                                        img2 = new LinkedResource(fn)
                                                        {
                                                            ContentId = cid,
                                                            TransferEncoding = System.Net.Mime.TransferEncoding.Base64,
                                                            ContentType = new System.Net.Mime.ContentType("image/png"),
                                                        };
                                                        HelperUtils.Try(() => File.Delete(fn));
                                                    }
                                                    var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                                                    view.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
                                                    if (img1 != null)
                                                        view.LinkedResources.Add(img1);
                                                    if (img2 != null)
                                                        view.LinkedResources.Add(img2);

                                                    var message = new MailMessage
                                                                      {
                                                                          IsBodyHtml = true,
                                                                          Subject = EmailSubject,
                                                                          BodyEncoding = new UTF8Encoding(),
                                                                          From = new MailAddress(app.Email, app.Title),
                                                                          Priority = GetPriority2(),
                                                                          SubjectEncoding = new UTF8Encoding(),
                                                                      };
                                                    if (dbc != null)
                                                        message.Subject = message.Subject.Replace("_ctitle_", dbc.Title);
                                                    message.To.Add(new MailAddress(email, dbe == null ? string.Empty : dbe.Title));
                                                    message.AlternateViews.Add(view);

                                                    foreach (var att in AttachedFileList)
                                                    {
                                                        var dba = new DBEMEmailAttachment(uow)
                                                                      {
                                                                          Oid = Guid.NewGuid(),
                                                                          FileName = att.FileName,
                                                                          Size = att.FileLen
                                                                      };
                                                        dba.Save();

                                                        attCount++;
                                                        var bb = System.IO.File.ReadAllBytes(att.FilePath);
                                                        var ms = new MemoryStream(bb);
                                                        var attachment = new Attachment(ms, att.FileName);
                                                        attachment.ContentDisposition.FileName = att.FileName;
                                                        attachment.ContentDisposition.Size = bb.Length;
                                                        message.Attachments.Add(attachment);

                                                        listFileAtt.Add(dba);

                                                    }
                                                    foreach (var att in AttachedFieldList)
                                                    {

                                                        if (dbe == null) continue;
                                                        if (dbc == null) continue;
                                                        var dbi = DBCTProfileValue.FindByContactAndItem(uow, dbc.Oid, att.Oid);
                                                        if (dbi == null) continue;
                                                        var dbb = DBCTBytes.FindByOid(uow, dbi.Guid1);
                                                        if (dbb == null) continue;
                                                        var dba = new DBEMEmailAttachment(uow);
                                                        dba.FileName = dbi.String1;
                                                        dba.Size = dbb.DataByte.Length;
                                                        dba.Save();

                                                        attCount++;
                                                        var ms = new MemoryStream(dbb.DataByte);

                                                        var attachment = new Attachment(ms, dba.FileName);
                                                        attachment.ContentDisposition.FileName = dba.FileName;
                                                        attachment.ContentDisposition.Size = dbb.DataByte.Length;
                                                        message.Attachments.Add(attachment);

                                                        listFileAtt.Add(dba);
                                                    }



                                                    var ddddd = message.SaveToString();
                                                    var builder = new MailBuilder();
                                                    var mm = builder.CreateFromEml(ddddd);


                                                    var dbs = new DBEMEmailInbox(uow);
                                                    dbs.AttachmentCount = listFileAtt.Count;
                                                    dbs.Attachments.AddRange(listFileAtt);
                                                    dbs.Contact = dbc;

                                                    dbs.Body = ddddd;
                                                    dbs.Header = string.Empty;


                                                    dbs.BodyCache = HelperEmail.RenderInlines(mm.Attachments, body);

                                                    dbs.Date = senddate;
                                                    dbs.FromAddress = email;
                                                    dbs.FromName = dbe == null ? string.Empty : dbe.Title;
                                                    dbs.ParentFolder = folder;
                                                    dbs.Priority = GetPriority();
                                                    dbs.SendByUser = AMembership.ActiveUser.UserName;
                                                    dbs.Size = dbs.Body.Length;
                                                    dbs.Subject = EmailSubject;
                                                    if (dbc != null)
                                                        dbs.Subject = dbs.Subject.Replace("_ctitle_", dbc.Title);
                                                    dbs.UIDL = string.Empty;
                                                    dbs.Save();


                                                    uow.CommitChanges();

                                                    emailCount++;
                                                }
                                            }
                                            catch
                                            {
                                                failedCount++;
                                            }

                                        }
                                        uowSelectedTemplate = null;
                                    },
                                    pb =>
                                    {
                                        if (pb.NeedToCancel)
                                            POLMessageBox.ShowWarning("عملیات متوقف شد", DynamicOwner);
                                        if (attCount > 0 || emailCount > 0 || failedCount > 0)
                                        {
                                            POLMessageBox.ShowInformation(
                                                string.Format(
                                                    "تعداد ضمائم ثبت شده : {0}{1}تعداد ایمیل ثبت شده : {2}{1} خطا : {3}",
                                                    attCount, Environment.NewLine, emailCount, failedCount), DynamicOwner);
                                        }

                                        MainView.Close();
                                    },
                                    DynamicOwner);
            }
        }

        private MailPriority GetPriority2()
        {
            if (SelectedPriority == 1)
                return MailPriority.High;
            if (SelectedPriority == 2)
                return MailPriority.Low;
            return MailPriority.Normal;
        }
        private EnumEmailPriority GetPriority()
        {
            if (SelectedPriority == 1)
                return EnumEmailPriority.Highest;
            if (SelectedPriority == 2)
                return EnumEmailPriority.Lowest;
            return EnumEmailPriority.Normal;
        }

        private DBEMTemplate uowSelectedTemplate;
        private List<DBCTProfileItem> uowProfileItemList;
        private string GenerateBody(UnitOfWork uow, DBCTContact dbc)
        {
            if (uowSelectedTemplate == null)
            {
                uowSelectedTemplate = DBEMTemplate.FindByOid(uow, SelectedTemplate.Oid);
            }

            var rv = uowSelectedTemplate.HTMLBody;
            if (dbc != null)
                rv = rv.Replace("_ctitle_", dbc.Title);

            var q = from n in uowSelectedTemplate.Parameters
                    where n.TagType != EnumEmailTemplateTagType.ProfileItem
                    select n;
            if (!q.Any())
                return string.Format("{0}{1}{2}", SelectedEmailApp.ContentSendHeader, rv, SelectedEmailApp.ContentSendFooter);
            q.ToList().ForEach(
                p =>
                {
                    var v = ParamList.Where(i => i.TagTitle == p.Tag).Select(i => i.Data).FirstOrDefault();
                    if (rv.Contains(p.Tag))
                        rv = rv.Replace(p.Tag, v);
                });

            var q2 = from n in uowSelectedTemplate.Parameters
                     where n.TagType == EnumEmailTemplateTagType.ProfileItem
                     select n;
            q2.ToList().ForEach(
                p =>
                {
                    if (dbc == null)
                    {
                        if (rv.Contains(p.Tag))
                            rv = rv.Replace(p.Tag, string.Empty);
                    }
                    else
                    {
                        if (uowProfileItemList == null)
                            uowProfileItemList = new List<DBCTProfileItem>();
                        var dbpi = (from n in uowProfileItemList where n.Oid == p.ProfileItem.Oid select n).FirstOrDefault();
                        if (dbpi == null)
                        {
                            dbpi = DBCTProfileItem.FindByOid(uow, p.ProfileItem.Oid);
                            uowProfileItemList.Add(dbpi);
                        }

                        var dbv = DBCTProfileValue.FindByContactAndItem(uow, dbc.Oid, dbpi.Oid);
                        if (dbv == null)
                        {
                            if (rv.Contains(p.Tag))
                                rv = rv.Replace(p.Tag, string.Empty);
                        }
                        else
                        {
                            var df = ADataFieldManager.FindByType(dbpi.ItemType);
                            var ed = df.GetEmailData(dbv, dbpi, new DataFieldSettings());
                            rv = rv.Replace(p.Tag, ed);
                        }
                    }
                });
            return string.Format("{0}{1}{2}", SelectedEmailApp.ContentSendHeader, rv, SelectedEmailApp.ContentSendFooter);
        }

        private List<string> CollectAllEmailAddress()
        {
            var rv = new List<string>();
            if (SendToEmailAdresses)
            {
                HelperUtils.Try(
                    () =>
                    {
                        var ss = EmailAddresses.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        rv = ss.ToList();
                    });
                return rv;
            }
            if (SendToContact)
            {
                HelperUtils.Try(
                    () =>
                    {
                        rv = SelectedContact == null ? new List<string>() : SelectedContact.Emails.Select(n => n.AddressLower).ToList();
                    });
                return rv;
            }
            if (SendToCategory)
            {
                HelperUtils.Try(
                    () =>
                    {
                        if (SelectedCategory == null) return;
                        var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);
                        mainSearchCriteria.Operands.Add(new ContainsOperator("Contact.Categories", new BinaryOperator("Oid", SelectedCategory.Oid)));
                        var xpq = new XPQuery<DBCTEmail>(ADatabase.Dxs);
                        var xpq2 = xpq.AppendWhere(new CriteriaToExpressionConverter(), mainSearchCriteria) as XPQuery<DBCTEmail>;
                        if (xpq2 == null) return;
                        foreach (var dbe in xpq2)
                        {
                            if (FilterByEmailTitle)
                            {
                                if (dbe.Title == SelectedEmailTitle)
                                    rv.Add(dbe.AddressLower);
                            }
                            else
                                rv.Add(dbe.AddressLower);
                        }
                    });
                return rv;
            }
            if (SendToBasket)
            {
                HelperUtils.Try(
                    () =>
                    {
                        if (SelectedBasket == null) return;
                        var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);
                        mainSearchCriteria.Operands.Add(new ContainsOperator("Contact.Selections", new BinaryOperator("Oid", SelectedBasket.Oid)));
                        var xpq = new XPQuery<DBCTEmail>(ADatabase.Dxs);
                        var xpq2 = xpq.AppendWhere(new CriteriaToExpressionConverter(), mainSearchCriteria) as XPQuery<DBCTEmail>;
                        if (xpq2 == null) return;
                        foreach (var dbe in xpq2)
                        {
                            if (FilterByEmailTitle)
                            {
                                if (dbe.Title == SelectedEmailTitle)
                                    rv.Add(dbe.AddressLower);
                            }
                            else
                                rv.Add(dbe.AddressLower);
                        }
                    });
                return rv;
            }
            if (SendToFile)
            {
                HelperUtils.Try(
                    () =>
                    {
                        var list = File.ReadAllText(SelectedFile).Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        rv = list.Where(to => !to.StartsWith(";")).Distinct().ToList();
                    });
                return rv;
            }
            if (SendToAll)
            {
                HelperUtils.Try(
                    () =>
                    {
                        var xpq2 = new XPQuery<DBCTEmail>(ADatabase.Dxs);
                        foreach (var dbe in xpq2)
                        {
                            if (FilterByEmailTitle)
                            {
                                if (dbe.Title == SelectedEmailTitle)
                                    rv.Add(dbe.AddressLower);
                            }
                            else
                                rv.Add(dbe.AddressLower);
                        }
                    });
                return rv;
            }
            return rv;
        }
        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(EmailSubject))
            {
                POLMessageBox.ShowWarning("لطفا موضوع را وارد كنید.", DynamicOwner);
                return false;
            }
            if (!(SendToEmailAdresses || SendToContact || SendToCategory || SendToBasket || SendToAll))
            {
                POLMessageBox.ShowWarning("لطفا مقصد را انتخاب كنید.", DynamicOwner);
                return false;
            }
            if (SendToEmailAdresses && string.IsNullOrWhiteSpace(EmailAddresses))
            {
                POLMessageBox.ShowWarning("لطفا آدرس ایمیل را وارد كنید.", DynamicOwner);
                return false;
            }
            if (SendToContact && SelectedContact == null)
            {
                POLMessageBox.ShowWarning("لطفا پرونده را انتخاب كنید.", DynamicOwner);
                return false;
            }
            if (SendToCategory && SelectedCategory == null)
            {
                POLMessageBox.ShowWarning("لطفا دسته را انتخاب كنید.", DynamicOwner);
                return false;
            }
            if (SendToBasket && SelectedBasket == null)
            {
                POLMessageBox.ShowWarning("لطفا سبد انتخاب را تعیین كنید.", DynamicOwner);
                return false;
            }

            CalculateSendCount();
            if (SendCount == 0)
            {
                POLMessageBox.ShowWarning("هیچ مقصدی یافت نشد.", DynamicOwner);
                return false;
            }
            if (SelectedTemplate == null)
            {
                POLMessageBox.ShowWarning("هیچ قالبی برای ارسال انتخاب نشد است.", DynamicOwner);
                return false;
            }

            long limitsize = SelectedEmailApp.LimitSizePerEmail * 1024;
            long emailsize = PreviewData.Length + AttachedFileList.ToList().Sum(f => f.FileLen);
            if (limitsize != 0 && limitsize < emailsize)
            {
                POLMessageBox.ShowWarning(string.Format("خطا : حجم ایمیل به انظمام ضمائم بیش از حد مجاز می باشد.{0}حجم ایمیل : {1}{0}حد مجاز : {2}", Environment.NewLine, emailsize, limitsize)
                    , DynamicOwner);
                return false;
            }

            if (!SendDate.HasValue)
            {
                POLMessageBox.ShowWarning("لطفا تاریخ ارسال را مشخص كنید.", DynamicOwner);
                return false;
            }
            return true;
        }

        private void SelectContact()
        {
            var poc = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var ct = poc.ShowSelectEmail(DynamicOwner, null);
            if (ct is DBCTEmail)
            {
                SelectedContact = ((DBCTEmail)ct).Contact;
                SelectedEmailTitle = ((DBCTEmail)ct).Title;
            }
        }
        private void SelectFile()
        {
            var sf = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "Text (*.txt)|*.txt",
                FilterIndex = 0,
                RestoreDirectory = true,
            };
            if (sf.ShowDialog() != true) return;
            SelectedFile = sf.FileName;
        }
        private void CalculateSendCount()
        {
            SendCount = 0;
            if (SendToEmailAdresses)
            {
                HelperUtils.Try(
                    () =>
                    {
                        var ss = EmailAddresses.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        SendCount = ss.Length;
                    });
            }
            if (SendToContact)
            {
                HelperUtils.Try(
                    () =>
                    {
                        SendCount = SelectedContact.Emails.Count;
                    });
            }
            if (SendToCategory)
            {
                HelperUtils.Try(
                    () =>
                    {
                        if (SelectedCategory == null) return;
                        var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);
                        mainSearchCriteria.Operands.Add(new ContainsOperator("Contact.Categories", new BinaryOperator("Oid", SelectedCategory.Oid)));
                        var xpq = new XPQuery<DBCTEmail>(ADatabase.Dxs);
                        var xpq2 = xpq.AppendWhere(new CriteriaToExpressionConverter(), mainSearchCriteria) as XPQuery<DBCTEmail>;
                        SendCount = FilterByEmailTitle ? xpq2.Count(n => n.Title == SelectedEmailTitle) : xpq2.Count();
                    });
            }
            if (SendToBasket)
            {
                HelperUtils.Try(
                    () =>
                    {
                        if (SelectedBasket == null) return;
                        var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);
                        mainSearchCriteria.Operands.Add(new ContainsOperator("Contact.Selections", new BinaryOperator("Oid", SelectedBasket.Oid)));
                        var xpq = new XPQuery<DBCTEmail>(ADatabase.Dxs);
                        var xpq2 = xpq.AppendWhere(new CriteriaToExpressionConverter(), mainSearchCriteria) as XPQuery<DBCTEmail>;
                        SendCount = FilterByEmailTitle ? xpq2.Count(n => n.Title == SelectedEmailTitle) : xpq2.Count();
                    });
            }
            if (SendToFile)
            {
                var count = 0;
                POLProgressBox.Show("پردازش اطلاعات", false, 0, 0, 1,
                    pb =>
                    {
                        try
                        {
                            var list = File.ReadAllText(SelectedFile).Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                            count = list.Where(to => !to.StartsWith(";")).Distinct().Count();
                        }
                        catch
                        {
                        }
                    },
                    pb =>
                    {
                        SendCount = count;
                    },
                    DynamicOwner);
            }
            if (SendToAll)
            {
                HelperUtils.Try(
                    () =>
                    {
                        var xpq = new XPQuery<DBCTEmail>(ADatabase.Dxs);
                        SendCount = FilterByEmailTitle ? xpq.Count(n => n.Title == SelectedEmailTitle) : xpq.Count();
                    });
            }
        }

        private void SaveCurrentParamValueToCach()
        {
            if (SelectedTemplate == null) return;
            if (ParamList.Count == 0) return;
            ParamList.ToList().ForEach(
                p =>
                {
                    var pval = DBEMTempParamsValue.FindByOidTemplateAndTag(ADatabase.LocalSession, SelectedTemplate.Oid, p.TagTitle);
                    if (pval == null)
                    {
                        pval = new DBEMTempParamsValue(ADatabase.LocalSession)
                                   {
                                       TemplateOid = SelectedTemplate.Oid,
                                       Tag = p.TagTitle,
                                       StringValue = p.Data
                                   };
                    }
                    else
                    {
                        pval.StringValue = p.Data;
                    }
                    pval.Save();
                });
        }
        private void LoadCurrentParamValueFromCach()
        {
            if (SelectedTemplate == null) return;
            if (ParamList.Count == 0) return;
            for (var i = 0; i < ParamList.Count; i++)
            {
                var p = ParamList[i];
                var pval = DBEMTempParamsValue.FindByOidTemplateAndTag(ADatabase.LocalSession, SelectedTemplate.Oid, p.TagTitle);
                if (pval != null)
                    p.Data = pval.StringValue;
            }
        }
        private void DeleteTemplateCach(DBEMTemplate temp)
        {
            if (temp == null) return;
            var xpc = DBEMTempParamsValue.GetByTemplate(ADatabase.LocalSession, temp.Oid);
            xpc.ToList().ForEach(
                p =>
                {
                    p.Delete();
                });
        }

        private string GetTemplateData()
        {
            if (SelectedTemplate == null) return string.Empty;
            var rv = SelectedTemplate.HTMLBody;
            var q = from n in SelectedTemplate.Parameters
                    where n.TagType != EnumEmailTemplateTagType.ProfileItem
                    select n;
            if (!q.Any()) return rv;
            q.ToList().ForEach(
                p =>
                {
                    var v = ParamList.Where(i => i.TagTitle == p.Tag).Select(i => i.Data).FirstOrDefault();
                    if (rv.Contains(p.Tag))
                        rv = rv.Replace(p.Tag, v);
                });
            return rv;
        }
        private void TemplateExport()
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "Template.xml"
            };
            if (sf.ShowDialog() != true) return;

            try
            {
                var io = new EmailTemplateIO()
                {
                    Titles = SelectedTemplate.Title,
                    HTMLBody = SelectedTemplate.HTMLBody
                };
                var pList = new List<EmailParameterIO>();
                foreach (var p in SelectedTemplate.Parameters)
                {
                    var pio = new EmailParameterIO
                                  {
                                      Tag = p.Tag,
                                      TagType = p.TagType,
                                  };
                    if (pio.TagType == EnumEmailTemplateTagType.ProfileItem && p.ProfileItem != null)
                    {
                        pio.ProfileItemTitle = p.ProfileItem.Title;
                        pio.ProfileGroupTitle = p.ProfileItem.ProfileGroup.Title;
                        pio.ProfileRootTitle = p.ProfileItem.ProfileGroup.ProfileRoot.Title;
                    }
                    pList.Add(pio);
                }
                io.Parameters = pList.ToArray();

                var serializer = new XmlSerializer(io.GetType());
                using (var f = new StreamWriter(sf.FileName))
                {
                    serializer.Serialize(f, io);
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", DynamicOwner);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }
        private void TemplateImport()
        {
            var sf = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
            };
            if (sf.ShowDialog() != true) return;
            try
            {
                var serializer = new XmlSerializer(typeof(EmailTemplateIO));
                using (var f = new StreamReader(sf.FileName))
                {
                    var io = serializer.Deserialize(f) as EmailTemplateIO;
                    if (io == null)
                        throw new Exception("محتوای فایل معتبر نمی باشد.");
                    var dbet = DBEMTemplate.FindDuplicateTitleExcept(ADatabase.Dxs, null, io.Titles);
                    if (dbet != null)
                    {
                        POLMessageBox.ShowInformation("خطا : عنوان قالب تكراری می باشد.", DynamicOwner);
                        return;
                    }
                    dbet = new DBEMTemplate(ADatabase.Dxs) { Title = io.Titles, HTMLBody = io.HTMLBody };
                    dbet.Save();

                    foreach (var p in io.Parameters)
                    {
                        var dbep = new DBEMTempParams(ADatabase.Dxs) { Tag = p.Tag, TagType = p.TagType, Template = dbet };
                        if (dbep.TagType == EnumEmailTemplateTagType.ProfileItem)
                        {
                            var pi = DBCTProfileItem.FindByAllTitles(ADatabase.Dxs, p.ProfileRootTitle,
                                                                     p.ProfileGroupTitle, p.ProfileItemTitle);
                            if (pi != null)
                            {
                                dbep.ProfileItem = pi;
                            }
                        }
                        dbep.Save();
                    }
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", DynamicOwner);
                PopulateTemplateList();
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp41);
        }
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandSend { get; set; }
        public RelayCommand CommandSelectContact { get; set; }
        public RelayCommand CommandSelectFile { get; set; }
        public RelayCommand CommandCalculateSendCount { get; set; }

        public RelayCommand CommandAttacheFileAdd { get; set; }
        public RelayCommand CommandAttacheFileDelete { get; set; }
        public RelayCommand CommandAttacheFieldAdd { get; set; }
        public RelayCommand CommandAttacheFieldDelete { get; set; }

        public RelayCommand CommandTemplateAdd { get; set; }
        public RelayCommand CommandTemplateEdit { get; set; }
        public RelayCommand CommandTemplateDelete { get; set; }
        public RelayCommand CommandTemplateExport { get; set; }
        public RelayCommand CommandTemplateImport { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
