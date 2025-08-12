using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using System;
using System.Linq;
using System.Windows;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIStringCombo
    {
        public UIStringCombo(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;

                cbeKeyboard.SelectedIndex = 0;
                HelperUtils.Try(() => { cbeKeyboard.SelectedIndex = Item.Int3; });
                cbeKeyboard.SelectedIndexChanged += (s1, e1)
                    => HelperUtils.Try(() =>
                    {
                        Item.Int3 = cbeKeyboard.SelectedIndex; HasChanges = true;
                        teDefaultValue_GotFocus(null, null);
                    });

                var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();

                cbeTable.SelectedItem = (from n in aCacheData.GetProfileTableList() where ((DBCTProfileTable)n.Tag).Oid == Item.Guid1 select n).FirstOrDefault();
                cbeTable.ItemsSource = aCacheData.GetProfileTableList(); 
                cbeTable.SelectedIndexChanged += (s1, e1)
                    =>
                    {
                        try
                        {
                            Item.Guid1 = ((DBCTProfileTable)((CacheItemProfileTable)cbeTable.SelectedItem).Tag).Oid;
                        }
                        catch
                        {
                            Item.Guid1 = Guid.Empty;
                        }
                        HasChanges = true;
                    };

                HelperUtils.Try(
                    () =>
                    {
                        var ss = Item.DefaultValue.Split('|');
                        teDefaultValue.EditValue = ss[0];
                        teDefaultValue2.EditValue = ss[1];
                    });

                teDefaultValue_GotFocus(null, null);
                teDefaultValue.EditValueChanging +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                    };
                teDefaultValue2.EditValueChanging +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                    };

                teDefaultValue.GotFocus += teDefaultValue_GotFocus;
                teDefaultValue2.GotFocus += teDefaultValue_GotFocus;
                teDefaultValue.LostFocus += (s1, e1) => HelperLocalize.SetLanguageToDefault();
                teDefaultValue2.LostFocus += (s1, e1) => HelperLocalize.SetLanguageToDefault();

                teDefaultValue.MaxLength = 64;
                teDefaultValue2.MaxLength = 64;

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
                if (!HasChanges) return;
                Item.DefaultValue = string.Format("{0}|{1}", teDefaultValue.Text, teDefaultValue2.Text);
                Item.Save();
            };
        }

        void teDefaultValue_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Item.Int3 == 0)
            {
                teDefaultValue.FlowDirection = HelperLocalize.ApplicationFlowDirection;
                teDefaultValue2.FlowDirection = HelperLocalize.ApplicationFlowDirection;
                HelperLocalize.SetLanguageToDefault();
            }
            else if (Item.Int3 == 1)
            {
                teDefaultValue.FlowDirection = FlowDirection.RightToLeft;
                teDefaultValue2.FlowDirection = FlowDirection.RightToLeft;
                HelperLocalize.SetLanguageToRTL();
            }
            else if (Item.Int3 == 2)
            {
                teDefaultValue.FlowDirection = FlowDirection.LeftToRight;
                teDefaultValue2.FlowDirection = FlowDirection.LeftToRight;
                HelperLocalize.SetLanguageToLTR();
            }
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }
        private void ButtonInfo_Click_1(object sender, RoutedEventArgs e)
        {
            var mw = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var v = mw.ShowManageProfileTable(Window.GetWindow(this));

            cbeTable.ItemsSource = null;

            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            cbeTable.ItemsSource = aCacheData.GetProfileTableList();


            var dbctProfileTable = v as DBCTProfileTable;
            if (dbctProfileTable != null)
            {
                cbeTable.SelectedItem =
                    (from n in aCacheData.GetProfileTableList()
                     where ((DBCTProfileTable)n.Tag).Oid == dbctProfileTable.Oid
                     select n).FirstOrDefault();
            }
        }
    }
}
