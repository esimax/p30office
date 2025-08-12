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

namespace POC.Module.Email.Views
{
    public partial class UPopupEmail : UserControl, INotifyPropertyChanged, IPopupItem
    {
        private POCCore APOCCore { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        public UPopupEmail(bool isreceive, string from, string to, string body, ImageSource img)
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
                    title.Text = isreceive ? "دریافت ایمیل" : "ارسال ایمیل";
                    HelperUtils.Try(() => imgSide.Source = img);
                    HelperUtils.Try(() => tbTextFrom.Text = from);
                    HelperUtils.Try(() => tbTextTo.Text = to);
                    HelperUtils.Try(() => tbTextBody.Text = body);
                };
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
            get { return APOCCore.EmailAllowCardTable ? Visibility.Visible : Visibility.Collapsed; }
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
            get { return APOCCore.CallPopupDurationIndex < 6;  }
        }

        public bool PopupCanTimeOut
        {
            get { return IsReceive ? APOCCore.EmailReceivePopupDurationIndex < 6 : APOCCore.EmailSendPopupDurationIndex < 6; }
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
                switch (IsReceive ? APOCCore.EmailReceivePopupDurationIndex : APOCCore.EmailSendPopupDurationIndex)
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
