using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace POL.Lib.Utils
{
    public class HelperUtils
    {
        public static void Try(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch
            {
            }
        }

        public static void Try(Action action, string message)
        {
            try
            {
                action.Invoke();
            }
            catch
            {
                throw new Exception(message);
            }
        }

        public static void AllowUIToUpdate()
        {
            var frame = new DispatcherFrame();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render,
                new DispatcherOperationCallback(delegate
                {
                    frame.Continue = false;
                    return null;
                }), null);
            Dispatcher.PushFrame(frame);
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        public static void DoDispatcher(Action action, DispatcherPriority priority = DispatcherPriority.Send)
        {
            Application.Current.Dispatcher.Invoke(priority, new Action(action.Invoke));
        }

        public static Window GetActiveWindow()
        {
            return Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
        }

        public static List<string> WrapText(string text, double pixels, string fontFamily, float emSize)
        {
            var originalLines = text.Split(new[] {" "}, StringSplitOptions.None);
            var wrappedLines = new List<string>();

            var actualLine = new StringBuilder();
            double actualWidth = 0;

            foreach (var item in originalLines)
            {
                var formatted = new FormattedText(item, CultureInfo.CurrentCulture,
                    HelperLocalize.ApplicationFlowDirection,
                    new Typeface(fontFamily), emSize, Brushes.Black);
                actualLine.Append(item + " ");
                actualWidth += formatted.Width;

                if (!(actualWidth > pixels)) continue;
                wrappedLines.Add(actualLine.ToString());
                actualLine.Clear();
                actualWidth = 0;
            }

            return wrappedLines;
        }

        public static string WrapText(string text, double pixels)
        {
            var sl = WrapText(text, pixels, HelperLocalize.ApplicationFontName,
                (float) HelperLocalize.ApplicationFontSize);
            return sl.Aggregate("", (current, v) => current + v + Environment.NewLine);
        }
    }
}
