using System.ComponentModel;
using POL.Lib.Interfaces;
using POL.WPF.Controles.MVVM;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace POC.Module.Popup.Views
{
    public partial class WPopup : Window, INotifyPropertyChanged
    {
        public WPopup(IPopupItem item)
        {
            InitializeComponent();

            this.ShowActivated = false;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitCommands();
            }

            Item = item;
            Width = item.PopupWidth;
            Height = 50;


            if(double.IsNaN(item.PopupHeight))
            {
                SizeToContent = SizeToContent.Height;
            } 
            else
                Height = item.PopupHeight;

            CanClose = true;
            Percent = 0;

            borderContent.Child = Item.PopupElement;
            Item.PopupElement.LayoutUpdated += (s, e) =>
                                                   {
                                                       UpdateLayout(); 
                                                   };

            Timer = new DispatcherTimer(DispatcherPriority.Normal);
            Timer.Interval = TimeSpan.FromMilliseconds(item.PopupTimeOut.TotalMilliseconds / 100);
            Timer.Tick += (s, e) =>
            {
                Percent += 0.01;
                if (!(Percent >= 0.99)) return;
                CanClose = true;
                this.Close();
            };
            if (item.PopupCanTimeOut)
                Timer.Start();


            Loaded += (s, e) =>
            {
                if (DataContext != this)
                    DataContext = this;

                UpdateLayout();

                
            };
            Closing += (s, e) =>
            {
                if (!CanClose)
                {
                    e.Cancel = true;
                }
            };
        }

        
        private IPopupItem Item { get; set; }
        private DispatcherTimer Timer { get; set; }
        private bool CanClose { get; set; }

        #region IsPin
        private bool _IsPin;
        public bool IsPin
        {
            get
            {
                return _IsPin;
            }
            set
            {
                if (_IsPin == value)
                    return;
                _IsPin = value;
                if (value)
                    Timer.Stop();
                else
                    Timer.Start();
                RaisePropertyChanged("IsPin");
            }
        }
        #endregion
        #region Percent
        private double _Percent;
        public double Percent
        {
            get
            {
                return _Percent;
            }
            set
            {
                if (_Percent == value)
                    return;
                _Percent = value;
                RaisePropertyChanged("Percent");
            }
        }
        #endregion
        #region IsClosed
        public bool IsClosed
        {
            get
            {
                if (Item == null)
                    return false;
                return Item.IsClosed;
            }
        }
        #endregion

        public Brush PopupBrush
        {
            get
            {
                return Item.PopupBrush;
            }
        }

        #region VisClose
        public Visibility VisClose
        {
            get
            {
                return Item.PopupCanClose ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
        #endregion
        #region VisPin
        public Visibility VisPin
        {
            get
            {
                return Item.PopupCanPin ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
        #endregion
        #region VisTimeout
        public Visibility VisTimeout
        {
            get
            {
                return Item.PopupCanTimeOut ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
        #endregion

        private void InitCommands()
        {
            cmdClose = new RelayCommand<object>((o) => { CanClose = true; Close(); }, o => !IsPin);
        }

        #region [COMMANDS]
        public RelayCommand<object> cmdClose { get; set; }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string pname)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(pname));
        }
        #endregion
    }
}
