using System;
using DevExpress.Xpf.Editors;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.DataField
{
    class BoolDataField : IDataField
    {
        public string Title
        {
            get { return "جعبه انتخاب"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Bool; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Standard/16/_16_UICheckBox.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;

            dbpv.Int1NP = dbpv.Int1;

            var rv = new CheckEdit
            {
                IsThreeState = dbpv.ProfileItem.Int1 == 1,
                IsChecked = false,
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };

            switch (dbpv.Int1)
            {
                case 1:
                    rv.IsChecked = true;
                    break;
                case 2:
                    rv.IsChecked = null;
                    break;
            }

            rv.Unchecked += (s, e) =>
            {
                var te = (CheckEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                db.Int1NP = 0;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.Int1 != db.Int1NP);
            };
            rv.Checked += (s, e) =>
            {
                var te = (CheckEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                db.Int1NP = 1;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.Int1 != db.Int1NP);
            };
            rv.Indeterminate += (s, e) =>
            {
                var te = (CheckEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                db.Int1NP = 2;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.Int1 != db.Int1NP);
            };

            return rv;
        }






        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIBool(pi);
        }
        public void SetValueToDefault(object dbValue, object dbItem)
        {
            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            switch (dbi.DefaultValue)
            {
                case "0":
                    dbv.Int1 = 0;
                    break;
                case "1":
                    dbv.Int1 = 1;
                    break;
                default:
                    dbv.Int1 = 2;
                    break;
            }
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Int1 = db.Int1NP;
            db.Save();
        }


        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.BoolSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            if (dbv.Int1 == 0) return st.BoolSettings.ReturnForFalse;
            if (dbv.Int1 == 1) return st.BoolSettings.ReturnForTrue;
            return st.BoolSettings.ReturnForNull;
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            return true;
        }
    }
}
