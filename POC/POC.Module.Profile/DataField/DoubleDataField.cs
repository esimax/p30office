using System;
using DevExpress.Xpf.Editors;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using System.Windows;

namespace POC.Module.Profile.DataField
{
    class DoubleDataField : IDataField
    {
        public string Title
        {
            get { return "عدد"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Double; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UIDigit.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;

            dbpv.Double1NP = dbpv.Double1;

            var rv = new SpinEdit
            {
                MaskType = MaskType.Numeric,
                Mask = dbpv.ProfileItem.String1,
                MaskUseAsDisplayFormat = true,
                FlowDirection = FlowDirection.LeftToRight,
                EditValue = dbpv.Double1,
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };

            rv.EditValueChanged += (s, e) =>
            {
                var te = (SpinEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                HelperUtils.Try(() =>
                {
                    var newVal = Convert.ToDouble(te.EditValue);
                    var changed = false;
                    if (db.ProfileItem.Int1 > 0)
                    {
                        if (db.ProfileItem.Double1 > newVal)
                        {
                            newVal = db.ProfileItem.Double1;
                            changed = true;
                        }
                    }
                    if (db.ProfileItem.Int2 > 0)
                    {
                        if (db.ProfileItem.Double2 < newVal)
                        {
                            newVal = db.ProfileItem.Double2;
                            changed = true;
                        }
                    }
                    db.Double1NP = newVal;
                    if (changed)
                        te.EditValue = newVal;
                    if (updateSaveStatus != null)
                        updateSaveStatus(db, Math.Abs(db.Double1NP - db.Double1) > 0.0000009);
                });
            };

            return rv;
        }





        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIDouble(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            var d = 0.0;
            Double.TryParse(dbi.DefaultValue, out d);
            if (dbi.Int1 == 1 && d < dbi.Double1) d = dbi.Double1;
            if (dbi.Int2 == 1 && d > dbi.Double2) d = dbi.Double2;
            dbv.Double1 = d;
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
            if (st.DoubleSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;

            switch (dbi.String1)
            {
                case "d":
                    return string.Format("{0}", dbv.Double1);
                case "n2":
                    return string.Format("{0}", dbv.Double1);
                case "n6":
                    return string.Format("{0}", dbv.Double1);
                case "P0":
                    return string.Format("{0}", dbv.Double1);
                case "P2":
                    return string.Format("{0}", dbv.Double1);
                default:
                    return string.Format("{0}", dbv.Double1);
            }
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return !double.IsNaN(db.Double1NP);
        }
    }
}
