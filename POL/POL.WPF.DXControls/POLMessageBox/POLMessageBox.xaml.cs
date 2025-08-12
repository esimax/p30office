using System.Media;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;
using POL.Lib.Utils;

namespace POL.WPF.DXControls
{
    public partial class POLMessageBox : DXWindow
    {
        public POLMessageBox(FrameworkElement owner, string message, string caption, MessageBoxButton buttons,
            MessageBoxImage image, MessageBoxResult defaultresult)
        {
            InitializeComponent();

            ResizeMode = ResizeMode.NoResize;
            MinWidth = 320;
            MinHeight = 160;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth - SystemParameters.MaximizedPrimaryScreenWidth/4;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStyle = WindowStyle.ToolWindow;

            if (owner != null)
            {
                Owner = GetWindow(owner);
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            tbMessage.Text = message;
            Title = caption;

            bOK.Visibility = Visibility.Collapsed;
            bYes.Visibility = Visibility.Collapsed;
            bNo.Visibility = Visibility.Collapsed;
            bCancel.Visibility = Visibility.Collapsed;

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    bOK.Visibility = Visibility.Visible;
                    bOK.IsDefault = true;
                    bOK.IsCancel = true;
                    break;
                case MessageBoxButton.OKCancel:
                    bOK.Visibility = Visibility.Visible;
                    bCancel.Visibility = Visibility.Visible;
                    bOK.IsDefault = true;
                    bCancel.IsCancel = true;
                    break;
                case MessageBoxButton.YesNoCancel:
                    bYes.Visibility = Visibility.Visible;
                    bNo.Visibility = Visibility.Visible;
                    bCancel.Visibility = Visibility.Visible;
                    bCancel.IsCancel = true;
                    break;
                case MessageBoxButton.YesNo:
                    bYes.Visibility = Visibility.Visible;
                    bNo.Visibility = Visibility.Visible;
                    bNo.IsCancel = true;
                    break;
            }

            switch (image)
            {
                case MessageBoxImage.None:
                    imgIcon.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxImage.Error:
                    imgIcon.Source = HelperImage.GetSpecialImage48("_48_Error.png");
                    Task.Factory.StartNew(() => SystemSounds.Hand.Play());
                    break;
                case MessageBoxImage.Question:
                    imgIcon.Source = HelperImage.GetSpecialImage48("_48_Question.png");
                    Task.Factory.StartNew(() => SystemSounds.Question.Play());
                    break;
                case MessageBoxImage.Warning:
                    imgIcon.Source = HelperImage.GetSpecialImage48("_48_Warning.png");
                    Task.Factory.StartNew(() => SystemSounds.Exclamation.Play());
                    break;
                case MessageBoxImage.Information:
                    imgIcon.Source = HelperImage.GetSpecialImage48("_48_Information.png");
                    Task.Factory.StartNew(() => SystemSounds.Asterisk.Play());
                    break;
            }

            Result = MessageBoxResult.None;

            switch (defaultresult)
            {
                case MessageBoxResult.None:
                    break;
                case MessageBoxResult.OK:
                    bOK.IsDefault = true;
                    break;
                case MessageBoxResult.Cancel:
                    bCancel.IsDefault = true;
                    break;
                case MessageBoxResult.Yes:
                    bYes.IsDefault = true;
                    break;
                case MessageBoxResult.No:
                    bNo.IsDefault = true;
                    break;
            }


        }

        #region Properties

        public MessageBoxResult Result { get; set; }

        #endregion

        #region Static Methods

        public static MessageBoxResult Show(FrameworkElement owner, string message, string caption,
            MessageBoxButton buttons, MessageBoxImage image, MessageBoxResult defaultresult)
        {
            var w = new POLMessageBox(owner, message, caption, buttons, image, defaultresult);
            w.ShowDialog();
            return w.Result;
        }

        public static MessageBoxResult Show(FrameworkElement owner, string message, string caption,
            MessageBoxButton buttons, MessageBoxImage image)
        {
            var w = new POLMessageBox(owner, message, caption, buttons, image, MessageBoxResult.OK);
            w.ShowDialog();
            return w.Result;
        }

        public static MessageBoxResult Show(FrameworkElement owner, string message, string caption,
            MessageBoxButton buttons)
        {
            var w = new POLMessageBox(owner, message, caption, buttons, MessageBoxImage.Information, MessageBoxResult.OK);
            w.ShowDialog();
            return w.Result;
        }

        public static MessageBoxResult Show(FrameworkElement owner, string message, string caption)
        {
            var w = new POLMessageBox(owner, message, caption, MessageBoxButton.OK, MessageBoxImage.Information,
                MessageBoxResult.OK);
            w.ShowDialog();
            return w.Result;
        }

        public static MessageBoxResult Show(FrameworkElement owner, string message)
        {
            var w = new POLMessageBox(owner, message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information,
                MessageBoxResult.OK);
            w.ShowDialog();
            return w.Result;
        }


        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons,
            MessageBoxImage image, MessageBoxResult defaultresult)
        {
            var w = new POLMessageBox(null, message, caption, buttons, image, defaultresult);
            w.ShowDialog();
            return w.Result;
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons,
            MessageBoxImage image)
        {
            var w = new POLMessageBox(null, message, caption, buttons, image, MessageBoxResult.OK);
            w.ShowDialog();
            return w.Result;
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton buttons)
        {
            var w = new POLMessageBox(null, message, caption, buttons, MessageBoxImage.Information, MessageBoxResult.OK);
            w.ShowDialog();
            return w.Result;
        }

        public static MessageBoxResult Show(string message, string caption)
        {
            var w = new POLMessageBox(null, message, caption, MessageBoxButton.OK, MessageBoxImage.Information,
                MessageBoxResult.OK);
            w.ShowDialog();
            return w.Result;
        }

        public static MessageBoxResult Show(string message)
        {
            var w = new POLMessageBox(null, message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information,
                MessageBoxResult.OK);
            w.ShowDialog();
            return w.Result;
        }

        public static void ShowInformation(string message, Window owner = null)
        {
            Show(owner, message, "پیغام", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        public static void ShowWarning(string message, Window owner = null)
        {
            Show(owner, message, "اخطار", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
        }

        public static void ShowError(string message, Window owner = null)
        {
            Show(owner, message, "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        public static MessageBoxResult ShowQuestionYesNo(string message, Window owner = null)
        {
            return Show(owner, message, "سوال", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
        }

        public static MessageBoxResult ShowQuestionYesNoCancel(string message, Window owner = null)
        {
            return Show(owner, message, "سوال", MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                MessageBoxResult.Yes);
        }

        #endregion

        #region Internal Events

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void bYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void bNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        #endregion
    }
}
