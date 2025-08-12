using System.Linq;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UICountry
    {


        public UICountry(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();


            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                cbHasDefault.IsChecked = !string.IsNullOrWhiteSpace(Item.DefaultValue);
                var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();

                var cl = aCacheData.GetCountryList().ToList();
                lueCountry.ItemsSource = cl;
                var q = from n in cl where n.ISO3 == Item.DefaultValue select n;

                lueCountry.SelectedItem = q.FirstOrDefault();
                lueCountry.SelectedIndexChanged +=
                    (s1, e1) =>
                    {
                        Item.DefaultValue = ((CacheItemCountry)lueCountry.SelectedItem).ISO3;
                        HasChanges = true;
                    };
                cbHasDefault.Unchecked +=
                    (s1, e1) =>
                    {
                        Item.DefaultValue = null;
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
