using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using POL.DB.P30Office;
using POL.DB.P30Office.GL;
using POL.DB.Root;
using POL.Lib.Interfaces;
using System;
using Microsoft.Practices.ServiceLocation;

namespace POC.Module.Profile.DataField
{
    class CityDataField : IDataField
    {
        public string Title
        {
            get { return "كشور / شهر"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.City; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UICity.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            dbpv.Guid1NP = dbpv.Guid1;

            var rv = new StackPanel();
            var city = DBGLCity.FindByOid(aDatabase.Dxs, dbpv.Guid1);

            var country = city == null ? null : (from n in aCacheData.GetCountryList()
                                                 where ((DBGLCountry)n.Tag).Oid == city.Country.Oid
                                                 select n).FirstOrDefault();

            var lueCountry = new LookUpEdit
            {
                IsTextEditable = false,
                StyleSettings = new SearchLookUpEditStyleSettings(),
                DisplayMember = "Title",
                AutoPopulateColumns = false,
                IncrementalFiltering = true,
                ImmediatePopup = true,
                PopupMinWidth = 320,
                PopupContentTemplate = ((object[])tag)[0] as ControlTemplate,
                ItemsSource = aCacheData.GetCountryList(),
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };
            if (dbpv.ProfileItem.Int1 == 1)
            {
                lueCountry.EditValue = country;
                lueCountry.IsEnabled = false;
            }
            else
            {
                lueCountry.EditValue = country;
            }
            rv.Children.Add(lueCountry);


            var lueCity = new LookUpEdit
            {
                IsTextEditable = false,
                StyleSettings = new SearchLookUpEditStyleSettings(),
                AutoPopulateColumns = false,
                IncrementalFiltering = true,
                ImmediatePopup = true,
                PopupMinWidth = 320,
                PopupContentTemplate = ((object[])tag)[1] as ControlTemplate,
                ItemsSource = DBGLCity.GetByCountryWithoutTeleCode(aDatabase.Dxs, country == null ? null : country.Tag as DBGLCountry, string.Empty),
                Tag = dbpv,
                Name = "city",
                IsReadOnly = isReadOnly,
            };
            lueCity.EditValue = city;

            rv.Children.Add(lueCity);
            lueCity.EditValueChanged +=
                (s, e) =>
                {
                    var te = (LookUpEdit)s;
                    var db = (DBCTProfileValue)te.Tag;
                    if (e.NewValue == null) return;
                    db.Guid1NP = ((DBGLCity)e.NewValue).Oid;
                    db.String1NP = string.Format("{0}, {1}", ((DBGLCity)e.NewValue).StateTitle, ((DBGLCity)e.NewValue).TitleXX);
                    if (updateSaveStatus != null)
                        updateSaveStatus(db, db.Guid1NP != db.Guid1);
                };

            lueCountry.EditValueChanged +=
                (s, e) =>
                {
                    var te = (LookUpEdit)s;
                    var sp = (StackPanel)te.Parent;
                    var uicity = (from n in sp.Children.Cast<FrameworkElement>() where n.Name == "city" select n as LookUpEdit).FirstOrDefault();
                    if (uicity == null) return;
                    uicity.EditValue = null;

                    var ciCountry = te.EditValue as CacheItemCountry;
                    uicity.ItemsSource = DBGLCity.GetByCountryWithoutTeleCode(aDatabase.Dxs, ciCountry == null ? null : (DBGLCountry)ciCountry.Tag, string.Empty);
                };



            var bi = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
                IsEnabled = !isReadOnly,
            };
            lueCity.Buttons.Add(bi);
            bi.Click +=
                (s, e) =>
                {
                    lueCity.EditValue = null;
                };
            return rv;
        }












        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UICity(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            var g = Guid.Empty;
            Guid.TryParse(dbi.DefaultValue, out g);
            dbv.Guid1 = g;
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Guid1 = db.Guid1NP;
            db.String1 = db.String1NP;
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.CitySettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            return string.Format(st.CitySettings.Format, dbv.String1, dbv.String2);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return db.Guid1NP != Guid.Empty;
        }
    }
}
