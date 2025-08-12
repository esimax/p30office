using System;
using POL.DB.P30Office;
using POL.Lib.Utils;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UILocation
    {
        public UILocation(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                cbShowMap.IsChecked = Item.Int1 == 0;

                cbShowMap.Checked += (s1, e1) => { Item.Int1 = 0; HasChanges = true; };
                cbShowMap.Unchecked += (s1, e1) => { Item.Int1 = 1; HasChanges = true; };

                HelperUtils.Try(
                    () =>
                    {
                        var dv = Item.DefaultValue;
                        var ss = dv.Split('|');
                        seLatMax.Value = Convert.ToDecimal(ss[0]);
                        seLongMax.Value = Convert.ToDecimal(ss[1]);
                        seZoom.Value = Convert.ToDecimal(ss[2]);
                    });

                seLatMax.EditValueChanging +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                    };
                seLongMax.EditValueChanging +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                    };
                seZoom.EditValueChanging +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
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
                if (!HasChanges) return;
                Item.DefaultValue = string.Format("{0}|{1}|{2}", seLatMax.Value, seLongMax.Value, seZoom.Value);
                Item.Save();
            };
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }
    }
}
