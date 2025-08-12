using System;
using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpo;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using Microsoft.Practices.ServiceLocation;
using POL.WPF.Controles.MVVM;

namespace POC.Module.Call.Views
{
    public partial class UPopupCallerID : UserControl, INotifyPropertyChanged, IPopupItem
    {
        private POCCore APOCCore { get; set; }
        private IPopup APopup { get; set; }
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }

        public UPopupCallerID(CallInfo call, ContactInfo contact)
        {
            InitializeComponent();

            Call = call;
            Contact = contact;

            FlowDirection = HelperLocalize.ApplicationFlowDirection;

            if (FlowDirection == FlowDirection.RightToLeft)
                tbPhoneNumberFull.HorizontalAlignment = HorizontalAlignment.Left;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
                APopup = ServiceLocator.Current.GetInstance<IPopup>();
                ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                AMembership = ServiceLocator.Current.GetInstance<IMembership>();
                APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                InitCommands();
            }


            Loaded += (s, e) =>
            {
                DataContext = this;

                if (Call != null)
                {
                    LineTitle = String.Format("خط : {0} - {1}", call.Line,
                                               APOCCore.LineNames==null? string.Empty:( APOCCore.LineNames.ContainsKey(call.Line)
                                                  ? APOCCore.LineNames[call.Line]
                                                  : string.Empty));
                    LastTalk = string.Format("( {0} )", Call.LastExt.ToString());
                }
                var window = Window.GetWindow(this);
                if (window != null)
                    window.Closed +=
                        (s1, e1) =>
                        {
                            _isclosed = true;
                            APopup.RemovePopup(this);
                        };

                if (!string.IsNullOrWhiteSpace(contact.Cats))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", "دسته", contact.Cats) };
                    spProfile.Children.Add(tb);
                }

                if (APOCCore.CallPopupCustCol1 && !string.IsNullOrWhiteSpace(contact.CCText1))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle1, contact.CCText1) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol2 && !string.IsNullOrWhiteSpace(contact.CCText2))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle2, contact.CCText2) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol3 && !string.IsNullOrWhiteSpace(contact.CCText3))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle3, contact.CCText3) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol4 && !string.IsNullOrWhiteSpace(contact.CCText4))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle4, contact.CCText4) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol5 && !string.IsNullOrWhiteSpace(contact.CCText5))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle5, contact.CCText5) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol6 && !string.IsNullOrWhiteSpace(contact.CCText6))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle6, contact.CCText6) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol7 && !string.IsNullOrWhiteSpace(contact.CCText7))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle7, contact.CCText7) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol8 && !string.IsNullOrWhiteSpace(contact.CCText8))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle8, contact.CCText8) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol9 && !string.IsNullOrWhiteSpace(contact.CCText9))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle9, contact.CCText9) };
                    spProfile.Children.Add(tb);
                }
                if (APOCCore.CallPopupCustCol0 && !string.IsNullOrWhiteSpace(contact.CCText0))
                {
                    var tb = new TextBlock() { Margin = new Thickness(3), Text = string.Format("{0} : {1}", APOCCore.STCI.ContactCustColTitle0, contact.CCText0) };
                    spProfile.Children.Add(tb);
                }
                UpdateLayout();

                if (APOCCore.CallPopupSoundIsActive)
                {
                    HelperUtils.Try(
                        () =>
                        {
                            var file = System.IO.Path.Combine(APOCCore.SoundTrackPath,
                                                              APOCCore.CallPopupSoundTrackIndex + ".wav");
                            var sound = new SoundPlayer(file);
                            sound.Play();
                        });
                }

                var w = Window.GetWindow(this);
                if (w != null)
                    w.Closing +=
                        (s1, e1) =>
                        {
                            if (!string.IsNullOrWhiteSpace(NoteText))
                            {
                                using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                                {
                                    var dbcall = DBCLCall.FindByOid(uow, call.Oid);
                                    if (dbcall != null)
                                    {
                                        dbcall.NoteFlag = NoteImageIndex;
                                        dbcall.NoteText = NoteText;
                                        dbcall.NoteWriter = AMembership.ActiveUser.UserName;
                                        dbcall.Save();
                                        uow.CommitChanges();
                                    }
                                }
                            }
                        };
            };
        }


        #region Call
        public CallInfo Call { get; set; }
        #endregion
        #region Contact
        public ContactInfo Contact { get; set; }
        #endregion
        #region LineTitle
        private string _LineTitle;
        public string LineTitle
        {
            get
            {
                return _LineTitle;
            }
            set
            {
                if (_LineTitle == value)
                    return;
                _LineTitle = value;
                RaisePropertyChanged("LineTitle");
            }
        }
        #endregion
        #region PhoneNumberFull
        public string PhoneNumberFull
        {
            get
            {
                var rv = string.Empty;
                if (Call != null)
                {
                    if (Call.CountryCode >= 0)
                        rv += String.Format("+({0})", Call.CountryCode);
                    if (Call.CityCode >= 0)
                        rv += String.Format(" {0}", Call.CityCode);
                    rv += " " + Call.Phone;
                }
                return rv;
            }
        }
        #endregion
        #region ContactTitle
        public string ContactTitle
        {
            get
            {
                var rv = "{ نامشخص }";
                if (Contact.Code >= 0)
                {
                    rv = String.Format("{0} - {1}", Contact.Code, Contact.Title);
                }
                return rv;
            }
        }
        #endregion
        #region PhoneInternal
        public string PhoneInternal
        {
            get
            {
                return Call != null ? Call.PhoneInternal : string.Empty;
            }
        }
        #endregion
        #region ExtraCodeTitle
        public string ExtraCodeTitle
        {
            get
            {
                return Call != null ? Call.ExtraCodeTitle : string.Empty;
            }
        }
        #endregion

        

        #region HasImage
        public Visibility HasImage
        {
            get { return APOCCore.CallPopupImageIsActive ? Visibility.Visible : Visibility.Collapsed; }
        }
        #endregion

        #region ImageSource
        private byte[] _ImageSource;
        public byte[] ImageSource
        {
            get
            {
                if (!Equals(_ImageSource, null)) return _ImageSource;
                if (!APOCCore.CallPopupImageIsActive) return null;

                var dbv = DBCTProfileValue.FindByContactAndItem(ADatabase.Dxs, Contact.Oid, APOCCore.CallPopupImageProfile);
                if (dbv == null) return null;
                var dbb = DBCTBytes.FindByOid(ADatabase.Dxs, dbv.Guid1);
                if (dbb == null) return null;
                _ImageSource = dbb.DataByte;
                return _ImageSource;
            }
        }
        #endregion

        #region LastTalk
        private string _LastTalk;
        public string LastTalk
        {
            get
            {
                return _LastTalk;
            }
            set
            {
                if (_LastTalk == value)
                    return;
                _LastTalk = value;
                RaisePropertyChanged("LastTalk");
            }
        }
        #endregion
        

        public Visibility OldStatVisibility
        {
            get { return APOCCore.CallShowOldCallStat ? Visibility.Visible : Visibility.Collapsed; }
        }
        public string StatCountIn
        {
            get
            {
                return Call.CallStat == null ? "?" : Call.CallStat.CountIn.ToString(CultureInfo.InvariantCulture);
            }
        }
        public string StatCountOut
        {
            get
            {
                return Call.CallStat == null ? "?" : Call.CallStat.CountOut.ToString(CultureInfo.InvariantCulture);
            }
        }
        public string StatCountMiss
        {
            get
            {
                return Call.CallStat == null ? "?" : Call.CallStat.MissCall.ToString(CultureInfo.InvariantCulture);
            }
        }
        public string StatLastCallElapsed
        {
            get
            {
                if (Call.CallStat == null) return "نامشخص";
                var elapsed = DateTime.Now - Call.CallStat.LastCallDate;
                if (elapsed.TotalHours < 24)
                {
                    return DateTime.Now.Day == Call.CallStat.LastCallDate.Day ? "امروز" : "دیروز";
                }
                return ((int)elapsed.TotalDays) + " روز پیش";
            }
        }

        public Visibility TodayStatVisibility
        {
            get { return APOCCore.CallShowTodayCallStat ? Visibility.Visible : Visibility.Collapsed; }
        }
        public string TodayStatCountIn
        {
            get
            {
                return Call.TodayCallStat == null ? "?" : Call.TodayCallStat.CountIn.ToString(CultureInfo.InvariantCulture);
            }
        }
        public string TodayStatCountOut
        {
            get
            {
                return Call.TodayCallStat == null ? "?" : Call.TodayCallStat.CountOut.ToString(CultureInfo.InvariantCulture);
            }
        }
        public string TodayStatCountMiss
        {
            get
            {
                return Call.TodayCallStat == null ? "?" : Call.TodayCallStat.MissCall.ToString(CultureInfo.InvariantCulture);
            }
        }
        public string TodayStatLastCallElapsed
        {
            get
            {
                if (Call.TodayCallStat == null) return "نامشخص";
                var elapsed = DateTime.Now - Call.TodayCallStat.LastCallDate;
                return ((int)elapsed.TotalHours) + " ساعت پیش";
            }
        }



        #region ContactCreator
        public Visibility ContactCreatorVisibility
        {
            get { return (!APOCCore.CallShowContactCreator || Contact == null || string.IsNullOrWhiteSpace(Contact.Creator) ? Visibility.Collapsed : Visibility.Visible); }
        }
        public string ContactCreator
        {
            get
            {
                return string.Format("ثبت كننده : {0}", (Contact == null ? "نامشخص" : Contact.Creator));
            }
        }

        public Visibility ContactCreatedVisibility
        {
            get { return (!APOCCore.CallShowContactCreatedDate || Contact == null || Contact.CreatedDate == DateTime.MinValue ? Visibility.Collapsed : Visibility.Visible); }
        }
        public string ContactCreated
        {
            get
            {
                return string.Format("تاریخ ثبت : {0}", (Contact == null ? "نامشخص" : HelperLocalize.DateTimeToString(Contact.CreatedDate, EnumCalendarType.ApplicationSettings, HelperLocalize.ApplicationDateTimeFormat)));
            }
        }
        #endregion
        #region ContactEditor
        public Visibility ContactEditorVisibility
        {
            get { return (!APOCCore.CallShowContactEditor || Contact == null || string.IsNullOrWhiteSpace(Contact.Editor) ? Visibility.Collapsed : Visibility.Visible); }
        }
        public string ContactEditor
        {
            get
            {
                return string.Format("ویرایش كننده : {0}", (Contact == null ? "نامشخص" : Contact.Editor));
            }
        }

        public Visibility ContactEditedVisibility
        {
            get { return (!APOCCore.CallShowContactEditedDate || Contact == null || Contact.EditedDate == DateTime.MinValue ? Visibility.Collapsed : Visibility.Visible); }
        }
        public string ContactEdited
        {
            get
            {
                return string.Format("تاریخ ویرایش : {0}", (Contact == null ? "نامشخص" : HelperLocalize.DateTimeToString(Contact.EditedDate, EnumCalendarType.ApplicationSettings, HelperLocalize.ApplicationDateTimeFormat)));
            }
        }
        #endregion





        public Visibility CallNoteVisibility
        {
            get { return (APOCCore.CallAllowAddNote ? Visibility.Visible : Visibility.Collapsed); }
        }

        #region NoteImage
        private BitmapImage _NoteImage;
        public BitmapImage NoteImage
        {
            get { return _NoteImage; }
            set
            {
                if (Equals(value, _NoteImage))
                    return;
                _NoteImage = value;
                RaisePropertyChanged("NoteImage");
            }
        }
        #endregion

        #region NoteText
        private string _NoteText;
        public string NoteText
        {
            get { return _NoteText; }
            set
            {
                if (value == _NoteText)
                    return;

                _NoteText = value;
                RaisePropertyChanged("NoteText");
            }
        }
        #endregion


        public Visibility CardTableClickEnabled
        {
            get { return APOCCore.CallAllowCardTable ? Visibility.Visible : Visibility.Collapsed; }
        }


        private int NoteImageIndex { get; set; }

        private void InitCommands()
        {
            cmdContactClick = new RelayCommand<object>(o => ContactClick(), o => true);
            CommandChangeNoteImage = new RelayCommand(
                () =>
                {
                    NoteImageIndex++;
                    if (NoteImageIndex > 5) NoteImageIndex = 0;
                    switch (NoteImageIndex)
                    {
                        case 1:
                            NoteImage = HelperImage.GetSpecialImage16("_16_Flag_Gray.png");
                            break;
                        case 2:
                            NoteImage = HelperImage.GetSpecialImage16("_16_Flag_Yellow.png");
                            break;
                        case 3:
                            NoteImage = HelperImage.GetSpecialImage16("_16_Flag_Red.png");
                            break;
                        case 4:
                            NoteImage = HelperImage.GetSpecialImage16("_16_Flag_Green.png");
                            break;
                        case 5:
                            NoteImage = HelperImage.GetSpecialImage16("_16_Flag_Blue.png");
                            break;
                        default:
                            NoteImage = null;
                            break;
                    }

                });
            CommandCardTableClick = new RelayCommand(CardTableClick);
        }

        private void CardTableClick()
        {
            DBCTContact dbc = null;
            try
            {
                dbc = DBCTContact.FindByOid(ADatabase.Dxs, Contact.Oid);
            }
            catch
            {
            }
            APOCMainWindow.ShowAddCardTable(null, "كارتابل : " + (dbc == null ? string.Empty : dbc.Title), "",
                null, dbc, null, null, null);
        }

        private void ContactClick()
        {
            if (Contact.Code < 0)
            {
                var dbc = DBCLCall.FindByOid(ADatabase.Dxs, Call.Oid);
                if (dbc == null) return;
                var apocMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                var dbcall = apocMainWindow.ShowCallSync(null, dbc) as DBCLCall;
                if (dbcall != null && dbcall.Contact != null)
                {
                    var poc = ServiceLocator.Current.GetInstance<IPOCContactModule>();
                    poc.GotoContactByCode(dbcall.Contact.Code);

                }
                var window = Window.GetWindow(this);
                if (window != null) window.Close();
            }
            else
            {
                var dbc = DBCTContact.FindByOid(ADatabase.Dxs, Contact.Oid);
                if (dbc == null) return;
                var apocContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
                apocContactModule.GotoContactByCode(Contact.Code);
                var window = Window.GetWindow(this);
                if (window != null) window.Close();
            }
        }

        #region [COMMANDS]
        public RelayCommand<object> cmdContactClick { get; set; }
        public RelayCommand CommandChangeNoteImage { get; set; }
        public RelayCommand CommandCardTableClick { get; set; }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string pname)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(pname));
        }
        #endregion

        #region IPopupItem

        private Brush _PopupBrush = Brushes.Transparent;
        public Brush PopupBrush
        {
            get
            {
                if (!Equals(_PopupBrush, Brushes.Transparent)) return _PopupBrush;
                if (APOCCore.CallPopupColorIsFixed)
                    _PopupBrush = new SolidColorBrush(APOCCore.CallPopupColorFixed);
                else
                {
                    var dbv = DBCTProfileValue.FindByContactAndItem(ADatabase.Dxs, Contact.Oid, APOCCore.CallPopupColorProfile);
                    _PopupBrush = (dbv == null ? Brushes.White : new SolidColorBrush(dbv.GetColorValue()));
                }
                return _PopupBrush;
            }
        }

        public bool PopupCanClose
        {
            get { return true; }
        }

        public bool PopupCanPin
        {
            get { return APOCCore.CallPopupDurationIndex < 6; }
        }

        public bool PopupCanTimeOut
        {
            get { return APOCCore.CallPopupDurationIndex < 6; }
        }

        public UIElement PopupElement
        {
            get { return this; }
        }

        public double PopupHeight
        {
            get
            {
                var rv = 120.0 + APOCCore.CallPopupSizeIndex * 30;
                if (rv > 240) 
                {
                    rv = double.NaN;
                }
                return rv;
            }
        }

        public TimeSpan PopupTimeOut
        {
            get
            {
                switch (APOCCore.CallPopupDurationIndex)
                {
                    case 0:
                        return TimeSpan.FromSeconds(0);
                    case 1:
                        return TimeSpan.FromSeconds(10);
                    case 2:
                        return TimeSpan.FromSeconds(20);
                    case 3:
                        return TimeSpan.FromSeconds(30);
                    case 4:
                        return TimeSpan.FromMinutes(1);
                    case 5:
                        return TimeSpan.FromMinutes(5);
                }
                return TimeSpan.FromDays(1);
            }
        }

        public double PopupWidth
        {
            get { return APOCCore.CallPopupImageIsActive ? 320 + 64 + 8 : 320; }
        }

        private bool _isclosed = false;
        public bool IsClosed
        {
            get { return _isclosed; }
        }
        #endregion

    }
}
