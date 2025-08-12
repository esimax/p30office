using System.Windows;
using POL.Lib.Utils;

namespace POL.WPF.Controles.AttachProperties
{
    public class ApUIElement
    {
        #region EnKeyboardLayout attached property

        public static readonly DependencyProperty EnKeyboardLayoutProperty =
            DependencyProperty.RegisterAttached("EnKeyboardLayout", typeof (string), typeof (ApUIElement),
                new PropertyMetadata("-", OnEnKeyboardLayoutPropertyChanged));

        public static string GetEnKeyboardLayout(DependencyObject d)
        {
            return (string) d.GetValue(EnKeyboardLayoutProperty);
        }

        public static void SetEnKeyboardLayout(DependencyObject d, string value)
        {
            d.SetValue(EnKeyboardLayoutProperty, value);
        }

        private static void OnEnKeyboardLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = d as UIElement;
            if (uie == null) return;
            uie.GotFocus += (ns, ne) =>
            {
                var u = ns as UIElement;
                if (u == null) return;
                HelperLocalize.SetEnglishLanguage();
            };
            uie.LostFocus += (ns, ne) =>
            {
                var u = ns as UIElement;
                if (u == null) return;
                HelperLocalize.SetLanguageToDefault();
            };
        }

        #endregion
    }
}
