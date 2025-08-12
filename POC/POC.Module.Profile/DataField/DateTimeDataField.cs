using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.LayoutControl;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Module.Profile.DataField
{
    class DateTimeDataField : IDataField
    {
        public string Title
        {
            get { return "تاریخ / ساعت"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.DateTime; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UIDateTime.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            var li = (LayoutItem)dbpv.LayoutItemHolder;

            dbpv.DateTime1NP = dbpv.DateTime1;

            var rv = new Grid { Tag = dbpv, FlowDirection = FlowDirection.LeftToRight };
            rv.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            rv.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


            var dateUI = new POL.WPF.DXControls.POLDateEdit.POLDateEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                DateEditValue = (dbpv.DateTime1 == DateTime.MinValue ? (DateTime?)null : dbpv.DateTime1NP.Date),
                DateTimeType = GetDateTimeType(dbpv.ProfileItem.Int1),
                Tag = rv,
                Name = "dateUI",
                IsReadOnly = isReadOnly,
            };
            dateUI.SetValue(Grid.ColumnProperty, 0);
            dateUI.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(dateUI);


            var timeUI = new TextEdit
            {
                FlowDirection = FlowDirection.LeftToRight,
                Mask = @"([01]?[0-9]|2[0-3]):[0-5]\d",
                MaskType = MaskType.RegEx,
                MaskUseAsDisplayFormat = true,
                EditValue = string.Format("{0}:{1}", dbpv.DateTime1NP.Hour, dbpv.DateTime1NP.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')),
                Tag = rv,
                Name = "timeUI",
                IsReadOnly = isReadOnly,
            };
            timeUI.SetValue(Grid.ColumnProperty, 1);
            timeUI.SetValue(Grid.RowProperty, 0);
            rv.Children.Add(timeUI);



            timeUI.EditValueChanged += (s, e) =>
            {
                var te = (TextEdit)s;
                var grid = (Grid)te.Tag;
                var db = (DBCTProfileValue)grid.Tag;
                HelperUtils.Try(() =>
                {
                    var newVal = te.EditValue == null ? "0:00" : te.EditValue.ToString();
                    var ss = newVal.Split(':');
                    SetDateTime(db, null, Convert.ToInt32(ss[0]), Convert.ToInt32(ss[1]), updateSaveStatus);
                });
            };

            dateUI.LostFocus += (s, e) =>
            {
                var te = (POL.WPF.DXControls.POLDateEdit.POLDateEdit)s;
                var grid = (Grid)te.Tag;
                var db = (DBCTProfileValue)grid.Tag;
                SetDateTime(db, te.DateEditValue, null, null, updateSaveStatus);
            };

            return rv;
        }












        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIDateTime(pi);
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
                dbv.DateTime1 = DateTime.Now.AddDays(dbi.Int3);
            }
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.DateTime1 = db.DateTime1NP;
            db.Save();
        }

        private EnumCalendarType GetDateTimeType(int p)
        {
            if (p == 1) return EnumCalendarType.Shamsi;
            if (p == 2) return EnumCalendarType.Hijri;
            if (p == 3) return EnumCalendarType.Gregorian;
            return HelperLocalize.ApplicationCalendar;
        }
        private void SetDateTime(DBCTProfileValue db, DateTime? date, int? hour, int? min, Action<object, bool> updateSaveStatus)
        {
            if (date == null && hour == null && min == null)
            {
                db.DateTime1NP = DateTime.MinValue;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.DateTime1NP != db.DateTime1);
                return;
            }
            if (date == null && hour != null && min != null)
            {
                db.DateTime1NP = new DateTime(db.DateTime1NP.Year, db.DateTime1NP.Month, db.DateTime1NP.Day, (int)hour, (int)min, 0);
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.DateTime1NP != db.DateTime1);
                return;
            }
            if (date != null && hour == null && min == null)
            {
                var d = (DateTime)date;
                db.DateTime1NP = new DateTime(d.Year, d.Month, d.Day, db.DateTime1NP.Hour, db.DateTime1NP.Minute, 0);
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.DateTime1NP != db.DateTime1);
            }
        }


        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.DateTimeSettings == null) return string.Empty;
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
