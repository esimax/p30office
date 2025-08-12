using System;
using System.Windows;
using Microsoft.Win32;
using POL.Lib.Utils;

namespace POL.WPF.Controles.AttachProperties
{
    public class APWindow
    {
        private static void SaveSettings(Window target)
        {
            SaveSettings(target, target.GetValue(StateSizePositionProperty).ToString());
        }

        private static void LoadSettings(Window target)
        {
            LoadSettings(target, target.GetValue(StateSizePositionProperty).ToString());
        }

        public static void SaveSettings(Window win, string name)
        {
            if (win == null) return;
            if (string.IsNullOrWhiteSpace(name)) return;

            RootKey.SetValue(name + "_Top", win.Top.ToString(), RegistryValueKind.String);
            RootKey.SetValue(name + "_Left", win.Left.ToString(), RegistryValueKind.String);
            RootKey.SetValue(name + "_Width", win.Width.ToString(), RegistryValueKind.String);
            RootKey.SetValue(name + "_Height", win.Height.ToString(), RegistryValueKind.String);
            RootKey.SetValue(name + "_WindowState", win.WindowState.ToString(), RegistryValueKind.String);
        }

        public static void LoadSettings(Window win, string name)
        {
            if (win == null) return;
            if (string.IsNullOrWhiteSpace(name)) return;
            try
            {
                win.WindowState =
                    (WindowState) Enum.Parse(typeof (WindowState), RootKey.GetValue(name + "_WindowState").ToString());
                if (win.WindowState == WindowState.Minimized || win.WindowState == WindowState.Normal)
                {
                    win.Width = Convert.ToInt32(RootKey.GetValue(name + "_Width").ToString());
                    win.Height = Convert.ToInt32(RootKey.GetValue(name + "_Height").ToString());
                }
                if (win.WindowState == WindowState.Normal)
                {
                    win.Top = Convert.ToInt32(RootKey.GetValue(name + "_Top").ToString());
                    win.Left = Convert.ToInt32(RootKey.GetValue(name + "_Left").ToString());
                }
            }
            catch
            {
            }
        }

        #region RootKey

        private static RegistryKey _RootKey;

        private static RegistryKey RootKey
        {
            get
            {
                if (_RootKey == null)
                    HelperUtils.Try(() =>
                    {
                        _RootKey =
                            Registry.CurrentUser.CreateSubKey("SOFTWARE").CreateSubKey("P30Office").CreateSubKey(
                                "WindowsState");
                    });
                return _RootKey;
            }
        }

        #endregion

        #region Attached Property StateSizePosition

        public static readonly DependencyProperty StateSizePositionProperty = DependencyProperty.RegisterAttached(
            "StateSizePosition",
            typeof (string),
            typeof (APWindow),
            new FrameworkPropertyMetadata("", OnStateSizePositionChanged));

        public static string GetStateSizePosition(Window d)
        {
            return (string) d.GetValue(StateSizePositionProperty);
        }

        public static void SetStateSizePosition(Window d, string value)
        {
            d.SetValue(StateSizePositionProperty, value);
        }

        private static void OnStateSizePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as Window;
            if (target != null)
            {
                target.Initialized += (s, e1) => LoadSettings(target);
                target.Closing += (s, e1) => SaveSettings(target);
            }
        }

        #endregion
    }
}
