using System;
using System.Linq;
using System.Windows.Controls;
using POC.Module.Profile.Views;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using System.Windows;

namespace POC.Module.Profile.DataField
{
    class ListDataField : IDataField
    {
        public string Title
        {
            get { return "لیست"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.List; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UIList.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;

            var rv = new Button
            {
                Content = dbpv.ProfileItem.String1,
                Tag = dbpv,
            };

            rv.Click += (s, e)
            =>
            {
                var bt = (Button)s;
                var db = (DBCTProfileValue)bt.Tag;

                var q = from n in Application.Current.Windows.Cast<Window>()
                        where n is WListViewer && ((WListViewer)n).DynamicDBProfileValue.Oid == db.Oid
                        select n;
                if (q.Any())
                    q.First().Activate();
                else
                {
                    var w = new WListViewer(db, !isReadOnly);
                    w.Show();
                }
            };
            return rv;
        }



        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIList(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            return;
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            return string.Empty;
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            return true;
        }
    }
}
