using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;

namespace POL.Lib.Common.POLMembership
{
    public partial class WMembershipUserCalcPermissions : INotifyPropertyChanged
    {
        public WMembershipUserCalcPermissions(DBMSUser2 selectedusers)
        {
            InitializeComponent();

            SelectedUsers = selectedusers;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                mDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                mMembership = ServiceLocator.Current.GetInstance<IMembership>();
                InitCommands();
            }


            Title = "جمع بندی مجوز ها";
            Loaded += (s, e) =>
            {
                DataContext = this;
                GetPermissionsEnum = UMembershipUserManagment.GetPermissionsEnum;
                PopulatePermissionList();

                PopulatePermissons();

                PopulatePermissionStructure();
            };
        }

        private IDatabase mDatabase { get; set; }
        private IMembership mMembership { get; set; }

        public new bool? DialogResult { get; set; }

        #region Commands

        public RelayCommand<object> cmdCancel { get; set; }

        #endregion

        #region SelectedUsers

        private DBMSUser2 _SelectedUsers;

        public DBMSUser2 SelectedUsers
        {
            get { return _SelectedUsers; }
            set
            {
                if (ReferenceEquals(_SelectedUsers, value))
                    return;
                _SelectedUsers = value;
                RaisePropertyChanged("SelectedUsers");
            }
        }

        #endregion

        #region Methods

        private void InitCommands()
        {
            cmdCancel = new RelayCommand<object>(o =>
            {
                DialogResult = false;
                Close();
            }, o => true);
        }

        private List<CheckBox> GetAllCheckBoxes()
        {
            var qsp = from g in spPermissions.Children.Cast<FrameworkElement>()
                let gb = g as GroupBox
                where gb != null && gb.Content is StackPanel
                select gb.Content as StackPanel;

            if (!qsp.Any()) return null;


            var qcb = from sp in qsp
                from cb in sp.Children.Cast<CheckBox>()
                select cb;
            return qcb.Any() ? qcb.ToList() : null;
        }

        private List<CheckBox> GetCheckBoxes(CheckBox cb)
        {
            if (cb == null) return null;
            var gbCat = cb.Parent as GroupBox;
            if (gbCat == null) return null;
            var spCat = gbCat.Content as StackPanel;
            if (spCat == null) return null;
            var cbList = from n in spCat.Children.Cast<FrameworkElement>()
                where n is CheckBox
                select n as CheckBox;
            return cbList.Any() ? cbList.ToList() : null;
        }

        private List<PermissionPack> PPList { get; set; }

        private void PopulatePermissionList()
        {
            if (GetPermissionsEnum == null) return;
            if (PPList == null)
                PPList = new List<PermissionPack>();
            else
                return;

            var eType = GetPermissionsEnum.Invoke();
            var eList = Enum.GetNames(eType);

            eList.ToList().ForEach(
                e =>
                {
                    var memInfo = eType.GetMember(e);
                    var attBrowsable = memInfo[0].GetCustomAttributes(typeof (BrowsableAttribute), false);
                    var isenabled = ((BrowsableAttribute) attBrowsable[0]).Browsable;
                    if (!isenabled) return;

                    var attCategory = memInfo[0].GetCustomAttributes(typeof (CategoryAttribute), false);
                    var attDescription = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
                    var attinInTamas = memInfo[0].GetCustomAttributes(typeof (InTamasAttribute), false);

                    var pp = new PermissionPack
                    {
                        Name = e,
                        Position = (int) Enum.Parse(eType, e),
                        Category = ((CategoryAttribute) attCategory[0]).Category,
                        Description = ((DescriptionAttribute) attDescription[0]).Description,
                        InTamas = attinInTamas.Count() != 0
                    };

                    PPList.Add(pp);
                });
        }

        private Func<Type> GetPermissionsEnum { get; set; }

        private void PopulatePermissionStructure()
        {
            spPermissions.BeginInit();
            spPermissions.Children.Clear();
            if (GetPermissionsEnum == null)
            {
                spPermissions.EndInit();
                return;
            }
            PopulatePermissionList();

            var poccore = ServiceLocator.Current.GetInstance<POCCore>();

            var cbAll = new CheckBox
            {
                IsChecked = null,
                IsThreeState = true,
                Content = "انتخاب دسته جمعی",
                Margin = new Thickness(3),
                IsHitTestVisible = false
            };

            spPermissions.Children.Add(cbAll);

            var groups = (from n in PPList select n.Category).Distinct().ToList();

            groups.ForEach(
                g =>
                {
                    var sp = new StackPanel();
                    var items = from n in PPList where n.Category == g orderby n.Position select n;
                    if (items.Any())
                    {
                        items.ToList().ForEach(
                            item =>
                            {
                                var cb = new CheckBox
                                {
                                    IsThreeState = true,
                                    Content = item.Description,
                                    Tag = item,
                                    Margin = new Thickness(11, 3, 3, 3),
                                    IsHitTestVisible = false
                                };

                                cb.IsChecked = TotalPer.Contains(item.Position);

                                sp.Children.Add(cb);
                            });
                    }

                    var cbCat = new CheckBox
                    {
                        IsChecked = null,
                        IsThreeState = true,
                        Content = g,
                        Margin = new Thickness(3),
                        IsHitTestVisible = false
                    };
                    var gb = new GroupBox
                    {
                        Header = cbCat,
                        Content = sp,
                        Margin = new Thickness(6)
                    };
                    spPermissions.Children.Add(gb);
                });
            spPermissions.EndInit();
        }

        private List<int> TotalPer = new List<int>();

        private void PopulatePermissons()
        {
            var per = new List<int>();

            var max = UMembershipUserManagment.GetPermissionMax() + 1;
            var permissions = new byte[SelectedUsers.Roles.Count, max];
            for (var ir = 0; ir < SelectedUsers.Roles.Count; ir++)
            {
                var r = SelectedUsers.Roles[ir];
                for (var i = 0; i < max; i++) permissions[ir, i] = 1;
                var chs = r.Permissions.ToCharArray();
                for (var i = 0; i < Math.Min(chs.Length, max); i++)
                {
                    switch (chs[i])
                    {
                        case '0':
                            permissions[ir, i] = 0;
                            break;
                        case '1':
                            permissions[ir, i] = 1;
                            break;
                        case '2':
                            permissions[ir, i] = 2;
                            break;
                    }
                }
            }

            if (SelectedUsers.Roles.Count > 1)
            {
                for (var i = 1; i < SelectedUsers.Roles.Count; i++)
                {
                    for (var j = 1; j < max; j++)
                    {
                        var v = permissions[0, j]*permissions[i, j];
                        if (v > 2) v = 2;
                        permissions[0, j] = (byte) v;
                    }
                }
            }

            if (SelectedUsers.Roles.Count > 0)
                for (var j = 0; j < max; j++)
                    if (permissions[0, j] > 1) per.Add(j);

            TotalPer = per;
        }

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
