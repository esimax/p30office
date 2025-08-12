using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Utils;
using POL.WPF.DXControls;

namespace POC.Module.Email.Models
{
    public class MEmailSync : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }


        private dynamic MainView { get; set; }
        public ListBoxEdit DynamicListBoxCat { get; set; }
        private Window DynamicOwner { get; set; }
        private DBEMEmailInbox DynamicDBEmail { get; set; }
        private DispatcherTimer ContactUpdateTimer { get; set; }

        public MEmailSync(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();


            InitDynamics();
            InitCommands();
            PopulateContactCatList();
            PopulateEmailTitle();
            UpdateSearch();
        }

        #region WindowTitle
        public string WindowTitle
        {
            get { return "تطبیق ایمیل با پرونده"; }
        }
        #endregion

        #region SelectedEmailAddress
        public string SelectedEmailAddress
        {
            get { return DynamicDBEmail == null ? string.Empty : DynamicDBEmail.FromAddress; }
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
        #region EmailTitle
        private string _EmailTitle;
        public string EmailTitle
        {
            get { return _EmailTitle; }
            set
            {
                if (ReferenceEquals(_EmailTitle, value)) return;
                _EmailTitle = value;
                RaisePropertyChanged("EmailTitle");
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
            DynamicDBEmail = MainView.DynamicDBEmail;
            DynamicListBoxCat = MainView.DynamicListBoxCat;
        }
        private void PopulateContactCatList()
        {
            ContactCatList = (from n in ACacheData.GetContactCatList() orderby n.Title select (DBCTContactCat)n.Tag).ToList();
        }
        private void PopulateEmailTitle()
        {
            EmailTitleList = (from n in POL.DB.P30Office.BT.DBBTEmailTitle2.GetAll(ADatabase.Dxs) select n.Title).ToList();
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
                ContactUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
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
            CommandManageEmailTitle = new RelayCommand(ManageEmailTitle, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp42 != "");
        }

        private bool Save()
        {
            if (DynamicDBEmail == null) return false;

            if (EmailTitle == null) EmailTitle = string.Empty;
            EmailTitle = EmailTitle.Trim();

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
                    var dbc = new DBCTContact(DynamicDBEmail.Session) { Title = s, Code = DBCTContact.GetNextCode(DynamicDBEmail.Session) };
                    var list = (from n in DynamicListBoxCat.SelectedItems.Cast<DBCTContactCat>()
                                select DBCTContactCat.FindByOid(DynamicDBEmail.Session, n.Oid)).ToList();
                    list.ForEach(n => dbc.Categories.Add(n));
                    dbc.Save();

                    var dbp = new DBCTEmail(DynamicDBEmail.Session)
                                  {
                                      Address = DynamicDBEmail.FromAddress,
                                      Title = EmailTitle,
                                      Contact = dbc,
                                  };
                    dbp.Save();

                    DynamicDBEmail.Contact = dbc;
                    DynamicDBEmail.Save();

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
                    var contact = DBCTContact.FindByOid(DynamicDBEmail.Session, FocusedContact.Oid);
                    var dbp = new DBCTEmail(DynamicDBEmail.Session)
                    {
                        Address = DynamicDBEmail.FromAddress,
                        Title = EmailTitle,
                        Contact = contact
                    };
                    dbp.Save();

                    DynamicDBEmail.Contact = contact;
                    DynamicDBEmail.Save();

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
            return true;
        }
        private void ManageEmailTitle()
        {
            var poc = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var rv = poc.ShowManageEmailTitle(DynamicOwner);
            if (rv != null)
            {
                EmailTitle = rv.ToString();
            }
            PopulateEmailTitle();
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp42);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandManageEmailTitle { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
