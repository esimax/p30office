using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using POL.WPF.DXControls;
using POL.Lib.Common;

namespace POC.Module.Profile.Models
{
    public class MProfileReportColumnAddEdit : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        private dynamic MainView { get; set; }
        private MetaDataProfileReportItem DynamicSelectedData { get; set; }

        #region CTOR
        public MProfileReportColumnAddEdit(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

            InitCommands();
            GetDynamicData();
            PopulateData();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "اطلاعات ستون"; }
        }
        #endregion



        #region Title
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (ReferenceEquals(_Title, value)) return;
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        #endregion

        #region FieldTitle
        private string _FieldTitle;
        public string FieldTitle
        {
            get { return _FieldTitle; }
            set
            {
                if (value == _FieldTitle)
                    return;

                _FieldTitle = value;
                RaisePropertyChanged("FieldTitle");
            }
        }
        #endregion


        #region SelectedProfileItem
        private DBCTProfileItem _SelectedProfileItem;
        public DBCTProfileItem SelectedProfileItem
        {
            get { return _SelectedProfileItem; }
            set
            {
                if (ReferenceEquals(value, _SelectedProfileItem))
                    return;

                _SelectedProfileItem = value;
                RaisePropertyChanged("SelectedProfileItem");
                FieldTitle = value == null ? string.Empty : value.FullPathString;
                Title = value == null ? Title : value.Title;
            }
        }
        #endregion





        private void InitCommands()
        {
            CommandOK = new RelayCommand(OK, () => SelectedProfileItem != null && !string.IsNullOrWhiteSpace(Title));
            CommandSelectField = new RelayCommand(SelectField);
        }

        private void SelectField()
        {
            var o = APOCMainWindow.ShowSelectProfileItem(MainView, null);
            SelectedProfileItem = o as DBCTProfileItem;
        }

        private void OK()
        {
            if (Validate())
                if (Save())
                {
                    MainView.DynamicSelectedData = DynamicSelectedData;
                    MainView.DialogResult = true;
                    MainView.Close();
                }
        }



        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Title.Trim()))
            {
                POLMessageBox.ShowError("عنوان معتبر نمی باشد. امكان ثبت وجود ندارد.", MainView);
                return false;
            }

            if (SelectedProfileItem == null)
            {
                POLMessageBox.ShowError("لطفا فیلد مورد نظر را انتخاب كنید.", MainView);
                return false;
            }
            return true;
        }
        private bool Save()
        {
            DynamicSelectedData = new MetaDataProfileReportItem
                                      {
                                          Title = Title,
                                          Description = FieldTitle,
                                          ProfileItemOid = SelectedProfileItem.Oid,
                                          Settings = string.Empty,
                                          Order = 1000,
                                          ProfileItem = SelectedProfileItem,
                                          Image = HelperP30office.GetProfileItemImage(SelectedProfileItem.ItemType)
                                      };

            return true;
        }

        private void PopulateData()
        {
            if (DynamicSelectedData == null) return;
            SelectedProfileItem = (DynamicSelectedData.ProfileItem as DBCTProfileItem);
            Title = DynamicSelectedData.Title;
        }
        private void GetDynamicData()
        {
            DynamicSelectedData = MainView.DynamicSelectedData;
        }

        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandSelectField { get; set; }
        #endregion
    }
}
