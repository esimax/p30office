using System.Windows;
using POL.DB.Membership;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.DXControls.MVVM;
using POL.Lib.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace POC.Module.Email.Models
{
    public class MSettingsEmailPopup : NotifyObjectBase
    {
        private IMembership AMembership { get; set; }
        private IDatabase ADatabase { get; set; }
        private POCCore APOCCore { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }


        public MSettingsEmailPopup(object mainView)
        {
            MainView = mainView;
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            ADataFieldManager = ServiceLocator.Current.GetInstance<IDataFieldManager>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                    RaisePropertyChanged("IsAuthorizedVis");
                    RaisePropertyChanged("IsNotAuthorizedVis");

                    if (e.Status == EnumMembershipStatus.AfterLogin)
                    {
                        HelperUtils.Try(() => { SelectedSendDurationIndex = DBMSSetting2.LoadSettings<int>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.EmailSendPopupDurationIndex); });
                        HelperUtils.Try(() => { SelectedReceiveDurationIndex = DBMSSetting2.LoadSettings<int>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.EmailReceivePopupDurationIndex); });
                        HelperUtils.Try(() => { AllowCardTable = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.EmailAllowCardTable); });
                    }
                };
            InitDynamics();
        }

        #region IsAuthorizedVis
        public Visibility IsAuthorizedVis { get { return AMembership.IsAuthorized ? Visibility.Visible : Visibility.Collapsed; } }
        #endregion
        #region IsNotAuthorizedVis
        public Visibility IsNotAuthorizedVis { get { return !AMembership.IsAuthorized ? Visibility.Visible : Visibility.Collapsed; } }
        #endregion

        #region SelectedSendDurationIndex
        private int _SelectedSendDurationIndex;
        public int SelectedSendDurationIndex
        {
            get { return _SelectedSendDurationIndex; }
            set
            {
                _SelectedSendDurationIndex = value;
                RaisePropertyChanged("SelectedSendDurationIndex");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.EmailSendPopupDurationIndex, _SelectedSendDurationIndex);
                APOCCore.EmailSendPopupDurationIndex = value;
            }
        }
        #endregion

        #region SelectedReceiveDurationIndex
        private int _SelectedReceiveDurationIndex;
        public int SelectedReceiveDurationIndex
        {
            get { return _SelectedReceiveDurationIndex; }
            set
            {
                _SelectedReceiveDurationIndex = value;
                RaisePropertyChanged("SelectedReceiveDurationIndex");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.EmailReceivePopupDurationIndex, _SelectedReceiveDurationIndex);
                APOCCore.EmailReceivePopupDurationIndex = value;
            }
        }
        #endregion
        
        #region AllowCardTable
        private bool _AllowCardTable;
        public bool AllowCardTable
        {
            get { return _AllowCardTable; }
            set
            {
                _AllowCardTable = value;
                RaisePropertyChanged("AllowCardTable");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.EmailAllowCardTable, _AllowCardTable);
                APOCCore.EmailAllowCardTable = value;
            }
        }
        #endregion









        #region [METHODS]
        private void InitDynamics()
        {
            DynamicOwner = Window.GetWindow((UIElement)MainView);
        }
        #endregion
    }
}
