using System.Globalization;
using System.Windows;
using DevExpress.Xpf.Editors;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using System;
using POL.Lib.Utils;

namespace POC.Module.Profile.DataField
{
    class TimeDataField : IDataField
    {
        public string Title
        {
            get { return "ساعت"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Time; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UITime.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;

            dbpv.Int1NP = dbpv.Int1;
            dbpv.Int2NP = dbpv.Int2;

            var rv = new TextEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                Mask = @"([01]?[0-9]|2[0-3]):[0-5]\d",
                MaskType = MaskType.RegEx,
                MaskUseAsDisplayFormat = true,
                EditValue = string.Format("{0}:{1}", dbpv.Int1, dbpv.Int2.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')),
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };

            rv.EditValueChanged += (s, e) =>
            {
                var te = (TextEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                HelperUtils.Try(() =>
                {
                    var newVal = te.EditValue == null ? "0:00" : te.EditValue.ToString();
                    var ss = newVal.Split(':');
                    db.Int1NP = Convert.ToInt32(ss[0]);
                    db.Int2NP = Convert.ToInt32(ss[1]);
                    if (updateSaveStatus != null)
                        updateSaveStatus(db, (db.Int1NP != db.Int1) || (db.Int2NP != db.Int2));
                });
            };

            return rv;
        }




        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIDate(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {

            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            if (dbi.Int3 == 0) 
            {
                HelperUtils.Try(
                    () =>
                    {
                        var ts = TimeSpan.FromTicks(Convert.ToInt64(dbi.DefaultValue));
                        dbv.Int1 = ts.Hours;
                        dbv.Int2 = ts.Minutes;
                    });
            }
            else 
            {
                dbv.Int1 = DateTime.Now.Hour;
                dbv.Int2 = DateTime.Now.Minute;
            }

        }
        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Int1 = db.Int1NP;
            db.Int2 = db.Int2NP;
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.TimeSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;

            return string.Format(st.TimeSettings.Format, dbv.Int1, dbv.Int2);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            return true;
        }
    }
}
