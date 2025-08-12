using POL.DB.P30Office;
using POL.Lib.Utils;
using System;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIFile
    {
        public UIFile(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                cbeUnit.SelectedIndex = 0;
                HelperUtils.Try(() => { cbeUnit.SelectedIndex = Item.Int1; });
                cbeUnit.SelectedIndexChanged += (s1, e1)
                  => HelperUtils.Try(
                      () =>
                      {
                          Item.Int1 = cbeUnit.SelectedIndex;
                          teSize.IsEnabled = (Item.Int1 != 0);
                          HasChanges = true;
                      });

                teSize.IsEnabled = (Item.Int1 != 0);
                teSize.EditValue = Item.Int2;
                teSize.EditValueChanging += (s1, e1)
                    => HelperUtils.Try(() => { Item.Int2 = Convert.ToInt32(teSize.EditValue); HasChanges = true; });
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
                {
                    Item.Save();
                }
            };
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }
    }
}
