using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.XtraEditors.DXErrorProvider;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls;

namespace POL.Lib.Common.POLMembership
{
    public partial class WMembershipRoleAddEdit : DXWindow, INotifyPropertyChanged
    {
        #region CTOR

        public WMembershipRoleAddEdit(DBMSRole2 dba)
        {
            InitializeComponent();

            Title = "اطلاعات سطوح دسترسی";
            MinWidth = 420;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();

                SelectedRole = dba ?? new DBMSRole2(ADatabase.Dxs);

                InitCommands();
            }


            Loaded += (sender, e) =>
            {
                if (DataContext != this)
                {
                    DataContext = this;
                    SizeToContent = SizeToContent.WidthAndHeight;
                }
                teTitle.Validate += (s, e2) =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(e2.Value.ToString()))
                            throw null;
                    }
                    catch
                    {
                        e2.IsValid = false;
                    }
                    if (!e2.IsValid)
                    {
                        e2.ErrorType = ErrorType.Critical;
                        e2.ErrorContent = "مقدار وارد شده معتبر نمی باشد.";
                    }
                    e2.Handled = true;
                    RoleValid = e2.IsValid;
                };
            };
        }

        #endregion

        private IDatabase ADatabase { get; }
        public bool RoleValid { get; set; }
        public DBMSRole2 ParentAttFolder { get; set; }
        public new bool? DialogResult { get; set; }

        #region SelectedAttFolder

        private DBMSRole2 _SelectedRole;

        public DBMSRole2 SelectedRole
        {
            get { return _SelectedRole; }
            set
            {
                if (!ReferenceEquals(_SelectedRole, value))
                {
                    _SelectedRole = value;
                    RaisePropertyChanged("SelectedRole");
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
                }, o => RoleValid);

            cmdCancel = new RelayCommand<object>(
                o =>
                {
                    SelectedRole.Reload();
                    DialogResult = false;
                    Close();
                });
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(SelectedRole.Title))
            {
                POLMessageBox.ShowError("سطح دسترسی معتبر نمی باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            var invalidFileNameCharacters = Path.GetInvalidFileNameChars();
            if (SelectedRole.Title.ContainsAny(invalidFileNameCharacters))
            {
                POLMessageBox.ShowError("عنوان سطح دسترسی معتبر نمی باشد. لطفا از كاراكتر های غیر مجاز استفاده نكنید.");
                return false;
            }

            var f = DBMSRole2.FindByTitleExcept(ADatabase.Dxs, SelectedRole.Title, SelectedRole);

            if (f != null)
            {
                POLMessageBox.ShowError("عنوان تكراری می باشد. امكان ثبت وجود ندارد.");
                return false;
            }

            return true;
        }

        private void Save()
        {
            try
            {
                SelectedRole.Save();

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
