using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Email.Models
{
    public class MEmailSelect : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private Window Owner { get; set; }
        private DBCTContact Data { get; set; }

        private DispatcherTimer ContactUpdateTimer { get; set; }
        private dynamic MainView { get; set; }

        #region CTOR
        public MEmailSelect(object mainView)
        {
            MainView = mainView;
            Owner = MainView as Window;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

            InitDynamics();
            InitCommands();

            UpdateSearch();
        }


        #endregion

        #region WindowTitle
        public string WindowTitle
        {
            get { return "انتخاب ایمیل"; }
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
        #endregion
        #region EmailList
        private XPServerCollectionSource _EmailtList;
        public XPServerCollectionSource EmailList
        {
            get { return _EmailtList; }
            set
            {
                _EmailtList = value;
                RaisePropertyChanged("EmailList");
            }
        }
        #endregion
        #region FocusedEmail
        private DBCTEmail _FocusedEmail;
        public DBCTEmail FocusedEmail
        {
            get
            {
                return _FocusedEmail;
            }
            set
            {
                if (ReferenceEquals(value, _FocusedEmail)) return;
                _FocusedEmail = value;
                RaisePropertyChanged("FocusedEmail");
            }
        }
        #endregion 
      


       
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
                innerCriteria.Operands.Add(new BinaryOperator("Contact.Code", code));


            if (string.IsNullOrWhiteSpace(_SearchText)) _SearchText = string.Empty;
            var st = SearchText.Replace("*", "").Replace("%", "").Trim();
            if (!string.IsNullOrEmpty(st))
            {
                var txt = String.Format("%{0}%", st);
                HelperConvert.CorrectPersianBug(ref txt);
                innerCriteria.Operands.Add(new BinaryOperator("Contact.Title", txt, BinaryOperatorType.Like));
                innerCriteria.Operands.Add(new BinaryOperator("Address", txt, BinaryOperatorType.Like));
                innerCriteria.Operands.Add(new BinaryOperator("Title", txt, BinaryOperatorType.Like));
            }
            mainSearchCriteria.Operands.Add(innerCriteria);

            var xpi = new XPServerCollectionSource(ADatabase.Dxs, typeof(DBCTEmail))
                          {DisplayableProperties = "Oid;Contact.Code;Contact.Title;Address;Title", FixedFilterCriteria = mainSearchCriteria};

            xpi.ResolveSession += (s, e) =>
            {
                e.Session = ADatabase.Dxs;
            };
            EmailList = null;
            EmailList = xpi;
        }
        private void UpdateSearchWithDelay()
        {
            if (ContactUpdateTimer == null)
            {
                ContactUpdateTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(500)};
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
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                    {
                        MainView.SelectedEmail = FocusedEmail;
                        MainView.DialogResult = true;
                        MainView.Close();
                    }, () => FocusedEmail != null);
        }


        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        #endregion
    }
}
