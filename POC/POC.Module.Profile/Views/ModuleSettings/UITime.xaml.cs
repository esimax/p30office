using POL.DB.P30Office;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UITime
    {
        public UITime(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;

                cbRequired.IsChecked = item.IsRequired;
                cbRequired.EditValueChanged +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        Item.IsRequired = cbRequired.IsChecked==true;
                    };
                teDefaultValue.EditValue = Item.DefaultValue;
                teDefaultValue.EditValueChanging +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        Item.DefaultValue = teDefaultValue.Text;
                    };


                cbDefaultIsNow.IsChecked = (Item.Int3 != 0);
                cbDefaultIsNow.Checked += (s1, e1) => { Item.Int3 = 1; HasChanges = true; };
                cbDefaultIsNow.Unchecked += (s1, e1) => { Item.Int3 = 0; HasChanges = true; };
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
