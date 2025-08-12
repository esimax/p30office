using System;
using POL.DB.P30Office;
using Microsoft.Practices.ServiceLocation;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIList
    {
        public UIList(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                cbeList.SelectedItem = DBCTList.FindByOid(aDatabase.Dxs, Item.Guid1);
                cbeList.ItemsSource = DBCTList.GetAll(aDatabase.Dxs);

                cbeList.SelectedIndexChanged +=
                    (s1, e1) =>
                    {
                        if (cbeList.SelectedItem == null)
                        {
                            Item.Guid1 = Guid.Empty;
                            Item.String1 = string.Empty;
                        }
                        else
                        {
                            Item.Guid1 = ((DBCTList)cbeList.SelectedItem).Oid;
                            Item.String1 = ((DBCTList)cbeList.SelectedItem).Title;
                        }
                        HasChanges = true;
                    };
            };
            Unloaded += (s, e) =>
            {
                if (HasChanges)
                {
                    Item.Save();
                }
            };
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }
    }
}
