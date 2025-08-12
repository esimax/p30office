using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;
using DevExpress.Xpf.Editors;
using DevExpress.Xpo;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using POL.DB.Membership;
using POL.Lib.Interfaces;
using POL.Lib.Interfaces.Model;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;

namespace POL.Lib.Common.POLMembership
{
    public partial class UMembershipUserManagment : INotifyPropertyChanged
    {
        public UMembershipUserManagment()
        {
            InitializeComponent();


            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                HelperUtils.Try(() => { ADatabase = ServiceLocator.Current.GetInstance<IDatabase>(); },
                    "Please Implement IDatabase before using it in UMembershipUserManagment");
                AMembership = ServiceLocator.Current.GetInstance<IMembership>();
                ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

                InitCommands();

            }
            else
            {
                return;
            }

            Loaded += (s, e) =>
            {
                if (DataContext != this)
                {
                    DataContext = this;

                    tvUser.MouseDoubleClick += (s2, e2) =>
                    {
                        var i = tvUser.GetRowHandleByMouseEventArgs(e2);
                        if (i < 0) return;
                        if (cmdUserEdit.CanExecute(null))
                        {
                            cmdUserEdit.Execute(null);
                        }
                        e2.Handled = true;
                    };

                    tvRole.MouseDoubleClick += (s2, e2) =>
                    {
                        var i = tvRole.GetRowHandleByMouseEventArgs(e2);
                        if (i < 0) return;
                        if (cmdRoleEdit.CanExecute(null))
                        {
                            cmdRoleEdit.Execute(null);
                        }
                        e2.Handled = true;
                    };
                }
                PopulateUsers();
                PopulateRoles();
                PopulatePermissionStructure();
                HighLightPermissions();
            };
        }


        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; }
        private ICacheData ACacheData { get; }
        private DispatcherTimer PermissionHighLightTimer { get; set; }

        #region CanSetPermissions

        public bool CanSetPermissions
        {
            get
            {
                try
                {
                    return FocusedRole != null && AMembership.HasPermission(PCOPermissions.Membership_Permission_Set);
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion

        private static List<PermissionPack> PPList { get; set; }
        public static Func<Type> GetPermissionsEnum { get; set; }


        private void InitCommands()
        {
            cmdUserAdd = new RelayCommand<object>(o => UserAdd(), o => CanAddUser());
            cmdUserEdit = new RelayCommand<object>(o => UserEdit(), o => CanUserEdit());
            cmdUserDelete = new RelayCommand<object>(o => UserDelete(), o => CanUserDelete());
            cmdUserResetPassword = new RelayCommand<object>(o => UserResetPassword(), o => CanResetPassword());
            cmdUserReload = new RelayCommand<object>(o => UserReload());

            cmdRoleLink = new RelayCommand<object>(o => UserRoleLink(),
                o =>
                {
                    return AMembership.HasPermission(PCOPermissions.Membership_User_LinkRole) && FocusedUser != null;
                });
            cmdRoleRemove = new RelayCommand<object>(UserRoleRemove,
                o =>
                {
                    return AMembership.HasPermission(PCOPermissions.Membership_User_UnLinkRole) && FocusedUser != null;
                });

            cmdRoleAdd = new RelayCommand<object>(o => RoleAdd(), o => CanRoleAdd());
            cmdRoleEdit = new RelayCommand<object>(o => RoleEdit(), o => CanRoleEdit());
            cmdRoleDelete = new RelayCommand<object>(o => RoleDelete(), o => CanRoleDelete());
            cmdRoleClone = new RelayCommand<object>(o => RoleClone(), o => CanRoleClone());
            cmdRoleReload = new RelayCommand<object>(o => RoleReload());
            cmdRoleImport = new RelayCommand<object>(o => RoleImport(), o => CanRoleImport());
            cmdRoleExport = new RelayCommand<object>(o => RoleExport(), o => CanRoleExport());

            cmdCalcPermissions = new RelayCommand<object>(o => CalcPermissions(), o => true);
        }

        private void CalcPermissions()
        {
            if (FocusedUser == null)
            {
                POLMessageBox.ShowWarning("هیچ گزینه ای جهت محاسبه مجوزها انتخاب نشده است.");
                return;
            }
            var w = new WMembershipUserCalcPermissions(FocusedUser) {Owner = Window.GetWindow(this)};
            w.ShowDialog();
        }

        private object RoleExport()
        {
            var role = FocusedRole;
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = role.Title + ".xml"
            };
            if (sf.ShowDialog() != true) return null;

            try
            {
                var io = new ModelRole
                {
                    Title = role.Title,
                    Permissions = role.Permissions
                };
                var serializer = new XmlSerializer(io.GetType());
                using (var f = new StreamWriter(sf.FileName))
                {
                    serializer.Serialize(f, io);
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", Window.GetWindow(this));
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, Window.GetWindow(this));
            }
            return null;
        }

        private object RoleImport()
        {
            var sf = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true
            };
            if (sf.ShowDialog() != true) return null;
            try
            {
                var serializer = new XmlSerializer(typeof (ModelRole));
                using (var f = new StreamReader(sf.FileName))
                {
                    var io = serializer.Deserialize(f) as ModelRole;
                    if (io == null)
                        throw new Exception("محتوای فایل معتبر نمی باشد.");


                    var dup = (from n in DSRoles where n.TitleLower == io.Title.ToLower() select n).FirstOrDefault();
                    if (dup != null)
                    {
                        POLMessageBox.ShowInformation(
                            "سطح دسترسی با نام زیر وجود دارد، امكان ثبت مجدد وجود ندارد." + Environment.NewLine +
                            dup.Title, Window.GetWindow(this));
                        return null;
                    }
                    var dbr = new DBMSRole2(ADatabase.Dxs)
                    {
                        Title = io.Title,
                        TitleLower = io.Title.ToLower(),
                        Permissions = io.Permissions
                    };
                    dbr.Save();
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", Window.GetWindow(this));
                PopulateRoles();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, Window.GetWindow(this));
            }
            return null;
        }


        private void UserAdd()
        {
            var w = new WMembershipUserAddEdit(null) {Owner = Window.GetWindow(this)};
            w.ShowDialog();
            if (w.DialogResult == true)
                DSUser.Reload();
        }

        private void UserEdit()
        {
            if (FocusedUser == null)
            {
                POLMessageBox.ShowWarning("هیچ گزینه ای جهت حذف انتخاب نشده است.");
                return;
            }
            var w = new WMembershipUserAddEdit(FocusedUser) {Owner = Window.GetWindow(this)};
            w.ShowDialog();
            if (w.DialogResult == true)
                DSUser.Reload();
        }

        private void UserDelete()
        {
            if (FocusedUser == null)
            {
                POLMessageBox.ShowWarning("هیچ گزینه ای جهت ویرایش انتخاب نشده است.");
                return;
            }
            if (FocusedUser.UsernameLower == "admin")
            {
                POLMessageBox.ShowWarning("امكان حذف كاربر admin وجود ندارد.");
                return;
            }
            var tdr = POLMessageBox.ShowQuestionYesNo("كاربر انتخاب شده حذف شود؟");
            if (tdr != MessageBoxResult.Yes) return;
            try
            {
                var v = FocusedUser;
                v.Delete();
                v.Save();
                DSUser.Reload();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message);
            }
        }

        private void UserResetPassword()
        {
            if (FocusedUser == null)
            {
                POLMessageBox.ShowWarning("هیچ گزینه ای جهت تغییر رمز انتخاب نشده است.");
                return;
            }
            var w = new WMembershipUserResetPassword(FocusedUser) {Owner = Window.GetWindow(this)};
            w.ShowDialog();
            if (w.DialogResult == true)
                DSUser.Reload();
        }

        private void UserReload()
        {
            PopulateUsers();
        }

        private void PopulateUsers()
        {
            DSUser = DBMSUser2.UserGetAll(ADatabase.Dxs, null);
            BestFitDelayed();
        }

        private void PopulateUserRoles()
        {
            gUserRoles.BeginInit();
            gUserRoles.Children.Clear();
            gUserRoles.RowDefinitions.Clear();

            if (FocusedUser != null)
            {
                foreach (var role in FocusedUser.Roles)
                {
                    gUserRoles.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});

                    var bRoleGoto = new Button
                    {
                        Content = role.Title,
                        Margin = new Thickness(3),
                        Command = cmdRoleGoto,
                        CommandParameter = role
                    };
                    bRoleGoto.SetValue(Grid.RowProperty, gUserRoles.RowDefinitions.Count - 1);
                    bRoleGoto.SetValue(Grid.ColumnProperty, 0);
                    gUserRoles.Children.Add(bRoleGoto);

                    var bRoleRemove = new Button
                    {
                        Content = new Image
                        {
                            Source = HelperImage.GetStandardImage16("_16_Delete.png"),
                            Width = 16,
                            Height = 16,
                            Margin = new Thickness(3)
                        },
                        Margin = new Thickness(3),
                        Command = cmdRoleRemove,
                        CommandParameter = role
                    };
                    bRoleRemove.SetValue(Grid.RowProperty, gUserRoles.RowDefinitions.Count - 1);
                    bRoleRemove.SetValue(Grid.ColumnProperty, 1);
                    gUserRoles.Children.Add(bRoleRemove);
                }

                gUserRoles.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});

                var bRoleCombo = new ComboBoxEdit
                {
                    Margin = new Thickness(3),
                    IsTextEditable = false,
                    ItemsSource = DSRoles,
                    DisplayMember = "Title",
                    AllowSpinOnMouseWheel = false,
                };

                bRoleCombo.SetValue(Grid.RowProperty, gUserRoles.RowDefinitions.Count - 1);
                bRoleCombo.SetValue(Grid.ColumnProperty, 0);
                gUserRoles.Children.Add(bRoleCombo);

                var bRoleAdd = new Button
                {
                    Content = new Image
                    {
                        Source = HelperImage.GetStandardImage16("_16_Add.png"),
                        Width = 16,
                        Height = 16,
                        Margin = new Thickness(3)
                    },
                    Margin = new Thickness(3),
                    Command = cmdRoleLink,
                    IsEnabled = FocusedUser.UsernameLower != "admin"
                };
                bRoleAdd.SetValue(Grid.RowProperty, gUserRoles.RowDefinitions.Count - 1);
                bRoleAdd.SetValue(Grid.ColumnProperty, 1);
                gUserRoles.Children.Add(bRoleAdd);
            }

            gUserRoles.EndInit();
        }

        private void BestFitDelayed()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                Dispatcher.Invoke(DispatcherPriority.Send, new Action(() => tvUser.BestFitColumns()));
            });
        }

        private bool CanAddUser()
        {
            return AMembership.HasPermission((int) PCOPermissions.Membership_User_Add);
        }

        private bool CanUserDelete()
        {
            if (!AMembership.HasPermission(PCOPermissions.Membership_User_Delete)) return false;
            return FocusedUser != null && FocusedUser.UsernameLower != "admin";
        }

        private bool CanUserEdit()
        {
            return AMembership.HasPermission(PCOPermissions.Membership_User_Edit) && FocusedUser != null;
        }

        private bool CanResetPassword()
        {
            return AMembership.HasPermission(PCOPermissions.Membership_User_ResetPassword) && FocusedUser != null;
        }


        private void UserRoleLink()
        {
            var role = GetSelectedRole();
            if (role == null) return;
            if (FocusedUser == null) return;
            FocusedUser.Roles.Add(role);
            FocusedUser.Save();
            PopulateUserRoles();
        }

        private void UserRoleRemove(object paramiter)
        {
            var role = paramiter as DBMSRole2;
            if (role == null) return;
            if (FocusedUser == null) return;
            FocusedUser.Roles.Remove(role);
            FocusedUser.Save();
            PopulateUserRoles();
        }

        private DBMSRole2 GetSelectedRole()
        {
            try
            {
                var cb = (from n in gUserRoles.Children.Cast<FrameworkElement>()
                    where n != null && n is ComboBoxEdit
                    select n as ComboBoxEdit).First();

                return cb.SelectedItem as DBMSRole2;
            }
            catch
            {
                return null;
            }
        }


        private void RoleAdd()
        {
            var v = new DBMSRole2(ADatabase.Dxs);
            var w = new WMembershipRoleAddEdit(v) {Owner = Window.GetWindow(this)};
            w.ShowDialog();
            if (w.DialogResult == true)
            {
                DSRoles.Reload();
                ACacheData.ForcePopulateCache(EnumCachDataType.Roles, false, w.SelectedRole);
                ACacheData.RaiseCacheChanged(EnumCachDataType.Roles);
            }
        }

        private void RoleEdit()
        {
            var w = new WMembershipRoleAddEdit(FocusedRole) {Owner = Window.GetWindow(this)};
            w.ShowDialog();
            if (w.DialogResult == true)
            {
                DSRoles.Reload();
                ACacheData.ForcePopulateCache(EnumCachDataType.Roles, false, w.SelectedRole);
                ACacheData.RaiseCacheChanged(EnumCachDataType.Roles);
            }
        }

        private void RoleClone()
        {
            if (FocusedRole == null) return;
            using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
            {
                var dbr1 = DBMSRole2.FindByOid(uow, FocusedRole.Oid);
                if (dbr1 == null) return;
                var newtitle = dbr1.Title + "-كپی";
                var dbtemp = DBMSRole2.FindByTitleExcept(uow, newtitle, null);
                if (dbtemp != null) return;
                var dbr2 = new DBMSRole2(uow);
                dbr2.Permissions = dbr1.Permissions;
                dbr2.Title = newtitle;
                dbr2.Save();
                uow.CommitChanges();
            }

            DSRoles.Reload();
            ACacheData.ForcePopulateCache(EnumCachDataType.Roles, false, null);
            ACacheData.RaiseCacheChanged(EnumCachDataType.Roles);
        }

        private void RoleDelete()
        {
            var tdr = POLMessageBox.ShowQuestionYesNo("سطح دسترسی انتخاب شده حذف شود؟");
            if (tdr != MessageBoxResult.Yes) return;
            try
            {
                FocusedRole.Delete();
                FocusedRole.Save();

                DSRoles.Reload();
                ACacheData.ForcePopulateCache(EnumCachDataType.Roles, false, null);
                ACacheData.RaiseCacheChanged(EnumCachDataType.Roles);
            }
            catch
            {
                POLMessageBox.ShowError("", null);
            }
        }

        private void RoleReload()
        {
            PopulateRoles();
        }

        private bool CanRoleAdd()
        {
            return AMembership.HasPermission(PCOPermissions.Membership_Role_Add);
        }

        private bool CanRoleEdit()
        {
            return AMembership.HasPermission(PCOPermissions.Membership_Role_Edit) && FocusedRole != null;
        }

        private bool CanRoleClone()
        {
            return AMembership.HasPermission(PCOPermissions.Membership_Role_Add) && FocusedRole != null;
        }

        private bool CanRoleDelete()
        {
            return AMembership.HasPermission(PCOPermissions.Membership_Role_Delete) && FocusedRole != null;
        }

        private bool CanRoleImport()
        {
            return AMembership.HasPermission(PCOPermissions.Membership_Role_Import) && FocusedRole != null;
        }

        private bool CanRoleExport()
        {
            return AMembership.HasPermission(PCOPermissions.Membership_Role_Export) && FocusedRole != null;
        }

        private void PopulateRoles()
        {
            DSRoles = DBMSRole2.RoleGetAll(ADatabase.Dxs, null);
        }

        private void PopulatePermissionValue(DBMSRole2 role)
        {
            if (role == null) return;
            if (PPList == null) return;
            var max = GetPermissionMax() + 1; 
            var pData = new byte[max + 1];
            for (var i = 0; i < pData.Length; i++) pData[i] = 1;
            var chars = (role.Permissions ?? string.Empty).ToCharArray();
            for (var i = 0; i < Math.Min(max, chars.Length); i++)
            {
                switch (chars[i])
                {
                    case '0':
                        pData[i] = 0;
                        break;
                    case '2':
                        pData[i] = 2;
                        break;
                    default:
                        pData[i] = 1;
                        break;
                }
            }

            var cbAll = GetAllCheckBoxes();

            foreach (var cb in cbAll)
            {
                var pp = cb.Tag as PermissionPack;
                if (pp == null) break;

                switch (pData[pp.Position])
                {
                    case 2:
                        cb.IsChecked = true;
                        break;
                    case 1:
                        cb.IsChecked = null;
                        break;
                    case 0:
                        cb.IsChecked = false;
                        break;
                }
            }
        }

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
                Margin = new Thickness(3)
            };
            cbAll.Checked += (s, e) =>
            {
                var cbList = GetAllCheckBoxes();
                if (cbList == null) return;
                cbList.ForEach(cb1 => { if (cb1.IsEnabled) cb1.IsChecked = true; });
            };
            cbAll.Unchecked += (s, e) =>
            {
                var cbList = GetAllCheckBoxes();
                if (cbList == null) return;
                cbList.ForEach(cb1 => { if (cb1.IsEnabled) cb1.IsChecked = false; });
            };
            cbAll.Indeterminate += (s, e) =>
            {
                var cbList = GetAllCheckBoxes();
                if (cbList == null) return;
                cbList.ForEach(cb1 => { if (cb1.IsEnabled) cb1.IsChecked = null; });
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
                                    IsEnabled = !poccore.STCI.IsTamas || item.InTamas
                                };
                                if (!string.IsNullOrWhiteSpace(PermissionSearchText))
                                {
                                    if (item.Description.Contains(PermissionSearchText.Trim()))
                                    {
                                        cb.FontWeight = FontWeights.UltraBold;
                                    }
                                }
                                sp.Children.Add(cb);
                            });
                    }

                    var cbCat = new CheckBox
                    {
                        IsChecked = null,
                        IsThreeState = true,
                        Content = g,
                        Margin = new Thickness(3)
                    };
                    cbCat.Checked += (s, e) =>
                    {
                        var cbList = GetCheckBoxes(s as CheckBox);
                        if (cbList == null) return;
                        cbList.ForEach(cb1 => { if (cb1.IsEnabled) cb1.IsChecked = true; });
                    };
                    cbCat.Unchecked += (s, e) =>
                    {
                        var cbList = GetCheckBoxes(s as CheckBox);
                        if (cbList == null) return;
                        cbList.ForEach(cb1 => { if (cb1.IsEnabled) cb1.IsChecked = false; });
                    };
                    cbCat.Indeterminate += (s, e) =>
                    {
                        var cbList = GetCheckBoxes(s as CheckBox);
                        if (cbList == null) return;
                        cbList.ForEach(cb1 => { if (cb1.IsEnabled) cb1.IsChecked = null; });
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

        private void SavePermissions(DBMSRole2 role)
        {
            if (role == null) return;
            if (role.IsDeleted) return;
            if (PPList == null) return;
            var max = GetPermissionMax() + 1; 
            var pData = new byte[max + 1];
            var cbAll = GetAllCheckBoxes();
            for (var i = 0; i < pData.Length; i++) pData[i] = 1;
            foreach (var cb in cbAll)
            {
                var pp = cb.Tag as PermissionPack;
                if (pp == null) break;
                if (cb.IsChecked == true)
                    pData[pp.Position] = 2;
                else if (cb.IsChecked == null) 
                    pData[pp.Position] = 1;
                else if (cb.IsChecked == false) 
                    pData[pp.Position] = 0;
            }
            var sb = new StringBuilder();
            foreach (var b in pData)
                sb.Append(b == 0 ? "0" : (b == 1 ? "1" : "2"));
            role.Permissions = sb.ToString();
            role.Save();
        }

        public static int GetPermissionMax()
        {
            if (PPList == null)
                PopulatePermissionList();
            return PPList == null ? 0 : (from n in PPList select n.Position).Max();
        }

        private static void PopulatePermissionList()
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

        private void HighLightPermissionsWithDelat()
        {
            if (PermissionHighLightTimer == null)
            {
                PermissionHighLightTimer = new DispatcherTimer();
                PermissionHighLightTimer.Interval = TimeSpan.FromMilliseconds(1000);
                PermissionHighLightTimer.Tick += (s, e) =>
                {
                    PermissionHighLightTimer.Stop();
                    HighLightPermissions();
                };
            }
            PermissionHighLightTimer.Stop();
            PermissionHighLightTimer.Start();
        }

        private void HighLightPermissions()
        {
            var cbAll = GetAllCheckBoxes();
            var count = 0;
            var effect = 0;
            foreach (var cb in cbAll)
            {
                count++;
                var pp = cb.Content as string;
                if (pp == null) continue;

                if (!string.IsNullOrWhiteSpace(PermissionSearchText) &&
                    pp.ToLower().Contains(PermissionSearchText.ToLower()))
                {
                    cb.FontWeight = FontWeights.Bold;
                    effect++;
                }
                else
                {
                    cb.FontWeight = FontWeights.Normal;
                }
            }
            SearchResultCount = string.Format("{0}/{1}", count, effect);
        }

        private void DXTabItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool)) return;
            var b = (bool) e.NewValue;
            if (!b && FocusedRole != null)
                FocusedRole = null;
        }

        #region DSUser

        private XPCollection<DBMSUser2> _DSUser;

        public XPCollection<DBMSUser2> DSUser
        {
            get { return _DSUser; }
            set
            {
                _DSUser = value;
                RaisePropertyChanged("DSUser");
            }
        }

        #endregion

        #region FocusedUser

        private DBMSUser2 _FocusedUser;

        public DBMSUser2 FocusedUser
        {
            get { return _FocusedUser; }
            set
            {
                if (ReferenceEquals(value, _FocusedUser)) return;
                _FocusedUser = value;
                RaisePropertyChanged("FocusedUser");
                PopulateUserRoles();
            }
        }

        #endregion

        #region DSRoles

        private XPCollection<DBMSRole2> _DSRoles;

        public XPCollection<DBMSRole2> DSRoles
        {
            get { return _DSRoles; }
            set
            {
                _DSRoles = value;
                RaisePropertyChanged("DSRoles");
            }
        }

        #endregion

        #region FocusedRole

        private DBMSRole2 _FocusedRole;

        public DBMSRole2 FocusedRole
        {
            get { return _FocusedRole; }
            set
            {
                if (ReferenceEquals(_FocusedRole, value)) return;
                if (_FocusedRole != null && !_FocusedRole.IsDeleted)
                    SavePermissions(_FocusedRole);
                _FocusedRole = value;
                RaisePropertyChanged("FocusedRole");
                PopulatePermissionValue(value);
                RaisePropertyChanged("CanSetPermissions");
            }
        }

        #endregion

        #region PermissionSearchText

        private string _PermissionSearchText;

        public string PermissionSearchText
        {
            get { return _PermissionSearchText; }
            set
            {
                _PermissionSearchText = value;
                RaisePropertyChanged("PermissionSearchText");
                HighLightPermissionsWithDelat();
            }
        }

        #endregion

        #region SearchResultCount

        private string _SearchResultCount;

        public string SearchResultCount
        {
            get { return _SearchResultCount; }
            set
            {
                _SearchResultCount = value;
                RaisePropertyChanged("SearchResultCount");
            }
        }

        #endregion

        #region Commands

        public RelayCommand<object> cmdUserAdd { get; set; }
        public RelayCommand<object> cmdUserEdit { get; set; }
        public RelayCommand<object> cmdUserDelete { get; set; }
        public RelayCommand<object> cmdUserResetPassword { get; set; }
        public RelayCommand<object> cmdUserReload { get; set; }

        public RelayCommand<object> cmdRoleGoto { get; set; }
        public RelayCommand<object> cmdRoleRemove { get; set; }
        public RelayCommand<object> cmdRoleLink { get; set; }


        public RelayCommand<object> cmdRoleAdd { get; set; }
        public RelayCommand<object> cmdRoleEdit { get; set; }
        public RelayCommand<object> cmdRoleDelete { get; set; }
        public RelayCommand<object> cmdRoleClone { get; set; }
        public RelayCommand<object> cmdRoleReload { get; set; }
        public RelayCommand<object> cmdRoleExport { get; set; }
        public RelayCommand<object> cmdRoleImport { get; set; }

        public RelayCommand<object> cmdCalcPermissions { get; set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string pname)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(pname));
        }

        #endregion
    }
}
