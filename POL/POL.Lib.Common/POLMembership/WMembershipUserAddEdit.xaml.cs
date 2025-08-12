using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;

namespace POL.Lib.Common.POLMembership
{
    public partial class WMembershipUserAddEdit : INotifyPropertyChanged
    {
        private IDatabase ADatabase { get; set; }
        public new bool? DialogResult { get; set; }

        #region CTOR

        public WMembershipUserAddEdit(Guid userguid)
        {
            InitializeComponent();

            DBMSUser2 dbu = null;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                try
                {
                    dbu = DBMSUser2.UserGetByOid(ADatabase.Dxs, userguid);
                }
                catch
                {
                }
            }

            tkIsApproved.Visibility = Visibility.Collapsed;
            ceIsApproved.Visibility = Visibility.Collapsed;

            CTORInit(dbu);
        }

        public WMembershipUserAddEdit(DBMSUser2 user)
        {
            InitializeComponent();
            CTORInit(user);
        }

        private void CTORInit(DBMSUser2 user)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (ADatabase == null)
                    ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();

                SelectedUser = user ?? new DBMSUser2(ADatabase.Dxs);

                InitCommands();
            }

            Title = "اطلاعات كاربر";
            MinWidth = 320;

            Loaded += (s, e) =>
            {
                SizeToContent = SizeToContent.WidthAndHeight;
                if (DataContext != this)
                {
                    DataContext = this;
                }

                if (SelectedUser.UsernameLower == "admin")
                {
                    teUserName.IsEnabled = false;
                    ceIsApproved.IsEnabled = false;
                    teUserInternalCode.IsEnabled = false;
                    teUserTitle.Focus();
                }
            };

            teUserName.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
            teUserName.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();

            teUserEmail.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
            teUserEmail.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();

            teUserInternalCode.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
            teUserInternalCode.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();

            teUserMobile.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
            teUserMobile.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
        }

        #endregion

        #region SelectedUser

        private DBMSUser2 _SelectedUser;

        public DBMSUser2 SelectedUser
        {
            get { return _SelectedUser; }
            set
            {
                if (!ReferenceEquals(_SelectedUser, value))
                {
                    _SelectedUser = value;
                    RaisePropertyChanged("SelectedUser");
                }
            }
        }

        #endregion

        #region Methods

        private void InitCommands()
        {
            cmdOK = new RelayCommand<object>(
                o =>
                {
                    if (Validate())
                    {
                        Save();
                    }
                }, o => true);

            cmdCancel = new RelayCommand<object>(
                o =>
                {
                    SelectedUser.Reload();
                    DialogResult = false;
                    Close();
                });
        }

        private bool Validate()
        {
            if (SelectedUser.Username == null)
            {
                POLMessageBox.ShowError("نام كاربر معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedUser.Username) || SelectedUser.Username.Contains(" "))
            {
                POLMessageBox.ShowError("نام كاربر معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            var invalidFileNameCharacters = Path.GetInvalidFileNameChars();
            if (SelectedUser.Username.ContainsAny(invalidFileNameCharacters))
            {
                POLMessageBox.ShowError("نام كاربر معتبر نمی باشد. لطفا از كاراكتر های غیر مجاز استفاده نكنید.");
                return false;
            }

            var f = DBMSUser2.FindDuplicateUserNameExcept(ADatabase.Dxs, SelectedUser, SelectedUser.Username);

            if (f != null)
            {
                if (f.Oid != SelectedUser.Oid)
                    POLMessageBox.ShowError("نام كاربری تكراری می باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(teUserTitle.Text))
            {
                POLMessageBox.ShowError("عنوان كاربر معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(teUserEmail.Text) || !teUserEmail.Text.IsValidEmailAddress())
            {
                POLMessageBox.ShowError("ایمیل كاربر معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            return true;
        }

        private void Save()
        {
            try
            {
                SelectedUser.Save();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message);
            }
        }

        #endregion

        #region Commands

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
