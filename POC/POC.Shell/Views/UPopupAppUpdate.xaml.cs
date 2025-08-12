using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using POL.Lib.Interfaces;

namespace POC.Shell.Views
{
    public partial class UPopupAppUpdate : UserControl, INotifyPropertyChanged, IPopupItem
    {
        public UPopupAppUpdate(string version)
        {
            InitializeComponent();
            Loaded +=
                (s, e) =>
                {
                    var run = new Run(version);
                    hlVersion.Inlines.Clear();
                    hlVersion.Inlines.Add(run);
                    hlVersion.NavigateUri = new Uri(string.Format("http://{0}/index.php/download/", POL.Lib.XOffice.ConstantGeneral.WebUrl));
                    img.EditValue = POL.Lib.Utils.HelperImage.GetSpecialImage64("_64_AutoUpdate.png");
                };
        }




        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
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
                return 120;
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
