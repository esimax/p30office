using System.Windows;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using System;
using POL.Lib.Utils;

namespace POC.Module.Profile.DataField
{
    class DateDataField : IDataField
    {
        public string Title
        {
            get { return "تاریخ"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Date; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Standard/16/_16_UICalendar.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;

            dbpv.DateTime1NP = dbpv.DateTime1NP;
            var rv = new POL.WPF.DXControls.POLDateEdit.POLDateEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                DateEditValue = (dbpv.DateTime1 == DateTime.MinValue ? (DateTime?)null : dbpv.DateTime1),
                DateTimeType = GetDateTimeType(dbpv.ProfileItem.Int1),
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };

            rv.EditValueChanged += (s, e) =>
            {
                var te = (POL.WPF.DXControls.POLDateEdit.POLDateEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                var newVal = te.DateEditValue;
                db.DateTime1NP = newVal == null ? DateTime.MinValue : (DateTime)newVal;
                db.Int1NP = newVal == null ? 0 : 1;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.DateTime1NP != db.DateTime1);
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

            if (dbi.Int2 == 0) 
            {
                if (dbi.DefaultValue == null)
                {
                    dbv.Int1 = 0;
                }
                else
                {
                    dbv.Int1 = 1;
                    dbv.DateTime1 = new DateTime(Convert.ToInt64(dbi.DefaultValue));
                }
            }
            else 
            {
                dbv.Int1 = 1;
                dbv.DateTime1 = DateTime.Now.Date.AddDays(dbi.Int3);
            }
        }


        private EnumCalendarType GetDateTimeType(int p)
        {
            if (p == 1) return EnumCalendarType.Shamsi;
            if (p == 2) return EnumCalendarType.Hijri;
            if (p == 3) return EnumCalendarType.Gregorian;
            return HelperLocalize.ApplicationCalendar;
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Int1 = db.Int1NP;

            db.DateTime1 = db.DateTime1NP;
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.DateSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;
            if (dbv.Int1 == 0) return st.DateSettings.NullValue;

            var ect = EnumCalendarType.ApplicationSettings;
            if (dbi.Int1 == 1) ect = EnumCalendarType.Hijri;
            if (dbi.Int1 == 2) ect = EnumCalendarType.Gregorian;
            if (dbi.Int1 == 3) ect = EnumCalendarType.Shamsi;

            return HelperLocalize.DateTimeToString(dbv.DateTime1, ect, st.DateSettings.Format);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return db.DateTime1NP != DateTime.MaxValue && db.DateTime1NP != DateTime.MinValue;
        }
    }
}
