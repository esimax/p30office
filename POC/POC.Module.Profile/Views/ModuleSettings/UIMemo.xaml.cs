using System;
using System.Windows;
using POL.DB.P30Office;
using POL.Lib.Utils;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIMemo
    {
        public UIMemo(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                cbeHeight.SelectedIndex = 2;
                HelperUtils.Try(() => { cbeHeight.SelectedIndex = Item.Int2; });
                cbeHeight.SelectedIndexChanged += (s1, e1)
                   => HelperUtils.Try(() => { Item.Int2 = cbeHeight.SelectedIndex; HasChanges = true; });

                cbeKeyboard.SelectedIndex = 0;
                HelperUtils.Try(() => { cbeKeyboard.SelectedIndex = Item.Int3; });
                cbeKeyboard.SelectedIndexChanged += (s1, e1)
                    => HelperUtils.Try(() =>
                    {
                        Item.Int3 = cbeKeyboard.SelectedIndex; HasChanges = true;
                        teDefaultValue_GotFocus(null, null);
                    });

                teLimit.EditValue = Item.Int1;
                teDefaultValue_GotFocus(null, null);
                teLimit.EditValueChanging += (s1, e1)
                    => HelperUtils.Try(() =>
                    {
                        Item.Int1 = Convert.ToInt32(teLimit.EditValue); HasChanges = true;
                        if (teDefaultValue.Text.Length > Item.Int1)
                        {
                            teDefaultValue.Text = teDefaultValue.Text.Substring(0, item.Int1);
                        }
                        teDefaultValue.MaxLength = item.Int1;
                    });

                teDefaultValue.EditValue = Item.DefaultValue;
                teDefaultValue.EditValueChanged +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        Item.DefaultValue = teDefaultValue.Text;
                    };

                teDefaultValue.GotFocus += teDefaultValue_GotFocus; ;
                teDefaultValue.LostFocus +=
                    (s1, e1) =>
                    {
                        HelperLocalize.SetLanguageToDefault();
                    };

                cbRequired.IsChecked = item.IsRequired;
                cbRequired.EditValueChanged +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        Item.IsRequired = cbRequired.IsChecked == true;
                    };
            };
            Unloaded += (s, e) =>
            {
                if (HasChanges)
                    try
                    {
                        Item.Save();
                    }
                    catch
                    {
                        if (item.DefaultValue.Length > 100)
                        {
                            item.DefaultValue = item.DefaultValue.Substring(0, 100);
                            item.Save();
                        }
                    }
            };


        }

        void teDefaultValue_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Item.Int3 == 0)
            {
                teDefaultValue.FlowDirection = HelperLocalize.ApplicationFlowDirection;
                HelperLocalize.SetLanguageToDefault();
            }
            else if (Item.Int3 == 1)
            {
                teDefaultValue.FlowDirection = FlowDirection.RightToLeft;
                HelperLocalize.SetLanguageToRTL();
            }
            else if (Item.Int3 == 2)
            {
                teDefaultValue.FlowDirection = FlowDirection.LeftToRight;
                HelperLocalize.SetLanguageToLTR();
            }
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }
    }
}
