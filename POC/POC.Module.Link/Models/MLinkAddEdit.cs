using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Link.Models
{
    public class MLinkAddEdit : NotifyObjectBase, IDisposable
    {
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }

        private DispatcherTimer ContactUpdateTimer { get; set; }

        private dynamic MainView { get; set; }
        private GridControl DynamicGrid { get; set; }
        private TableView DynamicTableView { get; set; }
        private Window DynamicOwner { get; set; }

        #region CTOR
        public MLinkAddEdit(object mainView)
        {
            MainView = mainView;
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

            InitDynamics();
            PopulateRelMain();
            UpdateSearch();
            InitCommands();
        }
        #endregion

        #region WindowTitle
        public string WindowTitle
        {
            get { return "ارتباط بین پرونده ها"; }
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
                RaisePropertyChanged("ContactTitle2");
            }
        }
        #endregion FocusedContact

        public DBCTContact Contact1 { get; set; }
        public DBCTContactRelation Relation { get; set; }

        #region RelMainList
        private List<string> _RelMainList;
        public List<string> RelMainList
        {
            get
            {
                return _RelMainList;
            }
            set
            {
                if (_RelMainList == value)
                    return;
                _RelMainList = value;
                RaisePropertyChanged("RelMainList");
            }
        }
        #endregion
        #region RelSubList1
        private List<string> _RelSubList1;
        public List<string> RelSubList1
        {
            get
            {
                return _RelSubList1;
            }
            set
            {
                if (_RelSubList1 == value)
                    return;
                _RelSubList1 = value;
                RaisePropertyChanged("RelSubList1");
            }
        }
        #endregion
        #region RelSubList2
        private List<string> _RelSubList2;
        public List<string> RelSubList2
        {
            get
            {
                return _RelSubList2;
            }
            set
            {
                if (_RelSubList2 == value)
                    return;
                _RelSubList2 = value;
                RaisePropertyChanged("RelSubList2");
            }
        }
        #endregion

        #region RelMain1
        private string _RelMain1;
        public string RelMain1
        {
            get
            {
                return _RelMain1;
            }
            set
            {
                if (_RelMain1 == value)
                    return;
                _RelMain1 = value;
                RaisePropertyChanged("RelMain1");
                PopulateRelSub1();
            }
        }
        #endregion
        #region RelMain2
        private string _RelMain2;
        public string RelMain2
        {
            get
            {
                return _RelMain2;
            }
            set
            {
                if (_RelMain2 == value)
                    return;
                _RelMain2 = value;
                RaisePropertyChanged("RelMain2");
                PopulateRelSub2();
            }
        }
        #endregion
        #region RelSub1
        private string _RelSub1;
        public string RelSub1
        {
            get
            {
                return _RelSub1;
            }
            set
            {
                if (_RelSub1 == value)
                    return;
                _RelSub1 = value;
                RaisePropertyChanged("RelSub1");
            }
        }
        #endregion
        #region RelSub2
        private string _RelSub2;
        public string RelSub2
        {
            get
            {
                return _RelSub2;
            }
            set
            {
                if (_RelSub2 == value)
                    return;
                _RelSub2 = value;
                RaisePropertyChanged("RelSub2");
            }
        }
        #endregion


        public string ContactTitle1
        {
            get { return Contact1.Title; }
        }
        public string ContactTitle2
        {
            get
            {
                return FocusedContact == null ? "{نامشخص}" : FocusedContact.Title;
            }
        }

        public bool CanSearch { get; set; }




        #region [METHODS]
        private void PopulateRelMain()
        {
            RelMainList = (from n in DBCTContactRelMain.GetAll(ADatabase.Dxs) select n.Title).ToList();
        }
        private void PopulateRelSub1()
        {
            RelSubList1 = (from n in DBCTContactRelSub.GetByMainTitle(ADatabase.Dxs, RelMain1) select n.Title).ToList();
        }
        private void PopulateRelSub2()
        {
            RelSubList2 = (from n in DBCTContactRelSub.GetByMainTitle(ADatabase.Dxs, RelMain2) select n.Title).ToList();
        }
        private void UpdateSearch()
        {
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

            if (string.IsNullOrWhiteSpace(_SearchText)) _SearchText = string.Empty;
            var st = SearchText.Replace("*", "").Replace("%", "").Trim();
            if (!string.IsNullOrEmpty(st))
            {
                var txt = String.Format("%{0}%", st);
                HelperConvert.CorrectPersianBug(ref txt);
                innerCriteria.Operands.Add(new BinaryOperator("Title", txt, BinaryOperatorType.Like));
            }

            var outterCriteria = new GroupOperator();
            outterCriteria.Operands.Add(innerCriteria);
            outterCriteria.Operands.Add(new BinaryOperator("Oid", Contact1.Oid, BinaryOperatorType.NotEqual));

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBCTContact))
                          {
                              DisplayableProperties = "Oid;Code;Title",
                              FixedFilterCriteria = outterCriteria
                          };
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
        private void InitDynamics()
        {
            DynamicOwner = Window.GetWindow((UIElement)MainView);
            DynamicGrid = MainView.DynamicDynamicGrid;
            DynamicTableView = DynamicGrid.View as TableView;
            Contact1 = MainView.SelectedContact;
            Relation = MainView.SelectedRelation;
            CanSearch = true;
            if (Relation != null)
            {
                CanSearch = false;
                RelMain1 = Relation.TitleMain1;
                RelSub1 = Relation.TitleSub1;
                RelMain2 = Relation.TitleMain2;
                RelSub2 = Relation.TitleSub2;
            }
        }


        private void InitCommands()
        {
            CommandManageRelMain = new RelayCommand(ManageRelMain, () => true);
            CommandManageRelSub1 = new RelayCommand(ManageRelSub1, () => !string.IsNullOrEmpty(RelMain1));
            CommandManageRelSub2 = new RelayCommand(ManageRelSub2, () => !string.IsNullOrEmpty(RelMain2));

            CommandOK = new RelayCommand(OK, () => (FocusedContact != null || !CanSearch) && !string.IsNullOrEmpty(RelMain1)
                && !string.IsNullOrEmpty(RelMain2) && !string.IsNullOrEmpty(RelSub1) && !string.IsNullOrEmpty(RelSub2));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp46 != "");
        }

        private void OK()
        {
            if (Validate())
                if (Save())
                {
                    MainView.DialogResult = true;
                    MainView.Close();
                }
        }

        private bool Save()
        {
            try
            {
                if (Relation == null)
                {
                    using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                    {
                        Relation = new DBCTContactRelation(uow)
                        {
                            Contact1 = DBCTContact.FindByOid(uow, Contact1.Oid),
                            Contact2 = DBCTContact.FindByOid(uow, FocusedContact.Oid),
                            TitleMain1 = RelMain1,
                            TitleMain2 = RelMain2,
                            TitleSub1 = RelSub1,
                            TitleSub2 = RelSub2,
                        };
                        uow.CommitChanges();
                    }
                    Relation = DBCTContactRelation.FindByOid(ADatabase.Dxs, Relation.Oid);
                }
                else
                {
                    Relation.TitleMain1 = RelMain1;
                    Relation.TitleMain2 = RelMain2;
                    Relation.TitleSub1 = RelSub1;
                    Relation.TitleSub2 = RelSub2;
                    Relation.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowWarning(ex.Message, MainView);
                return false;
            }
        }

        private bool Validate()
        {
            if (!CanSearch) return true;
            var rel = DBCTContactRelation.FindDuplicateByContact(ADatabase.Dxs, Relation, Contact1.Oid, FocusedContact.Oid);
            if (rel != null)
            {
                if (rel.TitleMain1 == RelMain1 && rel.TitleMain2 == RelMain2 && rel.TitleSub1 == RelSub1 && rel.TitleSub2 == RelSub2)
                {
                    POLMessageBox.ShowError("چنین ارتباطی از قبل ثبت شده است.", MainView);
                    return false;
                }
                if (rel.TitleMain1 == RelMain2 && rel.TitleMain2 == RelMain1 && rel.TitleSub1 == RelSub2 && rel.TitleSub2 == RelSub1)
                {
                    POLMessageBox.ShowError("چنین ارتباطی از قبل ثبت شده است.", MainView);
                    return false;
                }
            }
            return true;
        }


        private void ManageRelMain()
        {
            APOCMainWindow.ShowManageRelMain(DynamicOwner);
            PopulateRelMain();
        }
        private void ManageRelSub1()
        {
            var main = DBCTContactRelMain.FindDuplicateTitleExcept(ADatabase.Dxs, null, RelMain1);
            if (main != null)
            {
                APOCMainWindow.ShowManageRelSub(DynamicOwner, main);
                PopulateRelSub1();
            }
        }
        private void ManageRelSub2()
        {
            var main = DBCTContactRelMain.FindDuplicateTitleExcept(ADatabase.Dxs, null, RelMain2);
            if (main != null)
            {
                APOCMainWindow.ShowManageRelSub(DynamicOwner, main);
                PopulateRelSub2();
            }
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp46);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandManageRelMain { get; set; }
        public RelayCommand CommandManageRelSub1 { get; set; }
        public RelayCommand CommandManageRelSub2 { get; set; }

        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
        public void Dispose()
        {

        }
    }
}
