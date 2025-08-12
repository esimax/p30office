using POL.DB.P30Office;
using POL.Lib.Utils;
using System;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIDouble
    {
        public UIDouble(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                cbNumberMask.SelectedIndex = 0;
                if (Item != null)
                {
                    if (string.IsNullOrEmpty(Item.String1))
                    {
                        Item.String1 = "n0";
                        Item.Save();
                    }
                    switch (Item.String1)
                    {
                        case "n0":
                            cbNumberMask.SelectedIndex = 0;
                            break;
                        case "d":
                            cbNumberMask.SelectedIndex = 1;
                            break;
                        case "n2":
                            cbNumberMask.SelectedIndex = 2;
                            break;
                        case "n6":
                            cbNumberMask.SelectedIndex = 3;
                            break;
                        case "P0":
                            cbNumberMask.SelectedIndex = 4;
                            break;
                        case "P2":
                            cbNumberMask.SelectedIndex = 5;
                            break;
                    }
                    cbMin.IsChecked = Item.Int1 == 1;
                    cbMax.IsChecked = Item.Int2 == 1;

                    teMin.EditValue = Item.Double1;
                    teMax.EditValue = Item.Double2;

                    teMin.Mask = Item.String1;
                    teMax.Mask = Item.String1;
                    teDefaultValue.Mask = Item.String1;
                    teDefaultValue.EditValue = 0;
                    HelperUtils.Try(() => { teDefaultValue.EditValue = Convert.ToDouble(Item.DefaultValue); });

                    cbRequired.IsChecked = item.IsRequired;
                    cbRequired.EditValueChanged +=
                        (s1, e1) =>
                        {
                            HasChanges = true;
                            Item.IsRequired = cbRequired.IsChecked == true;
                        };
                }

                cbNumberMask.SelectedIndexChanged += (s1, e1) =>
                {
                    if (Item == null) return;
                    HasChanges = true;
                    Item.String1 = GetMask(cbNumberMask.SelectedIndex);
                    teMin.Mask = Item.String1;
                    teMax.Mask = Item.String1;
                    teDefaultValue.Mask = Item.String1;
                };
                teMin.EditValueChanged += (s1, e1) => HelperUtils.Try(
                    () => { Item.Double1 = Convert.ToDouble(teMin.EditValue); HasChanges = true; });
                teMax.EditValueChanged += (s1, e1) => HelperUtils.Try(
                    () => { Item.Double2 = Convert.ToDouble(teMax.EditValue); HasChanges = true; }
                );

                cbMin.Checked += (s1, e1) => { Item.Int1 = 1; HasChanges = true; };
                cbMin.Unchecked += (s1, e1) => { Item.Int1 = 0; HasChanges = true; };
                cbMax.Checked += (s1, e1) => { Item.Int2 = 1; HasChanges = true; };
                cbMax.Unchecked += (s1, e1) => { Item.Int2 = 0; HasChanges = true; };

                teDefaultValue.EditValueChanged +=
                    (s1, e1) =>
                    {
                        Item.DefaultValue = teDefaultValue.Text;
                        HasChanges = true;
                    };
            };


            Unloaded += (s, e) =>
            {
                if (Item == null) return;
                if (HasChanges)
                {
                    Item.Save();
                }
            };
        }

        private string GetMask(int index)
        {
            switch (index)
            {
                case 0:
                    return "n0";
                case 1:
                    return "d";
                case 2:
                    return "n2";
                case 3:
                    return "n6";
                case 4:
                    return "P0";
                case 5:
                    return "P2";
                default:
                    return "d";
            }
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }
    }
}

