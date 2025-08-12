using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POL.WPF.DXControls.POLDateEdit
{
    public class POLDateEdit : ButtonEdit
    {
        public POLDateEdit()
        {
            _DefaultBackground = Background;
            AllowDefaultButton = false;
            Buttons.Clear();
            _DateTypeButtonInfo = new ButtonInfo
            {
                ButtonKind = ButtonKind.Simple,
                GlyphKind = GlyphKind.User,
                Content = "*"
            };
            _DateTypeButtonInfo.Click += (sender, e) =>
            {
                if (DateTimeType == EnumCalendarType.Shamsi)
                    DateTimeType = EnumCalendarType.Hijri;
                else if (DateTimeType == EnumCalendarType.Hijri)
                    DateTimeType = EnumCalendarType.Gregorian;
                else if (DateTimeType == EnumCalendarType.Gregorian)
                    DateTimeType = EnumCalendarType.Shamsi;
            };
            Buttons.Add(_DateTypeButtonInfo);

            DateTimeType = EnumCalendarType.Shamsi;

            if (_ContextMenu == null)
                GenerateContextMenu();
            ContextMenu = _ContextMenu;

            EditValueChanged += (sender, e) =>
            {
                _TextIsChanging = true;
                try
                {
                    var newdate = StringToDateTime(Text);
                    Background = _DefaultBackground;
                    ValidDateTime = true;
                    if (DateEditValue != newdate)
                        DateEditValue = newdate;
                }
                catch
                {
                    Background = new SolidColorBrush(Color.FromArgb(0xff, 0xFF, 0xD4, 0xD4));
                    ValidDateTime = false;
                }
                _TextIsChanging = false;
            };
            LostFocus += (sender, e) =>
            {
                if (DateEditValue == null)
                {
                    Text = string.Empty;
                    return;
                }
                Text = DateTimeToString((DateTime) DateEditValue);
            };

            SetButtonStyle();
            UpdateToolTip();

            MinWidth = 100;
        }

        #region Events

        public event EventHandler DateEditValueChanged;

        #endregion

        #region DateEditValue Dependancy Property

        public static readonly DependencyProperty DateEditValueProperty = DependencyProperty.Register(
            "DateEditValue",
            typeof (DateTime?),
            typeof (POLDateEdit),
            new UIPropertyMetadata(null, DateEditValue_CallBack));

        public DateTime? DateEditValue
        {
            get { return (DateTime?) GetValue(DateEditValueProperty); }
            set { SetValue(DateEditValueProperty, value); }
        }

        private static void DateEditValue_CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var deex = (POLDateEdit) d;
                if (deex == null) return;

                if (e.NewValue == e.OldValue) return;
                if (deex.DateEditValue == null)
                {
                    deex.Text = string.Empty;
                    deex.UpdateToolTip();
                    deex.RaiseDateEditValueChanged();
                    return;
                }
                deex.Text = deex.DateTimeToString((DateTime) deex.DateEditValue);
                deex.RaiseDateEditValueChanged();
                deex.UpdateToolTip();
            }
            catch
            {
            }
        }

        private void RaiseDateEditValueChanged()
        {
            if (DateEditValueChanged != null)
                DateEditValueChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region DateTimeType Dependancy Property

        public static readonly DependencyProperty DateTimeTypeProperty = DependencyProperty.Register(
            "DateTimeType",
            typeof (EnumCalendarType),
            typeof (POLDateEdit),
            new UIPropertyMetadata(EnumCalendarType.Shamsi, DateTimeType_CallBack));

        public EnumCalendarType DateTimeType
        {
            get { return (EnumCalendarType) GetValue(DateTimeTypeProperty); }
            set { SetValue(DateTimeTypeProperty, value); }
        }

        private static void DateTimeType_CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var deex = (POLDateEdit) d;
                if (deex == null) return;

                deex.SetButtonStyle();
                if (deex.DateEditValue == null)
                {
                    deex.UpdateToolTip();
                    return;
                }
                deex.Text = deex.DateTimeToString((DateTime) deex.DateEditValue);
                deex.UpdateToolTip();
            }
            catch
            {
            }
        }

        #endregion

        #region AllowEmptyDate Dependancy Property

        public static readonly DependencyProperty AllowEmptyDateProperty = DependencyProperty.Register(
            "AllowEmptyDate",
            typeof (bool),
            typeof (POLDateEdit),
            new UIPropertyMetadata(true, AllowEmptyDate_CallBack));

        public bool AllowEmptyDate
        {
            get { return (bool) GetValue(AllowEmptyDateProperty); }
            set { SetValue(AllowEmptyDateProperty, value); }
        }

        private static void AllowEmptyDate_CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var deex = (POLDateEdit) d;
                if (deex == null) return;
                var q = from n in deex.ContextMenu.Items.Cast<MenuItem>() where n.Name == "miClearDate" select n;
                var menuItems = q as MenuItem[] ?? q.ToArray();
                if (!menuItems.Any()) return;
                menuItems.First().Visibility = deex.AllowEmptyDate ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
            }
        }

        #endregion

        #region Properties

        public bool ValidDateTime { get; set; }
        private ContextMenu _ContextMenu { get; set; }
        private bool _TextIsChanging { get; set; }
        private Brush _DefaultBackground { get; }
        private ButtonInfo _DateTypeButtonInfo { get; }

        #endregion

        #region Methods

        private void GenerateContextMenu()
        {
            var cm = new ContextMenu
            {
                FontFamily = GetFontFamily(),
                FontSize = GetFontSize(),
                FlowDirection = HelperLocalize.ApplicationFlowDirectio
            };

            {
                var mi = new MenuItem {Header = "امروز"};
                mi.Click += (s, e) => { Text = DateTimeToString(DateTime.Now.Date); };
                cm.Items.Add(mi);
            }

            {
                var mi = new MenuItem {Header = "بدون تاریخ", Name = "miClearDate"};
                mi.Click += (s, e) => { Text = string.Empty; };
                cm.Items.Add(mi);
            }
            _ContextMenu = cm;
        }

        private double GetFontSize()
        {
            if (HelperLocalize.ApplicationFontSize > 26)
                return 26;
            if (HelperLocalize.ApplicationFontSize < 8)
                return 8;
            if (double.IsNaN(HelperLocalize.ApplicationFontSize))
                return 12;
            return HelperLocalize.ApplicationFontSize;
        }

        private FontFamily GetFontFamily()
        {
            return new FontFamily(HelperLocalize.ApplicationFontName ?? "Tahoma");
        }

        private void UpdateToolTip()
        {
            if (DateEditValue == null)
            {
                _DateTypeButtonInfo.ToolTip = null;
                return;
            }
            try
            {
                var sp = new StackPanel {FlowDirection = FlowDirection};
                {
                    var tb1 = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontFamily = GetFontFamily(), 
                        FontSize = GetFontSize(),
                        Text =
                            HelperLocalize.DateTimeToString((DateTime) DateEditValue,
                                EnumCalendarType.Shamsi, "dddd dd MMMM yyyy")
                    };
                    if (DateTimeType == EnumCalendarType.Shamsi)
                        tb1.FontWeight = FontWeights.Bold;
                    sp.Children.Add(tb1);
                }
                {
                    var tb1 = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontFamily = GetFontFamily(),
                        FontSize = GetFontSize(),
                        Text =
                            HelperLocalize.DateTimeToString((DateTime) DateEditValue,
                                EnumCalendarType.Hijri, "dddd dd MMMM yyyy")
                    };
                    if (DateTimeType == EnumCalendarType.Hijri)
                        tb1.FontWeight = FontWeights.Bold;
                    sp.Children.Add(tb1);
                }
                {
                    var tb1 = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontFamily = GetFontFamily(),
                        FontSize = GetFontSize(),
                        Text =
                            HelperLocalize.DateTimeToString((DateTime) DateEditValue,
                                EnumCalendarType.Gregorian,
                                "dddd dd MMMM yyyy")
                    };
                    if (DateTimeType == EnumCalendarType.Gregorian)
                        tb1.FontWeight = FontWeights.Bold;
                    sp.Children.Add(tb1);
                }

                _DateTypeButtonInfo.ToolTip = sp;
            }
            catch
            {
                _DateTypeButtonInfo.ToolTip = null;
            }
        }

        private void SetButtonStyle()
        {
            switch (DateTimeType)
            {
                case EnumCalendarType.Shamsi:
                    _DateTypeButtonInfo.Content = "ش";
                    break;
                case EnumCalendarType.Hijri:
                    _DateTypeButtonInfo.Content = "ق";
                    break;
                case EnumCalendarType.Gregorian:
                    _DateTypeButtonInfo.Content = "م";
                    break;
            }
        }

        private DateTime? StringToDateTime(string value)
        {
            return HelperLocalize.StringToDateTime(value, DateTimeType);
        }

        private string DateTimeToString(DateTime d)
        {
            return HelperLocalize.DateTimeToString(d, DateTimeType, HelperLocalize.ApplicationDateTimeFormat);
        }

        #endregion
    }
}
