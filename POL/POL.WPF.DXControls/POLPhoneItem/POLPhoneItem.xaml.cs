using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace POL.WPF.DXControls.POLPhoneItem
{
    public partial class POLPhoneItem : UserControl
    {
        public POLPhoneItem()
        {
            InitializeComponent();
            bLineDuration.MouseUp +=
                (s, e) =>
                {
                    if (MouseUpLineDuration != null)
                        MouseUpLineDuration.Invoke(s, e);
                };
        }




        public event MouseButtonEventHandler MouseUpLineDuration;

        #region DP BackgroundHeader

        public Brush BackgroundHeader
        {
            get { return (Brush) GetValue(BackgroundHeaderProperty); }
            set { SetValue(BackgroundHeaderProperty, value); }
        }

        public static readonly DependencyProperty BackgroundHeaderProperty =
            DependencyProperty.Register("BackgroundHeader", typeof (Brush), typeof (POLPhoneItem),
                new PropertyMetadata(Brushes.Transparent,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.dpHeader.Background = (Brush) e.NewValue;
                        self.bRoot.BorderBrush = (Brush) e.NewValue;
                    }));

        #endregion

        #region DP TextLineNumber

        public string TextLineNumber
        {
            get { return (string) GetValue(TextLineNumberProperty); }
            set { SetValue(TextLineNumberProperty, value); }
        }

        public static readonly DependencyProperty TextLineNumberProperty =
            DependencyProperty.Register("TextLineNumber", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbLineNumber.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP TextLineName

        public string TextLineName
        {
            get { return (string) GetValue(TextLineNameProperty); }
            set { SetValue(TextLineNameProperty, value); }
        }

        public static readonly DependencyProperty TextLineNameProperty =
            DependencyProperty.Register("TextLineName", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbLineName.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP VisibilityLineName

        public Visibility VisibilityLineName
        {
            get { return (Visibility) GetValue(VisibilityLineNameProperty); }
            set { SetValue(VisibilityLineNameProperty, value); }
        }

        public static readonly DependencyProperty VisibilityLineNameProperty =
            DependencyProperty.Register("VisibilityLineName", typeof (Visibility), typeof (POLPhoneItem),
                new PropertyMetadata(Visibility.Visible,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.bLineName.Visibility = (Visibility) e.NewValue;
                    }));

        #endregion

        #region DP TextLineDuration

        public string TextLineDuration
        {
            get { return (string) GetValue(TextLineDurationProperty); }
            set { SetValue(TextLineDurationProperty, value); }
        }

        public static readonly DependencyProperty TextLineDurationProperty =
            DependencyProperty.Register("TextLineDuration", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbLineDuration.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP VisibilityLineDuration

        public Visibility VisibilityLineDuration
        {
            get { return (Visibility) GetValue(VisibilityLineDurationProperty); }
            set { SetValue(VisibilityLineDurationProperty, value); }
        }

        public static readonly DependencyProperty VisibilityLineDurationProperty =
            DependencyProperty.Register("VisibilityLineDuration", typeof (Visibility), typeof (POLPhoneItem),
                new PropertyMetadata(Visibility.Visible,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.bLineDuration.Visibility = (Visibility) e.NewValue;
                    }));

        #endregion

        #region DP BackgroundLineDuration

        public Brush BackgroundLineDuration
        {
            get { return (Brush) GetValue(BackgroundLineDurationProperty); }
            set { SetValue(BackgroundLineDurationProperty, value); }
        }

        public static readonly DependencyProperty BackgroundLineDurationProperty =
            DependencyProperty.Register("BackgroundLineDuration", typeof (Brush), typeof (POLPhoneItem),
                new PropertyMetadata(Brushes.Transparent,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.bLineDuration.Background = (Brush) e.NewValue;
                    }));

        #endregion

        #region DP VisibilityCallIn

        public Visibility VisibilityCallIn
        {
            get { return (Visibility) GetValue(VisibilityCallInProperty); }
            set { SetValue(VisibilityCallInProperty, value); }
        }

        public static readonly DependencyProperty VisibilityCallInProperty =
            DependencyProperty.Register("VisibilityCallIn", typeof (Visibility), typeof (POLPhoneItem),
                new PropertyMetadata(Visibility.Visible,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        var val = (Visibility) e.NewValue;
                        self.eCallIn1.Visibility = val;
                        self.eCallIn2.Visibility = val;
                        self.tbCallInDuration.Visibility = val;
                        self.tbCallInPhone.Visibility = val;
                        self.tbCallInTime.Visibility = val;
                        self.tbCallInTitle.Visibility = val;
                    }));

        #endregion

        #region DP TextCallInTitle

        public string TextCallInTitle
        {
            get { return (string) GetValue(TextCallInTitleProperty); }
            set { SetValue(TextCallInTitleProperty, value); }
        }

        public static readonly DependencyProperty TextCallInTitleProperty =
            DependencyProperty.Register("TextCallInTitle", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbCallInTitle.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP TextCallInPhone

        public string TextCallInPhone
        {
            get { return (string) GetValue(TextCallInPhoneProperty); }
            set { SetValue(TextCallInPhoneProperty, value); }
        }

        public static readonly DependencyProperty TextCallInPhoneProperty =
            DependencyProperty.Register("TextCallInPhone", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbCallInPhone.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP TextCallInTime

        public string TextCallInTime
        {
            get { return (string) GetValue(TextCallInTimeProperty); }
            set { SetValue(TextCallInTimeProperty, value); }
        }

        public static readonly DependencyProperty TextCallInTimeProperty =
            DependencyProperty.Register("TextCallInTime", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbCallInTime.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP TextCallInDuration

        public string TextCallInDuration
        {
            get { return (string) GetValue(TextCallInDurationProperty); }
            set { SetValue(TextCallInDurationProperty, value); }
        }

        public static readonly DependencyProperty TextCallInDurationProperty =
            DependencyProperty.Register("TextCallInDuration", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbCallInDuration.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP VisibilityCallOut

        public Visibility VisibilityCallOut
        {
            get { return (Visibility) GetValue(VisibilityCallOutProperty); }
            set { SetValue(VisibilityCallOutProperty, value); }
        }

        public static readonly DependencyProperty VisibilityCallOutProperty =
            DependencyProperty.Register("VisibilityCallOut", typeof (Visibility), typeof (POLPhoneItem),
                new PropertyMetadata(Visibility.Visible,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        var val = (Visibility) e.NewValue;
                        self.eCallOut1.Visibility = val;
                        self.eCallOut2.Visibility = val;
                        self.tbCallOutDuration.Visibility = val;
                        self.tbCallOutPhone.Visibility = val;
                        self.tbCallOutTime.Visibility = val;
                        self.tbCallOutTitle.Visibility = val;
                    }));

        #endregion

        #region DP TextCallOutTitle

        public string TextCallOutTitle
        {
            get { return (string) GetValue(TextCallOutTitleProperty); }
            set { SetValue(TextCallOutTitleProperty, value); }
        }

        public static readonly DependencyProperty TextCallOutTitleProperty =
            DependencyProperty.Register("TextCallOutTitle", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbCallOutTitle.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP TextCallOutPhone

        public string TextCallOutPhone
        {
            get { return (string) GetValue(TextCallOutPhoneProperty); }
            set { SetValue(TextCallOutPhoneProperty, value); }
        }

        public static readonly DependencyProperty TextCallOutPhoneProperty =
            DependencyProperty.Register("TextCallOutPhone", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbCallOutPhone.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP TextCallOutTime

        public string TextCallOutTime
        {
            get { return (string) GetValue(TextCallOutTimeProperty); }
            set { SetValue(TextCallOutTimeProperty, value); }
        }

        public static readonly DependencyProperty TextCallOutTimeProperty =
            DependencyProperty.Register("TextCallOutTime", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbCallOutTime.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP TextCallOutDuration

        public string TextCallOutDuration
        {
            get { return (string) GetValue(TextCallOutDurationProperty); }
            set { SetValue(TextCallOutDurationProperty, value); }
        }

        public static readonly DependencyProperty TextCallOutDurationProperty =
            DependencyProperty.Register("TextCallOutDuration", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbCallOutDuration.Text = (string) e.NewValue;
                    }));

        #endregion

        #region DP VisibilityExtra

        public Visibility VisibilityExtra
        {
            get { return (Visibility) GetValue(VisibilityExtraProperty); }
            set { SetValue(VisibilityExtraProperty, value); }
        }

        public static readonly DependencyProperty VisibilityExtraProperty =
            DependencyProperty.Register("VisibilityExtra", typeof (Visibility), typeof (POLPhoneItem),
                new PropertyMetadata(Visibility.Visible,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        var val = (Visibility) e.NewValue;
                        self.eExtra.Visibility = val;
                        self.tbExtraTitle.Visibility = val;
                    }));

        #endregion

        #region DP TextExtraTitle

        public string TextExtraTitle
        {
            get { return (string) GetValue(TextExtraTitleProperty); }
            set { SetValue(TextExtraTitleProperty, value); }
        }

        public static readonly DependencyProperty TextExtraTitleProperty =
            DependencyProperty.Register("TextExtraTitle", typeof (string), typeof (POLPhoneItem),
                new PropertyMetadata(string.Empty,
                    (s, e) =>
                    {
                        var self = s as POLPhoneItem;
                        if (self == null) return;
                        self.tbExtraTitle.Text = (string) e.NewValue;
                    }));

        #endregion
    }
}
