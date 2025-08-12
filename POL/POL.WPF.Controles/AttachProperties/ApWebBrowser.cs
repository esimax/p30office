using System;
using System.Windows;
using System.Windows.Controls;

namespace POL.WPF.Controles.AttachProperties
{
    public class ApWebBrowser
    {
        #region BindableSource

        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof (object), typeof (ApWebBrowser),
                new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static object GetBindableSource(DependencyObject obj)
        {
            return (string) obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, object value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var browser = o as WebBrowser;
            if (browser == null) return;

            Uri uri = null;

            if (e.NewValue is string)
            {
                var uriString = e.NewValue as string;
                uri = string.IsNullOrWhiteSpace(uriString) ? null : new Uri(uriString);
            }
            else if (e.NewValue is Uri)
            {
                uri = e.NewValue as Uri;
            }

            browser.Source = uri;
        }

        #endregion

        #region Html

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof (string),
            typeof (ApWebBrowser),
            new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof (WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string) d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        private static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = dependencyObject as WebBrowser;
            if (webBrowser != null)
                webBrowser.NavigateToString(e.NewValue as string);
        }

        #endregion
    }
}
