using System.Globalization;
using POL.DB.P30Office;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIBool
    {
        public UIBool(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                    HasChanges = false;
                    cbType.SelectedIndex = 0;
                    cbDefaultValue.SelectedIndex = 0;

                    if (Item != null)
                    {
                        if (Item.Int1 == 1)
                            cbType.SelectedIndex = 1;
                        switch (Item.DefaultValue)
                        {
                            case "0":
                                cbDefaultValue.SelectedIndex = 0;
                                break;
                            case "1":
                                cbDefaultValue.SelectedIndex = 1;
                                break;
                            default:
                                cbDefaultValue.SelectedIndex = 2;
                                break;
                        }
                    }

                    cbDefaultValue.SelectedIndexChanged +=
                        (s1, e1) =>
                            {
                                Item.DefaultValue = cbDefaultValue.SelectedIndex.ToString(CultureInfo.InvariantCulture);
                                HasChanges = true;
                            };
                };
            cbType.SelectedIndexChanged +=
                (s, e) =>
                {
                    if (Item == null) return;
                    HasChanges = true;
                    Item.Int1 = cbType.SelectedIndex;
                };
            Unloaded +=
                (s, e) =>
                {
                    if (Item == null) return;
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
