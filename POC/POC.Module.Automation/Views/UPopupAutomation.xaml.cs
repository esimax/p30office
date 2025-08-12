using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Module.Automation.Views
{
    public partial class UPopupAutomation : UserControl, INotifyPropertyChanged, IPopupItem
    {
        private POCCore APOCCore { get; set; }
        private AutomationPopupInfo Info { get; set; }
        public UPopupAutomation(AutomationPopupInfo info)
        {
            InitializeComponent();
            Info = info;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            }

            Loaded +=
                (s, e) =>
                    {
                        HelperUtils.Try(() => imgSide.Source = HelperImage.GetStandardImage32("_32_Automation.png"));
                        HelperUtils.Try(() => title.Text = info.Title);
                        HelperUtils.Try(() => tbText.Text = info.PopupText);
                        HelperUtils.Try(() => hlCreator.Inlines.Add(info.CardTableCreatorTitle));
                        if (Info.AutomationType == EnumAutomationType.Ending)
                            tbIsEnding.Visibility = Visibility.Visible;
                        if (Info.AutomationType == EnumAutomationType.Result)
                            tbIsResult.Visibility = Visibility.Visible;

                    };
            hlCreator.Click += (s, e) =>
                                   {
                                       var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                                       aPOCMainWindow.ShowEditCardTable(null, Info.CardTableOid);
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
                return Brushes.Yellow;
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
                return TimeSpan.FromMinutes(5);
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
