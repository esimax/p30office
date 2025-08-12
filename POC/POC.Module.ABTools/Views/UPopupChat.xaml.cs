using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Module.ABTools.Views
{
    public partial class UPopupChat : UserControl, INotifyPropertyChanged, IPopupItem
    {
        private POCCore APOCCore { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        public UPopupChat(string from, string text)
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
                    title.Text = from + " :";  
                    HelperUtils.Try(() => imgSide.Source = HelperImage.GetSpecialImage64("_64_Chat.png"));
                    HelperUtils.Try(() => tbTextBody.Text = text);
                };
        }

        private void InitCommands()
        {
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


        #region IPopupItem

        public Brush PopupBrush
        {
            get
            {
                return Brushes.Green;
            }
        }

        public bool PopupCanClose
        {
            get { return true; }
        }

        public bool PopupCanPin
        {
            get
            {
                return true;
            }
        }

        public bool PopupCanTimeOut
        {
            get
            {
                return true;
            }
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
                return TimeSpan.FromMinutes(3);
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
        #endregion
    }
}
