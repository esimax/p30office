using System;
using System.Globalization;
using System.Windows.Media;
using POL.DB.P30Office;
using POL.Lib.Utils;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIColor
    {
        public UIColor(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;

                HelperUtils.Try(() =>
                {
                    pceDefaultValue.EditValue = HelperConvert.DoubleToColor(Convert.ToDouble(Item.DefaultValue));
                });

                pceDefaultValue.EditValueChanging +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        HelperUtils.Try(() =>
                        {
                            Item.DefaultValue = HelperConvert.ColorToDouble((Color)pceDefaultValue.EditValue).ToString(CultureInfo.InvariantCulture);
                        });
                    };
                cbRequired.IsChecked = item.IsRequired;
                cbRequired.EditValueChanged +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        Item.IsRequired = cbRequired.IsChecked == true;
                    };
            };
            Unloaded += (s, e) => {
                                      if (HasChanges)
                                      {
                                          Item.Save();
                                      } };
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }
    }
}
