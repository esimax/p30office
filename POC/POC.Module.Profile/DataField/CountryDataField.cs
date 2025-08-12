using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.P30Office.GL;
using POL.DB.Root;
using POL.Lib.Interfaces;
using System;

namespace POC.Module.Profile.DataField
{
    class CountryDataField : IDataField
    {
        public string Title
        {
            get { return "كشور"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Country; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UICountry.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;

            dbpv.Guid1NP = dbpv.Guid1;

            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            var rv = new LookUpEdit
            {
                IsTextEditable = false,
                StyleSettings = new SearchLookUpEditStyleSettings(),
                DisplayMember = "Title",
                AutoPopulateColumns = false,
                IncrementalFiltering = true,
                ImmediatePopup = true,
                PopupMinWidth = 320,
                PopupContentTemplate = tag as ControlTemplate,
                ItemsSource = aCacheData.GetCountryList(),
                SelectedItem = (from n in aCacheData.GetCountryList() where ((DBGLCountry)n.Tag).Oid == dbpv.Guid1 select n).FirstOrDefault(),
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };
            rv.EditValueChanged +=
                (s, e) =>
                {
                    var te = (LookUpEdit)s;
                    var db = (DBCTProfileValue)te.Tag;
                    var newVal = te.EditValue == null ? Guid.Empty : ((DBGLCountry)((CacheItemCountry)te.EditValue).Tag).Oid;
                    db.Guid1NP = newVal;
                    db.String1NP = te.EditValue == null ? string.Empty : ((DBGLCountry)((CacheItemCountry)te.EditValue).Tag).TitleXX;
                    if (updateSaveStatus != null)
                        updateSaveStatus(db, db.Guid1 != db.Guid1NP);
                };
            var bi = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
                IsEnabled = !isReadOnly,
            };
            rv.Buttons.Add(bi);
            bi.Click +=
                (s, e) =>
                {
                    rv.SelectedItem = null;
                };
            return rv;
        }



        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UICountry(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            var aDatabse = ServiceLocator.Current.GetInstance<IDatabase>();
            var dbc = DBGLCountry.FindByISO3(aDatabse.Dxs, dbi.DefaultValue);
            if (dbc != null)
                dbv.Guid1 = dbc.Oid;
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
            if (st.ContactSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            return string.Format(st.ContactSettings.Format, dbv.Int1, dbv.String1);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return db.Guid1NP != Guid.Empty && !string.IsNullOrWhiteSpace(db.String1NP);

        }
    }
}
