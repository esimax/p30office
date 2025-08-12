using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.P30Office.GL;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UICity
    {
        public UICity(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
                var cl = aCacheData.GetCountryList().ToList();

                HasChanges = false;
                cbLimit.IsChecked = Item.Int1 == 1;
                var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                lueCountry.ItemsSource = cl;
                var q = from n in cl where n.ISO3 == Item.String1 select n;
                lueCountry.SelectedItem = q.FirstOrDefault();

                lueCountry.SelectedIndexChanged += (s1, e1) =>
                {
                    Item.String1 = ((CacheItemCountry)lueCountry.SelectedItem).ISO3;
                    HasChanges = true;
                };

                cbLimit.Checked += (s1, e1) => { Item.Int1 = 1; HasChanges = true; };
                cbLimit.Unchecked += (s1, e1) => { Item.Int1 = 0; HasChanges = true; };


                Guid guid;
                lueCountryDefault.ItemsSource = cl;
                if (Guid.TryParse(Item.DefaultValue, out guid))
                {
                    if (guid == Guid.Empty) return;
                    var db = DBGLCity.FindByOid(aDatabase.Dxs, guid);
                    if (db == null)
                    {
                        cbHasDefault.IsChecked = false;
                        return;
                    }
                    cbHasDefault.IsChecked = true;

                    lueCountryDefault.SelectedItem = (from n in cl where db.Country.ISO3 == n.ISO3 select n).FirstOrDefault();

                    lueCityDefault.ItemsSource = DBGLCity.GetByCountryWithoutTeleCode(db.Session, db.Country, null);
                    lueCityDefault.SelectedItem = db;
                }

                lueCountryDefault.SelectedIndexChanged +=
                    (s1, e1) =>
                    {
                        if (lueCountryDefault.SelectedItem == null)
                        {
                            lueCityDefault.ItemsSource = null;
                            return;
                        }
                        var db = lueCountryDefault.SelectedItem as CacheItemCountry;
                        if (db == null)
                        {
                            lueCityDefault.ItemsSource = null;
                            return;
                        }
                        lueCityDefault.ItemsSource = DBGLCity.GetByCountryWithoutTeleCode(aDatabase.Dxs, db.Tag as DBGLCountry, null);
                        lueCityDefault.SelectedItem = null;
                    };

                lueCityDefault.SelectedIndexChanged +=
                    (s1, e1) =>
                    {
                        var db = lueCityDefault.SelectedItem as DBGLCity;
                        HasChanges = true;
                        Item.DefaultValue = db == null ? Guid.Empty.ToString() : db.Oid.ToString();
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
