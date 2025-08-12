using System;
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;

namespace POL.Lib.Common.POLMembership
{
    public partial class WMembershipUserResetPassword : DXWindow, INotifyPropertyChanged
    {
        #region CTOR

        public WMembershipUserResetPassword(DBMSUser2 user)
        {
            InitializeComponent();

            SelectedUser = user;

            Title = "ایجاد رمز جدید";
            HelperLocalize.SetLanguageToLTR();
            MinWidth = 320;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                mDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                InitCommands();
            }

            Loaded += (s, e) =>
            {
                if (DataContext != this)
                {
                    DataContext = this;
                    SizeToContent = SizeToContent.WidthAndHeight;
                }
            };
        }

        #endregion

        private IDatabase mDatabase { get; }
        public new bool? DialogResult { get; set; }

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
            cmdOK = new RelayCommand<object>(o =>
            {
                if (Validate())
                {
                    Save();
                }
            },
                o => true);

            cmdCancel = new RelayCommand<object>(o =>
            {
                SelectedUser.Reload();
                DialogResult = false;
                Close();
            });
        }

        private bool Validate()
        {
            if (SelectedUser == null)
            {
                POLMessageBox.ShowError("كاربر شناسایی نشد. امكان ثبت وجود ندارد.");
                return false;
            }

            if ((tePassword1.EditValue == null) || (tePassword2.EditValue == null))
            {
                POLMessageBox.ShowError("رمز و یا تكرار آن معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            if (tePassword1.EditValue.ToString() != tePassword2.EditValue.ToString())
            {
                POLMessageBox.ShowError("رمز و تكرار آن یكسان نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }


            return true;
        }

        private void Save()
        {
            try
            {
                DBMSUser2.UserResetPassword(mDatabase.Dxs, SelectedUser.Username, tePassword1.EditValue.ToString());

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
