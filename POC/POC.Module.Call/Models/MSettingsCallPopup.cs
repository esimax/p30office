using System.Media;
using System.Windows;
using System.Windows.Media;
using POL.DB.Membership;
using POL.DB.P30Office;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using POL.Lib.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;

namespace POC.Module.Call.Models
{
    public class MSettingsCallPopup : NotifyObjectBase
    {
        private IMembership AMembership { get; set; }
        private IDatabase ADatabase { get; set; }
        private POCCore APOCCore { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }
        private ICacheData ACacheData { get; set; }

        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private bool SoundIsPlaying { get; set; }
        private bool InLoadMode { get; set; }



        public MSettingsCallPopup(object mainView)
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
                    DBMSSetting2.InLoadMode = true;
                    RaisePropertyChanged("IsAuthorizedVis");
                    RaisePropertyChanged("IsNotAuthorizedVis");

                    if (e.Status == EnumMembershipStatus.AfterLogin)
                    {

                        PopulateProfileColorList();
                        PopulateProfileImageList();
                        PopulateSoundTrackList();


                        HelperUtils.Try(() => { HorizontalAlignmentOnLeft = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.PopupHorizontalAlignmentOnLeft); });

                        HelperUtils.Try(() => { SelectedDurationIndex = DBMSSetting2.LoadSettings<int>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupDurationIndex); });
                        HelperUtils.Try(() => { SelectedSizeIndex = DBMSSetting2.LoadSettings<int>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupSizeIndex); });
                        HelperUtils.Try(() => { ColorIsFixed = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupColorIsFixed); });

                        HelperUtils.Try(
                            () =>
                            {
                                var v = DBMSSetting2.LoadSettings<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupColorFixed);
                                var convertFromString = ColorConverter.ConvertFromString(v);
                                if (convertFromString != null) ColorFixed = (Color)convertFromString;
                            });
                        HelperUtils.Try(
                            () =>
                            {
                                ProfileColorSelected = null;
                                var sguid = DBMSSetting2.LoadSettings<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupColorProfile);
                                var oid = Guid.Parse(sguid);
                                var db = DBCTProfileItem.FindByOid(ADatabase.Dxs, oid);
                                var field = ADataFieldManager.FindByType(db.ItemType);
                                ProfileColorSelected = new Tuple<object, Uri, Thickness>(db, field == null ? null : new Uri(field.ImageUriString), new Thickness(32, 0, 0, 0));
                            });
                        HelperUtils.Try(() => { ImageIsActive = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupImageIsActive); });
                        HelperUtils.Try(
                            () =>
                            {
                                ProfileImageSelected = null;
                                var sguid = DBMSSetting2.LoadSettings<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupImageProfile);
                                var oid = Guid.Parse(sguid);
                                var db = DBCTProfileItem.FindByOid(ADatabase.Dxs, oid);
                                var field = ADataFieldManager.FindByType(db.ItemType);
                                ProfileImageSelected = new Tuple<object, Uri, Thickness>(db, field == null ? null : new Uri(field.ImageUriString), new Thickness(32, 0, 0, 0));
                            });

                        HelperUtils.Try(() => { SoundIsActive = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupSoundIsActive); });
                        HelperUtils.Try(() => { SoundTrackSelected = DBMSSetting2.LoadSettings<string>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupSoundTrackIndex); });

                        HelperUtils.Try(() => { CCSelected1 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol1); });
                        HelperUtils.Try(() => { CCSelected2 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol2); });
                        HelperUtils.Try(() => { CCSelected3 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol3); });
                        HelperUtils.Try(() => { CCSelected4 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol4); });
                        HelperUtils.Try(() => { CCSelected5 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol5); });
                        HelperUtils.Try(() => { CCSelected6 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol6); });
                        HelperUtils.Try(() => { CCSelected7 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol7); });
                        HelperUtils.Try(() => { CCSelected8 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol8); });
                        HelperUtils.Try(() => { CCSelected9 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol9); });
                        HelperUtils.Try(() => { CCSelected0 = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol0); });

                        Enumerable.Range(0, 10).ToList().ForEach(
                            i =>
                            {
                                RaisePropertyChanged(string.Format("CCEnable{0}", i));
                                RaisePropertyChanged(string.Format("CCText{0}", i));
                            });

                        HelperUtils.Try(() => { ExportAsPopFile = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupExportAsPopFile); });

                        HelperUtils.Try(() => { ShowTodayCallStat = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowTodayCallStat); });
                        HelperUtils.Try(() => { ShowOldCallStat = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowOldCallStat); });
                        HelperUtils.Try(() => { ShowContactCreator = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowContactCreator); });
                        HelperUtils.Try(() => { ShowContactCreatedDate = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowContactCreatedDate); });
                        HelperUtils.Try(() => { ShowContactEditor = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowContactEditor); });
                        HelperUtils.Try(() => { ShowContactEditedDate = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowContactEditedDate); });
                        HelperUtils.Try(() => { ShowLastNote = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowLastNote); });
                        HelperUtils.Try(() => { AllowAddNote = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallAllowAddNote); });

                        try
                        {
                            APOCCore.CallOnlyInternal = !string.IsNullOrWhiteSpace(AMembership.ActiveUser.InternalPhone);
                            APOCCore.CallInternalCode = AMembership.ActiveUser.InternalPhone;
                        }
                        catch
                        {

                        }

                        HelperUtils.Try(() => { AllowCardTable = DBMSSetting2.LoadSettings<bool>(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallAllowCardTable); });
                        DBMSSetting2.InLoadMode = false;

                    }
                };
            InitCommands();
            InitDynamics();
        }



        #region HorizontalAlignmentOnLeft
        private bool _HorizontalAlignmentOnLeft;
        public bool HorizontalAlignmentOnLeft
        {
            get { return _HorizontalAlignmentOnLeft; }
            set
            {
                _HorizontalAlignmentOnLeft = value;
                RaisePropertyChanged("HorizontalAlignmentOnLeft");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.PopupHorizontalAlignmentOnLeft, _HorizontalAlignmentOnLeft);
                APOCCore.PopupHorizontalAlignmentOnLeft = value;
            }
        }
        #endregion
        #region IsAuthorizedVis
        public Visibility IsAuthorizedVis { get { return AMembership.IsAuthorized ? Visibility.Visible : Visibility.Collapsed; } }
        #endregion
        #region IsNotAuthorizedVis
        public Visibility IsNotAuthorizedVis { get { return !AMembership.IsAuthorized ? Visibility.Visible : Visibility.Collapsed; } }
        #endregion

        #region SelectedDurationIndex
        private int _SelectedDurationIndex;
        public int SelectedDurationIndex
        {
            get { return _SelectedDurationIndex; }
            set
            {
                _SelectedDurationIndex = value;
                RaisePropertyChanged("SelectedDurationIndex");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupDurationIndex, _SelectedDurationIndex);
                APOCCore.CallPopupDurationIndex = value;
            }
        }
        #endregion
        #region SelectedSizeIndex
        private int _SelectedSizeIndex;
        public int SelectedSizeIndex
        {
            get { return _SelectedSizeIndex; }
            set
            {
                _SelectedSizeIndex = value;
                RaisePropertyChanged("SelectedSizeIndex");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupSizeIndex, _SelectedSizeIndex);
                APOCCore.CallPopupSizeIndex = value;
            }
        }
        #endregion

        #region ColorIsFixed
        private bool _ColorIsFixed;
        public bool ColorIsFixed
        {
            get { return _ColorIsFixed; }
            set
            {
                _ColorIsFixed = value;
                RaisePropertyChanged("ColorIsFixed");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupColorIsFixed, _ColorIsFixed);
                APOCCore.CallPopupColorIsFixed = value;
            }
        }
        #endregion
        #region ColorIsProfile
        public bool ColorIsProfile
        {
            get { return !_ColorIsFixed; }
            set
            {
                ColorIsFixed = !value;
                RaisePropertyChanged("ColorIsProfile");
                APOCCore.CallPopupColorIsFixed = !value;
            }
        }
        #endregion
        #region ColorFixed
        private Color _ColorFixed;
        public Color ColorFixed
        {
            get { return _ColorFixed; }
            set
            {
                _ColorFixed = value;
                RaisePropertyChanged("ColorFixed");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupColorFixed, _ColorFixed.ToString());
                APOCCore.CallPopupColorFixed = value;
            }
        }
        #endregion
        #region ProfileColorList
        public List<Tuple<object, Uri, Thickness>> ProfileColorList { get; set; }
        #endregion
        #region ProfileColorSelected
        private Tuple<object, Uri, Thickness> _ProfileColorSelected;
        public Tuple<object, Uri, Thickness> ProfileColorSelected
        {
            get
            {
                return _ProfileColorSelected;
            }
            set
            {
                if (Equals(_ProfileColorSelected, value))
                    return;
                if (value == null)
                {
                    _ProfileColorSelected = null;
                    return;
                }
                if (!(value.Item1 is DBCTProfileItem))
                {
                    _ProfileColorSelected = null;
                    APOCCore.CallPopupColorProfile = Guid.Empty;
                    DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupColorProfile, string.Empty);
                    return;
                }
                _ProfileColorSelected = value;
                var g = ((DBCTProfileItem)_ProfileColorSelected.Item1).Oid;
                APOCCore.CallPopupColorProfile = g;
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupColorProfile, g.ToString());
                RaisePropertyChanged("ProfileColorSelected");
            }
        }
        #endregion


        #region ImageIsActive
        private bool _ImageIsActive;
        public bool ImageIsActive
        {
            get { return _ImageIsActive; }
            set
            {
                _ImageIsActive = value;
                RaisePropertyChanged("ImageIsActive");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupImageIsActive, _ImageIsActive);
                APOCCore.CallPopupImageIsActive = value;
            }
        }
        #endregion
        #region ProfileImageList
        public List<Tuple<object, Uri, Thickness>> ProfileImageList { get; set; }
        #endregion
        #region ProfileImageSelected
        private Tuple<object, Uri, Thickness> _ProfileImageSelected;
        public Tuple<object, Uri, Thickness> ProfileImageSelected
        {
            get
            {
                return _ProfileImageSelected;
            }
            set
            {
                if (value == null)
                {
                    _ProfileImageSelected = null;
                    RaisePropertyChanged("ProfileImageSelected");
                    return;
                }
                if (Equals(_ProfileImageSelected, value))
                    return;
                if (!(value.Item1 is DBCTProfileItem))
                {
                    _ProfileImageSelected = null;
                    APOCCore.CallPopupImageProfile = Guid.Empty;
                    DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupImageProfile, string.Empty);
                    return;
                }
                _ProfileImageSelected = value;
                var g = ((DBCTProfileItem)_ProfileImageSelected.Item1).Oid;
                APOCCore.CallPopupImageProfile = g;
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupImageProfile, g.ToString());
                RaisePropertyChanged("ProfileImageSelected");
            }
        }
        #endregion

        #region SoundIsActive
        private bool _SoundIsActive;
        public bool SoundIsActive
        {
            get { return _SoundIsActive; }
            set
            {
                _SoundIsActive = value;
                RaisePropertyChanged("SoundIsActive");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupSoundIsActive, _SoundIsActive);
                APOCCore.CallPopupSoundIsActive = value;
            }
        }
        #endregion
        #region SoundTrackSelected
        private string _SoundTrackSelected;
        public string SoundTrackSelected
        {
            get { return _SoundTrackSelected; }
            set
            {
                _SoundTrackSelected = value;
                RaisePropertyChanged("SoundTrackSelected");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupSoundTrackIndex, _SoundTrackSelected);
                APOCCore.CallPopupSoundTrackIndex = value;
            }
        }
        #endregion

        #region SoundTrackList
        public List<string> SoundTrackList { get; set; }
        #endregion


        #region [Custome Columns]
        public bool CCEnable1 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable1; } }
        public bool CCEnable2 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable2; } }
        public bool CCEnable3 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable3; } }
        public bool CCEnable4 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable4; } }
        public bool CCEnable5 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable5; } }
        public bool CCEnable6 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable6; } }
        public bool CCEnable7 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable7; } }
        public bool CCEnable8 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable8; } }
        public bool CCEnable9 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable9; } }
        public bool CCEnable0 { get { return APOCCore.STCI != null && APOCCore.STCI.ContactCustColEnable0; } }

        public string CCText1 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle1 : string.Empty; } }
        public string CCText2 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle2 : string.Empty; } }
        public string CCText3 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle3 : string.Empty; } }
        public string CCText4 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle4 : string.Empty; } }
        public string CCText5 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle5 : string.Empty; } }
        public string CCText6 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle6 : string.Empty; } }
        public string CCText7 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle7 : string.Empty; } }
        public string CCText8 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle8 : string.Empty; } }
        public string CCText9 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle9 : string.Empty; } }
        public string CCText0 { get { return APOCCore.STCI != null ? APOCCore.STCI.ContactCustColTitle0 : string.Empty; } }

        private bool _CCSelected1;
        private bool _CCSelected2;
        private bool _CCSelected3;
        private bool _CCSelected4;
        private bool _CCSelected5;
        private bool _CCSelected6;
        private bool _CCSelected7;
        private bool _CCSelected8;
        private bool _CCSelected9;
        private bool _CCSelected0;

        public bool CCSelected1 { get { return _CCSelected1; } set { _CCSelected1 = value; RaisePropertyChanged("CCSelected1"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol1, _CCSelected1); APOCCore.CallPopupCustCol1 = value; } }
        public bool CCSelected2 { get { return _CCSelected2; } set { _CCSelected2 = value; RaisePropertyChanged("CCSelected2"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol2, _CCSelected2); APOCCore.CallPopupCustCol2 = value; } }
        public bool CCSelected3 { get { return _CCSelected3; } set { _CCSelected3 = value; RaisePropertyChanged("CCSelected3"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol3, _CCSelected3); APOCCore.CallPopupCustCol3 = value; } }
        public bool CCSelected4 { get { return _CCSelected4; } set { _CCSelected4 = value; RaisePropertyChanged("CCSelected4"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol4, _CCSelected4); APOCCore.CallPopupCustCol4 = value; } }
        public bool CCSelected5 { get { return _CCSelected5; } set { _CCSelected5 = value; RaisePropertyChanged("CCSelected5"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol5, _CCSelected5); APOCCore.CallPopupCustCol5 = value; } }
        public bool CCSelected6 { get { return _CCSelected6; } set { _CCSelected6 = value; RaisePropertyChanged("CCSelected6"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol6, _CCSelected6); APOCCore.CallPopupCustCol6 = value; } }
        public bool CCSelected7 { get { return _CCSelected7; } set { _CCSelected7 = value; RaisePropertyChanged("CCSelected7"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol7, _CCSelected7); APOCCore.CallPopupCustCol7 = value; } }
        public bool CCSelected8 { get { return _CCSelected8; } set { _CCSelected8 = value; RaisePropertyChanged("CCSelected8"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol8, _CCSelected8); APOCCore.CallPopupCustCol8 = value; } }
        public bool CCSelected9 { get { return _CCSelected9; } set { _CCSelected9 = value; RaisePropertyChanged("CCSelected9"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol9, _CCSelected9); APOCCore.CallPopupCustCol9 = value; } }
        public bool CCSelected0 { get { return _CCSelected0; } set { _CCSelected0 = value; RaisePropertyChanged("CCSelected0"); DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupCustCol0, _CCSelected0); APOCCore.CallPopupCustCol0 = value; } }

        #endregion

        #region ExportAsPopFile
        private bool _ExportAsPopFile;
        public bool ExportAsPopFile
        {
            get { return _ExportAsPopFile; }
            set
            {
                _ExportAsPopFile = value;
                RaisePropertyChanged("ExportAsPopFile");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallPopupExportAsPopFile, _ExportAsPopFile);
                APOCCore.CallPopupExportAsPopFile = value;
            }
        }
        #endregion


        #region ShowTodayCallStat
        private bool _ShowTodayCallStat;
        public bool ShowTodayCallStat
        {
            get { return _ShowTodayCallStat; }
            set
            {
                _ShowTodayCallStat = value;
                RaisePropertyChanged("ShowTodayCallStat");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowTodayCallStat, _ShowTodayCallStat);
                APOCCore.CallShowTodayCallStat = value;
            }
        }
        #endregion
        #region ShowOldCallStat
        private bool _ShowOldCallStat;
        public bool ShowOldCallStat
        {
            get { return _ShowOldCallStat; }
            set
            {
                _ShowOldCallStat = value;
                RaisePropertyChanged("ShowOldCallStat");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowOldCallStat, _ShowOldCallStat);
                APOCCore.CallShowOldCallStat = value;
            }
        }
        #endregion
        #region ShowContactCreator
        private bool _ShowContactCreator;
        public bool ShowContactCreator
        {
            get { return _ShowContactCreator; }
            set
            {
                _ShowContactCreator = value;
                RaisePropertyChanged("ShowContactCreator");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowContactCreator, _ShowContactCreator);
                APOCCore.CallShowContactCreator = value;
            }
        }
        #endregion
        #region ShowContactCreatedDate
        private bool _ShowContactCreatedDate;
        public bool ShowContactCreatedDate
        {
            get { return _ShowContactCreatedDate; }
            set
            {
                _ShowContactCreatedDate = value;
                RaisePropertyChanged("ShowContactCreatedDate");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowContactCreatedDate, _ShowContactCreatedDate);
                APOCCore.CallShowContactCreatedDate = value;
            }
        }
        #endregion
        #region ShowContactEditor
        private bool _ShowContactEditor;
        public bool ShowContactEditor
        {
            get { return _ShowContactEditor; }
            set
            {
                _ShowContactEditor = value;
                RaisePropertyChanged("ShowContactEditor");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowContactEditor, _ShowContactEditor);
                APOCCore.CallShowContactEditor = value;
            }
        }
        #endregion
        #region ShowContactEditedDate
        private bool _ShowContactEditedDate;
        public bool ShowContactEditedDate
        {
            get { return _ShowContactEditedDate; }
            set
            {
                _ShowContactEditedDate = value;
                RaisePropertyChanged("ShowContactEditedDate");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowContactEditedDate, _ShowContactEditedDate);
                APOCCore.CallShowContactEditedDate = value;
            }
        }
        #endregion
        #region ShowLastNote
        private bool _ShowLastNote;
        public bool ShowLastNote
        {
            get { return _ShowLastNote; }
            set
            {
                _ShowLastNote = value;
                RaisePropertyChanged("ShowLastNote");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallShowLastNote, _ShowLastNote);
                APOCCore.CallShowLastNote = value;
            }
        }
        #endregion
        #region AllowAddNote
        private bool _AllowAddNote;
        public bool AllowAddNote
        {
            get { return _AllowAddNote; }
            set
            {
                _AllowAddNote = value;
                RaisePropertyChanged("AllowAddNote");
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallAllowAddNote, _AllowAddNote);
                APOCCore.CallAllowAddNote = value;
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
                DBMSSetting2.SaveSetting(ADatabase.Dxs, AMembership.ActiveUser.UserID, ConstantDBSettings.CallAllowCardTable, _AllowCardTable);
                APOCCore.CallAllowCardTable = value;
            }
        }
        #endregion




        #region [COMMANDS]
        public RelayCommand CommandReloadProfileColorList { get; set; }
        public RelayCommand CommandReloadProfileImageList { get; set; }
        public RelayCommand CommandSoundTrackPlay { get; set; }
        #endregion






        #region [METHODS]
        private void InitDynamics()
        {
            DynamicOwner = Window.GetWindow((UIElement)MainView);
        }
        private void PopulateProfileColorList()
        {
            var tp = new Thickness(0);
            var tg = new Thickness(16, 0, 0, 0);
            var tm = new Thickness(32, 0, 0, 0);

            ProfileColorList = new List<Tuple<object, Uri, Thickness>>();
            var ps = (from n in ACacheData.GetProfileItemList() select (DBCTProfileRoot)n.Tag).ToList();
            foreach (var p in ps)
            {
                var list1 = new List<Tuple<object, Uri, Thickness>>
                                {
                                    new Tuple<object, Uri, Thickness>(p,
                                                                      new Uri(
                                                                          "pack://application:,,,/POL.Lib.Resources;component/Special/16/_16_TabPage.png",
                                                                          UriKind.Absolute), tp)
                                };
                var xpcg =
                    (from n in ACacheData.GetProfileItemList()
                     where ((DBCTProfileRoot)n.Tag).Oid == p.Oid
                     select n.ChildList.Select(m => (DBCTProfileGroup)m.Tag).ToList()).FirstOrDefault();
                if (xpcg == null) continue;
                foreach (var g in xpcg)
                {
                    var list2 = new List<Tuple<object, Uri, Thickness>>
                                    {
                                        new Tuple<object, Uri, Thickness>(g,
                                                                          new Uri(
                                                                              "pack://application:,,,/POL.Lib.Resources;component/Standard/16/_16_Group.png",
                                                                              UriKind.Absolute), tg)
                                    };
                    var xpci = (from r in ACacheData.GetProfileItemList()
                                from gr in r.ChildList
                                where ((DBCTProfileGroup)gr.Tag).Oid == g.Oid
                                select gr.ChildList.Select(m => (DBCTProfileItem)m.Tag).ToList()).FirstOrDefault();
                    if (xpci != null && xpci.Any())
                    {
                        list2.AddRange(from m in xpci
                                       let field = ADataFieldManager.FindByType(m.ItemType)
                                       where m.ItemType == EnumProfileItemType.Color
                                       select
                                           new Tuple<object, Uri, Thickness>(m,
                                                                             field == null
                                                                                 ? null
                                                                                 : new Uri(field.ImageUriString)
                                           ,
                                                                             tm));
                        if (list2.Count > 1)
                            list2.ForEach(list1.Add);
                    }
                }
                if (list1.Count > 1)
                    list1.ForEach(i => ProfileColorList.Add(i));
            }
            RaisePropertyChanged("ProfileColorList");
        }
        private void PopulateProfileImageList()
        {
            var tp = new Thickness(0);
            var tg = new Thickness(16, 0, 0, 0);
            var tm = new Thickness(32, 0, 0, 0);

            ProfileImageList = new List<Tuple<object, Uri, Thickness>>();
            var ps = (from n in ACacheData.GetProfileItemList() select (DBCTProfileRoot)n.Tag).ToList();
            foreach (var p in ps)
            {
                var list1 = new List<Tuple<object, Uri, Thickness>>
                                {
                                    new Tuple<object, Uri, Thickness>(p,
                                                                      new Uri(
                                                                          "pack://application:,,,/POL.Lib.Resources;component/Special/16/_16_TabPage.png",
                                                                          UriKind.Absolute), tp)
                                };
                var xpcg =
                    (from n in ACacheData.GetProfileItemList()
                     where ((DBCTProfileRoot)n.Tag).Oid == p.Oid
                     select n.ChildList.Select(m => (DBCTProfileGroup)m.Tag).ToList()).FirstOrDefault();
                if (xpcg != null && xpcg.Any())
                {
                    foreach (var g in xpcg)
                    {
                        var list2 = new List<Tuple<object, Uri, Thickness>>
                                        {
                                            new Tuple<object, Uri, Thickness>(g,
                                                                              new Uri(
                                                                                  "pack://application:,,,/POL.Lib.Resources;component/Standard/16/_16_Group.png",
                                                                                  UriKind.Absolute), tg)
                                        };
                        var xpci = (from r in ACacheData.GetProfileItemList()
                                    from gr in r.ChildList
                                    where ((DBCTProfileGroup)gr.Tag).Oid == g.Oid
                                    select gr.ChildList.Select(m => (DBCTProfileItem)m.Tag).ToList()).FirstOrDefault();
                        if (xpci != null && xpci.Any())
                        {
                            list2.AddRange(from m in xpci
                                           let field = ADataFieldManager.FindByType(m.ItemType)
                                           where m.ItemType == EnumProfileItemType.Image && m.Int1 == 64 && m.Int2 == 64
                                           select
                                               new Tuple<object, Uri, Thickness>(m,
                                                                                 field == null
                                                                                     ? null
                                                                                     : new Uri(field.ImageUriString)
                                               ,
                                                                                 tm));
                            if (list2.Count > 1)
                                list2.ForEach(list1.Add);
                        }
                    }
                }
                if (list1.Count > 1)
                    list1.ForEach(i => ProfileImageList.Add(i));
            }
            RaisePropertyChanged("ProfileImageList");
        }
        private void PopulateSoundTrackList()
        {
            var files = System.IO.Directory.GetFiles(APOCCore.SoundTrackPath, "*.wav");
            SoundTrackList = new List<string>();
            files.ToList().ForEach(
                f => SoundTrackList.Add(System.IO.Path.GetFileNameWithoutExtension(f)));
            RaisePropertyChanged("SoundTrackList");
        }



        private void InitCommands()
        {
            CommandReloadProfileColorList = new RelayCommand(ReloadProfileColorList, () => true);
            CommandReloadProfileImageList = new RelayCommand(ReloadProfileImageList, () => true);
            CommandSoundTrackPlay = new RelayCommand(SoundTrackPlay, () => !string.IsNullOrWhiteSpace(SoundTrackSelected));
        }

        private void SoundTrackPlay()
        {
            if (SoundIsPlaying)
                return;
            HelperUtils.Try(
                () =>
                {
                    SoundIsPlaying = true;
                    var file = System.IO.Path.Combine(APOCCore.SoundTrackPath, SoundTrackSelected + ".wav");
                    var s = new SoundPlayer(file);
                    HelperUtils.Try(s.Play);
                    Task.Factory.StartNew(
                        () =>
                        {
                            System.Threading.Thread.Sleep(4000);
                            HelperUtils.DoDispatcher(
                                () =>
                                {
                                    HelperUtils.Try(s.Stop);
                                    SoundIsPlaying = false;
                                });
                        });
                });
        }
        private void ReloadProfileColorList()
        {
            PopulateProfileColorList();
        }
        private void ReloadProfileImageList()
        {
            PopulateProfileImageList();
        }
        #endregion
    }
}
