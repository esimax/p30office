using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.Lib.Utils;
using POL.WPF.Controles.MVVM;
using Microsoft.Practices.ServiceLocation;

namespace POC.Module.Profile.Views.ModuleSettings
{
    public partial class UIDate
    {
        public UIDate(DBCTProfileItem item)
        {
            Item = item;
            InitializeComponent();
            JustConstructed = true;
            Loaded += (s, e) =>
            {
                if (!JustConstructed) return;
                JustConstructed = false;
                HasChanges = false;
                cbeCalendar.SelectedIndex = 0;
                HelperUtils.Try(() => { cbeCalendar.SelectedIndex = Item.Int1; });
                cbeCalendar.SelectedIndexChanged += (s1, e1)
                    => HelperUtils.Try(() => { Item.Int1 = cbeCalendar.SelectedIndex; HasChanges = true; });

                cbDefaultIsOffset.IsChecked = (Item.Int2 != 0);
                seDayOffset.Value = Item.Int3;
                cbDefaultIsOffset.Checked += (s1, e1) => { Item.Int2 = 1; HasChanges = true; };
                cbDefaultIsOffset.Unchecked += (s1, e1) => { Item.Int2 = 0; HasChanges = true; };
                seDayOffset.EditValueChanging += (s1, e1) =>
                {
                    Item.Int3 = Convert.ToInt32(e1.NewValue);
                    HasChanges = true;
                };


                if (Item.DefaultValue == null)
                {
                    poldeDefaultValue.DateEditValue = null;
                }
                else
                {
                    HelperUtils.Try(
                        () =>
                        {
                            var d = new DateTime(Convert.ToInt64(Item.DefaultValue));
                            poldeDefaultValue.DateEditValue = d;
                        });
                }
                poldeDefaultValue.DateEditValueChanged +=
                    (s1, e1) =>
                    {
                        HasChanges = true;
                        Item.DefaultValue = poldeDefaultValue.DateEditValue == null ? null : poldeDefaultValue.DateEditValue.Value.Ticks.ToString(CultureInfo.InvariantCulture);
                    };






                ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
                APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();

                PopulateAutomationList();
                cbeAutomation.DataContext = this;
                CommandAddAutomation = new RelayCommand(
                    () =>
                    {
                        APOCMainWindow.ShowAddEditAutomation(System.Windows.Window.GetWindow(this), null);
                        PopulateAutomationList();
                    });
                LoadAutomation();
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
                                      }
                                      SaveAutomation(); };
        }

        public DBCTProfileItem Item { get; set; }
        private bool HasChanges { get; set; }
        private bool JustConstructed { get; set; }




        private IDatabase ADatabase { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private List<DBTMAutomation> AutomationList { get; set; }
        private void cbAutomation_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            gbAutomation.IsEnabled = true;
        }
        private void cbAutomation_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            gbAutomation.IsEnabled = false;
        }
        private void PopulateAutomationList()
        {
            var v = DBTMAutomation.GetAll(ADatabase.Dxs);
            AutomationList = (from u in v select u).ToList();
            cbeAutomation.ItemsSource = AutomationList;
        }
        private void SaveAutomation()
        {
            var dbpi = DBTMProfileItem.FindByProfileItemOid(ADatabase.Dxs, Item.Oid);
            if (cbAutomation.IsChecked == true)
            {
                if (dbpi == null)
                {
                    dbpi = new DBTMProfileItem(ADatabase.Dxs);
                    dbpi.DataItemOid = Item.Oid;
                    dbpi.ReactionTime = teTime.Text.PadLeft(5, '0');
                    var automation = cbeAutomation.SelectedItem as DBTMAutomation;
                    if(automation !=null)
                        dbpi.AutomationOid = automation.Oid;
                    dbpi.OnOnce = rbOnce.IsChecked == true;
                    dbpi.OnYear = rbYear.IsChecked == true;
                    dbpi.OnMonth = rbMonth.IsChecked == true;
                    dbpi.Save();
                }
                else
                {

                    var automation = cbeAutomation.SelectedItem as DBTMAutomation;
                    if (automation != null)
                        dbpi.AutomationOid = automation.Oid;
                    dbpi.ReactionTime = teTime.Text.PadLeft(5, '0');
                    dbpi.OnOnce = rbOnce.IsChecked == true;
                    dbpi.OnYear = rbYear.IsChecked == true;
                    dbpi.OnMonth = rbMonth.IsChecked == true;
                    dbpi.Save();
                }
            }
            else
            {
                if (dbpi != null)
                {
                    dbpi.Delete();
                    dbpi.Save();
                }
            }
        }
        private void LoadAutomation()
        {
            var dbpi = DBTMProfileItem.FindByProfileItemOid(ADatabase.Dxs, Item.Oid);
            if (dbpi != null)
            {
                cbAutomation.IsChecked = true;
                var dba = (from n in AutomationList where n.Oid == dbpi.AutomationOid select n).FirstOrDefault();
                cbeAutomation.SelectedItem = dba;
                teTime.Text = dbpi.ReactionTime;
                rbOnce.IsChecked = dbpi.OnOnce;
                rbYear.IsChecked = dbpi.OnYear;
                rbMonth.IsChecked = dbpi.OnMonth;
            }

        }
        public RelayCommand CommandAddAutomation { get; set; }
    }
}

