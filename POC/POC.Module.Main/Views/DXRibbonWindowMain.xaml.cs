using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Ribbon;
using Microsoft.Practices.ServiceLocation;
using POC.Module.Main.Other;
using POL.Lib.Interfaces;
using DevExpress.Xpf.Bars;
using POL.Lib.XOffice;
using POL.WPF.DXControls;


namespace POC.Module.Main.Views
{
    public partial class DXRibbonWindowMain : IMainWindow, IModuleRibbon
    {
        private IModuleSyncronizer AModuleSyncronizer { get; set; }
        private IMembership AMmembership { get; set; }
        private IPOCRootTools APOCRootTools { get; set; }

        public DXRibbonWindowMain()
        {
            InitializeComponent();

            Title = "بسم الله الرحمن الرحیم";
            Opacity = 0;
            ShowInTaskbar = false;
            ForceClose = false;

            AModuleSyncronizer = ServiceLocator.Current.GetInstance<IModuleSyncronizer>();
            AMmembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCRootTools = ServiceLocator.Current.GetInstance<IPOCRootTools>();

            AMmembership.OnMembershipStatusChanged += 
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogin)
                    {
                        UpdateHomePage();
                        ShowWindow();
                    }

                    if (e.Status == EnumMembershipStatus.AfterLogout)
                    {
                        rdpcMain.Pages.Clear();
                        LoadContent(null, null);
                    }
                };
            Closing +=
                (s, e) =>
                {
                    if (Environment.ExitCode == -2)
                        ForceClose = true;
                    if (ForceClose) return;
                    e.Cancel = true;
                    HideWindow();
                };

            StateChanged +=
                (s, e) =>
                {
                    if (WindowState == WindowState.Minimized)
                    {
                        HideWindow();
                    }
                    else
                    {
                        LastWindowsState = WindowState;
                    }
                };

            bExit.ItemClick += (s, e) =>
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
            };

            Loaded +=
                (s, e) =>
                {
                    LastWindowsState = WindowState;
                };
            HideWindow();
        }

        private void UpdateHomePage()
        {
            var pocCore = ServiceLocator.Current.GetInstance<POCCore>();
            rdpcMain.Pages.Clear();
            var rp = new RibbonPage { Caption = "خانه" };
            rp.Groups.Clear();
            var rpg = new RibbonPageGroup { Caption = "ارجاع به" };

            var list = APOCRootTools.GetList();
            rpg.ItemLinks.Clear();
            if (list != null)
            {
                foreach (var item in list)
                {
                    if (pocCore.STCI.IsTamas)
                    {
                        if (!item.InTamas)
                            continue;
                    }
                    var bbi = new BarButtonItem
                                  {
                                      Content = item.Title,
                                      Name = item.Key,
                                      LargeGlyph = item.Image,
                                      Command = item.Command,
                                      BarItemDisplayMode = BarItemDisplayMode.ContentAndGlyph,
                                      GlyphSize = GlyphSize.Large,
                                      Glyph = item.Image,
                                  };
                    var item1 = item;
                    var q = from n in barManager.Items where n.Name == item1.Key select n;

                    if (!q.Any())
                        barManager.Items.Add(bbi);

                    rpg.ItemLinks.Add(new BarButtonItem
                    {
                        BarItemName = item.Key,
                        GlyphSize = GlyphSize.Large,
                        LargeGlyph = item.Image,
                        Content = item.Title,
                        Command = item.Command,
                        BarItemDisplayMode = BarItemDisplayMode.ContentAndGlyph
                    });
                }
            }
            rp.Groups.Add(rpg);
            rdpcMain.Pages.Add(rp);
        }

        private bool ForceClose { get; set; }
        private static readonly Dictionary<Type, UIElement> MainModuleHolder = new Dictionary<Type, UIElement>();

        #region IMainWindow
        public void HideWindow()
        {
            WindowState = WindowState.Minimized;
        }
        public void ShowWindow()
        {


            ShowInTaskbar = true;
            Opacity = 1;
            Show();
            Activate();
            if (WindowState == WindowState.Minimized)
                WindowState = LastWindowsState;
        }

        public void ShowBusyIndicator()
        {
            SplashScreenHelper.Instance.ShowSplashScreen();
        }
        public void HideBusyIndicator()
        {
            SplashScreenHelper.Instance.HideSplashScreen();
        }

        public void LoadContent(Type view, Type model)
        {
            if (moduleContent.Content != null)
            {
                if (moduleContent.Content.GetType() == view)
                {
                    HideBusyIndicator();
                    return;
                }

            }

            if (moduleContent.Content != null)
            {
                var c2 = moduleContent.Content;

                if (c2 is IModuleRibbon)
                {
                    var v = c2 as IModuleRibbon;
                    v.UnloadChildRibbons();
                    var rc = v.GetRibbon() as RibbonControl;
                    if (rc != null)
                    {
                        mainRibbonControl.UnMerge(rc);
                    }
                }


            }
            if (view == null)
            {
                moduleContent.Content = null;
                return;
            }

            object content;
            if (MainModuleHolder.ContainsKey(view))
            {
                content = MainModuleHolder[view];
                if (content is IModuleRibbon)
                    ((IModuleRibbon)content).LoadChildRibbons();
                HideBusyIndicator();
            }
            else
            {
                content = Activator.CreateInstance(view);
                if (content is FrameworkElement)
                {
                    var fe = content as FrameworkElement;
                    fe.Loaded += (s, e) => HideBusyIndicator();
                    if (model != null)
                        fe.DataContext = Activator.CreateInstance(model, content);
                    MainModuleHolder.Add(view, (UIElement)content);
                }
            }

            if (content is IModuleRibbon)
            {
                var v = content as IModuleRibbon;
                var rc = v.GetRibbon() as RibbonControl;
                if (rc != null)
                    mainRibbonControl.Merge(rc);
            }
            moduleContent.Content = content;
        }


        public Window GetWindow()
        {
            return this;
        }
        #endregion

        #region IModuleRibbon
        public object GetRibbon()
        {
            return mainRibbonControl;
        }
        public void LoadChildRibbons()
        {
        }
        public void UnloadChildRibbons()
        {
        }
        #endregion











        public WindowState LastWindowsState { get; set; }
    }
}
