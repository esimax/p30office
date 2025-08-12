using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.Lib.XOffice;

namespace POC.Shell.Views
{
    public partial class WShellView
    {
        private IApplicationBar AApplicationBar { get; set; }
        private IMessagingClient AMessageClient { get; set; }
        private IPopup APopup { get; set; }

        public WShellView()
        {
            InitializeComponent();
            AllowsTransparency = true;
            ResizeMode = ResizeMode.NoResize;

            Background = Brushes.Transparent;
            WindowStartupLocation = WindowStartupLocation.Manual;
            WindowStyle = WindowStyle.None;

            Title = ConstantGeneral.ApplicationTitle;
            
            Topmost = true;
            ShowInTaskbar =false;
            
            Height = 24;
            Width = 24;
            Left = (int)SystemParameters.PrimaryScreenWidth - Width + 1;
            Top = ((int)SystemParameters.PrimaryScreenHeight - Height) / 2;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                APopup = ServiceLocator.Current.GetInstance<IPopup>();
                AMessageClient = ServiceLocator.Current.GetInstance<IMessagingClient>();
                AApplicationBar = ServiceLocator.Current.GetInstance<IApplicationBar>();
                AApplicationBar.OnSlideIn += (s, e) =>
                {
                    var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5)) { EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseOut } };
                    BeginAnimation(Window.OpacityProperty, anim);
                };
                AApplicationBar.OnSlideOut += (s, e) =>
                {
                    var anim = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5)) { EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseIn } };
                    BeginAnimation(Window.OpacityProperty, anim);
                };



                #region EnumMessagKind.ApplicationUpdate
                AMessageClient.RegisterHookForMessage(
                        m =>
                        {
                            var version = m.MessageData[0];
                            var ui = new UPopupAppUpdate(version);
                            APopup.AddPopup(ui);
                        }, EnumMessageKind.ApplicationUpdate);
                #endregion
                #region EnumMessagKind.BlackList
                AMessageClient.RegisterHookForMessage(
                        m =>
                        {
                            var msg = m.MessageData[0];
                            var ui = new UPopupMessage(msg, new SolidColorBrush(Colors.Gold), 
                                HelperImage.GetSpecialImage64("_64_License.png"));
                            APopup.AddPopup(ui);
                        }, EnumMessageKind.BlackList);
                #endregion
                #region EnumMessagKind.Support
                AMessageClient.RegisterHookForMessage(
                        m =>
                        {
                            var msg = m.MessageData[0];
                            var ui = new UPopupMessage(msg, new SolidColorBrush(Colors.Red),
                                HelperImage.GetSpecialImage64("_64_License.png"));
                            APopup.AddPopup(ui);
                        }, EnumMessageKind.Support);
                #endregion
                #region EnumMessagKind.Backup
                AMessageClient.RegisterHookForMessage(
                        m =>
                        {
                            var msg = m.MessageData[0];
                            var ui = new UPopupMessage(msg, new SolidColorBrush(Colors.Red),
                                HelperImage.GetSpecialImage64("_64_Database.png"));
                            APopup.AddPopup(ui);
                        }, EnumMessageKind.Backup);
                #endregion
                #region EnumMessagKind.Backup
                AMessageClient.RegisterHookForMessage(
                        m =>
                        {
                            var msg = m.MessageData[0];
                            var ui = new UPopupMessage(msg, new SolidColorBrush(Colors.Red),
                                HelperImage.GetSpecialImage64("_64_hdd.png"));
                            APopup.AddPopup(ui);
                        }, EnumMessageKind.DriveStorage);
                #endregion
            }

            MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            };

            LocationChanged += (s, e) =>
            {
                var l = (int)SystemParameters.PrimaryScreenWidth - Width + 1;
                if (Math.Abs(Left - l) > 0.05)
                    Left = l;
            };


            MouseDoubleClick += (s, e) => AApplicationBar.SlideIn(s, null);

            Loaded += (s, e) => { WAppBarView.ShowWindow(); HelperLocalize.SetLanguageToDefault(); };



        }
    }
}
