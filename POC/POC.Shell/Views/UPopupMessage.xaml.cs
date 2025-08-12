using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Shell.Views
{
    public partial class UPopupMessage : UserControl, INotifyPropertyChanged, IPopupItem
    {
        public UPopupMessage(string text, Brush background, ImageSource img)
        {
            InitializeComponent();
            Loaded +=
                (s, e) =>
                {
                    HelperUtils.Try(() => imgSide.Source = img);
                    HelperUtils.Try(() => tbText.Text = text);
                    if (background != null)
                        gridMain.Background = background;
                };
        }
    
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string pname)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(pname));
        }
        #endregion

        #region IPopupItem

        public Brush PopupBrush
        {
            get
            {
                return Brushes.DarkOrange;
            }
        }

        public bool PopupCanClose
        {
            get { return true; }
        }

        public bool PopupCanPin
        {
            get { return false; }
        }

        public bool PopupCanTimeOut
        {
            get { return false; }
        }

        public UIElement PopupElement
        {
            get { return this; }
        }

        public double PopupHeight
        {
            get
            {
                return 140;
            }
        }

        public TimeSpan PopupTimeOut
        {
            get
            {
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
    }
}
