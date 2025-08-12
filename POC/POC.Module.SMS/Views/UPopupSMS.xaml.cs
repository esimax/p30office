using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;

namespace POC.Module.SMS.Views
{
    public partial class UPopupSMS : UserControl, INotifyPropertyChanged, IPopupItem
    {
        private POCCore APOCCore { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        public UPopupSMS(bool isreceive, SMSInfo info, ImageSource img, EnumMessageKind mk)
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
                APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                InitCommands();
            }

            Loaded +=
                (s, e) =>
                {
                    DataContext = this;
                    IsReceive = isreceive;
                    title.Text = isreceive ? "دریافت پیامك" : "ارسال پیامك";
                    HelperUtils.Try(() => imgSide.Source = img);
                    HelperUtils.Try(() => tbTextFrom.Text = "از : " + info.From);
                    HelperUtils.Try(() => tbTextTo.Text = "به : " + info.To);
                    HelperUtils.Try(() => tbTextBody.Text = info.Text);

                    HelperUtils.Try(() => tbKind.Text = GetMessageKind(mk));
                    switch (mk)
                    {
                        case EnumMessageKind.SMSReceive:
                            gradFrom.Color = Colors.LawnGreen;
                            break;
                        case EnumMessageKind.SMSDeliveryResult:
                            tbTextBody.Text = info.DelivaryResult;
                            gradFrom.Color = Colors.Green;
                            break;
                        case EnumMessageKind.SMSSendFailed:
                            gradFrom.Color = Colors.Tomato;
                            break;
                        case EnumMessageKind.SMSForwardOnCredit:
                            gradFrom.Color = Colors.Wheat;
                            break;
                        case EnumMessageKind.SMSForwardOnFailed:
                            gradFrom.Color = Colors.Wheat;
                            break;
                        case EnumMessageKind.SMSWaitForDelivery:
                            gradFrom.Color = Colors.Yellow;
                            break;
                    }

                };
        }

        private string GetMessageKind(EnumMessageKind mk)
        {
            switch (mk)
            {
                case EnumMessageKind.SMSSendSuccess:
                    return "با موفقیت ارسال شد.";
                case EnumMessageKind.SMSReceive:
                    return "دریافت شد.";
                case EnumMessageKind.SMSSendFailed:
                    return "خطا در ارسال.";
                case EnumMessageKind.SMSForwardOnCredit:
                    return "اتمام اعتبار : ارجاع شد.";
                case EnumMessageKind.SMSForwardOnFailed:
                    return "خطا : ارجاع شد.";
                case EnumMessageKind.SMSWaitForDelivery:
                    return "ارسال شد، منتظر تاییدیه.";
                case EnumMessageKind.SMSDeliveryResult:
                    return "نتیجه تاییدیه.";
            }
            return string.Empty;
        }


        private void InitCommands()
        {
            CommandCardTableClick = new RelayCommand(CardTableClick);
        }

        private void CardTableClick()
        {
            DBCTContact dbc = null;

            APOCMainWindow.ShowAddCardTable(null, "كارتابل : " + (dbc == null ? string.Empty : dbc.Title), "",
                null, dbc, null, null, null);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string pname)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(pname));
        }
        #endregion

        public bool IsReceive { get; set; }
        public Visibility CardTableClickEnabled
        {
            get { return APOCCore.SMSAllowCardTable ? Visibility.Visible : Visibility.Collapsed; }
        }

        #region IPopupItem

        public Brush PopupBrush
        {
            get
            {
                if (IsReceive)
                    return Brushes.LightGreen;
                return Brushes.LightCoral;
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
            get { return IsReceive ? APOCCore.SMSReceivePopupDurationIndex < 6 : APOCCore.SMSSendPopupDurationIndex < 6; }
        }

        public UIElement PopupElement
        {
            get { return this; }
        }

        public double PopupHeight
        {
            get
            {
                return double.NaN;
            }
        }

        public TimeSpan PopupTimeOut
        {
            get
            {
                switch (IsReceive ? APOCCore.SMSReceivePopupDurationIndex : APOCCore.SMSSendPopupDurationIndex)
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
            get { return 320; }
        }

        private bool _isclosed = false;
        public bool IsClosed
        {
            get { return _isclosed; }
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandCardTableClick { get; set; }
        #endregion
    }
}
