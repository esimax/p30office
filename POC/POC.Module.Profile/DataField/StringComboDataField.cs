using System;
using System.Linq;
using System.Windows.Controls;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using Microsoft.Practices.ServiceLocation;
using DevExpress.Xpf.Editors;
using System.Windows;
using System.Windows.Media;

namespace POC.Module.Profile.DataField
{
    class StringComboDataField : IDataField
    {
        public string Title
        {
            get { return "متن تك انتخابی"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.StringCombo; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UICombo.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            var owner = ((object[])tag)[0] as Window;

            dbpv.String1NP = dbpv.String1;
            dbpv.String2NP = dbpv.String2;

            var dbt = DBCTProfileTable.FindByOid(aDatabase.Dxs, dbpv.ProfileItem.Guid1);
            if (dbt == null)
            {
                return new TextBlock { Text = "جدول پیدا نشد!", Foreground = Brushes.Red };
            }
            var ct = (from n in aCacheData.GetProfileTableList() where n.Title == dbt.Title select n).FirstOrDefault();


            var sp = new StackPanel();

            ComboBoxEdit rv2 = null;
            if (dbt.ValueDepth > 1)
            {
                rv2 = new ComboBoxEdit
                {
                    IsTextEditable = true,
                    EditValue = dbpv.String2,
                    Tag = dbpv,
                    ItemsSource = ct == null ? null : (from n in ct.ChildList where n.Title == dbpv.String1 select n.ChildList).FirstOrDefault(),
                    AutoComplete = true,
                    IsReadOnly = isReadOnly,
                    AllowSpinOnMouseWheel = false,
                };

                rv2.EditValueChanged += (s, e) =>
                {
                    var te = (ComboBoxEdit)s;
                    var db = (DBCTProfileValue)te.Tag;
                    var newVal = te.EditValue == null ? null : te.EditValue.ToString();
                    db.String2NP = newVal;
                    if (updateSaveStatus != null)
                        updateSaveStatus(db, db.String2NP != db.String2);
                };
            }

            var rv = new ComboBoxEdit
            {
                IsTextEditable = true,
                EditValue = dbpv.String1,
                Tag = dbpv,
                ItemsSource = ct == null ? null : ct.ChildList,
                AutoComplete = true,
                IsReadOnly = isReadOnly,
                AllowSpinOnMouseWheel = false
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

            rv.EditValueChanged += (s, e) =>
            {
                var te = (ComboBoxEdit)s;
                var db = (DBCTProfileValue)te.Tag;
                var newVal = te.EditValue == null ? null : te.EditValue.ToString();
                db.String1NP = newVal;

                if (rv2 != null && ct != null)
                {
                    rv2.ItemsSource = (from n in ct.ChildList where n.Title == newVal select n.ChildList).FirstOrDefault();
                }
                if (updateSaveStatus != null)
                    updateSaveStatus(db, db.String1NP != db.String1);
            };

            var bi1 = new ButtonInfo
            {
                GlyphKind = GlyphKind.Regular,
                IsEnabled = !isReadOnly,
                Tag = new object[] { dbt, owner },
            };
            rv.Buttons.Add(bi1);
            bi1.Click +=
                (s, e) =>
                {
                    var bi = ((ButtonInfo)((ButtonContainer)((Button)e.Source).TemplatedParent).Content);
                    var db = ((DBCTProfileTable)((object[])bi.Tag)[0]);

                    var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                    var o = aPOCMainWindow.ShowManageProfileTValue(((Window)((object[])bi.Tag)[1]), db);
                    var v = o as DBCTProfileTValue;
                    if (v != null) rv.SelectedItem = v.Title;
                };

            var bi2 = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
                IsEnabled = !isReadOnly,
            };
            rv.Buttons.Add(bi2);
            bi2.Click += (s, e) => rv.EditValue = null;

            sp.Children.Add(rv);
            if (rv2 != null) sp.Children.Add(rv2);
            return sp;
        }














        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIStringCombo(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            var dbv = dbValue as DBCTProfileValue;
            var dbi = dbItem as DBCTProfileItem;

            if (dbv == null) return;
            if (dbi == null) return;

            HelperUtils.Try(
                    () =>
                    {
                        var ss = dbi.DefaultValue.Split('|');
                        dbv.String1 = ss[0];
                        dbv.String2 = ss[1];
                    });

        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.String1 = db.String1NP;
            db.String2 = db.String2NP;
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.ComboSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            var dbi = dbItem as DBCTProfileItem;
            if (dbi == null) return string.Empty;

            return string.Format(st.ComboSettings.Format, dbv.String1, dbv.String2);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return !string.IsNullOrWhiteSpace(db.String1NP);
        }
    }
}
