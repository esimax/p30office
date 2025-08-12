using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.DB.Root;
using POL.Lib.Interfaces;
using POL.WPF.DXControls;

namespace POC.Module.Profile.DataField
{
    class ContactDataField : IDataField
    {
        public string Title
        {
            get { return "پرونده"; }
        }
        public EnumProfileItemType ItemType
        {
            get { return EnumProfileItemType.Contact; }
        }
        public string ImageUriString
        {
            get { return string.Format("{0}Special/16/_16_UIContact.png", UsedConstants.POLImagePath); }
        }
        public string Note { get { return string.Empty; } }

        public object GetUIDisplayWpf(object dbValue, bool isReadOnly, object tag, Action<object, bool> updateSaveStatus)
        {
            var dbpv = dbValue as DBCTProfileValue;
            if (dbpv == null) return null;
            var owner = ((object[])tag)[0] as Window;


            dbpv.Guid1NP = dbpv.Guid1;
            dbpv.Int1 = dbpv.Int1;
            dbpv.String1NP = dbpv.String1;

            var rv = new ButtonEdit()
            {
                IsTextEditable = false,
                EditValue = dbpv.String1,
                AllowDefaultButton = false,
                Tag = dbpv,
                IsReadOnly = isReadOnly,
            };

            rv.MouseDoubleClick +=
                (s, e) =>
                {
                    var be = (ButtonEdit)s;
                    var dbpv1 = (DBCTProfileValue)be.Tag;
                    var dbc = DBCTContact.FindByOid(dbpv1.Session, dbpv.Guid1NP);

                    if (dbc == null) return;

                    var aMembership = ServiceLocator.Current.GetInstance<IMembership>();
                    var aPOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
                    var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
                    var roles = dbc.Categories.ToList().Select(n => n.Role);
                    if (aMembership.ActiveUser.Roles.Select(r => r.ToLower()).Intersect(roles.Select(r => r.ToLower())).Any() || aMembership.ActiveUser.UserName.ToLower() == "admin")
                        aPOCContactModule.GotoContactByCode(dbc.Code);
                    else
                    {
                        POLMessageBox.ShowWarning("خطا : سطوح دسترسی كافی جهت مشاهده پرونده وجود ندارد.", aPOCMainWindow.GetWindow());
                    }
                };

            var biOpen = new ButtonInfo
            {
                GlyphKind = GlyphKind.Regular,
                ToolTip = "انتخاب پرونده",
                IsEnabled = !isReadOnly,
                Tag = new object[] { dbpv, owner },
            };
            rv.Buttons.Add(biOpen);

            biOpen.Click +=
                (s, e) =>
                {
                    var bi = ((ButtonInfo)((ButtonContainer)((Button)e.Source).TemplatedParent).Content);
                    var db = ((DBCTProfileValue)((object[])bi.Tag)[0]);
                    var ow = ((Window)((object[])bi.Tag)[1]);

                    var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                    var aPOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

                    var cc = DBCTContactCat.FindByOid(aDatabase.Dxs, db.ProfileItem.Guid1);
                    var o = aPOCMainWindow.ShowSelectContact(ow, cc);
                    if (o is DBCTContact)
                    {
                        var dbc = o as DBCTContact;
                        rv.EditValue = dbc.Title;
                        dbpv.String1NP = dbc.Title;
                        dbpv.Int1 = dbc.Code;
                        dbpv.Guid1NP = dbc.Oid;
                        if (updateSaveStatus != null)
                            updateSaveStatus(db, dbpv.Guid1NP != dbpv.Guid1);
                    }
                };

            var biClear = new ButtonInfo
            {
                GlyphKind = GlyphKind.Cancel,
                ToolTip = "حذف پرونده",
                IsEnabled = !isReadOnly,
            };
            rv.Buttons.Add(biClear);
            biClear.Click += (s, e)
                =>
            {
                rv.EditValue = string.Empty;
                dbpv.String1NP = null;
                dbpv.Int1 = -1;
                dbpv.Guid1NP = Guid.Empty;
                if (updateSaveStatus != null)
                    updateSaveStatus(dbpv, dbpv.Guid1NP != dbpv.Guid1);
            };


            return rv;
        }













        public object GetUISettingsWpf(object dbProfileItem)
        {
            var pi = dbProfileItem as DBCTProfileItem;
            return new Views.ModuleSettings.UIContact(pi);
        }

        public void SetValueToDefault(object dbValue, object dbItem)
        {
            return;
        }

        public void Save(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;

            db.Guid1 = db.Guid1NP;
            db.Int1 = db.Int1;
            db.String1 = db.String1NP;
            db.Save();
        }

        public string GetEmailData(object dbValue, object dbItem, object settings)
        {
            var st = settings as DataFieldSettings;
            if (st == null) return string.Empty;
            if (st.ContactSettings == null) return string.Empty;
            var dbv = dbValue as DBCTProfileValue;
            if (dbv == null) return string.Empty;
            return string.Format(st.ContactSettings.Format, dbv.Int1, dbv.String1);
        }


        public bool IsRequiredSatisfied(object dbValue)
        {
            var db = (DBCTProfileValue)dbValue;
            if (!db.ProfileItem.IsRequired) return true;
            return db.Guid1NP != Guid.Empty;
        }
    }
}
