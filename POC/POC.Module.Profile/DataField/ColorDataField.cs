using System;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.DataField
{
    class ColorDataField : IDataField
    {
        public string Title
        {
            get { return "رنگ"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Color; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UIColor.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            dbpv.Double1NP = dbpv.Double1;

            var rv = new PopupColorEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = dbpv.GetColorValue(),
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };

            rv.EditValueChanged += (s, e) =>
            {
                var te = (PopupColorEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                db.Double1NP = POL.Lib.Utils.HelperConvert.ColorToDouble((Color)te.EditValue);
                if (updateSaveStatus != null)
                    updateSaveStatus(db, Math.Abs(db.Double1NP - db.Double1) >= 1);
            };

            return rv;
        }



        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIColor(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            var v = 0.0;
            Double.TryParse(dbi.DefaultValue, out v);
            dbv.Double1 = v;
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            db.Double1 = db.Double1NP;
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.ColorSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var color = dbv.GetColorValue();
            return string.Format(st.ColorSettings.Format, color.A, color.R, color.G, color.B);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return Convert.ToInt64(db.Double1NP) != 0;
        }
    }
}
