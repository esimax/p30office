using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using System.Windows;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIStringCheckList
    {
        public UIStringCheckList(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                cbeHeight.SelectedIndex = 2;
                HelperUtils.Try(() => { cbeHeight.SelectedIndex = Item.Int2; });
                cbeHeight.SelectedIndexChanged += (s1, e1)
                   => HelperUtils.Try(() => { Item.Int2 = cbeHeight.SelectedIndex; HasChanges = true; });

                cbeKeyboard.SelectedIndex = 0;
                HelperUtils.Try(() => { cbeKeyboard.SelectedIndex = Item.Int3; });
                cbeKeyboard.SelectedIndexChanged += (s1, e1)
                    => HelperUtils.Try(() => { Item.Int3 = cbeKeyboard.SelectedIndex; HasChanges = true; });

                var aDatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();

                cbeTable.SelectedItem = DBCTProfileTable.FindByOid(aDatabase.Dxs, Item.Guid1);
                cbeTable.ItemsSource = aCacheData.GetProfileTableList();
                cbeTable.SelectedIndexChanged += (s1, e1)
                    => HelperUtils.Try(
                    () =>
                    {
                        Item.Guid1 = ((DBCTProfileTable)((CacheItemProfileTable)cbeTable.SelectedItem).Tag).Oid; 
                        HasChanges = true;
                    });

                cbRequired.IsChecked = item.IsRequired;
                cbRequired.EditValueChanged +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        Item.IsRequired = cbRequired.IsChecked == true;
                    };
            };
            Unloaded += (s, e) => {
                                      if (HasChanges)
                                      {
                                          Item.Save();
                                      } };
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }

        private void ButtonInfo_Click_1(object sender, RoutedEventArgs e)
        {
            var mw = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            var v = mw.ShowManageProfileTable(Window.GetWindow(this));
            cbeTable.ItemsSource = null;
            var aCacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            cbeTable.ItemsSource = aCacheData.GetProfileTableList();
            if (v is DBCTProfileTable)
            {
                cbeTable.SelectedItem = v as DBCTProfileTable;
            }
        }
    }
}
