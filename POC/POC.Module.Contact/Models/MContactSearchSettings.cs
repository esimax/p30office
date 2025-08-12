using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Editors;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using POL.DB.Membership;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;

namespace POC.Module.Contact.Models
{
    public class MContactSearchSettings : NotifyObjectBase, IRequestCloseViewModel
    {

        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private Window Owner { get; set; }
        private POCCore APOCCore { get; set; }
        private IPOCContactModule APOCContactModule { get; set; }
        private ICacheData MCacheData { get; set; }

        private dynamic MainView { get; set; }

        public ListBoxEdit DynamicListBoxCat { get; set; }

        #region CTOR
        public MContactSearchSettings(Window owner)
        {
            Owner = owner;
            MainView = owner;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            MCacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            InitCommands();
            InitDynamics();
            PopulateContactCatList();
            LoadSettings();
        }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "تنظیمات جستجوی پرونده"; }
        }
        #endregion

        #region SearchInPhone
        private bool _SearchInPhone;
        public bool SearchInPhone
        {
            get { return _SearchInPhone; }
            set
            {
                _SearchInPhone = value;
                RaisePropertyChanged("SearchInPhone");
            }
        }
        #endregion
        #region SearchInAddress
        private bool _SearchInAddress;
        public bool SearchInAddress
        {
            get { return _SearchInAddress; }
            set
            {
                _SearchInAddress = value;
                RaisePropertyChanged("SearchInAddress");
            }
        }
        #endregion
        #region SearchInProfile
        private bool _SearchInProfile;
        public bool SearchInProfile
        {
            get { return _SearchInProfile; }
            set
            {
                _SearchInProfile = value;
                RaisePropertyChanged("SearchInProfile");
            }
        }
        #endregion
        #region SearchInEmail
        private bool _SearchInEmail;
        public bool SearchInEmail
        {
            get { return _SearchInEmail; }
            set
            {
                _SearchInEmail = value;
                RaisePropertyChanged("SearchInEmail");
            }
        }
        #endregion
        #region SearchInSMS
        private bool _SearchInSMS;
        public bool SearchInSMS
        {
            get { return _SearchInSMS; }
            set
            {
                _SearchInSMS = value;
                RaisePropertyChanged("SearchInSMS");
            }
        }
        #endregion
        #region SearchInCustCol1
        private bool _SearchInCustCol1;
        public bool SearchInCustCol1
        {
            get { return _SearchInCustCol1; }
            set
            {
                _SearchInCustCol1 = value;
                RaisePropertyChanged("SearchInCustCol1");
            }
        }
        #endregion
        #region SearchInCustCol2
        private bool _SearchInCustCol2;
        public bool SearchInCustCol2
        {
            get { return _SearchInCustCol2; }
            set
            {
                _SearchInCustCol2 = value;
                RaisePropertyChanged("SearchInCustCol2");
            }
        }
        #endregion
        #region SearchInCustCol3
        private bool _SearchInCustCol3;
        public bool SearchInCustCol3
        {
            get { return _SearchInCustCol3; }
            set
            {
                _SearchInCustCol3 = value;
                RaisePropertyChanged("SearchInCustCol3");
            }
        }
        #endregion
        #region SearchInCustCol4
        private bool _SearchInCustCol4;
        public bool SearchInCustCol4
        {
            get { return _SearchInCustCol4; }
            set
            {
                _SearchInCustCol4 = value;
                RaisePropertyChanged("SearchInCustCol4");
            }
        }
        #endregion
        #region SearchInCustCol5
        private bool _SearchInCustCol5;
        public bool SearchInCustCol5
        {
            get { return _SearchInCustCol5; }
            set
            {
                _SearchInCustCol5 = value;
                RaisePropertyChanged("SearchInCustCol5");
            }
        }
        #endregion
        #region SearchInCustCol6
        private bool _SearchInCustCol6;
        public bool SearchInCustCol6
        {
            get { return _SearchInCustCol6; }
            set
            {
                _SearchInCustCol6 = value;
                RaisePropertyChanged("SearchInCustCol6");
            }
        }
        #endregion
        #region SearchInCustCol7
        private bool _SearchInCustCol7;
        public bool SearchInCustCol7
        {
            get { return _SearchInCustCol7; }
            set
            {
                _SearchInCustCol7 = value;
                RaisePropertyChanged("SearchInCustCol7");
            }
        }
        #endregion
        #region SearchInCustCol8
        private bool _SearchInCustCol8;
        public bool SearchInCustCol8
        {
            get { return _SearchInCustCol8; }
            set
            {
                _SearchInCustCol8 = value;
                RaisePropertyChanged("SearchInCustCol8");
            }
        }
        #endregion
        #region SearchInCustCol9
        private bool _SearchInCustCol9;
        public bool SearchInCustCol9
        {
            get { return _SearchInCustCol9; }
            set
            {
                _SearchInCustCol9 = value;
                RaisePropertyChanged("SearchInCustCol9");
            }
        }
        #endregion
        #region SearchInCustCol0
        private bool _SearchInCustCol0;
        public bool SearchInCustCol0
        {
            get { return _SearchInCustCol0; }
            set
            {
                _SearchInCustCol0 = value;
                RaisePropertyChanged("SearchInCustCol0");
            }
        }
        #endregion


        #region CanSearchInPhone
        public bool CanSearchInPhone
        {
            get { return true; }
        }
        #endregion
        #region CanSearchInAddress
        public bool CanSearchInAddress
        {
            get { return true; }
        }
        #endregion
        #region CanSearchInProfile
        public bool CanSearchInProfile
        {
            get { return true; }
        }
        #endregion
        #region CanSearchInEmail
        public bool CanSearchInEmail
        {
            get { return true; }
        }
        #endregion
        #region CanSearchInSMS
        public bool CanSearchInSMS
        {
            get { return true; }
        }
        #endregion
        #region CanSearchInCustCol1
        public bool CanSearchInCustCol1
        {
            get { return APOCCore.STCI.ContactCustColEnable1; }
        }
        #endregion
        #region CanSearchInCustCol2
        public bool CanSearchInCustCol2
        {
            get { return APOCCore.STCI.ContactCustColEnable2; }
        }
        #endregion
        #region CanSearchInCustCol3
        public bool CanSearchInCustCol3
        {
            get { return APOCCore.STCI.ContactCustColEnable3; }
        }
        #endregion
        #region CanSearchInCustCol4
        public bool CanSearchInCustCol4
        {
            get { return APOCCore.STCI.ContactCustColEnable4; }
        }
        #endregion
        #region CanSearchInCustCol5
        public bool CanSearchInCustCol5
        {
            get { return APOCCore.STCI.ContactCustColEnable5; }
        }
        #endregion
        #region CanSearchInCustCol6
        public bool CanSearchInCustCol6
        {
            get { return APOCCore.STCI.ContactCustColEnable6; }
        }
        #endregion
        #region CanSearchInCustCol7
        public bool CanSearchInCustCol7
        {
            get { return APOCCore.STCI.ContactCustColEnable7; }
        }
        #endregion
        #region CanSearchInCustCol8
        public bool CanSearchInCustCol8
        {
            get { return APOCCore.STCI.ContactCustColEnable8; }
        }
        #endregion
        #region CanSearchInCustCol9
        public bool CanSearchInCustCol9
        {
            get { return APOCCore.STCI.ContactCustColEnable9; }
        }
        #endregion
        #region CanSearchInCustCol0
        public bool CanSearchInCustCol0
        {
            get { return APOCCore.STCI.ContactCustColEnable0; }
        }
        #endregion




        #region ShowCreatorName
        private bool _ShowCreatorName;
        public bool ShowCreatorName
        {
            get { return _ShowCreatorName; }
            set
            {
                _ShowCreatorName = value;
                RaisePropertyChanged("ShowCreatorName");
            }
        }
        #endregion
        #region ShowCreatedDate
        private bool _ShowCreatedDate;
        public bool ShowCreatedDate
        {
            get { return _ShowCreatedDate; }
            set
            {
                _ShowCreatedDate = value;
                RaisePropertyChanged("ShowCreatedDate");
            }
        }
        #endregion
        #region ShowEditorName
        private bool _ShowEditorName;
        public bool ShowEditorName
        {
            get { return _ShowEditorName; }
            set
            {
                _ShowEditorName = value;
                RaisePropertyChanged("ShowEditorName");
            }
        }
        #endregion
        #region ShowEditedDate
        private bool _ShowEditedDate;
        public bool ShowEditedDate
        {
            get { return _ShowEditedDate; }
            set
            {
                _ShowEditedDate = value;
                RaisePropertyChanged("ShowEditedDate");
            }
        }
        #endregion
        #region ShowCustCol1
        private bool _ShowCustCol1;
        public bool ShowCustCol1
        {
            get { return _ShowCustCol1; }
            set
            {
                _ShowCustCol1 = value;
                RaisePropertyChanged("ShowCustCol1");
            }
        }
        #endregion
        #region ShowCustCol2
        private bool _ShowCustCol2;
        public bool ShowCustCol2
        {
            get { return _ShowCustCol2; }
            set
            {
                _ShowCustCol2 = value;
                RaisePropertyChanged("ShowCustCol2");
            }
        }
        #endregion
        #region ShowCustCol3
        private bool _ShowCustCol3;
        public bool ShowCustCol3
        {
            get { return _ShowCustCol3; }
            set
            {
                _ShowCustCol3 = value;
                RaisePropertyChanged("ShowCustCol3");
            }
        }
        #endregion
        #region ShowCustCol4
        private bool _ShowCustCol4;
        public bool ShowCustCol4
        {
            get { return _ShowCustCol4; }
            set
            {
                _ShowCustCol4 = value;
                RaisePropertyChanged("ShowCustCol4");
            }
        }
        #endregion
        #region ShowCustCol5
        private bool _ShowCustCol5;
        public bool ShowCustCol5
        {
            get { return _ShowCustCol5; }
            set
            {
                _ShowCustCol5 = value;
                RaisePropertyChanged("ShowCustCol5");
            }
        }
        #endregion
        #region ShowCustCol6
        private bool _ShowCustCol6;
        public bool ShowCustCol6
        {
            get { return _ShowCustCol6; }
            set
            {
                _ShowCustCol6 = value;
                RaisePropertyChanged("ShowCustCol6");
            }
        }
        #endregion
        #region ShowCustCol7
        private bool _ShowCustCol7;
        public bool ShowCustCol7
        {
            get { return _ShowCustCol7; }
            set
            {
                _ShowCustCol7 = value;
                RaisePropertyChanged("ShowCustCol7");
            }
        }
        #endregion
        #region ShowCustCol8
        private bool _ShowCustCol8;
        public bool ShowCustCol8
        {
            get { return _ShowCustCol8; }
            set
            {
                _ShowCustCol8 = value;
                RaisePropertyChanged("ShowCustCol8");
            }
        }
        #endregion
        #region ShowCustCol9
        private bool _ShowCustCol9;
        public bool ShowCustCol9
        {
            get { return _ShowCustCol9; }
            set
            {
                _ShowCustCol9 = value;
                RaisePropertyChanged("ShowCustCol9");
            }
        }
        #endregion
        #region ShowCustCol0
        private bool _ShowCustCol0;
        public bool ShowCustCol0
        {
            get { return _ShowCustCol0; }
            set
            {
                _ShowCustCol0 = value;
                RaisePropertyChanged("ShowCustCol0");
            }
        }
        #endregion



        #region CanShowCreatorName
        public bool CanShowCreatorName
        {
            get { return true; }
        }
        #endregion
        #region CanShowCreatedDate
        public bool CanShowCreatedDate
        {
            get { return true; }
        }
        #endregion
        #region CanShowEditorName
        public bool CanShowEditorName
        {
            get { return true; }
        }
        #endregion
        #region CanShowEditedDate
        public bool CanShowEditedDate
        {
            get { return true; }
        }
        #endregion
        #region CanShowCustCol1
        public bool CanShowCustCol1
        {
            get { return APOCCore.STCI.ContactCustColEnable1; }
        }
        #endregion
        #region CanShowCustCol2
        public bool CanShowCustCol2
        {
            get { return APOCCore.STCI.ContactCustColEnable2; }
        }
        #endregion
        #region CanShowCustCol3
        public bool CanShowCustCol3
        {
            get { return APOCCore.STCI.ContactCustColEnable3; }
        }
        #endregion
        #region CanShowCustCol4
        public bool CanShowCustCol4
        {
            get { return APOCCore.STCI.ContactCustColEnable4; }
        }
        #endregion
        #region CanShowCustCol5
        public bool CanShowCustCol5
        {
            get { return APOCCore.STCI.ContactCustColEnable5; }
        }
        #endregion
        #region CanShowCustCol6
        public bool CanShowCustCol6
        {
            get { return APOCCore.STCI.ContactCustColEnable6; }
        }
        #endregion
        #region CanShowCustCol7
        public bool CanShowCustCol7
        {
            get { return APOCCore.STCI.ContactCustColEnable7; }
        }
        #endregion
        #region CanShowCustCol8
        public bool CanShowCustCol8
        {
            get { return APOCCore.STCI.ContactCustColEnable8; }
        }
        #endregion
        #region CanShowCustCol9
        public bool CanShowCustCol9
        {
            get { return APOCCore.STCI.ContactCustColEnable9; }
        }
        #endregion
        #region CanShowCustCol0
        public bool CanShowCustCol0
        {
            get { return APOCCore.STCI.ContactCustColEnable0; }
        }
        #endregion


        #region AutoSaveProfile
        private bool _AutoSaveProfile;
        public bool AutoSaveProfile
        {
            get { return _AutoSaveProfile; }
            set
            {
                _AutoSaveProfile = value;
                RaisePropertyChanged("AutoSaveProfile");
            }
        }
        #endregion

        #region ContactModuleList
        public List<string> ContactModuleList
        {
            get
            {
                var q = from n in APOCContactModule.GetList() select n.Title;
                return !q.Any() ? null : q.ToList();
            }
        } 
        #endregion

        #region SelectedContactModule
        private string _SelectedContactModule;
        public string SelectedContactModule
        {
            get { return _SelectedContactModule; }
            set
            {
                _SelectedContactModule = value;
                RaisePropertyChanged("SelectedContactModule");
            }
        }
        #endregion

        #region ContactFastRegistration
        private bool _ContactFastRegistration;
        public bool ContactFastRegistration
        {
            get { return _ContactFastRegistration; }
            set
            {
                _ContactFastRegistration = value;
                RaisePropertyChanged("ContactFastRegistration");
            }
        }
        #endregion
        


        #region ContactCatList
        private ObservableCollection<CacheItemContactCat> _ContactCatList;
        public ObservableCollection<CacheItemContactCat> ContactCatList
        {
            get { return _ContactCatList; }
            set
            {
                _ContactCatList = value;
                RaisePropertyChanged("ContactCatList");
            }
        }
        #endregion






        #region [METHODS]
        private void InitDynamics()
        {
            DynamicListBoxCat = MainView.DynamicListBoxCat;
            DynamicListBoxCat.SelectedIndexChanged +=
                (s, e) =>
                {

                };

        }

        private void PopulateContactCatList()
        {
            ContactCatList = new ObservableCollection<CacheItemContactCat>();

            MCacheData
                .GetContactCatList()
                .Where(n => (
                    AMembership.ActiveUser.UserName.ToLower() == "admin" || (
                    n.Role != null &&
                    AMembership.ActiveUser.Roles != null &&
                    AMembership.ActiveUser.Roles.Contains(n.Role))))
                .ToList()
                .ForEach(n => ContactCatList.Add(n));
        }

        private void LoadSettings()
        {
            HelperUtils.Try(() => { SearchInPhone = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInPhone); });
            HelperUtils.Try(() => { SearchInAddress = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInAddress); });
            HelperUtils.Try(() => { SearchInProfile = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInProfile); });
            HelperUtils.Try(() => { SearchInEmail = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInEmail); });
            HelperUtils.Try(() => { SearchInSMS = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInSMS); });

            HelperUtils.Try(() => { SearchInCustCol1 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable1); });
            HelperUtils.Try(() => { SearchInCustCol2 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable2); });
            HelperUtils.Try(() => { SearchInCustCol3 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable3); });
            HelperUtils.Try(() => { SearchInCustCol4 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable4); });
            HelperUtils.Try(() => { SearchInCustCol5 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable5); });
            HelperUtils.Try(() => { SearchInCustCol6 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable6); });
            HelperUtils.Try(() => { SearchInCustCol7 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable7); });
            HelperUtils.Try(() => { SearchInCustCol8 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable8); });
            HelperUtils.Try(() => { SearchInCustCol9 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable9); });
            HelperUtils.Try(() => { SearchInCustCol0 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable0); });

            HelperUtils.Try(() => { ShowCreatedDate = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowCreatedDate); });
            HelperUtils.Try(() => { ShowCreatorName = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowCreatorName); });
            HelperUtils.Try(() => { ShowEditedDate = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowEditedDate); });
            HelperUtils.Try(() => { ShowEditorName = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowEditorName); });
            HelperUtils.Try(() => { ShowCustCol1 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow1); });
            HelperUtils.Try(() => { ShowCustCol2 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow2); });
            HelperUtils.Try(() => { ShowCustCol3 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow3); });
            HelperUtils.Try(() => { ShowCustCol4 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow4); });
            HelperUtils.Try(() => { ShowCustCol5 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow5); });
            HelperUtils.Try(() => { ShowCustCol6 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow6); });
            HelperUtils.Try(() => { ShowCustCol7 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow7); });
            HelperUtils.Try(() => { ShowCustCol8 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow8); });
            HelperUtils.Try(() => { ShowCustCol9 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow9); });
            HelperUtils.Try(() => { ShowCustCol0 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow0); });

            HelperUtils.Try(() => { AutoSaveProfile = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ProfileAutoSave); });
            HelperUtils.Try(() =>
            {
                var v = DBMSSetting2.LoadSettings<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactInitModule);
                SelectedContactModule = (from n in ContactModuleList where n == v select n).FirstOrDefault();
            });

            HelperUtils.Try(() =>
            {
                var catsString = DBMSSetting2.LoadSettings<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.DefaultCategoryOnCreation);
                var cats = catsString.Split(',');

                foreach (var v in cats)
                {
                    var item = (from n in ContactCatList where n.Title == v select n).FirstOrDefault();
                    if (item != null)
                        DynamicListBoxCat.SelectedItems.Add(item);
                }
            });

            HelperUtils.Try(() => { ContactFastRegistration = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactFastRegistration); });
        }

        private void InitCommands()
        {
            CommandOK = new RelayCommand(
                () =>
                {
                    if (Validate())
                        if (Save())
                            RaiseRequestClose(true);
                }, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp31 != "");
        }
        private bool Validate()
        {
            return true;
        }
        private bool Save()
        {
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInPhone, SearchInPhone);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInAddress, SearchInAddress);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInProfile, SearchInProfile);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInEmail, SearchInEmail);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactSearchInSMS, SearchInSMS);

            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable1, SearchInCustCol1);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable2, SearchInCustCol2);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable3, SearchInCustCol3);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable4, SearchInCustCol4);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable5, SearchInCustCol5);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable6, SearchInCustCol6);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable7, SearchInCustCol7);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable8, SearchInCustCol8);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable9, SearchInCustCol9);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnSearchable0, SearchInCustCol0);



            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowCreatorName, ShowCreatorName);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowCreatedDate, ShowCreatedDate);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowEditorName, ShowEditorName);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactShowEditedDate, ShowEditedDate);

            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow1, ShowCustCol1);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow2, ShowCustCol2);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow3, ShowCustCol3);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow4, ShowCustCol4);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow5, ShowCustCol5);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow6, ShowCustCol6);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow7, ShowCustCol7);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow8, ShowCustCol8);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow9, ShowCustCol9);
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactCustomColumnShow0, ShowCustCol0);



            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ProfileAutoSave, AutoSaveProfile);
            DBMSSetting2.SaveSetting<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactInitModule, SelectedContactModule);

            try
            {
                var cats = (from n in DynamicListBoxCat.SelectedItems.Cast<CacheItemContactCat>()
                            select ((DBCTContactCat)n.Tag).Title).ToList().Aggregate((i, j) => i + "," + j);
                DBMSSetting2.SaveSetting<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID,
                    ConstantDBSettings.DefaultCategoryOnCreation, cats);
            }
            catch
            {
                DBMSSetting2.SaveSetting<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID,
                    ConstantDBSettings.DefaultCategoryOnCreation, "");
            }
            DBMSSetting2.SaveSetting<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.ContactFastRegistration, ContactFastRegistration);
            return true;
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp31);
        } 
        #endregion


        #region [COMMANDS]
        public RelayCommand CommandOK { get; set; }
        public RelayCommand CommandCancel { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IRequestCloseViewModel
        public event EventHandler<RequestCloseEventArgs> RequestClose;
        private void RaiseRequestClose(bool? dialogResult)
        {
            if (RequestClose != null)
                RequestClose.Invoke(this, new RequestCloseEventArgs { DialogResult = dialogResult });
        }
        #endregion
    }
}
