using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Contact.Views;
using POL.DB.Membership;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;
using System.Windows.Controls;

namespace POC.Module.Contact.Models
{
    public class MContact : NotifyObjectBase, IDisposable
    {
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private RibbonControl MainRibbonControl { get; set; }
        private GridControl DynamicGrid { get; set; }
        private TableView DynamicTableView { get; set; }
        private UserControl ActiveView { get; set; }
        private DevExpress.Xpf.Editors.ButtonEdit DynamicSerachTextEdit { get; set; }

        private DispatcherTimer ContactUpdateTimer { get; set; }
        private bool HasLoadedLayout { get; set; }
        private const string ModuleID = "9D4E66E6-ACED-4F0D-91C8-8CD01B1EBA08";
        private Dictionary<Type, object> ModuleCatch { get; set; }

        #region CTOR
        public MContact(object mainView)
        {
            MainView = mainView;
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            ModuleCatch = new Dictionary<Type, object>();

            InitDynamics();
            LoadSettings();
            PopulateSelectionBasket();
            PopulateContactCat();
            UpdateSearchWithDelay();
            InitCommands();

            APOCContactModule.OnModuleInvoked += APOCContactModule_OnModuleInvoked;
            APOCContactModule.HookGotoContactByCode(
                code => HelperUtils.Try(
                    () =>
                    {
                        if (SearchType == EnumContactSearchType.SingleContact)
                        {
                            SearchType = EnumContactSearchType.Normal;
                        }
                        FocusedContact = DBCTContact.FindDuplicateCodeExcept(ADatabase.Dxs, null, code);
                        SearchType = EnumContactSearchType.SingleContact;
                    }), 10);
            AMembership.OnMembershipStatusChanged += AMembership_OnMembershipStatusChanged;
            var q =
                (from n in APOCContactModule.GetList() where n.Title == APOCCore.ContactInitModule select n).
                    FirstOrDefault();
            if (q != null)
            {
                APOCContactModule_OnModuleInvoked(this, new POCContactModuleEventArgs(q));
            }


            if (APOCContactModule.LastContactCode >= 0)
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        Thread.Sleep(500);
                        HelperUtils.DoDispatcher(() => APOCContactModule.GotoContactByCode(APOCContactModule.LastContactCode));
                    });

            }

            ContactModuleList = new List<POCContactModuleItem>();
            ContactModuleList =
                (from n in APOCContactModule.GetList() where !APOCCore.STCI.IsTamas || n.InTamas select n).ToList();
            RaisePropertyChanged("ContactModuleList");
        }

        void AMembership_OnMembershipStatusChanged(object sender, MembershipStatusEventArg e)
        {
            if (e.Status == EnumMembershipStatus.AfterLogin)
            {
                PopulateContactCat();
            }
        }
        void APOCContactModule_OnModuleInvoked(object sender, POCContactModuleEventArgs e)
        {
            if (e != null && e.Item != null && e.Item.ModelType != null)
                POCContactModuleItem.LasteSelectedVmType = e.Item.ModelType;

            if (ModuleContent != null)
            {
                if (ModuleContent.GetType() == e.Item.ViewType)
                    return;

                var fe = ModuleContent as FrameworkElement;
                if (fe != null && (fe.DataContext is ISave))
                    ((ISave)fe.DataContext).Save();

                if (ModuleContent is IModuleRibbon)
                {
                    var imr = ModuleContent as IModuleRibbon;
                    var ribbon = imr.GetRibbon();
                    if (ribbon != null)
                    {
                        var rootRibbon = (RibbonControl)((IModuleRibbon)APOCMainWindow.GetWindow()).GetRibbon();
                        var rr = ribbon as RibbonControl;
                        rootRibbon.UnMerge(rr);

                        Task.Factory.StartNew(
                             () =>
                             {

                                 System.Threading.Thread.Sleep(3000);

                                 HelperUtils.DoDispatcher(UpdateHomePage, DispatcherPriority.Normal);
                             });
                    }
                }
                ModuleContent = null;
            }

            if (e.Item.ViewType == null)
            {
                ModuleContent = null;
                return;
            }





            if (ModuleCatch.ContainsKey(e.Item.ViewType))
            {
                var view = ModuleCatch[e.Item.ViewType];
                if (view is FrameworkElement)
                {
                    var module = ((FrameworkElement)view).DataContext;
                    if (view is IModuleRibbon)
                    {
                        var imr = view as IModuleRibbon;
                        var ribbon = imr.GetRibbon() as RibbonControl;
                        if (ribbon != null)
                        {
                            var rootRibbon = (RibbonControl)((IModuleRibbon)APOCMainWindow.GetWindow()).GetRibbon();
                            rootRibbon.Merge(ribbon);
                            rootRibbon.SelectedPage = null;
                            rootRibbon.SelectedPage = ribbon.GetFirstSelectablePage();

                        }
                    }

                    var refreshable = module as IRefrashable;
                    if (refreshable != null && refreshable.RequiresRefresh)
                    {
                        refreshable.DoRefresh();
                        refreshable.RequiresRefresh = false;
                    }
                }
                ModuleContent = view;
                return;
            }


            try
            {
                var view = Activator.CreateInstance(e.Item.ViewType);
                if (view is FrameworkElement)
                {
                    var viewFE = view as FrameworkElement;
                    var model = Activator.CreateInstance(e.Item.ModelType, view);
                    viewFE.DataContext = model;
                    if (view is IModuleRibbon)
                    {
                        var imr = view as IModuleRibbon;
                        var ribbon = imr.GetRibbon() as RibbonControl;
                        if (ribbon != null)
                        {
                            var rootRibbon = (RibbonControl)((IModuleRibbon)APOCMainWindow.GetWindow()).GetRibbon();
                            rootRibbon.Merge(ribbon);
                            rootRibbon.SelectedPage = null;
                            rootRibbon.SelectedPage = ribbon.GetFirstSelectablePage();
                        }
                    }
                }

                ModuleContent = view;
                ModuleCatch.Add(e.Item.ViewType, view);
            }
            catch
            {
                ModuleContent = null;
            }








            #region OLD


            #endregion
        }
        #endregion






        #region ContactModuleList
        public List<POCContactModuleItem> ContactModuleList { get; set; }
        #endregion

        #region ModuleContent
        public object _ModuleContent;
        public object ModuleContent
        {
            get { return _ModuleContent; }
            set
            {
                _ModuleContent = value;
                RaisePropertyChanged("ModuleContent");
            }
        }
        #endregion

        #region CustomColumn
        public string CustomColumnText0
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle0; }
        }
        public string CustomColumnText1
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle1; }
        }
        public string CustomColumnText2
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle2; }
        }
        public string CustomColumnText3
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle3; }
        }
        public string CustomColumnText4
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle4; }
        }
        public string CustomColumnText5
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle5; }
        }
        public string CustomColumnText6
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle6; }
        }
        public string CustomColumnText7
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle7; }
        }
        public string CustomColumnText8
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle8; }
        }
        public string CustomColumnText9
        {
            get { return APOCCore.STCI == null ? string.Empty : APOCCore.STCI.ContactCustColTitle9; }
        }



        public bool CustomColumnEnable1
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable1 && CustomColumnShow1; }
        }
        public bool CustomColumnEnable2
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable2 && CustomColumnShow2; }
        }
        public bool CustomColumnEnable3
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3 && CustomColumnShow3; }
        }
        public bool CustomColumnEnable4
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3 && CustomColumnShow4; }
        }
        public bool CustomColumnEnable5
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3 && CustomColumnShow5; }
        }
        public bool CustomColumnEnable6
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3 && CustomColumnShow6; }
        }
        public bool CustomColumnEnable7
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3 && CustomColumnShow7; }
        }
        public bool CustomColumnEnable8
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3 && CustomColumnShow8; }
        }
        public bool CustomColumnEnable9
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3 && CustomColumnShow9; }
        }
        public bool CustomColumnEnable0
        {
            get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3 && CustomColumnShow0; }
        }

        private bool CustomColumnShow1 { get; set; }
        private bool CustomColumnSearchable1 { get; set; }

        private bool CustomColumnShow2 { get; set; }
        private bool CustomColumnSearchable2 { get; set; }

        private bool CustomColumnShow3 { get; set; }
        private bool CustomColumnSearchable3 { get; set; }

        private bool CustomColumnShow4 { get; set; }
        private bool CustomColumnSearchable4 { get; set; }

        private bool CustomColumnShow5 { get; set; }
        private bool CustomColumnSearchable5 { get; set; }

        private bool CustomColumnShow6 { get; set; }
        private bool CustomColumnSearchable6 { get; set; }

        private bool CustomColumnShow7 { get; set; }
        private bool CustomColumnSearchable7 { get; set; }

        private bool CustomColumnShow8 { get; set; }
        private bool CustomColumnSearchable8 { get; set; }

        private bool CustomColumnShow9 { get; set; }
        private bool CustomColumnSearchable9 { get; set; }

        private bool CustomColumnShow0 { get; set; }
        private bool CustomColumnSearchable0 { get; set; }
        #endregion

        #region Show User Columns
        public bool ShowCreatedDate { get; set; }
        public bool ShowCreatorName { get; set; }
        public bool ShowEditedDate { get; set; }
        public bool ShowEditorName { get; set; }
        #endregion

        #region Searchable data
        public bool SearchInPhone { get; set; }
        public bool SearchInAddress { get; set; }
        public bool SearchInProfile { get; set; }
        public bool SearchInEmail { get; set; }
        public bool SearchInSMS { get; set; }
        #endregion

        #region SearchType
        private EnumContactSearchType _SearchType;
        public EnumContactSearchType SearchType
        {
            get { return _SearchType; }
            set
            {
                if (_SearchType == value) return;
                _SearchType = value;
                RaisePropertyChanged("SearchType");
                UpdateSearchWithDelay();
                RaisePropertyChanged("ContactCode");
                RaisePropertyChanged("ContactTitle");
                RaisePropertyChanged("SearchByCodeVisibility");
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
            }
        }
        #endregion SearchText
        #region SearchInBasket
        private bool _SearchInBasket;
        public bool SearchInBasket
        {
            get
            {
                return _SearchInBasket;
            }
            set
            {
                _SearchInBasket = value;
                RaisePropertyChanged("SearchInBasket");
                UpdateSearchWithDelay();
                FocusedContact = null;
            }
        }
        #endregion SearchInBasket
        #region SelectionBasketList
        private XPCollection<DBCTContactSelection> _SelectionBasketList;
        public XPCollection<DBCTContactSelection> SelectionBasketList
        {
            get
            {
                return _SelectionBasketList;
            }
            set
            {
                if (_SelectionBasketList == value)
                    return;
                _SelectionBasketList = value;
                RaisePropertyChanged("SelectionBasketList");
            }
        }
        #endregion
        #region SelectedBasket
        private DBCTContactSelection _SelectedBasket;
        public DBCTContactSelection SelectedBasket
        {
            get
            {
                return _SelectedBasket;
            }
            set
            {
                if (ReferenceEquals(_SelectedBasket, value)) return;
                _SelectedBasket = value;
                RaisePropertyChanged("SelectedBasket");
                FocusedContact = null;
                UpdateSearch();
            }
        }
        #endregion

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
                APOCContactModule.SelectedContact = value;
                APOCContactModule.RaiseOnSelectedContactChanged();
                RaisePropertyChanged("ContactCode");
                RaisePropertyChanged("ContactTitle");
            }
        }
        #endregion FocusedContact

        public int SelectedRowCount
        {
            get
            {
                return DynamicGrid.GetSelectedRowHandles().Length;
            }
        }

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
                FocusedContact = null;
                UpdateSearchWithDelay();
            }
        }
        #endregion

        public string ContactCode
        {
            get
            {
                if (APOCContactModule.SelectedContact is DBCTContact)
                    return (APOCContactModule.SelectedContact as DBCTContact).Code.ToString();
                return string.Empty;
            }
        }
        public string ContactTitle
        {
            get
            {
                if (APOCContactModule.SelectedContact is DBCTContact)
                    return (APOCContactModule.SelectedContact as DBCTContact).Title;
                return string.Empty;
            }
        }
        public Visibility SearchByCodeVisibility
        {
            get
            {
                return SearchType == EnumContactSearchType.SingleContact ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private DBCTContact SingleSelectHolder { get; set; }








        #region [METHODS]
        private void PopulateSelectionBasket()
        {
            if (!AMembership.IsAuthorized) return;

            SelectionBasketList = DBCTContactSelection.GetByUser(ADatabase.Dxs, AMembership.ActiveUser.UserID, false, null);
            SelectionBasketList.Sorting = new SortingCollection(new SortProperty("Title", SortingDirection.Ascending));
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

        private void LoadSettings()
        {
            if (!AMembership.IsAuthorized) return;


            HelperUtils.Try(() => { ShowCreatedDate = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowCreatedDate); });
            HelperUtils.Try(() => { ShowCreatorName = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowCreatorName); });
            HelperUtils.Try(() => { ShowEditedDate = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowEditedDate); });
            HelperUtils.Try(() => { ShowEditorName = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowEditorName); });

            HelperUtils.Try(() => { SearchInPhone = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInPhone); });
            HelperUtils.Try(() => { SearchInAddress = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInAddress); });
            HelperUtils.Try(() => { SearchInProfile = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInProfile); });
            HelperUtils.Try(() => { SearchInEmail = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInEmail); });
            HelperUtils.Try(() => { SearchInSMS = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInSMS); });

            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable1)CustomColumnShow1 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow1); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable2)CustomColumnShow2 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow2); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable3)CustomColumnShow3 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow3); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable4)CustomColumnShow4 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow4); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable5)CustomColumnShow5 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow5); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable6)CustomColumnShow6 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow6); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable7)CustomColumnShow7 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow7); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable8)CustomColumnShow8 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow8); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable9)CustomColumnShow9 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow9); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable0)CustomColumnShow0 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow0); });


            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable1)CustomColumnSearchable1 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable1); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable2)CustomColumnSearchable2 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable2); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable3)CustomColumnSearchable3 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable3); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable4)CustomColumnSearchable4 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable4); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable5)CustomColumnSearchable5 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable5); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable6)CustomColumnSearchable6 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable6); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable7)CustomColumnSearchable7 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable7); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable8)CustomColumnSearchable8 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable8); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable9)CustomColumnSearchable9 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable9); });
            HelperUtils.Try(() => { if (APOCCore.STCI.ContactCustColEnable0)CustomColumnSearchable0 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable0); });


            if (!APOCCore.STCI.ContactCustColEnable1) RemoveColumnByField("CCText1");
            if (!APOCCore.STCI.ContactCustColEnable2) RemoveColumnByField("CCText2");
            if (!APOCCore.STCI.ContactCustColEnable3) RemoveColumnByField("CCText3");
            if (!APOCCore.STCI.ContactCustColEnable4) RemoveColumnByField("CCText4");
            if (!APOCCore.STCI.ContactCustColEnable5) RemoveColumnByField("CCText5");
            if (!APOCCore.STCI.ContactCustColEnable6) RemoveColumnByField("CCText6");
            if (!APOCCore.STCI.ContactCustColEnable7) RemoveColumnByField("CCText7");
            if (!APOCCore.STCI.ContactCustColEnable8) RemoveColumnByField("CCText8");
            if (!APOCCore.STCI.ContactCustColEnable9) RemoveColumnByField("CCText9");
            if (!APOCCore.STCI.ContactCustColEnable0) RemoveColumnByField("CCText0");

            HelperUtils.Try(() => { APOCCore.AutoSaveProfile = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ProfileAutoSave); });
            HelperUtils.Try(() => { APOCCore.ContactInitModule = DBMSSetting2.LoadSettings<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactInitModule); });
        }

        private void RemoveColumnByField(string fieldName)
        {
            var q = (from n in DynamicGrid.Columns where n.FieldName == fieldName select n).FirstOrDefault();
            if (q == null) return;
            DynamicGrid.Columns.Remove(q);
        }
        private void UpdateSearch()
        {
            if (!AMembership.IsAuthorized) return;

            var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);

            switch (SearchType)
            {
                case EnumContactSearchType.Normal:

                    if (!AMembership.HasPermission(PCOPermissions.Contact_Contact_AllowEditUnowned))
                        mainSearchCriteria.Operands.Add(new BinaryOperator("UserCreated", AMembership.ActiveUser.UserName));

                    if (SearchInBasket && SelectedBasket != null)
                    {
                        mainSearchCriteria.Operands.Add(new ContainsOperator("Selections", new BinaryOperator("Oid", SelectedBasket.Oid)));
                    }

                    var innerCriteria = new GroupOperator(GroupOperatorType.Or);
                    var code = -1;
                    try
                    {
                        code = Convert.ToInt32(_SearchText);
                    }
                    catch
                    {
                    }
                    if (code > 0)
                        innerCriteria.Operands.Add(new BinaryOperator("Code", code));

                    var selcat = SelectedContactCat as DBCTContactCat;
                    if (selcat != null)
                    {
                        mainSearchCriteria.Operands.Add(
                            new ContainsOperator("Categories", new BinaryOperator("Oid", selcat.Oid)));
                    }
                    if (selcat == null && AMembership.ActiveUser.UserName.ToLower() != "admin")
                    {
                        if (ContactCatList.Count <= 1)
                        {
                            ContactList = null;
                            return;
                        }
                        var go_or_cat = new GroupOperator(GroupOperatorType.Or);
                        ContactCatList.ForEach(
                            cat =>
                            {
                                if (cat is DBCTContactCat)
                                    go_or_cat.Operands.Add(
                                        new ContainsOperator("Categories", new BinaryOperator("Oid", ((DBCTContactCat)cat).Oid)));
                            });
                        mainSearchCriteria.Operands.Add(go_or_cat);
                    }

                    if (string.IsNullOrWhiteSpace(_SearchText)) _SearchText = string.Empty;
                    var st = SearchText.Replace("*", "").Replace("%", "").Trim();
                    if (!string.IsNullOrEmpty(st))
                    {
                        var txt = String.Format("%{0}%", st);
                        HelperConvert.CorrectPersianBug(ref txt);
                        innerCriteria.Operands.Add(new BinaryOperator("Title", txt, BinaryOperatorType.Like));

                        if (SearchInPhone)
                        {
                            innerCriteria.Operands.Add(new ContainsOperator("Phones", new BinaryOperator("PhoneNumber", txt, BinaryOperatorType.Like)));
                            innerCriteria.Operands.Add(new ContainsOperator("Phones", new BinaryOperator("Note", txt, BinaryOperatorType.Like)));
                        }

                        if (SearchInAddress)
                            if (APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL)
                            {
                                innerCriteria.Operands.Add(new ContainsOperator("Addresses", CriteriaOperator.Parse(string.Format("FullTextContains(Address, '\"{0}*\"') OR FullTextContains(Note, '\"{0}*\"') OR FullTextContains(Title, '\"{0}*\"')", st))));
                            }
                            else
                                innerCriteria.Operands.Add(new ContainsOperator("Addresses", new BinaryOperator("Address", txt, BinaryOperatorType.Like)));
                        if (SearchInProfile)
                        {

                            if (APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL)
                            {
                                innerCriteria.Operands.Add(new ContainsOperator("ProfileValues", CriteriaOperator.Parse(string.Format("FullTextContains(String1, '\"{0}*\"') OR FullTextContains(String2, '\"{0}*\"')", st))));
                            }
                            else
                            {
                                innerCriteria.Operands.Add(new ContainsOperator("ProfileValues", new BinaryOperator("String1", txt, BinaryOperatorType.Like)));
                                innerCriteria.Operands.Add(new ContainsOperator("ProfileValues", new BinaryOperator("String2", txt, BinaryOperatorType.Like)));
                            }
                        }
                        if (!SearchInProfile)
                        {
                            if (CustomColumnEnable1 && CustomColumnSearchable1)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText1, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText1", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable2 && CustomColumnSearchable2)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText2, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText2", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable3 && CustomColumnSearchable3)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText3, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText3", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable4 && CustomColumnSearchable4)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText4, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText4", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable5 && CustomColumnSearchable5)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText5, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText5", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable6 && CustomColumnSearchable6)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText6, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText6", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable7 && CustomColumnSearchable7)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText7, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText7", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable8 && CustomColumnSearchable8)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText8, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText8", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable9 && CustomColumnSearchable9)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText9, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText9", txt, BinaryOperatorType.Like));
                            if (CustomColumnEnable0 && CustomColumnSearchable0)
                                innerCriteria.Operands.Add(APOCCore.STCI.DatabaseProvider == EnumDatabaseProvider.MSSQL
                                                               ? CriteriaOperator.Parse(string.Format("FullTextContains(CCText0, '\"{0}*\"')", st))
                                                               : new BinaryOperator("CCText0", txt, BinaryOperatorType.Like));
                        }
                    }
                    mainSearchCriteria.Operands.Add(innerCriteria);
                    break;
                case EnumContactSearchType.Advanced:
                    break;
                case EnumContactSearchType.SingleContact:
                    if (SingleSelectHolder != null)
                    {
                        mainSearchCriteria.Operands.Add(new BinaryOperator("Code", SingleSelectHolder.Code));
                        FocusedContact = SingleSelectHolder;
                        SingleSelectHolder = null;
                    }
                    else
                        mainSearchCriteria.Operands.Add(new BinaryOperator("Code", APOCContactModule.LastContactCode));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }






            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBCTContact)) { DisplayableProperties = "Oid;Code;Title" };

            if (CustomColumnEnable1) xpi.DisplayableProperties += ";CCText1";
            if (CustomColumnEnable2) xpi.DisplayableProperties += ";CCText2";
            if (CustomColumnEnable3) xpi.DisplayableProperties += ";CCText3";
            if (CustomColumnEnable4) xpi.DisplayableProperties += ";CCText4";
            if (CustomColumnEnable5) xpi.DisplayableProperties += ";CCText5";
            if (CustomColumnEnable6) xpi.DisplayableProperties += ";CCText6";
            if (CustomColumnEnable7) xpi.DisplayableProperties += ";CCText7";
            if (CustomColumnEnable8) xpi.DisplayableProperties += ";CCText8";
            if (CustomColumnEnable9) xpi.DisplayableProperties += ";CCText9";
            if (CustomColumnEnable0) xpi.DisplayableProperties += ";CCText0";

            if (ShowCreatorName) xpi.DisplayableProperties += ";UserCreated";
            if (ShowCreatedDate) xpi.DisplayableProperties += ";DateCreated";
            if (ShowEditorName) xpi.DisplayableProperties += ";UserModified";
            if (ShowEditedDate) xpi.DisplayableProperties += ";DateModified";


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
        private void InitDynamics()
        {
            MainRibbonControl = MainView.DynamicRibbonControl;
            DynamicGrid = MainView.DynamicDynamicGrid;
            DynamicTableView = DynamicGrid.View as TableView;
            ActiveView = MainView as UserControl;
            DynamicSerachTextEdit = MainView.DynamicSerachTextEdit;

            if (DynamicTableView != null)
            {
                DynamicTableView.MouseDoubleClick += (s1, e1) =>
                {
                    var i = DynamicTableView.GetRowHandleByMouseEventArgs(e1);
                    if (i < 0) return;
                    if (CommandContactEdit.CanExecute(null))
                        CommandContactEdit.Execute(null);
                    e1.Handled = true;
                };

                DynamicTableView.PreviewKeyDown += (s1, e1) =>
                {
                    if (e1.Key == Key.Enter)
                    {
                        if (CommandContactEdit.CanExecute(null))
                            CommandContactEdit.Execute(null);
                        e1.Handled = true;
                    }
                    if (e1.Key == Key.Delete)
                    {
                        if (CommandContactDelete.CanExecute(null))
                            CommandContactDelete.Execute(null);
                        e1.Handled = true;
                    }
                };
            }

            APOCMainWindow.GetWindow().Closing +=
               (s, e) => SaveCallGridLayout();
            if (ActiveView != null)
                ActiveView.Loaded +=
                   (s, e) =>
                   {
                       if (HasLoadedLayout) return;
                       RestoreCallGridLayout();
                       HasLoadedLayout = true;
                   };

            DynamicSerachTextEdit.PreviewKeyDown += (s, e) =>
                                                        {
                                                            if (e.Key == Key.Enter)
                                                            {
                                                                UpdateSearch();
                                                            }
                                                        };
        }

        private void RestoreCallGridLayout()
        {
            HelperUtils.Try(
                () =>
                {
                    var fn = System.IO.Path.Combine(APOCCore.LayoutPath, ModuleID + "_Contact.XML");
                    DynamicGrid.RestoreLayoutFromXml(fn);
                });
        }
        private void SaveCallGridLayout()
        {
            HelperUtils.Try(
                () =>
                {
                    var fn = System.IO.Path.Combine(APOCCore.LayoutPath, ModuleID + "_Contact.XML");
                    DynamicGrid.SaveLayoutToXml(fn);
                });
        }

        public void DynamicLoadChildRibbon()
        {
            if (ModuleContent is IModuleRibbon)
            {
                var imr = ModuleContent as IModuleRibbon;
                var ribbon = imr.GetRibbon() as RibbonControl;
                if (ribbon != null)
                {
                    var mr = APOCMainWindow.GetWindow() as IModuleRibbon;
                    if (mr == null) return;
                    var rootRibbon = mr.GetRibbon() as RibbonControl;
                    if (rootRibbon == null) return;
                    rootRibbon.Merge(ribbon);
                }
            }
        }
        public void DynamicUnloadChildRibbon()
        {
            if (ModuleContent is IModuleRibbon)
            {
                var imr = ModuleContent as IModuleRibbon;
                var ribbon = imr.GetRibbon();
                if (ribbon != null)
                {
                    var mr = APOCMainWindow.GetWindow() as IModuleRibbon;
                    if (mr == null) return;
                    var rootRibbon = mr.GetRibbon() as RibbonControl;
                    if (rootRibbon == null) return;
                    rootRibbon.UnMerge(ribbon as RibbonControl);
                }
            }
        }




        private void InitCommands()
        {
            CommandContactNew = new RelayCommand(ContactNew, () => AMembership.HasPermission(PCOPermissions.Contact_Contact_New));
            CommandContactEdit = new RelayCommand(ContactEdit, () => FocusedContact != null && AMembership.HasPermission(PCOPermissions.Contact_Contact_Edit));
            CommandContactDelete = new RelayCommand(ContactDelete, () => FocusedContact != null && AMembership.HasPermission(PCOPermissions.Contact_Contact_Delete));
            CommandContactRefresh = new RelayCommand(ContactRefresh, () => true);

            CommandBasketNew = new RelayCommand(BasketNew, () => AMembership.HasPermission(PCOPermissions.Contact_Basket_New));
            CommandBasketEdit = new RelayCommand(BasketEdit, () => SelectedBasket != null && SearchInBasket && AMembership.HasPermission(PCOPermissions.Contact_Basket_Edit));
            CommandBasketDelete = new RelayCommand(BasketDelete, () => SelectedBasket != null && SearchInBasket && AMembership.HasPermission(PCOPermissions.Contact_Basket_Delete));
            CommandBasketRefresh = new RelayCommand(BasketRefresh, () => true);

            CommandBasketAdd = new RelayCommand(BasketAdd,
                () => (SearchInBasket ? SelectedBasket != null && FocusedContact != null : FocusedContact != null) && AMembership.HasPermission(PCOPermissions.Contact_Basket_Add));
            CommandBasketAddAll = new RelayCommand(BasketAddAll,
                () => (SearchInBasket ? SelectedBasket != null && FocusedContact != null : FocusedContact != null) && AMembership.HasPermission(PCOPermissions.Contact_Basket_Add));
            CommandBasketRemove = new RelayCommand(BasketRemove, () => (SelectedBasket != null && SearchInBasket && FocusedContact != null) && AMembership.HasPermission(PCOPermissions.Contact_Basket_Remove));
            CommandBasketClear = new RelayCommand(BasketClear, () => SelectedBasket != null && SearchInBasket && AMembership.HasPermission(PCOPermissions.Contact_Basket_Remove));



            CommandBasketAnd = new RelayCommand(BasketAnd, () => AMembership.HasPermission(PCOPermissions.Contact_Basket_Operation));
            CommandBasketOr = new RelayCommand(BasketOr, () => AMembership.HasPermission(PCOPermissions.Contact_Basket_Operation));
            CommandBasketMinus = new RelayCommand(BasketMinus, () => AMembership.HasPermission(PCOPermissions.Contact_Basket_Operation));

            CommandPrint = new RelayCommand(Print, () => AMembership.HasPermission(PCOPermissions.Contact_Tools_Print));
            CommandSettings = new RelayCommand(Settings, () => AMembership.HasPermission(PCOPermissions.Contact_Tools_Settings));

            CommandClearSearchText = new RelayCommand(ClearSearchText, () => true);
            CommandSimpleSearchSettings = new RelayCommand(SimpleSearchSettings, () => AMembership.HasPermission(PCOPermissions.Contact_Tools_Settings));

            CommandSetNormalSearch = new RelayCommand(SetNormalSearch, () => true);

            CommandCategoryManage = new RelayCommand(CategoryManage, () => true);
            CommandCategoryLink = new RelayCommand(CategoryLink, () => true);
            CommandCategoryRefresh = new RelayCommand(CategoryRefresh, () => true);

            CommandContactSendSMS = new RelayCommand(ContactSendSMS, () => AMembership.HasPermission(PCOPermissions.SMS_Send) && !APOCCore.STCI.IsTamas);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp26 != "");
        }





        private void ContactNew()
        {
            var w = new WContactAddEdit(null)
                        {
                            Owner = APOCMainWindow.GetWindow()
                        };
            if (w.ShowDialog() != true) return;

            var contactFastRegistration = false;
            HelperUtils.Try(() => { contactFastRegistration = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactFastRegistration); });
            if (contactFastRegistration)
                APOCMainWindow.ShowShowFastContact(null, w.AddedContact);

            SingleSelectHolder = w.AddedContact;
            SearchType = EnumContactSearchType.SingleContact;
        }
        private void ContactEdit()
        {
            var w = new WContactAddEdit(FocusedContact)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                UpdateSearch();
        }
        private void ContactDelete()
        {
            if (SelectedRowCount <= 0) return;
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("تعداد {0} پرونده و تمامی اطلاعات وابسته حذف شود؟", SelectedRowCount), APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;
            var successCount = 0;
            var failedCount = 0;
            APOCContactModule.SelectedContact = null;
            APOCContactModule.RaiseOnSelectedContactChanged();

            DynamicGrid.BeginInit();


            POLProgressBox.Show("حذف پرونده", true, 0, SelectedRowCount, 3,
                w =>
                {
                    w.AsyncSetText(1, "در حال شمارش");
                    List<DBCTContact> list = null;
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Send,
                        new Action(() =>
                                       {
                                           list = DynamicGrid.GetSelectedRowHandles().Select(rowHandle => DynamicTableView.Grid.GetRow(rowHandle) as DBCTContact).ToList();
                                       }));

                    w.AsyncSetText(1, "در حال حذف");
                    foreach (var v in list)
                    {
                        if (w.NeedToCancel)
                            return;
                        try
                        {
                            w.AsyncSetText(2, v.Title);
                            v.Delete();
                            v.Save();
                            successCount++;
                        }
                        catch
                        {
                            failedCount++;
                        }

                        w.AsyncSetText(3, string.Format("موفقیت : {0}  - خطا : {1}", successCount, failedCount));
                    }
                },
                w =>
                {
                    DynamicGrid.EndInit();
                    APOCContactModule.SelectedContact = null;
                    POLMessageBox.ShowInformation(String.Format("تعداد {0} پرونده با موفقیت حذف شد.{1}تعداد خطا ها : {2}", successCount, Environment.NewLine, failedCount), w);
                    DynamicGrid.UnselectAll();
                    UpdateSearch();
                }, APOCMainWindow.GetWindow());
        }
        private void ContactRefresh()
        {
            UpdateSearch();
        }

        private void BasketNew()
        {
            var w = new WBasketAddEdit(null)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                SelectionBasketList.Reload();
        }
        private void BasketEdit()
        {
            var w = new WBasketAddEdit(SelectedBasket)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                SelectionBasketList.Reload();
        }
        private void BasketDelete()
        {
            if (SelectedBasket == null) return;
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("عنوان سبد : {0}{1}حذف شود؟", SelectedBasket.Title, Environment.NewLine), APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;
            try
            {
                using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                {
                    var db = DBCTContactSelection.FindByOid(uow, SelectedBasket.Oid);
                    db.Delete();
                    uow.CommitChanges();
                }
                SelectionBasketList.Reload();
                if (SelectionBasketList.Count != 0)
                {
                    SelectedBasket = SelectionBasketList[0];
                }
                else
                {
                    SelectedBasket = null;
                    SearchInBasket = false;
                }
                POLMessageBox.ShowInformation("سبد انتخاب با موفقیت حذف شد.", APOCMainWindow.GetWindow());
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError("بروز خطا در حذف سبد انتخاب.", APOCMainWindow.GetWindow());
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
            }
        }
        private void BasketRefresh()
        {
            if (SelectionBasketList != null)
                SelectionBasketList.Reload();
        }

        private void BasketAdd()
        {
            if (SearchInBasket && SelectedBasket == null) return;
            var oids = new List<Guid>();

            POLProgressBox.Show(1,
                pb =>
                {
                    pb.AsyncSetText(1, "در حال شمارش تعداد پرونده ها ...");
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.ContextIdle,
                        new Action(() =>
                        {
                            oids = DynamicGrid.GetSelectedRowHandles().Select(
                                rowHandle =>
                                {
                                    HelperUtils.AllowUIToUpdate();
                                    return DynamicTableView.Grid.GetRow(rowHandle) as DBCTContact;
                                }).Select(dbc => dbc.Oid).ToList();
                        }));
                }, pb =>
                       {
                           var w = new WContactToBasket(SearchInBasket ? SelectedBasket : null, null, oids)
                           {
                               Owner = APOCMainWindow.GetWindow()
                           };
                           w.ShowDialog();
                       }, APOCMainWindow.GetWindow());
        }
        private void BasketAddAll()
        {
            if (SearchInBasket && SelectedBasket == null) return;
            var w = new WContactToBasket(SearchInBasket ? SelectedBasket : null, ContactList.FixedFilterCriteria, null)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            w.ShowDialog();
        }
        private void BasketRemove()
        {
            if (!SearchInBasket && SelectedBasket == null) return;
            var oids = new List<Guid>();

            POLProgressBox.Show(1,
                pb =>
                {
                    pb.AsyncSetText(1, "در حال شمارش تعداد پرونده ها ...");
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.ContextIdle,
                        new Action(() =>
                        {
                            oids = DynamicGrid.GetSelectedRowHandles().Select(
                                rowHandle =>
                                {
                                    HelperUtils.AllowUIToUpdate();
                                    return DynamicTableView.Grid.GetRow(rowHandle) as DBCTContact;
                                }).Select(dbc => dbc.Oid).ToList();
                        }));
                }, pb =>
                {
                    var dr = POLMessageBox.ShowQuestionYesNo(String.Format("تعداد {0} پرونده از سبد پاك شود؟", oids.Count));
                    if (dr != MessageBoxResult.Yes)
                        return;
                    var done = 0;
                    POLProgressBox.Show(3,
                        w =>
                        {
                            var dxs = ADatabase.GetNewSession();
                            var bs = DBCTContactSelection.FindByOid(dxs, SelectedBasket.Oid);

                            w.AsyncSetText(1, "بررسی اطلاعات ...");

                            var xpcdel = new XPCollection<DBCTContact>(dxs) { Criteria = new InOperator("Oid", oids) };
                            xpcdel.Load();

                            w.AsyncSetMax(xpcdel.Count);
                            w.AsyncSetText(1, "در حال پاك كردن ...");
                            w.AsyncSetText(2, String.Format("تعداد : {0}", xpcdel.Count));
                            w.AsyncEnableCancel();
                            for (var i = 0; i < xpcdel.Count; i++)
                            {
                                if (w.NeedToCancel)
                                    return;
                                var c = xpcdel[i];
                                bs.Contacts.Remove(c);
                                w.AsyncSetText(3, c.Title);
                                w.AsyncSetValue(i);
                                done++;
                            }
                            bs.Save();
                        },
                        w =>
                        {
                            UpdateSearch();
                            if (w.NeedToCancel)
                                POLMessageBox.ShowWarning(string.Format("عملیات توسط كاربر لغو شد.{0}{0}تعداد پاك شده : {1}", Environment.NewLine, done), w);
                            else
                                POLMessageBox.ShowInformation(string.Format("عملیات بطور كامل انجام شد.{0}{0}تعداد پاك شده : {1}", Environment.NewLine, done), w);
                        }, pb);
                }, APOCMainWindow.GetWindow());
        }
        private void BasketMove()
        {
            throw new NotImplementedException();
        }
        private void BasketClear()
        {
            if (!SearchInBasket && SelectedBasket == null) return;
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("تمام پرونده ها از سبد زیر پاك شود؟{0}{0}عنوان سبد : {1}", Environment.NewLine, SelectedBasket.Title));
            if (dr != MessageBoxResult.Yes)
                return;
            var done = 0;
            POLProgressBox.Show(3,
                w =>
                {
                    var dxs = ADatabase.GetNewSession();
                    var bs = DBCTContactSelection.FindByOid(dxs, SelectedBasket.Oid);

                    w.AsyncSetText(1, "بررسی اطلاعات ...");

                    var xpcdel = new XPCollection<DBCTContact>(dxs) { Criteria = ContactList.FixedFilterCriteria };
                    xpcdel.Load();

                    w.AsyncSetMax(xpcdel.Count);
                    w.AsyncSetText(1, "در حال پاك كردن ...");
                    w.AsyncSetText(2, String.Format("تعداد : {0}", xpcdel.Count));
                    w.AsyncEnableCancel();
                    for (var i = 0; i < xpcdel.Count; i++)
                    {
                        if (w.NeedToCancel)
                            return;
                        var c = xpcdel[i];
                        bs.Contacts.Remove(c);
                        w.AsyncSetText(3, c.Title);
                        w.AsyncSetValue(i);
                        done++;
                    }
                    bs.Save();
                },
                w =>
                {
                    UpdateSearch();
                    if (w.NeedToCancel)
                        POLMessageBox.ShowWarning(string.Format("عملیات توسط كاربر لغو شد.{0}{0}تعداد پاك شده : {1}", Environment.NewLine, done), w);
                    else
                        POLMessageBox.ShowInformation(string.Format("عملیات بطور كامل انجام شد.{0}{0}تعداد پاك شده : {1}", Environment.NewLine, done), w);
                }, APOCMainWindow.GetWindow());
        }

        private void BasketAnd()
        {
            var w = new WBasketOperation(SelectedBasket, EnumBoolOperationType.And)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                SelectionBasketList.Reload();
        }
        private void BasketOr()
        {
            var w = new WBasketOperation(SelectedBasket, EnumBoolOperationType.Or)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                SelectionBasketList.Reload();
        }
        private void BasketMinus()
        {
            var w = new WBasketOperation(SelectedBasket, EnumBoolOperationType.Minus)
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
                SelectionBasketList.Reload();
        }

        private void Print()
        {
            var link = new PrintableControlLink(DynamicTableView);

            var preview = new DocumentPreview { Model = new LinkPreviewModel(link) };
            var v = (DataTemplate)ActiveView.FindResource("toolbarCustomization");
            var barManagerCustomizer = new TemplatedBarManagerController { Template = v };
            preview.BarManager.Controllers.Add(barManagerCustomizer);
            var previewWindow = new DocumentPreviewWindow
            {
                Owner = APOCMainWindow.GetWindow(),
                Content = preview,
                FlowDirection = HelperLocalize.ApplicationFlowDirection,
                FontFamily = new FontFamily(HelperLocalize.ApplicationFontName),
                FontSize = HelperLocalize.ApplicationFontSize,
                Title = "پیش نمایش",
            };
            preview.FlowDirection = FlowDirection.LeftToRight;


            link.ReportHeaderData = this;
            link.ReportHeaderTemplate = (DataTemplate)ActiveView.FindResource("reportHeaderTemplate");
            link.CreateDocument(true);
            previewWindow.ShowDialog();
        }
        private void Settings()
        {
            SimpleSearchSettings();
        }

        private void ClearSearchText()
        {
            SearchText = string.Empty;
            UpdateSearch();
        }
        private void SimpleSearchSettings()
        {
            var w = new WContactSearchSettings()
            {
                Owner = APOCMainWindow.GetWindow()
            };
            if (w.ShowDialog() == true)
            {
                LoadSettings();
                UpdateSearch();
            }
        }

        private void SetNormalSearch()
        {
            SearchType = EnumContactSearchType.Normal;
        }

        private void CategoryLink()
        {
            var oids = DynamicGrid.GetSelectedRowHandles().Select(
                                rowHandle =>
                                {
                                    HelperUtils.AllowUIToUpdate();
                                    return DynamicTableView.Grid.GetRow(rowHandle) as DBCTContact;
                                }).Select(dbc => dbc.Oid).ToList();
            if (oids.Count == 0) return;

            var w = new WContactCatSelect(false) { Owner = APOCMainWindow.GetWindow() };
            var dr = w.ShowDialog();
            if (dr != true) return;

            var toRemove = from n in w.CategoryList where n.CategoryState == false select n.Category;
            var toAdd = from n in w.CategoryList where n.CategoryState == true select n.Category;
            var success = 0;
            var failed = 0;

            POLProgressBox.Show("باز سازی دسته", true, 0, oids.Count, 1,
                pb =>
                {
                    var count = 0;
                    foreach (var oid in oids)
                    {
                        if (pb.NeedToCancel) break;
                        using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                        {
                            try
                            {
                                var dbc = DBCTContact.FindByOid(uow, oid);
                                pb.AsyncSetText(1, dbc.Title);
                                pb.AsyncSetValue(count);
                                count++;

                                foreach (var trCat in toRemove)
                                {
                                    var cat = DBCTContactCat.FindByOid(uow, trCat.Oid);
                                    dbc.Categories.Remove(cat);
                                }
                                foreach (var taCat in toAdd)
                                {
                                    var cat = DBCTContactCat.FindByOid(uow, taCat.Oid);
                                    dbc.Categories.Add(cat);
                                }
                                uow.CommitChanges();
                                success++;
                            }
                            catch
                            {
                                uow.RollbackTransaction();
                                failed++;
                            }
                        }
                        var db = new XPQuery<DBCTContact>(ADatabase.Dxs).FirstOrDefault(n => n.Oid == oid);
                        if (db != null)
                            db.Categories.Reload();
                    }
                },
                pb =>
                {
                    POLMessageBox.ShowInformation(string.Format(
                        "حذف و اضافه دسته : {0}{0}موفقیت : {1}{0}خطا : {2}", Environment.NewLine, success, failed));
                    UpdateSearchWithDelay();
                },
                APOCMainWindow.GetWindow());
        }
        private void CategoryManage()
        {
            APOCMainWindow.ShowManageCategory(APOCMainWindow.GetWindow());
            PopulateContactCat();
        }
        private void CategoryRefresh()
        {
            PopulateContactCat();
        }

        private void UpdateHomePage()
        {




        }

        private void ContactSendSMS()
        {
            var contactList = DynamicGrid.GetSelectedRowHandles().Select(
                                rowHandle =>
                                {
                                    HelperUtils.AllowUIToUpdate();
                                    return DynamicTableView.Grid.GetRow(rowHandle) as DBCTContact;
                                }).Select(dbc => dbc).ToList();

            APOCMainWindow.ShowSendSMS(APOCMainWindow.GetWindow(), EnumSelectionType.SelectedContact, null,
                                       FocusedContact, contactList, SelectedContactCat, SelectedBasket, new List<string>(), string.Empty);
        }

        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp26);
        }
        #endregion



        #region [COMMANDS]
        public RelayCommand CommandContactNew { get; set; }
        public RelayCommand CommandContactEdit { get; set; }
        public RelayCommand CommandContactDelete { get; set; }
        public RelayCommand CommandContactRefresh { get; set; }

        public RelayCommand CommandBasketNew { get; set; }
        public RelayCommand CommandBasketEdit { get; set; }
        public RelayCommand CommandBasketDelete { get; set; }
        public RelayCommand CommandBasketRefresh { get; set; }

        public RelayCommand CommandBasketAdd { get; set; }
        public RelayCommand CommandBasketAddAll { get; set; }
        public RelayCommand CommandBasketRemove { get; set; }
        public RelayCommand CommandBasketMove { get; set; }
        public RelayCommand CommandBasketClear { get; set; }

        public RelayCommand CommandBasketAnd { get; set; }
        public RelayCommand CommandBasketOr { get; set; }
        public RelayCommand CommandBasketMinus { get; set; }

        public RelayCommand CommandAdvancedSearch { get; set; }
        public RelayCommand CommandExportExcel { get; set; }
        public RelayCommand CommandImportExcel { get; set; }
        public RelayCommand CommandPrint { get; set; }
        public RelayCommand CommandReport { get; set; }
        public RelayCommand CommandSettings { get; set; }

        public RelayCommand CommandClearSearchText { get; set; }
        public RelayCommand CommandSimpleSearchSettings { get; set; }



        public RelayCommand CommandSetNormalSearch { get; set; }

        public RelayCommand CommandCategoryManage { get; set; }
        public RelayCommand CommandCategoryLink { get; set; }
        public RelayCommand CommandCategoryRefresh { get; set; }

        public RelayCommand CommandContactSendSMS { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion


        #region IDisposable
        public void Dispose()
        {
            if (ModuleContent is FrameworkElement)
            {
                APOCContactModule.InvokeContactModule(string.Empty);
            }
            APOCContactModule.OnModuleInvoked -= APOCContactModule_OnModuleInvoked;
        }
        #endregion
    }
}
