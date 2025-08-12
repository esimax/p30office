using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.Lib.Utils;
using POL.WPF.DXControls;

namespace POC.Shell.Views
{
    public partial class WAppBarView
    {
        private IApplicationBar AApplicationBar { get; set; }
        private IModuleSyncronizer AModuleSyncronizer { get; set; }
        private POCCore APOCCore { get; set; }
        private IMembership AMembership { get; set; }

        public WAppBarView()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.Manual;
            WindowStyle = WindowStyle.None;
            Title = ConstantGeneral.ApplicationTitle;
            hlSite1.NavigateUri = new Uri(string.Format("https://{0}", ConstantGeneral.WebUrl));
            hlSite1.Inlines.Clear();
            hlSite1.Inlines.Add(ConstantGeneral.WebUrl);

            hlSite2.NavigateUri = new Uri(string.Format("https://{0}/%d8%ab%d8%a8%d8%aa-%d8%aa%db%8c%da%a9%d8%aa", ConstantGeneral.WebUrl));
            hlSite2.Inlines.Clear();
            hlSite2.Inlines.Add("ارسال تیکت پشتیبانی");

            tbEmail.Text = ConstantGeneral.SupportEmail;

            Topmost = true;
            ShowInTaskbar = false;

            Height = SystemParameters.WorkArea.Height;
            try
            {
                var w = HelperSettingsClient.DockWidth;
                if (w < 160) w = 160;
                if (w > 280) w = 280;
                Width = w;
            }
            catch
            {
                Width = 220;
            }
            Left = (int)SystemParameters.PrimaryScreenWidth + 1;
            Top = 0;



            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                AApplicationBar = ServiceLocator.Current.GetInstance<IApplicationBar>();
                AApplicationBar.OnSlideIn += (s, e) =>
                {
                    var anim = new DoubleAnimation(SystemParameters.PrimaryScreenWidth - Width - 1, TimeSpan.FromSeconds(0.5)) { EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseOut } };
                    BeginAnimation(LeftProperty, anim);
                };
                AApplicationBar.OnSlideOut += (s, e) =>
                {
                    var anim = new DoubleAnimation(SystemParameters.PrimaryScreenWidth + 1, TimeSpan.FromSeconds(0.5)) { EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseIn } };
                    BeginAnimation(LeftProperty, anim);
                };

                AModuleSyncronizer = ServiceLocator.Current.GetInstance<IModuleSyncronizer>();
                AModuleSyncronizer.OnModuleFinilize += (s, e) => LoadApplicationBarContent();

                APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
                APOCCore.OnAppBarIsBussyChanged += (s, e) => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    new Action(
                        () =>
                        {
                            IsEnabled = !APOCCore.AppBarIsBussy;
                        }));

                AMembership = ServiceLocator.Current.GetInstance<IMembership>();
                AMembership.OnMembershipStatusChanged += (s, e) => 
                                                             {
                                                                 if (e.Status == EnumMembershipStatus.AfterLogin)
                                                                     EnableAllButtons();
                                                                 if (e.Status == EnumMembershipStatus.AfterLogout)
                                                                 {
                                                                     DiableAllButtons();
                                                                     var b =
                                                                         (from n in spButtons.Children.Cast<Button>()
                                                                          select n).First();
                                                                     b.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                                                                 }
                                                             };

                txVersion.Text = string.Format("Version : {0}", APOCCore.CTSI.ClientVersion);
            }


            MouseDoubleClick += (s, e) =>
            {
                if (imgDock.IsMouseOver) AApplicationBar.SlideOut(s, null);
                if (imgExit.IsMouseOver)
                {
                    if (HelperSettingsClient.SavePassword)
                    {
                        var dr = POLMessageBox.ShowQuestionYesNoCancel("رمزتان ذخیره شود؟", this);
                        if (dr == MessageBoxResult.Yes)
                        {
                            AModuleSyncronizer.RaiseOnApplicationExit(EnumApplicationExit.MainCloseClick);
                        }
                        if (dr == MessageBoxResult.No)
                        {
                            HelperSettingsClient.SavePassword = false;
                            AModuleSyncronizer.RaiseOnApplicationExit(EnumApplicationExit.MainCloseClick);
                        }
                    }
                    else
                    {
                        AModuleSyncronizer.RaiseOnApplicationExit(EnumApplicationExit.MainCloseClick);
                    }
                }
            };

            Loaded += (s, e) =>
            {
                if (HelperSettingsClient.DockIsFixed)
                {
                    HelperApplicationBar.SetAppBar(this, ABEdge.Right);
                    imgDock.Visibility = Visibility.Collapsed;
                }
            };

            Closing += (s, e) =>
                           {
                               HelperLocalize.SetLanguageToLTR();

                               var pocCore = ServiceLocator.Current.GetInstance<POCCore>();
                               if (pocCore == null) return;
                               var ms = ServiceLocator.Current.GetInstance<IMembership>();
                               if (ms == null) return;
                               if (ms.IsAuthorized)
                                   ms.LogoutUser(pocCore.InstanceGuid);

                               POL.WPF.Licensor.HookManager.RemoveAllHooks();
                           };
        }

        private void DiableAllButtons()
        {
            var q = from n in spButtons.Children.Cast<UIElement>() select n as Button;
            foreach (var button in q.Where(button => (button.Tag as ApplicationBarHolder) != null &&
                                                    !(button.Tag as ApplicationBarHolder).IsFirst &&
                                                    (button.Tag as ApplicationBarHolder).Name != ConstantApplicationBar.NameSettings
                                                    ))
            {
                button.IsEnabled = false;
            }
        }

        private void EnableAllButtons()
        {
            var q = from n in spButtons.Children.Cast<UIElement>() select n as Button;
            foreach (var button in q)
            {
                button.IsEnabled = true;
            }
        }

        private void LoadApplicationBarContent()
        {
            spButtons.BeginInit();
            TheGrid.BeginInit();
            var list = AApplicationBar.GetContentList();
            list.ForEach(
                h =>
                {
                    try
                    {
                        var view = Activator.CreateInstance(h.ViewType);
                        var viewFE = view as FrameworkElement;
                        if (viewFE == null)
                            return;
                        var model = Activator.CreateInstance(h.ModelType, new object[] { view });
                        viewFE.DataContext = model;
                        viewFE.SetValue(Panel.ZIndexProperty, h.IsFirst ? 1000 : 0);
                        TheGrid.Children.Add(viewFE);
                        var b = new Button
                            {
                                Content = h.Title,
                                Width = MeasureWidth(h.Title) + 6,
                                SnapsToDevicePixels = true,
                                Margin = new Thickness(3),
                                Style = Application.Current.Resources["UnderLineButton"] as Style,
                                LayoutTransform = new RotateTransform(90, 0.5, 0.5),
                                Tag = h,
                                IsEnabled = h.IsFirst || h.Name == ConstantApplicationBar.NameSettings,
                            };
                        b.Click += (s, e) =>
                            {
                                var button = s as Button;
                                if (button == null) return;
                                var holder = b.Tag as ApplicationBarHolder;
                                if (holder == null) return;
                                if (ActiveHolder != null)
                                    if (ActiveHolder.Name == holder.Name)
                                        return;
                                ActiveHolder = holder;

                                var qTop = (from n in TheGrid.Children.Cast<UIElement>() where Panel.GetZIndex(n) == 1000 select n).ToList();
                                if (qTop.Any()) qTop.First().SetValue(Panel.ZIndexProperty, 0);

                                var q = (from n in TheGrid.Children.Cast<UIElement>() where (n.GetType() == holder.ViewType) select n).ToList();
                                if (q.Any()) q.First().SetValue(Panel.ZIndexProperty, 1000);
                            };
                        spButtons.Children.Add(b);
                    }
                    catch
                    {

                    }
                });
            TheGrid.EndInit();


            spButtons.EndInit();
        }

        public double MeasureWidth(string text)
        {
            var formattedText = new FormattedText(text,
                                                System.Globalization.CultureInfo.GetCultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.LCID),
                                                HelperLocalize.ApplicationFlowDirection,
                                                new Typeface(HelperLocalize.ApplicationFontName),
                                                HelperLocalize.ApplicationFontSize, Brushes.Black);

            return formattedText.Width;
        }

        private ApplicationBarHolder ActiveHolder { get; set; }



        #region selfWindow
        private static WAppBarView SelfWindow { get; set; }
        #endregion

        #region Static Methods
        internal static void ShowWindow()
        {
            if (SelfWindow == null)
            {
                SelfWindow = new WAppBarView();
                SelfWindow.Show();
            }
            SelfWindow.Activate();
        }
        #endregion


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        private void Hyperlink2_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
