using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Editors;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using System.Windows.Controls;
using POL.Lib.Utils;
using Microsoft.Practices.ServiceLocation;

namespace POC.Module.Profile.DataField
{
    class StringCheckListDataField : IDataField
    {
        public string Title
        {
            get { return "متن چند انتخابی"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.StringCheckList; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UICheckList.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            var ct = (from n in aCacheData.GetProfileTableList() where ((DBCTProfileTable)n.Tag).Oid == dbpv.ProfileItem.Guid1 select n.ChildList).FirstOrDefault();

            var rv = new ListBoxEdit()
            {
                StyleSettings = new CheckedListBoxEditStyleSettings(),
                SelectionMode = SelectionMode.Multiple,
                ShowCustomItems = false,
                Height = (dbpv.ProfileItem.Int2 + 1) * 50,
                Tag = dbpv,
                ItemsSource = ct == null ? null : ct.ToList().Select(n => n.Title),
                IsReadOnly = isReadOnly,
            };

            var ss = dbpv.String1 == null ? new string[] { } : dbpv.String1.Split(new[] { '|' });
            ss.ToList().ForEach(i => rv.SelectedItems.Add(i));





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

            rv.SelectedIndexChanged += (s, e) =>
            {
                var te = (ListBoxEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                var newVal = String.Join("|", (from n in te.SelectedItems.Cast<string>() select n.Replace("|", "")).ToList());
                if (newVal == string.Empty) newVal = null;
                db.String1NP = newVal;
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.String1NP != db.String1);
            };

            return rv;
        }






        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIStringCheckList(pi);
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
            if (st.CheckListSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;


            var ss = dbv.String1 == null ? new string[] { } : dbv.String1.Split(new[] { '|' });
            var rv = string.Empty;
            for (var i = 0; i < ss.Length; i++)
            {
                rv = string.Format("{0}{1}{2}", st.CheckListSettings.Prefix, ss[i], st.CheckListSettings.Postfix);
                if (i != (ss.Length - 1))
                    rv += st.CheckListSettings.Seprator;
            }
            return rv;
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return !string.IsNullOrWhiteSpace(db.String1NP);
        }
    }
}
