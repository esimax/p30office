using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Contact.Models
{
    public class MContactSelect : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }
        private Window Owner { get; set; }
        private DBCTContact Data { get; set; }

        private DispatcherTimer ContactUpdateTimer { get; set; }
        private dynamic MainView { get; set; }
        private DBCTContactCat SelectedCat { get; set; }

        #region CTOR
        public MContactSelect(object mainView)
        {
            MainView = mainView;
            Owner = MainView as Window;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitDynamics();
            InitCommands();
            PopulateContactCat();
            if (SelectedCat != null)
                SelectedContactCat = SelectedCat;

            UpdateSearch();
        }


        #endregion

        #region WindowTitle
        public string WindowTitle
        {
            get { return "انتخاب پرونده"; }
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
                UpdateSearch();
            }
        }
        #endregion


        #region [METHODS]
        private void PopulateContactCat()
        {
            var xpc = from n in ACacheData.GetContactCatList() orderby n.Title select n.Tag;

            ContactCatList = new List<object> { "(همه دسته ها)" };
            xpc.ToList().ForEach(n => ContactCatList.Add(n));
        }
        private void UpdateSearch()
        {
            var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);
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

            if (SelectedContactCat is DBCTContactCat)
            {
                var cc = SelectedContactCat as DBCTContactCat;
                mainSearchCriteria.Operands.Add(
                    new ContainsOperator("Categories", new BinaryOperator("Oid", cc.Oid)));
            }

            if (string.IsNullOrWhiteSpace(_SearchText)) _SearchText = string.Empty;
            var st = SearchText.Replace("*", "").Replace("%", "").Trim();
            if (!string.IsNullOrEmpty(st))
            {
                var txt = String.Format("%{0}%", st);
                HelperConvert.CorrectPersianBug(ref txt);
                innerCriteria.Operands.Add(new BinaryOperator("Title", txt, BinaryOperatorType.Like));
            }
            mainSearchCriteria.Operands.Add(innerCriteria);

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

        private void InitDynamics()
        {
            SelectedCat = MainView.SelectedCat;
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    MainView.SelectedContact = FocusedContact;
                    MainView.DialogResult = true;
                    MainView.Close();
                }, () => FocusedContact != null);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp32 != "");
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp32);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion
    }
}
