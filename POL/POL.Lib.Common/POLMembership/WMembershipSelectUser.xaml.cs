using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Xpf.Editors;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;

namespace POL.Lib.Common.POLMembership
{
    public partial class WMembershipSelectUser : INotifyPropertyChanged
    {
        public WMembershipSelectUser(List<Guid> selectedusers)
        {
            InitializeComponent();

            if (selectedusers == null)
                selectedusers = new List<Guid>();
            SelectedUsers = selectedusers;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                mDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                mMembership = ServiceLocator.Current.GetInstance<IMembership>();
                InitCommands();
            }


            Title = "انتخاب كاربران";

            Loaded += (s, e) =>
            {
                DataContext = this;
                lbeUsers.StyleSettings = new CheckedListBoxEditStyleSettings();
                PopulateUsers();
            };
        }

        private IDatabase mDatabase { get; }
        private IMembership mMembership { get; }

        public new bool? DialogResult { get; set; }

        #region SelectedUsers

        private List<Guid> _SelectedUsers;

        public List<Guid> SelectedUsers
        {
            get { return _SelectedUsers; }
            set
            {
                if (_SelectedUsers == value)
                    return;
                _SelectedUsers = value;
                RaisePropertyChanged("SelectedUsers");
            }
        }

        #endregion

        #region Methods

        private void InitCommands()
        {
            cmdOK = new RelayCommand<object>(o =>
            {
                UpdateSelectedUsers();
                DialogResult = true;
                Close();
            }, o => true);
            cmdCancel = new RelayCommand<object>(o =>
            {
                DialogResult = false;
                Close();
            }, o => true);

            cmdSelectAll = new RelayCommand<object>(o => SelectAll(), o => true);
            cmdSelectNone = new RelayCommand<object>(o => SelectNone(), o => true);
            cmdSelectInvert = new RelayCommand<object>(o => SelectInvert(), o => true);
        }

        private void PopulateUsers()
        {
            if (!mMembership.IsAuthorized) return;
            var xpcu = DBMSUser2.UserGetAll(mDatabase.Dxs, null);
            xpcu.Sorting = new SortingCollection {new SortProperty("Title", SortingDirection.Ascending)};
            xpcu.ToList().ForEach(e =>
            {
                if (e.Oid == mMembership.ActiveUser.UserID)
                    return;
                var lbei = new ListBoxEditItem {Content = e};
                if (SelectedUsers.Contains(e.Oid))
                    lbei.IsSelected = true;
                lbeUsers.Items.Add(lbei);
            });
        }

        private void UpdateSelectedUsers()
        {
            SelectedUsers.Clear();
            foreach (
                var u in
                    (from ListBoxEditItem v in lbeUsers.Items where v.IsSelected select v.Content).OfType<DBMSUser2>())
            {
                SelectedUsers.Add(u.Oid);
            }
        }

        private void SelectAll()
        {
            foreach (ListBoxEditItem v in lbeUsers.Items)
            {
                v.IsSelected = true;
            }
        }

        private void SelectNone()
        {
            foreach (ListBoxEditItem v in lbeUsers.Items)
            {
                v.IsSelected = false;
            }
        }

        private void SelectInvert()
        {
            foreach (ListBoxEditItem v in lbeUsers.Items)
            {
                v.IsSelected = !v.IsSelected;
            }
        }

        #endregion

        #region Commands

        public RelayCommand<object> cmdSelectAll { get; set; }
        public RelayCommand<object> cmdSelectNone { get; set; }
        public RelayCommand<object> cmdSelectInvert { get; set; }

        public RelayCommand<object> cmdOK { get; set; }
        public RelayCommand<object> cmdCancel { get; set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private new void RaisePropertyChanged(string pname)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(pname));
        }

        #endregion
    }
}
