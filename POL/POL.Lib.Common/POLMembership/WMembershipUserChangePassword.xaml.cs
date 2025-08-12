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
    public partial class WMembershipUserChangePassword : DXWindow, INotifyPropertyChanged
    {
        public WMembershipUserChangePassword(Guid useroid)
        {
            DBMSUser2 dbu = null;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                mDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                try
                {
                    dbu = DBMSUser2.UserGetByOid(mDatabase.Dxs, useroid);
                }
                catch
                {
                }
            }
            InitializeComponent();
            CTORCommon(dbu);
        }

        public WMembershipUserChangePassword(DBMSUser2 user)
        {
            InitializeComponent();
            CTORCommon(user);
        }

        private IDatabase mDatabase { get; set; }

        public new bool? DialogResult { get; set; }

        private void CTORCommon(DBMSUser2 user)
        {
            SelectedUser = user;

            Title = "تغییر رمز";
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

            if ((tePasswordOld.EditValue == null) || (tePasswordNew.EditValue == null))
            {
                POLMessageBox.ShowError("اطلاعات وارد شده معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(tePasswordOld.EditValue.ToString()))
            {
                POLMessageBox.ShowError("رمز قدیم معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(tePasswordNew.EditValue.ToString()))
            {
                POLMessageBox.ShowError("رمز جدید معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }


            return true;
        }

        private void Save()
        {
            try
            {
                var u = DBMSUser2.UserChangePassword(mDatabase.Dxs, SelectedUser.Username,
                    tePasswordOld.EditValue.ToString(),
                    tePasswordNew.EditValue.ToString());
                if (u == null)
                {
                    POLMessageBox.ShowError("اطلاعات وارد شده صحیح نمی باشد. تغییری در رمز ایجاد نشد.");
                    return;
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message);
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private new void RaisePropertyChanged(string pname)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(pname));
        }

        #endregion

        #region Commands

        public RelayCommand<object> cmdOK { get; set; }
        public RelayCommand<object> cmdCancel { get; set; }

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
    }
}
