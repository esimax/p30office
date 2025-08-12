using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using POL.DB.Membership;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Utils;
using POL.WPF.DXControls;

namespace POC.Module.Call.Models
{
    public class MCallSync : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IMembership AMembership { get; set; }
        private ICacheData ACacheData { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        private dynamic MainView { get; set; }
        public ListBoxEdit DynamicListBoxCat { get; set; }
        public ListBoxEdit DynamicListBoxUser { get; set; }
        private Window DynamicOwner { get; set; }
        private DBCLCall DynamicDBCall { get; set; }
        private DispatcherTimer ContactUpdateTimer { get; set; }

        public MCallSync(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            InitDynamics();
            InitCommands();
            PopulateContactCatList();
            PopulatePhoneTitle();
            PopulateUserList();
            UpdateSearch();
        }

        #region WindowTitle
        public string WindowTitle
        {
            get { return "تطبیق شماره با پرونده"; }
        }
        #endregion

        #region SelectedPhoneNumber
        public string SelectedPhoneNumber
        {
            get { return DynamicDBCall == null ? string.Empty : DynamicDBCall.FullPhoneString; }
        }
        #endregion

        #region SelectedTabIndex
        private int _SelectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                RaisePropertyChanged("SelectedTabIndex");
            }
        }
        #endregion

        #region ContactTitle
        private string _ContactTitle;
        public string ContactTitle
        {
            get { return _ContactTitle; }
            set
            {
                _ContactTitle = value;
                RaisePropertyChanged("ContactTitle");
            }
        }
        #endregion
        #region ContactCatList
        private List<DBCTContactCat> _ContactCatList;
        public List<DBCTContactCat> ContactCatList
        {
            get { return _ContactCatList; }
            set
            {
                _ContactCatList = value;
                RaisePropertyChanged("ContactCatList");
            }
        }
        #endregion

        #region PhoneTitleList
        private List<string> _PhoneTitleList;
        public List<string> PhoneTitleList
        {
            get { return _PhoneTitleList; }
            set
            {
                _PhoneTitleList = value;
                RaisePropertyChanged("PhoneTitleList");
            }
        }
        #endregion
        #region PhoneTitle
        private string _PhoneTitle;
        public string PhoneTitle
        {
            get { return _PhoneTitle; }
            set
            {
                if (ReferenceEquals(_PhoneTitle, value)) return;
                _PhoneTitle = value;
                RaisePropertyChanged("PhoneTitle");
            }
        }
        #endregion
        #region Note
        private string _Note;
        public string Note
        {
            get { return _Note; }
            set
            {
                if (ReferenceEquals(_Note, value)) return;
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }
        #endregion


        #region SearchText
        private string _SearchText;
        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                _SearchText = value;
                RaisePropertyChanged("SearchText");
                UpdateSearchWithDelay();
            }
        }
        #endregion SearchText
        #region ContactList
        private XPServerCollectionSource _ContactList;
        public XPServerCollectionSource ContactList
        {
            get { return _ContactList; }
            set
            {
                _ContactList = value;
                RaisePropertyChanged("ContactList");
            }
        }
        #endregion
        #region FocusedContact
        private DBCTContact _FocusedContact;
        public DBCTContact FocusedContact
        {
            get
            {
                return _FocusedContact;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedContact)) return;
                _FocusedContact = value;
                RaisePropertyChanged("FocusedContact");
            }
        }
        #endregion FocusedContact

        #region UserList
        private List<DBMSUser2> _UserList;
        public List<DBMSUser2> UserList
        {
            get { return _UserList; }
            set
            {
                _UserList = value;
                RaisePropertyChanged("UserList");
            }
        }
        #endregion


        public bool CanOK
        {
            get
            {
                if (_SelectedTabIndex == 0)
                    return !string.IsNullOrWhiteSpace(ContactTitle);
                if (_SelectedTabIndex == 1)
                {
                    return FocusedContact != null;
                }
                return false;
            }
        }

        #region [METHODS]

        private void InitDynamics()
        {
            DynamicOwner = MainView as Window;
            DynamicDBCall = MainView.DynamicDBCall;
            DynamicListBoxCat = MainView.DynamicListBoxCat;
            DynamicListBoxUser = MainView.DynamicListBoxUser;

            DynamicListBoxUser.IsEnabled = AMembership.HasPermission(PCOPermissions.Call_AllowSelectUserInSync);
        }
        private void PopulateContactCatList()
        {
            ContactCatList = (from n in ACacheData.GetContactCatList() orderby n.Title select (DBCTContactCat)n.Tag).ToList(); 
        }
        private void PopulateUserList()
        {
            if (AMembership.HasPermission(PCOPermissions.Call_AllowSelectUserInSync))
            {
                var v = DBMSUser2.UserGetAll(ADatabase.Dxs, null, true);
                UserList = (from u in v select u).ToList();
            }
            else UserList = null;
        }

        private void PopulatePhoneTitle()
        {
            PhoneTitleList = (from n in POL.DB.P30Office.BT.DBBTPhoneTitle2.GetAll(ADatabase.Dxs) select n.Title).ToList();
        }
        private void UpdateSearch()
        {
            var mainSearchCriteria = new GroupOperator(GroupOperatorType.Or);
            var code = -1;
            HelperUtils.Try(() => code = Convert.ToInt32(_SearchText));
            if (code > 0)
                mainSearchCriteria.Operands.Add(new BinaryOperator("Code", code));
            if (string.IsNullOrWhiteSpace(_SearchText)) _SearchText = string.Empty;
            var st = SearchText.Replace("*", "").Replace("%", "").Trim();
            if (!string.IsNullOrEmpty(st))
            {
                var txt = String.Format("%{0}%", st);
                HelperConvert.CorrectPersianBug(ref txt);
                mainSearchCriteria.Operands.Add(new BinaryOperator("Title", txt, BinaryOperatorType.Like));
            }
            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBCTContact)) { DisplayableProperties = "Oid;Code;Title" };
            xpi.FixedFilterCriteria = mainSearchCriteria;
            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.Dxs;
            };
            ContactList = null;
            ContactList = xpi;
        }
        private void UpdateSearchWithDelay()
        {
            if (ContactUpdateTimer == null)
            {
                ContactUpdateTimer = new DispatcherTimer();
                ContactUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
                ContactUpdateTimer.Tick += (s, e) =>
                {
                    ContactUpdateTimer.Stop();
                    UpdateSearch();
                };
            }
            ContactUpdateTimer.Stop();
            ContactUpdateTimer.Start();
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                        {
                            DynamicOwner.DialogResult = true;
                            DynamicOwner.Close();
                        }
                }, () => CanOK);
            CommandManagePhoneTitle = new RelayCommand(ManagePhoneTitle, () => true);
            CommandManageCategory = new RelayCommand(ManageCategory, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp25 != "");
        }



        private bool Save()
        {
            if (DynamicDBCall == null) return false;

            if (PhoneTitle == null) PhoneTitle = string.Empty;
            PhoneTitle = PhoneTitle.Trim();

            if (Note == null) Note = string.Empty;
            Note = Note.Trim();

            if (ContactTitle == null) ContactTitle = string.Empty;
            ContactTitle = ContactTitle.Trim();

            if (SelectedTabIndex == 0) 
            {
                try
                {
                    var s = ContactTitle.Replace("*", "").Replace("%", "");
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        POLMessageBox.ShowError("عنوان وارد شده معتبر نمی باشد.", DynamicOwner);
                        return false;
                    }
                    if (DynamicListBoxCat.SelectedItems.Count == 0)
                    {
                        POLMessageBox.ShowError("حداقل یك دسته می بایست انتخاب شود.", DynamicOwner);
                        return false;
                    }
                    var dbc = new DBCTContact(DynamicDBCall.Session) { Title = s,  };
                    var list = (from n in DynamicListBoxCat.SelectedItems.Cast<DBCTContactCat>()
                                select DBCTContactCat.FindByOid(DynamicDBCall.Session, n.Oid)).ToList();
                    list.ForEach(n => dbc.Categories.Add(n));
                    var c = 0;
                    using (var uow = new UnitOfWork(DynamicDBCall.Session.DataLayer))
                    {
                        c = DBCTContact.GetNextCode(uow);
                    }
                    dbc.Code = c;
                    dbc.Save();

                    if (AMembership.HasPermission(PCOPermissions.Call_AllowSelectUserInSync))
                    {
                        dbc.UserCreated = ((DBMSUser2)DynamicListBoxUser.SelectedItem).UsernameLower;
                        dbc.Save();
                    }
                    var dbp = new DBCTPhoneBook(DynamicDBCall.Session)
                    {
                        PhoneNumber = DynamicDBCall.PhoneNumber,
                        Country = DynamicDBCall.Country,
                        City = DynamicDBCall.City,
                        Title = PhoneTitle,
                        Note = Note,
                        Contact = dbc
                    };
                    if (DynamicDBCall.PhoneNumber.StartsWith("09"))
                        dbp.PhoneType = EnumPhoneType.Mobile;
                    dbp.Save();

                    DynamicDBCall.Contact = dbc;
                    DynamicDBCall.Save();

                    if (true) 
                    {
                        var list2 = (from n in DynamicListBoxCat.SelectedItems.Cast<DBCTContactCat>() select n).ToList();
                        var profiles = new List<DBCTProfileRoot>();
                        list2.ForEach(cat => cat.ProfileRoots.ToList().ForEach(p => { if (!profiles.Contains(p)) profiles.Add(p); }));
                        if (profiles.Any())
                            profiles.ForEach(p => dbc.AddProfileObjectToContact(p));
                    }

                    var contactFastRegistration = false;
                    HelperUtils.Try(() => { contactFastRegistration = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactFastRegistration); });
                    if (contactFastRegistration)
                        APOCMainWindow.ShowShowFastContact(null, dbc);

                    return true;
                }
                catch (Exception ex)
                {
                    POLMessageBox.ShowError(ex.Message, DynamicOwner);
                    return false;
                }
            }

            if (SelectedTabIndex == 1)
            {
                try
                {
                    var contact = DBCTContact.FindByOid(DynamicDBCall.Session, FocusedContact.Oid);
                    var dbp = new DBCTPhoneBook(DynamicDBCall.Session)
                    {
                        PhoneNumber = DynamicDBCall.PhoneNumber,
                        Country = DynamicDBCall.Country,
                        City = DynamicDBCall.City,
                        Title = PhoneTitle,
                        Note = Note,
                        Contact = contact,
                    };
                    if (DynamicDBCall.PhoneNumber.StartsWith("09"))
                        dbp.PhoneType = EnumPhoneType.Mobile;
                    dbp.Save();

                    DynamicDBCall.Contact = contact;
                    DynamicDBCall.Save();

                    return true;
                }
                catch (Exception ex)
                {
                    POLMessageBox.ShowError(ex.Message, DynamicOwner);
                    return false;
                }
            }
            return false;
        }
        private bool Validate()
        {
            if (SelectedTabIndex == 0 && !AMembership.HasPermission(PCOPermissions.Contact_Contact_New))
            {
                POLMessageBox.ShowError("خطا : سطوح دسترسی شما جهت ایجاد پرونده كافی نمی باشد.", DynamicOwner);
                return false;
            }

            if (SelectedTabIndex == 0 && AMembership.HasPermission(PCOPermissions.Call_AllowSelectUserInSync))
            {
                if (DynamicListBoxUser.SelectedItems.Count != 1)
                {
                    POLMessageBox.ShowError("لطفا فقط یك كاربر مسئول را انتخاب كنید.", DynamicOwner);
                    return false;
                }
            }

            return true;
        }
        private void ManagePhoneTitle()
        {
            var poc = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var rv = poc.ShowManagePhoneTitle(DynamicOwner);
            if (rv != null)
            {
                PhoneTitle = rv.ToString();
            }
            PopulatePhoneTitle();
        }
        private void ManageCategory()
        {
            var apocMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            apocMainWindow.ShowManageCategory(apocMainWindow.GetWindow());
            PopulateContactCatList();
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp25);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandManagePhoneTitle { get; set; }
        public RelayCommand CommandManageCategory { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
