using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using DevExpress.Xpo;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POL.WPF.DXControls;
using DevExpress.Data.Filtering;
using POL.Lib.Utils;
using POC.Module.SMS.Views;
using POL.Lib.Interfaces.SmsSettings;

namespace POC.Module.SMS.Models
{
    public class MSMSSend : NotifyObjectBase
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ICacheData ACacheData { get; set; }
        private POCCore APOCCore { get; set; }
        private dynamic MainView { get; set; }

        private const int TAB_SEND_TO = 0;
        private const int TAB_SELECT_NUMBER = 1;
        private const int TAB_SELECT_TEXT = 2;
        private const int TAB_PREVIEW = 3;
        private const int TAB_SEND = 4;
        private string MobileStartingString { get; set; }

        public MSMSSend(object mainView)
        {
            MainView = mainView;


            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            if (!string.IsNullOrEmpty(APOCCore.STCI.MobileStartingCode))
            {
                MobileStartingString = APOCCore.STCI.MobileStartingCode;
                if (!MobileStartingString.StartsWith("0"))
                    MobileStartingString = "0" + MobileStartingString;
            }
            else
            {
                MobileStartingString = "09";
            }

            TabSendToIsVisibile = true;
            SelectedTabIndex = 0;

            TextFlowDirection = HelperLocalize.ApplicationFlowDirection;
            IsTextEntered = true;

            PopulateData();
            InitDynamics();
            InitCommands();

            TextEntered = DynamicText;

            if (CanSelectCategory)
                if (DynamicSelectedCategory != null)
                    SelectedCategory = (from n in CategoryList where n.Title == DynamicSelectedCategory.Title select n).FirstOrDefault();
                else
                    SelectedCategory = CategoryList.FirstOrDefault();

            if (CanSelectBasket)
                if (DynamicSelectedBasket != null)
                    SelectedBasket = (from n in BasketList where n.Oid == DynamicSelectedBasket.Oid select n).FirstOrDefault();
                else
                    SelectedBasket = BasketList.FirstOrDefault();

            if (DynamicCustomeList != null)
            {
                DynamicCustomeList.ForEach(n => CustomeValue += n + Environment.NewLine);
            }

            SmsServices = APOCCore.SmsAllSettings;
            if (SmsServices != null && SmsServices.Count > 0)
                SelectedSmsService = SmsServices[0];
        }





        #region Dynamic Properties
        public EnumSelectionType DynamicSelectionType { get; set; }
        public DBCTContact DynamicSelectedContact { get; set; }
        public List<DBCTContact> DynamicSelectedContactList { get; set; }
        public DBCTContactCat DynamicSelectedCategory { get; set; }
        public DBCTContactSelection DynamicSelectedBasket { get; set; }
        public List<string> DynamicCustomeList { get; set; }
        public string DynamicText { get; set; }
        #endregion


        #region WindowTitle
        public string WindowTitle
        {
            get { return "ارسال پیامك"; }
        }
        #endregion
        #region SelectedTabIndex
        private int _SelectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                if (value == _SelectedTabIndex)
                    return;

                _SelectedTabIndex = value;
                RaisePropertyChanged("SelectedTabIndex");
            }
        }
        #endregion

        #region TabSendToIsVisibile
        private bool _TabSendToIsVisibile;
        public bool TabSendToIsVisibile
        {
            get { return _TabSendToIsVisibile; }
            set
            {
                if (value == _TabSendToIsVisibile)
                    return;

                _TabSendToIsVisibile = value;
                RaisePropertyChanged("TabSendToIsVisibile");
            }
        }
        #endregion
        #region TabSelectNumberIsVisibile
        private bool _TabSelectNumberIsVisibile;
        public bool TabSelectNumberIsVisibile
        {
            get { return _TabSelectNumberIsVisibile; }
            set
            {
                if (value == _TabSelectNumberIsVisibile)
                    return;

                _TabSelectNumberIsVisibile = value;
                RaisePropertyChanged("TabSelectNumberIsVisibile");
            }
        }
        #endregion
        #region TabSelectTextIsVisibile
        private bool _TabSelectTextIsVisibile;
        public bool TabSelectTextIsVisibile
        {
            get { return _TabSelectTextIsVisibile; }
            set
            {
                if (value == _TabSelectTextIsVisibile)
                    return;

                _TabSelectTextIsVisibile = value;
                RaisePropertyChanged("TabSelectTextIsVisibile");
            }
        }
        #endregion
        #region TabPreviewIsVisibile
        private bool _TabPreviewIsVisibile;
        public bool TabPreviewIsVisibile
        {
            get { return _TabPreviewIsVisibile; }
            set
            {
                if (value == _TabPreviewIsVisibile)
                    return;

                _TabPreviewIsVisibile = value;
                RaisePropertyChanged("TabPreviewIsVisibile");
            }
        }
        #endregion
        #region TabSendIsVisibile
        private bool _TabSendIsVisibile;
        public bool TabSendIsVisibile
        {
            get { return _TabSendIsVisibile; }
            set
            {
                if (value == _TabSendIsVisibile)
                    return;

                _TabSendIsVisibile = value;
                RaisePropertyChanged("TabSendIsVisibile");
            }
        }
        #endregion




        #region Tab SendTo
        public bool CanSelectContact { get { return DynamicSelectedContact != null; } }
        public bool IsSelectContact
        {
            get { return DynamicSelectionType == EnumSelectionType.SelectedContact; }
            set
            {
                if (value && DynamicSelectionType != EnumSelectionType.SelectedContact)
                {
                    DynamicSelectionType = EnumSelectionType.SelectedContact;
                }
                RaiseAllIsSelected();
            }
        }

        public bool CanSelectContacts { get { return DynamicSelectedContactList != null; } }
        public bool IsSelectContacts
        {
            get { return DynamicSelectionType == EnumSelectionType.SelectedContactList; }
            set
            {
                if (value && DynamicSelectionType != EnumSelectionType.SelectedContactList)
                {
                    DynamicSelectionType = EnumSelectionType.SelectedContactList;
                }
                RaiseAllIsSelected();
            }
        }
        public string SelectedContactsCountText
        {
            get
            {
                return string.Format("تعداد {0} مورد انتخاب شده", (CanSelectContacts ? DynamicSelectedContactList.Count.ToString(CultureInfo.InvariantCulture) : "0"));
            }
        }

        public bool CanSelectCategory
        {
            get
            {
                return true;
            }
        }
        public bool IsSelectCategory
        {
            get { return DynamicSelectionType == EnumSelectionType.SelectedCategory; }
            set
            {
                if (value && DynamicSelectionType != EnumSelectionType.SelectedCategory)
                {
                    DynamicSelectionType = EnumSelectionType.SelectedCategory;
                }
                RaiseAllIsSelected();
            }
        }
        public ObservableCollection<CacheItemContactCat> CategoryList { get { return ACacheData.GetContactCatList(); } }
        #region SelectedCategory
        private CacheItemContactCat _SelectedCategory;
        public CacheItemContactCat SelectedCategory
        {
            get { return _SelectedCategory; }
            set
            {
                if (value == _SelectedCategory)
                    return;

                _SelectedCategory = value;
                RaisePropertyChanged("SelectedCategory");
            }
        }
        #endregion
        public bool CanDropCategory { get { return IsSelectCategory && CanSelectCategory; } }

        public bool CanSelectBasket
        {
            get
            {
                return true;
            }
        }
        public bool IsSelectBasket
        {
            get { return DynamicSelectionType == EnumSelectionType.SelectedBasket; }
            set
            {
                if (value && DynamicSelectionType != EnumSelectionType.SelectedBasket)
                {
                    DynamicSelectionType = EnumSelectionType.SelectedBasket;
                }
                RaiseAllIsSelected();
            }
        }
        private List<DBCTContactSelection> _BasketList;
        public List<DBCTContactSelection> BasketList { get { return _BasketList; } }
        #region SelectedBasket
        private DBCTContactSelection _SelectedBasket;
        public DBCTContactSelection SelectedBasket
        {
            get { return _SelectedBasket; }
            set
            {
                if (value == _SelectedBasket)
                    return;

                _SelectedBasket = value;
                RaisePropertyChanged("SelectedBasket");
            }
        }
        #endregion
        public bool CanDropBasket { get { return IsSelectBasket && CanSelectBasket; } }

        public bool CanSelectCustom { get { return DynamicCustomeList != null; } }
        public bool IsSelectCustom
        {
            get { return DynamicSelectionType == EnumSelectionType.CustomeSelection; }
            set
            {
                if (value && DynamicSelectionType != EnumSelectionType.CustomeSelection)
                {
                    DynamicSelectionType = EnumSelectionType.CustomeSelection;
                }
                RaiseAllIsSelected();
            }
        }
        #region CustomeValue
        private string _CustomeValue;
        public string CustomeValue
        {
            get { return _CustomeValue; }
            set
            {
                if (value == _CustomeValue)
                    return;

                _CustomeValue = value;
                RaisePropertyChanged("CustomeValue");
            }
        }
        #endregion
        public bool CanEditCustom { get { return IsSelectCustom && CanSelectCustom; } }
        #endregion

        #region Tab Select Number
        #region PhoneList
        private ObservableCollection<SMSPhoneHolder> _PhoneList;
        public ObservableCollection<SMSPhoneHolder> PhoneList
        {
            get { return _PhoneList; }
            set
            {
                if (value == _PhoneList)
                    return;

                _PhoneList = value;
                RaisePropertyChanged("PhoneList");
            }
        }
        #endregion


        #region CountPhoneHamrahAval
        private int _CountPhoneHamrahAval;
        public int CountPhoneHamrahAval
        {
            get { return _CountPhoneHamrahAval; }
            set
            {
                if (value == _CountPhoneHamrahAval)
                    return;

                _CountPhoneHamrahAval = value;
                RaisePropertyChanged("CountPhoneHamrahAval");
            }
        }
        #endregion
        #region CountPhoneIranCell
        private int _CountPhoneIranCell;
        public int CountPhoneIranCell
        {
            get { return _CountPhoneIranCell; }
            set
            {
                if (value == _CountPhoneIranCell)
                    return;

                _CountPhoneIranCell = value;
                RaisePropertyChanged("CountPhoneIranCell");
            }
        }
        #endregion
        #region CountPhoneRighTel
        private int _CountPhoneRighTel;
        public int CountPhoneRighTel
        {
            get { return _CountPhoneRighTel; }
            set
            {
                if (value == _CountPhoneRighTel)
                    return;

                _CountPhoneRighTel = value;
                RaisePropertyChanged("CountPhoneRighTel");
            }
        }
        #endregion
        #region CountPhoneOther
        private int _CountPhoneOther;
        public int CountPhoneOther
        {
            get { return _CountPhoneOther; }
            set
            {
                if (value == _CountPhoneOther)
                    return;

                _CountPhoneOther = value;
                RaisePropertyChanged("CountPhoneOther");
            }
        }
        #endregion
        #region CountPhoneSum
        private int _CountPhoneSum;
        public int CountPhoneSum
        {
            get { return _CountPhoneSum; }
            set
            {
                if (value == _CountPhoneSum)
                    return;

                _CountPhoneSum = value;
                RaisePropertyChanged("CountPhoneSum");
            }
        }
        #endregion

        #region CountPhoneSelHamrahAval
        private int _CountPhoneSelHamrahAval;
        public int CountPhoneSelHamrahAval
        {
            get { return _CountPhoneSelHamrahAval; }
            set
            {
                if (value == _CountPhoneSelHamrahAval)
                    return;

                _CountPhoneSelHamrahAval = value;
                RaisePropertyChanged("CountPhoneSelHamrahAval");
            }
        }
        #endregion
        #region CountPhoneSelIranCell
        private int _CountPhoneSelIranCell;
        public int CountPhoneSelIranCell
        {
            get { return _CountPhoneSelIranCell; }
            set
            {
                if (value == _CountPhoneSelIranCell)
                    return;

                _CountPhoneSelIranCell = value;
                RaisePropertyChanged("CountPhoneSelIranCell");
            }
        }
        #endregion
        #region CountPhoneSelRighTel
        private int _CountPhoneSelRighTel;
        public int CountPhoneSelRighTel
        {
            get { return _CountPhoneSelRighTel; }
            set
            {
                if (value == _CountPhoneSelRighTel)
                    return;

                _CountPhoneSelRighTel = value;
                RaisePropertyChanged("CountPhoneSelRighTel");
            }
        }
        #endregion
        #region CountPhoneSelOther
        private int _CountPhoneSelOther;
        public int CountPhoneSelOther
        {
            get { return _CountPhoneSelOther; }
            set
            {
                if (value == _CountPhoneSelOther)
                    return;

                _CountPhoneSelOther = value;
                RaisePropertyChanged("CountPhoneSelOther");
            }
        }
        #endregion
        #region CountPhoneSelSum
        private int _CountPhoneSelSum;
        public int CountPhoneSelSum
        {
            get { return _CountPhoneSelSum; }
            set
            {
                if (value == _CountPhoneSelSum)
                    return;

                _CountPhoneSelSum = value;
                RaisePropertyChanged("CountPhoneSelSum");
            }
        }
        #endregion
        #endregion

        #region Tab Select Text
        #region TextFlowDirection
        private FlowDirection _TextFlowDirection;
        public FlowDirection TextFlowDirection
        {
            get { return _TextFlowDirection; }
            set
            {
                if (value == _TextFlowDirection)
                    return;

                _TextFlowDirection = value;
                RaisePropertyChanged("TextFlowDirection");
            }
        }
        #endregion
        #region IsTextEntered
        private bool _IsTextEntered;
        public bool IsTextEntered
        {
            get { return _IsTextEntered; }
            set
            {
                if (value == _IsTextEntered)
                    return;

                _IsTextEntered = value;
                RaisePropertyChanged("IsTextEntered");
                if (value)
                {
                    IsTextStatic = false;
                    IsTextDynamic = false;
                }
            }
        }
        #endregion
        #region IsTextStatic
        private bool _IsTextStatic;
        public bool IsTextStatic
        {
            get { return _IsTextStatic; }
            set
            {
                if (value == _IsTextStatic)
                    return;

                _IsTextStatic = value;
                RaisePropertyChanged("IsTextStatic");

                if (value)
                {
                    IsTextEntered = false;
                    IsTextDynamic = false;
                }
            }
        }
        #endregion
        #region IsTextDynamic
        private bool _IsTextDynamic;
        public bool IsTextDynamic
        {
            get { return _IsTextDynamic; }
            set
            {
                if (value == _IsTextDynamic)
                    return;

                _IsTextDynamic = value;
                RaisePropertyChanged("IsTextDynamic");
                if (value)
                {
                    IsTextEntered = false;
                    IsTextStatic = false;
                }
            }
        }
        #endregion

        #region TextEntered
        private string _TextEntered;
        public string TextEntered
        {
            get { return _TextEntered; }
            set
            {
                if (value == _TextEntered)
                    return;

                _TextEntered = value;
                RaisePropertyChanged("TextEntered");
                RaisePropertyChanged("TextEnteredCountChar");
                RaisePropertyChanged("TextEnteredCountSMS");

            }
        }
        #endregion
        #region TextEnteredCountChar
        private int _TextEnteredCountChar;
        public string TextEnteredCountChar
        {
            get
            {
                _TextEnteredCountChar = string.IsNullOrEmpty(_TextEntered)
                                            ? 0
                                            : _TextEntered.Length * (_TextEntered.ContainsUnicodeCharacter() ? 2 : 1);
                return string.Format("كاراكتر : {0}", _TextEnteredCountChar);
            }
        }
        #endregion
        #region TextEnteredCountSMS
        public string TextEnteredCountSMS
        {
            get { return TextEnteredCountChar != null ? string.Format("صفحه : {0}", ((_TextEnteredCountChar / 160) + 1)) : ""; }
        }
        #endregion

        #region TextStaticSearch
        private string _TextStaticSearch;
        public string TextStaticSearch
        {
            get { return _TextStaticSearch; }
            set
            {
                if (value == _TextStaticSearch)
                    return;

                _TextStaticSearch = value;
                RaisePropertyChanged("TextStaticSearch");
                RaisePropertyChanged("TextStaticList");
            }
        }
        #endregion
        #region TextStaticList
        private List<DBSMTextStatic> _TextStaticList;
        public List<DBSMTextStatic> TextStaticList
        {
            get
            {
                if (_TextStaticList == null) return null;
                if (string.IsNullOrWhiteSpace(TextStaticSearch))
                    return _TextStaticList;
                return _TextStaticList.Where(n => n.Body.Contains(TextStaticSearch)).ToList();
            }
            set
            {
                if (value == _TextStaticList)
                    return;

                _TextStaticList = value;
                RaisePropertyChanged("TextStaticList");
            }
        }
        #endregion
        #region TextStaticSelected
        private DBSMTextStatic _TextStaticSelected;
        public DBSMTextStatic TextStaticSelected
        {
            get { return _TextStaticSelected; }
            set
            {
                if (value == _TextStaticSelected)
                    return;

                _TextStaticSelected = value;
                RaisePropertyChanged("TextStaticSelected");
            }
        }
        #endregion
        #endregion

        #region Tab Send
        #region CountSMS
        private int _CountSMS;
        public int CountSMS
        {
            get { return _CountSMS; }
        }
        #endregion

        #region CountPage
        private int _CountPage;
        public int CountPage
        {
            get { return _CountPage; }
        }
        #endregion

        #region TextTypeString
        public string TextTypeString
        {
            get { return IsTextDynamic ? "متغییر" : "ثابت"; }
        }
        #endregion

        #region SenderName
        public string SenderName
        {
            get { return AMembership.ActiveUser.UserName; }
        }
        #endregion

        #region SendDate
        private DateTime? _SendDate;
        public DateTime? SendDate
        {
            get { return _SendDate; }
            set
            {
                if (value == _SendDate)
                    return;

                _SendDate = value;
                RaisePropertyChanged("SendDate");
            }
        }
        #endregion

        #region SendTime
        private string _SendTime;
        public string SendTime
        {
            get { return _SendTime; }
            set
            {
                if (value == _SendTime)
                    return;

                _SendTime = value;
                RaisePropertyChanged("SendTime");
            }
        }
        #endregion

        #region SmsServices
        private List<SmsModuleSettings> _SmsServices;
        public List<SmsModuleSettings> SmsServices
        {
            get { return _SmsServices; }
            set
            {
                _SmsServices = value;
                RaisePropertyChanged("SmsServices");
            }
        }
        #endregion

        #region SmsServices
        private SmsModuleSettings _SelectedSmsService;
        public SmsModuleSettings SelectedSmsService
        {
            get { return _SelectedSmsService; }
            set
            {
                _SelectedSmsService = value;
                RaisePropertyChanged("SelectedSmsService");
            }
        }
        #endregion


        #endregion









        #region [METHODS]
        private void InitDynamics()
        {
            DynamicSelectionType = MainView.DynamicSelectionType;
            DynamicSelectedContact = MainView.DynamicSelectedContact;
            DynamicSelectedContactList = MainView.DynamicSelectedContactList;
            DynamicSelectedCategory = MainView.DynamicSelectedCategory;
            DynamicSelectedBasket = MainView.DynamicSelectedBasket;
            DynamicCustomeList = MainView.DynamicCustomeList;
            DynamicText = MainView.DynamicText;
        }
        private void InitCommands()
        {
            CommandNext = new RelayCommand(Next, ValidateNextButton);
            CommandPhoneSelectAll = new RelayCommand(PhoneSelectAll);
            CommandPhoneSelectInvert = new RelayCommand(PhoneSelectInvert);
            CommandPhoneSelectNone = new RelayCommand(PhoneSelectNone);
            CommandPhoneSelectDistinct = new RelayCommand(PhoneSelectDistinct);

            CommandPhoneSelectHamrahAval = new RelayCommand(PhoneSelectHamrahAval);
            CommandPhoneSelectIranCell = new RelayCommand(PhoneSelectIranCell);
            CommandPhoneSelectRighTel = new RelayCommand(PhoneSelectRighTel);
            CommandPhoneCopySelected = new RelayCommand(PhoneCopySelected);

            CommandPhoneCount = new RelayCommand(PhoneCount);

            CommandTextRTL = new RelayCommand(() => { TextFlowDirection = FlowDirection.RightToLeft; HelperLocalize.SetLanguageToRTL(); });
            CommandTextLTR = new RelayCommand(() => { TextFlowDirection = FlowDirection.LeftToRight; HelperLocalize.SetLanguageToLTR(); });

            CommandTextStaticAdd = new RelayCommand(TextStaticAdd, () => true);
            CommandTextStaticEdit = new RelayCommand(TextStaticEdit, () => TextStaticSelected != null);
            CommandTextStaticDelete = new RelayCommand(TextStaticDelete, () => TextStaticSelected != null);

            CommandTextStaticClear = new RelayCommand(() => { TextStaticSearch = string.Empty; }, () => true);

            CommandSend = new RelayCommand(Send, () => SendDate != null && _CountPage > 0);

            CommandSelectContact = new RelayCommand(SelectContact, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp64 != "");
        }

        private void SelectContact()
        {
            var a = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var dbc = a.ShowSelectContact(Window.GetWindow(MainView), null) as DBCTContact;
            if (dbc == null) return;

            foreach (var dbp in dbc.Phones)
            {
                if (dbp.PhoneType == EnumPhoneType.Mobile || dbp.PhoneType == EnumPhoneType.SMS)
                {
                    CustomeValue += Environment.NewLine + dbp.PhoneNumber;
                }
            }
        }

        private void Send()
        {
            Exception hasException = null;
            if (!SendDate.HasValue)
            {
                POLMessageBox.ShowError("لطفا تاریخ ارسال را مشخص كنید.");
                return;
            }
            if (SelectedSmsService == null)
            {
                POLMessageBox.ShowError("لطفا خط ارسال را مشخص كنید.");
                return;
            }

            var count = PhoneList.Count(n => n.Selected);

            POLProgressBox.Show("ثبت پیامك در صف ارسال", false, 0, count, 2,
                pb =>
                {
                    var dxs = ADatabase.GetNewSession();
                    var i = 0;
                    var body = TextEntered;

                    var ss = SendTime.Split(':');
                    var hour = Convert.ToInt32(ss[0]);
                    var min = Convert.ToInt32(ss[1]);
                    var senddate = new DateTime(SendDate.Value.Year,
                                                SendDate.Value.Month,
                                                SendDate.Value.Day,
                                                hour, min, 0);

                    try
                    {
                        foreach (var ph in PhoneList.Where(n => n.Selected))
                        {
                            pb.AsyncSetValue(i++);
                            pb.AsyncSetText(1, ph.PhoneNumber);
                            pb.AsyncSetText(2, ph.Title);

                            var body2 = body;
                            if (ph.PhoneBook != null && ph.PhoneBook.Contact != null)
                            {
                                body2 = body.Replace("_ctitle_", ph.PhoneBook.Contact.Title);
                                body2 = body2.Replace("_ccode_", ph.PhoneBook.Contact.Code.ToString());
                            }
                            var dbsms = new DBSMLog2(dxs)
                            {
                                Body = body2,
                                From = SelectedSmsService.SmsNumber,
                                Phone = ph.PhoneBook == null
                                        ? null
                                        : DBCTPhoneBook.FindByOid(dxs, ph.PhoneBook.Oid),
                                IsRTL = TextFlowDirection == FlowDirection.RightToLeft,
                                Sender = AMembership.ActiveUser.Title,
                                SmsPriority = EnumSmsPriority.Normal,
                                SmsResult = string.Empty,
                                SmsType = EnumSmsType.RequestToSend,
                                To = ph.PhoneNumber,
                                TransDate = senddate,
                                TransId = 0,
                                DelivaryResult = string.Empty,
                            };

                            dbsms.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        hasException = ex;
                        return;
                    }
                },
                pb =>
                {
                    if (hasException != null)
                    {
                        POLMessageBox.ShowError(hasException.Message);
                    }
                    else
                    {
                        POLMessageBox.ShowInformation(string.Format("{0} پیغام ارسال شد.", CountSMS));
                    }
                },
                MainView as Window);
        }

        private void TextStaticAdd()
        {
            var w = new WTextStaticAddEdit(null)
                        {
                            Owner = Window.GetWindow(MainView)
                        };
            if (w.ShowDialog() == true)
            {
                PopulateTextStaticList();
            }
        }
        private void TextStaticEdit()
        {
            var w = new WTextStaticAddEdit(TextStaticSelected)
            {
                Owner = Window.GetWindow(MainView)
            };
            if (w.ShowDialog() == true)
            {
                PopulateTextStaticList();
            }
        }
        private void TextStaticDelete()
        {
            if (TextStaticSelected == null) return;
            var dr = POLMessageBox.ShowQuestionYesNo("متن انتخاب شده حذف شود؟", MainView);
            if (dr != MessageBoxResult.Yes) return;
            try
            {
                TextStaticSelected.Delete();
                TextStaticSelected.Save();
                PopulateTextStaticList();
            }
            catch (Exception ex)
            {
                POLMessageBox.ShowError(ex.Message, MainView);
            }
        }

        private void PopulateTextStaticList()
        {
            _TextStaticList = DBSMTextStatic.GetAll(ADatabase.Dxs).ToList();
            RaisePropertyChanged("TextStaticList");
        }

        private void PhoneCount()
        {
            CountPhoneHamrahAval = (from n in PhoneList where n.CompanyType == EnumPhoneCompanyType.HamrahAval select n).Count();
            CountPhoneIranCell = (from n in PhoneList where n.CompanyType == EnumPhoneCompanyType.IranCell select n).Count();
            CountPhoneRighTel = (from n in PhoneList where n.CompanyType == EnumPhoneCompanyType.RighTel select n).Count();
            CountPhoneOther = (from n in PhoneList
                               where n.CompanyType != EnumPhoneCompanyType.HamrahAval &&
                                    n.CompanyType != EnumPhoneCompanyType.IranCell &&
                                    n.CompanyType != EnumPhoneCompanyType.RighTel
                               select n).Count();
            CountPhoneSum = PhoneList.Count();


            CountPhoneSelHamrahAval = (from n in PhoneList where n.CompanyType == EnumPhoneCompanyType.HamrahAval && n.Selected select n).Count();
            CountPhoneSelIranCell = (from n in PhoneList where n.CompanyType == EnumPhoneCompanyType.IranCell && n.Selected select n).Count();
            CountPhoneSelRighTel = (from n in PhoneList where n.CompanyType == EnumPhoneCompanyType.RighTel && n.Selected select n).Count();
            CountPhoneSelOther = (from n in PhoneList
                                  where n.CompanyType != EnumPhoneCompanyType.HamrahAval &&
                                       n.CompanyType != EnumPhoneCompanyType.IranCell &&
                                       n.CompanyType != EnumPhoneCompanyType.RighTel &&
                                       n.Selected
                                  select n).Count();
            CountPhoneSelSum = (from n in PhoneList where n.Selected select n).Count();
        }
        private void PhoneCopySelected()
        {
            var copyString = new StringBuilder();
            foreach (var ph in PhoneList)
            {
                if (ph.Selected)
                {
                    copyString.AppendLine(ph.PhoneNumber);
                }
            }
            System.Windows.Clipboard.SetText(copyString.ToString());
        }

        private void PhoneSelectHamrahAval()
        {
            foreach (var ph in PhoneList)
            {
                ph.Selected = ph.CompanyType == EnumPhoneCompanyType.HamrahAval;
            }
        }
        private void PhoneSelectIranCell()
        {
            foreach (var ph in PhoneList)
            {
                ph.Selected = ph.CompanyType == EnumPhoneCompanyType.IranCell;
            }
        }
        private void PhoneSelectRighTel()
        {
            foreach (var ph in PhoneList)
            {
                ph.Selected = ph.CompanyType == EnumPhoneCompanyType.RighTel;
            }
        }
        private void PhoneSelectDistinct()
        {
            var q = from n in PhoneList orderby n.ContactOid, n.PhoneNumber select n;
            var lastoid = Guid.Empty;
            foreach (var ph in q)
            {
                if (ph.ContactOid == Guid.Empty)
                {
                    ph.Selected = true;
                }
                else
                {
                    if (lastoid != ph.ContactOid)
                    {
                        lastoid = ph.ContactOid;
                        ph.Selected = true;
                    }
                    else
                    {
                        ph.Selected = false;
                    }
                }
            }
        }
        private void PhoneSelectNone()
        {
            if (PhoneList == null) return;
            foreach (var p in PhoneList)
            {
                p.Selected = false;
            }
        }
        private void PhoneSelectInvert()
        {
            if (PhoneList == null) return;
            foreach (var p in PhoneList)
            {
                p.Selected = !p.Selected;
            }
        }
        private void PhoneSelectAll()
        {
            if (PhoneList == null) return;
            foreach (var p in PhoneList)
            {
                p.Selected = true;
            }
        }

        private void Next()
        {
            if (TabSendToIsVisibile)
            {
                if (IsSelectContact && ValidateContactNumbers())
                {
                    PopulatePhoneListByContact();
                    MoveTabToSelectNumber();
                }
                if (IsSelectContacts && ValidateContactsNumbers())
                {
                    PopulatePhoneListByContacts();
                    MoveTabToSelectNumber();
                }
                if (IsSelectCategory && ValidateCategoryNumbers())
                {
                    PopulatePhoneListByCategory();
                    MoveTabToSelectNumber();
                }
                if (IsSelectBasket && ValidateBasketNumbers())
                {
                    PopulatePhoneListByBasket();
                    MoveTabToSelectNumber();
                }
                if (IsSelectCustom && ValidateCustomNumbers())
                {
                    PopulatePhoneListByCustom();
                    MoveTabToSelectNumber();
                }
            }
            else if (TabSelectNumberIsVisibile)
            {
                if (PhoneList == null || PhoneList.Count == 0 || PhoneList.Count(n => n.Selected) == 0)
                {
                    POLMessageBox.ShowWarning("خطا : هیچ شماره ای انتخاب نشده است!", MainView);
                    return;
                }
                MoveTabToSelectText();
            }
            else if (TabSelectTextIsVisibile)
            {
                if (IsTextDynamic)
                    MoveTabToPreview();
                else
                    MoveTabToSend();
            }
        }

        private void PopulatePhoneListByCustom()
        {
            var nums = CustomeValue.Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList().Distinct();
            var phones = (from n in nums
                          select
                              new SMSPhoneHolder()
                              {
                                  PhoneNumber = n,
                                  Selected = true
                              }).ToList();
            var cityOid = APOCCore.STCI.CurrentCityGuid;
            var dbcity = POL.DB.P30Office.GL.DBGLCity.FindByOid(ADatabase.Dxs, cityOid);
            if (dbcity == null)
            {
                POLMessageBox.ShowWarning("خطا : شهر جاری بدرستی تعیین نشده است.", MainView);
                return;
            }
            foreach (var p in phones)
            {
                var dbp = DBCTPhoneBook.FindByPhoneAndCountry(ADatabase.Dxs, p.PhoneNumber, dbcity.Country.Oid);
                if (dbp == null) continue;
                p.PhoneBook = dbp;
                p.Code = dbp.Contact.Code;
                p.Title = dbp.Contact.Title;
                p.ContactOid = dbp.Contact.Oid;
            }
            PhoneList = new ObservableCollection<SMSPhoneHolder>(phones);
        }
        private void PopulatePhoneListByBasket()
        {
            var mainSearchCriteria = new GroupOperator(GroupOperatorType.And);
            mainSearchCriteria.Operands.Add(new ContainsOperator("Selections", new BinaryOperator("Oid", SelectedBasket.Oid)));
            var xpc = new XPCollection<DBCTContact>(ADatabase.Dxs, mainSearchCriteria);
            var phones = (from c in xpc
                          from n in c.Phones
                          where
                          n.PhoneType == EnumPhoneType.SMS || n.PhoneType == EnumPhoneType.Mobile
                          select
                              new SMSPhoneHolder()
                              {
                                  Code = c.Code,
                                  Title = c.Title,
                                  PhoneBook = n,
                                  PhoneNumber = n.PhoneNumber,
                                  Selected = true,
                                  ContactOid = c.Oid,
                              });
            PhoneList = new ObservableCollection<SMSPhoneHolder>(phones);
        }
        private void PopulatePhoneListByCategory()
        {
            var phones = (from c in DynamicSelectedCategory.Contacts
                          from n in c.Phones
                          where
                          n.PhoneType == EnumPhoneType.SMS || n.PhoneType == EnumPhoneType.Mobile
                          select
                              new SMSPhoneHolder()
                              {
                                  Code = c.Code,
                                  Title = c.Title,
                                  PhoneBook = n,
                                  PhoneNumber = n.PhoneNumber,
                                  Selected = true,
                                  ContactOid = c.Oid,
                              });
            PhoneList = new ObservableCollection<SMSPhoneHolder>(phones);

        }
        private void PopulatePhoneListByContacts()
        {
            var phones = (from c in DynamicSelectedContactList
                          from n in c.Phones
                          where
                          n.PhoneType == EnumPhoneType.SMS || n.PhoneType == EnumPhoneType.Mobile
                          select
                              new SMSPhoneHolder()
                              {
                                  Code = c.Code,
                                  Title = c.Title,
                                  PhoneBook = n,
                                  PhoneNumber = n.PhoneNumber,
                                  Selected = true,
                                  ContactOid = c.Oid,
                              });
            PhoneList = new ObservableCollection<SMSPhoneHolder>(phones);
        }
        private void PopulatePhoneListByContact()
        {
            var phones = (from n in DynamicSelectedContact.Phones
                          where
                          n.PhoneType == EnumPhoneType.SMS || n.PhoneType == EnumPhoneType.Mobile
                          select
                              new SMSPhoneHolder()
                              {
                                  Code = DynamicSelectedContact.Code,
                                  Title = DynamicSelectedContact.Title,
                                  PhoneBook = n,
                                  PhoneNumber = n.PhoneNumber,
                                  Selected = true,
                                  ContactOid = DynamicSelectedContact.Oid,
                              });
            PhoneList = new ObservableCollection<SMSPhoneHolder>(phones);
        }

        private void MoveTabToSelectNumber()
        {
            HideAllTabs();
            TabSelectNumberIsVisibile = true;
            SelectedTabIndex = TAB_SELECT_NUMBER;
            PhoneCount();
        }
        private void MoveTabToSelectText()
        {
            HideAllTabs();
            TabSelectTextIsVisibile = true;
            SelectedTabIndex = TAB_SELECT_TEXT;
            PopulateTextStaticList();
        }
        private void MoveTabToPreview()
        {
            HideAllTabs();
            TabPreviewIsVisibile = true;
            SelectedTabIndex = TAB_PREVIEW;
        }
        private void MoveTabToSend()
        {
            HideAllTabs();
            TabSendIsVisibile = true;
            SelectedTabIndex = TAB_SEND;

            if (IsTextEntered)
            {
                _CountSMS = (from n in PhoneList where n.Selected select n).Count();
                _CountPage = _CountSMS * ((_TextEnteredCountChar / 160) + 1);
                SendDate = DateTime.Now.Date;
                SendTime = string.Format("{0}:{1}", DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                    DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));

                RaisePropertyChanged("CountSMS");
                RaisePropertyChanged("CountPage");
            }

        }
        private void HideAllTabs()
        {
            TabSendToIsVisibile = false;
            TabSelectNumberIsVisibile = false;
            TabSelectTextIsVisibile = false;
            TabPreviewIsVisibile = false;
            TabSendIsVisibile = false;
        }

        private bool ValidateCustomNumbers()
        {
            if (CustomeValue == null)
            {
                POLMessageBox.ShowWarning("هیچ شماره معتبری برای پردازش وجود ندارد.", MainView);
                return false;
            }
            var numbers = CustomeValue.Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var goodnumbers = (from n in numbers where n.Length == APOCCore.STCI.MobileLength + 1 && n.StartsWith(MobileStartingString) select n);
            if (!goodnumbers.Any())
            {
                POLMessageBox.ShowWarning("هیچ شماره معتبری وجود ندارد.", MainView);
                return false;
            }
            return true;
        }
        private bool ValidateBasketNumbers()
        {
            return true;
        }
        private bool ValidateCategoryNumbers()
        {
            if (SelectedCategory == null || SelectedCategory.Tag == null) return false;
            DynamicSelectedCategory = (DBCTContactCat)SelectedCategory.Tag;

            var hascontact = DynamicSelectedCategory.Contacts.Count > 0;
            if (!hascontact)
                POLMessageBox.ShowWarning("هیچ پرونده ای در دسته انتخاب شده وجود ندارد.", MainView);
            return hascontact;
        }
        private bool ValidateContactsNumbers()
        {
            var hasmobile = (from dbc in DynamicSelectedContactList
                             where dbc.Phones.Count > 0
                             select dbc.Phones.ToList().Any(p => p.PhoneNumber.StartsWith(MobileStartingString))).Any(any => any);
            if (!hasmobile)
                POLMessageBox.ShowWarning("هیچ شماره موبایلی در پرونده های انتخاب شده وجود ندارد.", MainView);
            return hasmobile;
        }
        private bool ValidateContactNumbers()
        {
            var hasmobile = false;
            if (!(DynamicSelectedContact.Phones == null || DynamicSelectedContact.Phones.Count == 0))
            {
                DynamicSelectedContact.Phones.ToList().ForEach(
                    p =>
                    {
                        if (!hasmobile)
                            if (p.PhoneNumber.StartsWith(MobileStartingString))
                                hasmobile = true;
                    });
            }
            if (!hasmobile)
                POLMessageBox.ShowWarning("هیچ شماره موبایلی در پرونده انتخاب شده وجود ندارد.", MainView);
            return hasmobile;
        }
        private bool ValidateNextButton()
        {
            if (TabSendToIsVisibile)
            {
                if (IsSelectContact) return CanSelectContact;
                if (IsSelectContacts) return CanSelectContacts && (DynamicSelectedContactList.Count > 0);
                if (IsSelectCategory) return CanSelectCategory && SelectedCategory != null;
                if (IsSelectBasket) return CanSelectBasket && SelectedBasket != null;
                if (IsSelectCustom) return true;
                return false;
            }
            else if (TabSelectNumberIsVisibile)
                return true;
            else if (TabSelectTextIsVisibile)
            {
                return !string.IsNullOrWhiteSpace(TextEntered);
            }
            return false;
        }


        private void RaiseAllIsSelected()
        {
            RaisePropertyChanged("IsSelectContact");
            RaisePropertyChanged("IsSelectContacts");
            RaisePropertyChanged("IsSelectCategory");
            RaisePropertyChanged("IsSelectBasket");
            RaisePropertyChanged("IsSelectCustom");

            RaisePropertyChanged("CanDropCategory");
            RaisePropertyChanged("CanDropBasket");
            RaisePropertyChanged("CanEditCustom");
        }
        private void PopulateData()
        {
            PopulateBaskets();
        }
        private void PopulateBaskets()
        {
            var selectionBasketList = DBCTContactSelection.GetByUser(ADatabase.Dxs, AMembership.ActiveUser.UserID, false, null);
            selectionBasketList.Sorting = new SortingCollection(new SortProperty("Title", DevExpress.Xpo.DB.SortingDirection.Ascending));

            _BasketList = selectionBasketList.ToList();
        }

        internal void SetTextEntered(bool clear)
        {
            if (TextStaticSelected == null) return;
            if (clear)
            {
                TextEntered = TextStaticSelected.Body;
            }
            else
            {
                TextEntered += TextStaticSelected.Body;
            }
        }

        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp64);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNext { get; set; }
        public RelayCommand CommandPhoneSelectAll { get; set; }
        public RelayCommand CommandPhoneSelectInvert { get; set; }
        public RelayCommand CommandPhoneSelectNone { get; set; }
        public RelayCommand CommandPhoneSelectDistinct { get; set; }
        public RelayCommand CommandPhoneSelectHamrahAval { get; set; }
        public RelayCommand CommandPhoneSelectIranCell { get; set; }
        public RelayCommand CommandPhoneSelectRighTel { get; set; }
        public RelayCommand CommandPhoneCopySelected { get; set; }
        public RelayCommand CommandPhoneCount { get; set; }

        public RelayCommand CommandTextRTL { get; set; }
        public RelayCommand CommandTextLTR { get; set; }

        public RelayCommand CommandTextStaticAdd { get; set; }
        public RelayCommand CommandTextStaticEdit { get; set; }
        public RelayCommand CommandTextStaticDelete { get; set; }
        public RelayCommand CommandTextStaticClear { get; set; }

        public RelayCommand CommandSend { get; set; }

        public RelayCommand CommandSelectContact { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion


    }
}
