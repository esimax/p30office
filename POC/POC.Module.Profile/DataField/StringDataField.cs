using System.Windows;
using DevExpress.Xpf.Editors;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using System;
using POL.Lib.Utils;

namespace POC.Module.Profile.DataField
{
    class StringDataField : IDataField
    {
        public string Title
        {
            get { return "متن ساده"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.String; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UIString.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;

            dbpv.String1NP = dbpv.String1;

            var rv = new TextEdit
            {
                EditValue = dbpv.String1,
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };

            if (dbpv.ProfileItem.Int3 == 1)
            {
                rv.FlowDirection = FlowDirection.RightToLeft;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToRTL();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }
            if (dbpv.ProfileItem.Int3 == 2)
            {
                rv.FlowDirection = FlowDirection.LeftToRight;
                rv.GotFocus += (s, e) => HelperLocalize.SetLanguageToLTR();
                rv.LostFocus += (s, e) => HelperLocalize.SetLanguageToDefault();
            }
            if (dbpv.ProfileItem.Int1 > 0)
                rv.MaxLength = dbpv.ProfileItem.Int1;


            rv.EditValueChanged += (s, e) =>
            {
                var te = (TextEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                var newVal = e.NewValue == null ? null : e.NewValue.ToString();
                db.String1NP = newVal;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.String1NP != db.String1);
            };

            return rv;
        }






        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIString(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            dbv.String1 = dbi.DefaultValue;
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.String1 = db.String1NP;
            db.Save();
        }


        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.StringSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;

            string dir = st.StringSettings.DirDefValue;
            if (dbi.Int3 == 1) dir = st.StringSettings.DirRTLValue;
            if (dbi.Int3 == 2) dir = st.StringSettings.DirLTRValue;

            return string.Format(st.MemoSettings.Format, dbv.String1, dir, dbi.Int1);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return !string.IsNullOrWhiteSpace(db.String1NP);
        }
    }
}
