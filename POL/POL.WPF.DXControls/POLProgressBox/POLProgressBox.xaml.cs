using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;

namespace POL.WPF.DXControls
{
    public partial class POLProgressBox : DXWindow
    {
        public POLProgressBox(Window owner, string title, bool canCancel, double min, double max, int lineCount,
            Action<POLProgressBox> action, Action<POLProgressBox> finishedAction)
        {
            InitializeComponent();

            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            if (title != null)
                Title = title;

            if (!canCancel)
                bCancel.IsEnabled = false;

            if (Math.Abs(min - max) < 0.01)
                pbeMain.StyleSettings = new ProgressBarMarqueeStyleSettings();
            else
            {
                pbeMain.StyleSettings = new ProgressBarStyleSettings();
                pbeMain.Minimum = min;
                pbeMain.Maximum = max;
            }

            Text1.Visibility = Visibility.Collapsed;
            Text2.Visibility = Visibility.Collapsed;
            Text3.Visibility = Visibility.Collapsed;
            Text4.Visibility = Visibility.Collapsed;
            Text5.Visibility = Visibility.Collapsed;
            Text6.Visibility = Visibility.Collapsed;

            if (lineCount >= 1)
                Text1.Visibility = Visibility.Visible;
            if (lineCount >= 2)
                Text2.Visibility = Visibility.Visible;
            if (lineCount >= 3)
                Text3.Visibility = Visibility.Visible;
            if (lineCount >= 4)
                Text4.Visibility = Visibility.Visible;
            if (lineCount >= 5)
                Text5.Visibility = Visibility.Visible;
            if (lineCount >= 6)
                Text6.Visibility = Visibility.Visible;

            Loaded += (s, e) =>
            {
                NeedToCancel = false;
                if (action != null)
                    Task.Factory.StartNew(() => action.Invoke(this))
                        .ContinueWith(e1 =>
                        {
                            if (finishedAction != null)
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, finishedAction, this);
                        })
                        .ContinueWith(e1 => AsyncClose());
            };
            Closing += (s, e) =>
            {
                if (!NeedToCancel)
                    e.Cancel = true;
            };
        }

        public bool NeedToCancel { get; set; }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            NeedToCancel = true;
            bCancel.IsEnabled = false;
        }

        public void AsyncSetText(int line, string text)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action(() =>
            {
                switch (line)
                {
                    case 1:
                        Text1.Text = text;
                        break;
                    case 2:
                        Text2.Text = text;
                        break;
                    case 3:
                        Text3.Text = text;
                        break;
                    case 4:
                        Text4.Text = text;
                        break;
                    case 5:
                        Text5.Text = text;
                        break;
                    case 6:
                        Text6.Text = text;
                        break;
                }
            }));
        }

        public void AsyncSetValue(double d)
        {
            Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { pbeMain.Value = d; }));
        }

        public void AsyncDisableCancel()
        {
            Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { bCancel.IsEnabled = false; }));
        }

        public void AsyncEnableCancel()
        {
            Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate
            {
                NeedToCancel = false;
                bCancel.IsEnabled = true;
            }));
        }

        public void AsyncSetMax(double max)
        {
            Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate
            {
                pbeMain.Maximum = max;
                if (Math.Abs(pbeMain.Minimum - max) < 0.01)
                    pbeMain.StyleSettings = new ProgressBarMarqueeStyleSettings();
                else
                {
                    pbeMain.StyleSettings = new ProgressBarStyleSettings();
                    pbeMain.Maximum = max;
                }
            }));
        }

        public void AsyncClose()
        {
            NeedToCancel = true;
            Dispatcher.Invoke(DispatcherPriority.Send, new Action(Close));
        }


        public static void Show(string title, bool canCancel, double min, double max, int lineCount,
            Action<POLProgressBox> action, Action<POLProgressBox> finishedAction, Window owner = null)
        {
            var w = new POLProgressBox(owner, title, canCancel, min, max, lineCount, action, finishedAction)
            {
                Owner = owner
            };
            w.ShowDialog();
        }

        public static void Show(int lineCount, Action<POLProgressBox> action, Action<POLProgressBox> finishedAction,
            Window owner = null)
        {
            var w = new POLProgressBox(owner, null, false, 0, 0, lineCount, action, finishedAction) {Owner = owner};
            w.ShowDialog();
        }
    }
}
