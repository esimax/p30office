using POL.DB.P30Office;
using POL.Lib.Utils;
using System;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIImage
    {
        public UIImage(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;

                HelperUtils.Try(() => { teWidth.EditValue = Item.Int1; });
                HelperUtils.Try(() => { teHeight.EditValue = Item.Int2; });


                teWidth.EditValueChanging += (s1, e1)
                    => HelperUtils.Try(() => { Item.Int1 = Convert.ToInt32(teWidth.EditValue); HasChanges = true; });
                teHeight.EditValueChanging += (s1, e1)
                    => HelperUtils.Try(() => { Item.Int2 = Convert.ToInt32(teHeight.EditValue); HasChanges = true; });

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

