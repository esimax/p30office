using System;
using System.Linq;
using POL.DB.P30Office;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIContact
    {
        public UIContact(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                cbFromCat.IsChecked = Item.Int1 == 1;
                cbFromCat.Checked += (s1, e1) => { Item.Int1 = 1; HasChanges = true; };
                cbFromCat.Unchecked += (s1, e1) =>
                {
                    Item.Int1 = 0;
                    Item.Guid1 = Guid.Empty;
                    HasChanges = true;
                };

                var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();

                cbeCat.SelectedItem = DBCTContactCat.FindByOid(aDatabase.Dxs, Item.Guid1);
                cbeCat.ItemsSource = (from n in aCacheData.GetContactCatList() orderby n.Title select (DBCTContactCat)n.Tag).ToList(); 

                cbeCat.SelectedIndexChanged +=
                    (s1, e1) =>
                    {
                        if (cbeCat.SelectedItem == null)
                        {
                            Item.Int1 = 0;
                            Item.Guid1 = Guid.Empty;
                        }
                        else
                        {
                            Item.Int1 = 1;
                            Item.Guid1 = ((DBCTContactCat)cbeCat.SelectedItem).Oid;
                        }
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
