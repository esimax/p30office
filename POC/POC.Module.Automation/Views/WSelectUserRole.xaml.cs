using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.Lib.Interfaces;



namespace POC.Module.Automation.Views
{

    public partial class WSelectUserRole : DXWindow
    {
        public int DefaultType { get; set; }
        public Guid DefaultUserRole { get; set; }

        public WSelectUserRole(int defaultType, Guid default_User_Role)
        {
            InitializeComponent();
            DefaultType = defaultType;
            DefaultUserRole = default_User_Role;

            Loaded += (s, e) =>
            {
                DataContext = this;
                firstFocused.Focus();
                POL.Lib.Utils.HelperLocalize.SetLanguageToDefault();
                Task.Factory.StartNew(
                    () =>
                    {
                        PopulateUserList();
                        PopulateRoleList();
                        System.Threading.Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => firstFocused.SelectAll()));
                    });
                firstFocused.SelectedIndex = defaultType;
            };
            Closing += (s, e) =>
            {
                if (DialogResult == true)
                {
                    if (firstFocused.SelectedIndex == 0)
                        SelectedData = new Tuple<int, Guid, string>(0, Guid.Empty, string.Empty);
                    if (firstFocused.SelectedIndex == 1)
                        SelectedData = new Tuple<int, Guid, string>(1, ((DBMSRole2)SelectedUserRole).Oid, ((DBMSRole2)SelectedUserRole).Title);
                    if (firstFocused.SelectedIndex == 2)
                        SelectedData = new Tuple<int, Guid, string>(1, ((DBMSUser2)SelectedUserRole).Oid, ((DBMSUser2)SelectedUserRole).Title);
                }
            };
        }




        public List<DBMSUser2> UserList { get; set; }
        public List<DBMSRole2> RoleList { get; set; }

        #region UserRoleList
        private List<object> _UserRoleList;
        public List<object> UserRoleList
        {
            get { return _UserRoleList; }
            set
            {
                _UserRoleList = value;
                RaisePropertyChanged("UserRoleList");
            }
        }
        #endregion
        #region SelectedUserRole
        private object _SelectedUserRole;
        public object SelectedUserRole
        {
            get { return _SelectedUserRole; }
            set
            {
                _SelectedUserRole = value;
                RaisePropertyChanged("SelectedUserRole");
            }
        }
        #endregion

        private void firstFocused_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (firstFocused.SelectedIndex == 0)
            {
                UserRoleList = null;
                cbeList.ItemsSource = null;
            }

            if (firstFocused.SelectedIndex == 1)
            {
                UserRoleList = (from n in RoleList select n as object).ToList();
                cbeList.ItemsSource = UserRoleList;
            }
            if (firstFocused.SelectedIndex == 2)
            {
                UserRoleList = (from n in UserList select n as object).ToList();
                cbeList.ItemsSource = UserRoleList;
            }
        }



        private void PopulateUserList()
        {
            var ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            var v = DBMSUser2.UserGetAll(ADatabase.Dxs, null, true);
            UserList = (from u in v select u).ToList();
        }
        private void PopulateRoleList()
        {
            var ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            var v = DBMSRole2.RoleGetAll(ADatabase.Dxs, null);
            RoleList = (from u in v select u).ToList();
        }


        public Tuple<int, Guid, string> SelectedData { get; set; }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
