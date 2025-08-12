using POL.Lib.XOffice;
using POL.WPF.DXControls.POLPhoneItem;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using POL.Lib.Interfaces;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Utils;

namespace POC.Module.ABPhoneMonitoring.Views
{
    public partial class UApplicationBarPhoneMonitoring : UserControl
    {
        private IMembership AMembership { get; set; }

        public UApplicationBarPhoneMonitoring()
        {
            InitializeComponent();

            AMembership = ServiceLocator.Current.GetInstance<IMembership>();

            AMembership.OnMembershipStatusChanged +=
                (s, e) =>
                {
                    if (e.Status == EnumMembershipStatus.AfterLogin)
                    {
                        HelperUtils.DoDispatcher(
                            () =>
                            {
                                var line = 1;
                                var APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
                                if (APOCCore == null) return;
                                var lines = APOCCore.LineNames;
                                if (lines == null || lines.Count == 0)
                                    return;

                                if (lines.Count > ConstantGeneral.MaximumPhoneLineSupported) return;
                                UpdatePhoneItems(APOCCore);
                                DynamicPhoneItems.ForEach(
                                    pi =>
                                    {
                                        pi.TextLineName = APOCCore.LineNames == null ? string.Empty : (APOCCore.LineNames.ContainsKey(line)
                                                              ? APOCCore.LineNames[line]
                                                              : string.Empty);

                                        pi.Visibility = Visibility.Visible;
                                        if (AMembership.ActiveUser.UserName.ToLower() != "admin")
                                        {
                                            if (line == 1 && !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_1))
                                                pi.Visibility = Visibility.Collapsed;
                                            if (line == 2 && !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_2))
                                                pi.Visibility = Visibility.Collapsed;
                                            if (line == 3 && !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_3))
                                                pi.Visibility = Visibility.Collapsed;
                                            if (line == 4 && !AMembership.ActiveUser.HasPermission((int)PCOPermissions.Line_Filter_4))
                                                pi.Visibility = Visibility.Collapsed;
                                        }

                                        line++;
                                    });
                            });
                    }
                };
            DynamicPhoneItems = new List<POLPhoneItem>();
        }

        private void UpdatePhoneItems(POCCore poccore)
        {
            DynamicPhoneItems = new List<POLPhoneItem>();
            for (var i = 0; i < ConstantGeneral.MaximumPhoneLineSupported; i++)
            {
                var line = i + 1;
                var pi = new POLPhoneItem
                {
                    BackgroundHeader = Brushes.DarkGray,
                    BackgroundLineDuration = Brushes.WhiteSmoke,
                    TextLineName = poccore.LineNames == null? string.Empty:  ( poccore.LineNames.ContainsKey(line) ? poccore.LineNames[line] : string.Empty),
                };

                pi.SetBinding(POLPhoneItem.TextLineDurationProperty, new Binding(string.Format("AllData[{0}].TextLineDuration", i)));
                pi.SetBinding(POLPhoneItem.BackgroundLineDurationProperty, new Binding(string.Format("AllData[{0}].BackgroundLineDuration", i)));
                pi.SetBinding(POLPhoneItem.VisibilityCallInProperty, new Binding("CallInVisibility"));
                pi.SetBinding(POLPhoneItem.VisibilityCallOutProperty, new Binding("CallOutVisibility"));
                pi.SetBinding(POLPhoneItem.VisibilityExtraProperty, new Binding("ExtraVisibility"));

                pi.SetBinding(POLPhoneItem.TextCallInDurationProperty, new Binding(string.Format("AllData[{0}].TextCallInDuration", i)));
                pi.SetBinding(POLPhoneItem.TextCallInPhoneProperty, new Binding(string.Format("AllData[{0}].TextCallInPhone", i)));
                pi.SetBinding(POLPhoneItem.TextCallInTimeProperty, new Binding(string.Format("AllData[{0}].TextCallInTime", i)));
                pi.SetBinding(POLPhoneItem.TextCallInTitleProperty, new Binding(string.Format("AllData[{0}].TextCallInTitle", i)));

                pi.SetBinding(POLPhoneItem.TextCallOutDurationProperty, new Binding(string.Format("AllData[{0}].TextCallOutDuration", i)));
                pi.SetBinding(POLPhoneItem.TextCallOutPhoneProperty, new Binding(string.Format("AllData[{0}].TextCallOutPhone", i)));
                pi.SetBinding(POLPhoneItem.TextCallOutTimeProperty, new Binding(string.Format("AllData[{0}].TextCallOutTime", i)));
                pi.SetBinding(POLPhoneItem.TextCallOutTitleProperty, new Binding(string.Format("AllData[{0}].TextCallOutTitle", i)));

                pi.SetBinding(POLPhoneItem.BackgroundHeaderProperty, new Binding(string.Format("AllData[{0}].BackgroundHeader", i)));
                pi.SetBinding(POLPhoneItem.TextLineDurationProperty, new Binding(string.Format("AllData[{0}].TextLineDuration", i)));

                pi.SetBinding(POLPhoneItem.TextExtraTitleProperty, new Binding(string.Format("AllData[{0}].TextExtraTitle", i)));


                DynamicPhoneItems.Add(pi);
                flcMain.Children.Add(pi);
            }
        }

        public List<POLPhoneItem> DynamicPhoneItems { get; set; }

    }
}
