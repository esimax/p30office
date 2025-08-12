using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace POL.WPF.DXControls.POLHeaderedSeparator
{
    public class POLHeaderedSeparator : Control
    {
        public static DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof (string),
            typeof (POLHeaderedSeparator));

        static POLHeaderedSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (POLHeaderedSeparator),
                new FrameworkPropertyMetadata(typeof (POLHeaderedSeparator)));
        }

        public POLHeaderedSeparator()
        {
            IsTabStop = false;
            Background = Brushes.Transparent;
        }

        public string Header
        {
            get { return (string) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
