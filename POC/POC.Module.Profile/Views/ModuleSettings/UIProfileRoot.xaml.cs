using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using System;
using POL.Lib.Utils;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIProfileRoot
    {
        private ICacheData MCacheData { get; set; }
        private ILoggerFacade MLogger { get; set; }
        public UIProfileRoot()
        {
            InitializeComponent();
            MCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            MLogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

            JustConstructed = true;
            Loaded += (s, e) =>
            {

                if (!JustConstructed) return;
                JustConstructed = false;
                var datasource = new List<CacheItemRoleItem> { new CacheItemRoleItem { Title = string.Empty, Tag = null } };
                datasource.AddRange(MCacheData.GetRoleList());

                cbRoleView.ItemsSource = datasource;
                cbRoleEdit.ItemsSource = datasource;

                lbeCat.ItemsSource = MCacheData.GetContactCatList();


                var selCatsOid = from n in Item.ContactCats.ToList() select n.Oid;
                var items = from n in MCacheData.GetContactCatList()
                            where selCatsOid.Contains(((DBCTContactCat)n.Tag).Oid)
                            select n;

                items.ToList().ForEach(n => lbeCat.SelectedItems.Add(n));


                HasChanges = false;


                cbRoleView.SelectedIndex = 0;
                if (Item != null && !string.IsNullOrWhiteSpace(Item.RoleViewer))
                    cbRoleView.EditValue = (from n in datasource where HelperConvert.CorrectPersianBug(n.Title) == HelperConvert.CorrectPersianBug(Item.RoleViewer) select n).FirstOrDefault();


                cbRoleEdit.SelectedIndex = 0;
                if (Item != null && !string.IsNullOrWhiteSpace(Item.RoleEditor))
                    cbRoleEdit.EditValue = (from n in datasource where HelperConvert.CorrectPersianBug(n.Title) == HelperConvert.CorrectPersianBug(Item.RoleEditor) select n).FirstOrDefault();

            };
            cbRoleView.SelectedIndexChanged +=
                (s, e) =>
                {
                    try
                    {
                        if (Item == null) return;
                        var nv = string.Empty;
                        if (cbRoleView.EditValue != null)
                        {
                            nv = HelperConvert.CorrectPersianBug(((CacheItemRoleItem)cbRoleView.EditValue).Title);
                        }
                        if (Item.RoleViewer == null)
                            Item.RoleViewer = string.Empty;
                        if (nv != HelperConvert.CorrectPersianBug(Item.RoleViewer.ToString(CultureInfo.InvariantCulture)))
                        {
                            Item.RoleViewer = nv;
                            HasChanges = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MLogger.Log("---------- E1-E : " + ex.Message, Category.Debug, Priority.High);
                    }
                };
            cbRoleEdit.SelectedIndexChanged +=
               (s, e) =>
               {
                   try
                   {
                       if (Item == null) return;
                       var nv = string.Empty;
                       if (cbRoleView.EditValue != null)
                       {
                           nv = HelperConvert.CorrectPersianBug(((CacheItemRoleItem)cbRoleEdit.EditValue).Title);
                       }

                       if (Item.RoleEditor == null)
                           Item.RoleEditor = string.Empty;
                       if (nv != HelperConvert.CorrectPersianBug(Item.RoleEditor.ToString(CultureInfo.InvariantCulture)))
                       {
                           Item.RoleEditor = nv;
                           HasChanges = true;
                       }
                   }
                   catch (Exception ex)
                   {
                       MLogger.Log("---------- E2-E : " + ex.Message, Category.Debug, Priority.High);
                   }
               };

            lbeCat.SelectedIndexChanged +=
                (s, e) =>
                {
                    if (Item == null) return;
                    HasChanges = true;
                };

            Unloaded +=
                (s, e) => SaveChanges();

            bSave.Click += (s, e) => SaveChanges();
        }

        public DBCTProfileRoot Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }

        private void SaveChanges()
        {
            if (Item == null) return;
            if (!HasChanges) return;

            var cats = lbeCat.SelectedItems;
            var curCats = Item.ContactCats.ToList();
            curCats.ForEach(cat => Item.ContactCats.Remove(cat));
            cats.ToList().ForEach(cat => Item.ContactCats.Add(((CacheItemContactCat)cat).Tag as DBCTContactCat));
            Item.Save();
            MCacheData.ForcePopulateCache(EnumCachDataType.ProfileItem, false, Item);
            MCacheData.RaiseCacheChanged(EnumCachDataType.ProfileItem);
            HasChanges = false;
        }
    }
}
