using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.Lib.Interfaces;

namespace POC.Module.Profile.Views.FastContactUnit
{
    public partial class UProfile : UserControl,IValidateSaveFastContactModule
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }
        private POCCore APOCCore { get; set; }


        private Style LableNormalStyle { get; set; }
        private Style LableNeedSaveStyle { get; set; }
        private ControlTemplate CountryPopupTemplate { get; set; }
        private ControlTemplate CityPopupTemplate { get; set; }
        private Window DynamicOwner { get; set; }
        private List<Guid> ItemsToSave { get; set; }
        private Dictionary<int, Guid> CustomColGuids { get; set; }
        private bool NeedToSaveContact { get; set; }

        public UProfile()
        {
            InitializeComponent();

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADataFieldManager = ServiceLocator.Current.GetInstance<IDataFieldManager>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();

            DynamicOwner = Window.GetWindow(this);
            ItemsToSave = new List<Guid>();
            NeedToSaveContact = false;
            CustomColGuids = new Dictionary<int, Guid>();
            if (APOCCore.STCI.ContactCustColEnable0 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid0)) CustomColGuids.Add(0, Guid.Parse(APOCCore.STCI.ContactCustColOid0));
            if (APOCCore.STCI.ContactCustColEnable1 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid1)) CustomColGuids.Add(1, Guid.Parse(APOCCore.STCI.ContactCustColOid1));
            if (APOCCore.STCI.ContactCustColEnable2 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid2)) CustomColGuids.Add(2, Guid.Parse(APOCCore.STCI.ContactCustColOid2));
            if (APOCCore.STCI.ContactCustColEnable3 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid3)) CustomColGuids.Add(3, Guid.Parse(APOCCore.STCI.ContactCustColOid3));
            if (APOCCore.STCI.ContactCustColEnable4 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid4)) CustomColGuids.Add(4, Guid.Parse(APOCCore.STCI.ContactCustColOid4));
            if (APOCCore.STCI.ContactCustColEnable5 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid5)) CustomColGuids.Add(5, Guid.Parse(APOCCore.STCI.ContactCustColOid5));
            if (APOCCore.STCI.ContactCustColEnable6 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid6)) CustomColGuids.Add(6, Guid.Parse(APOCCore.STCI.ContactCustColOid6));
            if (APOCCore.STCI.ContactCustColEnable7 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid7)) CustomColGuids.Add(7, Guid.Parse(APOCCore.STCI.ContactCustColOid7));
            if (APOCCore.STCI.ContactCustColEnable8 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid8)) CustomColGuids.Add(8, Guid.Parse(APOCCore.STCI.ContactCustColOid8));
            if (APOCCore.STCI.ContactCustColEnable9 && !string.IsNullOrWhiteSpace(APOCCore.STCI.ContactCustColOid9)) CustomColGuids.Add(9, Guid.Parse(APOCCore.STCI.ContactCustColOid9));

            LableNormalStyle = this.FindResource("LayoutItemLabelStyle") as Style;
            LableNeedSaveStyle = this.FindResource("LILStyle_NeedSave") as Style;
            CountryPopupTemplate = this.FindResource("CountryPopupTemplate") as ControlTemplate;
            CityPopupTemplate = this.FindResource("CityPopupTemplate") as ControlTemplate;

            Loaded +=
                (s, e) =>
                {
                    PopulateUI();
                };
            
        }

        private void PopulateUI()
        {
            var dbc = Contact as DBCTContact;
            if (dbc == null) return;

            var DynamicTabControl = new DXTabControl();
            DynamicTabControl.Margin = new Thickness(3);
            TheStackPanel.Children.Add(DynamicTabControl);

            DynamicTabControl.BeginInit();
            try
            {
                var valueXPC = DBCTProfileValue.GetAll(ADatabase.Dxs, dbc.Oid);
                var groupList = (from n in valueXPC orderby n.ProfileItem.ProfileGroup.Order, n.ProfileItem.ProfileGroup.Title select n.ProfileItem.ProfileGroup).Distinct().ToList();
                var rootList = (from n in groupList orderby n.ProfileRoot.Order, n.ProfileRoot.Title select n.ProfileRoot).Distinct().ToList();
                foreach (var root in rootList)
                {
                    var canEdit = true;
                    var canView = true;
                    if (AMembership.ActiveUser.UserName.ToLower() != "admin")
                    {
                        canEdit = AMembership.ActiveUser.Roles.Select(r => r.ToLower()).Contains(root.RoleEditor == null ? string.Empty : root.RoleEditor.ToLower());
                        canView = canEdit || AMembership.ActiveUser.Roles.Select(r => r.ToLower()).Contains(root.RoleViewer == null ? string.Empty : root.RoleViewer.ToLower());
                    }

                    var dxti = new DXTabItem
                    {
                        Padding = new Thickness(6),
                    };
                    dxti.Header = new TextBlock { Text = root.Title, };

                    if (!canView)
                    {
                        dxti.IsEnabled = false;
                        DynamicTabControl.Items.Add(dxti);
                        continue;
                    }

                    var flc = new FlowLayoutControl
                    {
                        AllowItemMoving = false,
                        AllowAddFlowBreaksDuringItemMoving = false,
                        AllowLayerSizing = false,
                        AnimateItemMoving = true,
                        Orientation = Orientation.Vertical,
                        BreakFlowToFit = false,
                        StretchContent = true,
                        AllowMaximizedElementMoving = true,
                        MaximizedElementPosition = MaximizedElementPosition.Right,
                        ScrollBars = ScrollBars.Auto,
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
                    };

                    dxti.Content = flc;

                    var currentGroups = (from n in groupList where n.ProfileRoot.Oid == root.Oid select n).ToList();
                    currentGroups.ForEach(
                        group1 =>
                        {
                            var gb = new DevExpress.Xpf.LayoutControl.GroupBox
                            {
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Top,
                                Width = double.NaN,
                                Margin = new Thickness(3, 3, 3, 6),
                                Tag = group1,
                                Background = new SolidColorBrush(Color.FromArgb(0xff, 0xe3, 0xef, 0xff)),
                            };
                            gb.Header = new TextBlock { Text = group1.Title, };
                            flc.Children.Add(gb);

                            var flcGroup = new FlowLayoutControl
                            {
                                AllowItemMoving = false,
                                AllowAddFlowBreaksDuringItemMoving = false,
                                AllowLayerSizing = false,
                                AnimateItemMoving = true,
                                Orientation = Orientation.Vertical,
                                BreakFlowToFit = false,
                                StretchContent = true,
                                AllowMaximizedElementMoving = true,
                                MaximizedElementPosition = MaximizedElementPosition.Right,
                                ScrollBars = ScrollBars.None,
                            };
                            gb.Content = flcGroup;
                            var currentValues = (from n in valueXPC
                                                 where n.ProfileItem.ProfileGroup.Oid == group1.Oid
                                                 orderby n.Order
                                                 select n).ToList();
                            foreach (var dbpv in currentValues)
                            {
                                var li = new LayoutItem
                                {
                                    Label = dbpv.ProfileItem.Title,
                                    LabelStyle = LableNormalStyle,
                                    Margin = new Thickness(0, 3, 0, 3),
                                    Tag = dbpv,
                                };
                                var df = ADataFieldManager.FindByType(dbpv.ProfileItem.ItemType);
                                object tag = null;
                                if (dbpv.ProfileItem.ItemType == EnumProfileItemType.Country)
                                    tag = CountryPopupTemplate;
                                if (dbpv.ProfileItem.ItemType == EnumProfileItemType.City)
                                    tag = new object[] { CountryPopupTemplate, CityPopupTemplate };
                                if (dbpv.ProfileItem.ItemType == EnumProfileItemType.Location)
                                    tag = new object[] { DynamicOwner };
                                if (dbpv.ProfileItem.ItemType == EnumProfileItemType.StringCombo)
                                    tag = new object[] { DynamicOwner };
                                if (dbpv.ProfileItem.ItemType == EnumProfileItemType.Contact)
                                    tag = new object[] { DynamicOwner };
                                if (dbpv.ProfileItem.ItemType == EnumProfileItemType.Image)
                                    tag = new object[] { DynamicOwner };
                                if (dbpv.ProfileItem.ItemType == EnumProfileItemType.File)
                                    tag = new object[] { DynamicOwner };
                                dbpv.LayoutItemHolder = li;
                                li.Content = df.GetUIDisplayWpf(dbpv, !canEdit, tag, (db, bb) => UpdateSaveStatus(db as DBCTProfileValue, bb)) as UIElement;
                                flcGroup.Children.Add(li);
                            }

                        });
                    DynamicTabControl.Items.Add(dxti);
                }
            }
            catch (Exception ex)
            {
                
            }
            DynamicTabControl.EndInit();
            }

        private void SaveData(DBCTProfileValue dbpv)
        {
            try
            {
                var df = ADataFieldManager.FindByType(dbpv.ProfileItem.ItemType);
                df.Save(dbpv);
                if (ItemsToSave.Contains(dbpv.Oid))
                    ItemsToSave.Remove(dbpv.Oid);
                var li = (LayoutItem) dbpv.LayoutItemHolder;
                li.LabelStyle = LableNormalStyle;

                SaveCustomColumn(dbpv);
            }
            catch
            {
            }
        }
        private void SaveCustomColumn(DBCTProfileValue val)
        {
            if (!CustomColGuids.ContainsValue(val.ProfileItem.Oid)) return;
            var i = (from n in CustomColGuids where n.Value == val.ProfileItem.Oid select n.Key).First();
            switch (i)
            {
                case 0: val.Contact.CCText0 = val.GetCustomColumnValue(); break;
                case 1: val.Contact.CCText1 = val.GetCustomColumnValue(); break;
                case 2: val.Contact.CCText2 = val.GetCustomColumnValue(); break;
                case 3: val.Contact.CCText3 = val.GetCustomColumnValue(); break;
                case 4: val.Contact.CCText4 = val.GetCustomColumnValue(); break;
                case 5: val.Contact.CCText5 = val.GetCustomColumnValue(); break;
                case 6: val.Contact.CCText6 = val.GetCustomColumnValue(); break;
                case 7: val.Contact.CCText7 = val.GetCustomColumnValue(); break;
                case 8: val.Contact.CCText8 = val.GetCustomColumnValue(); break;
                case 9: val.Contact.CCText9 = val.GetCustomColumnValue(); break;
            }
            NeedToSaveContact = true;
        }

        public object Contact { get; set; }

        public bool Save()
        {
            var DynamicTabControl = (from n in TheStackPanel.Children.Cast<UIElement>() where n is DXTabControl select n as DXTabControl).FirstOrDefault();
            if (DynamicTabControl == null) return true;
            var q = from tab in DynamicTabControl.Items.Cast<DXTabItem>()
                    where tab.Content != null
                    from groupBox in ((FlowLayoutControl)tab.Content).Children.Cast<FrameworkElement>()
                    where
                        (groupBox is DevExpress.Xpf.LayoutControl.GroupBox &&
                         ((DevExpress.Xpf.LayoutControl.GroupBox)groupBox).Content is FlowLayoutControl)
                    from layoutItem in
                        ((FlowLayoutControl)((DevExpress.Xpf.LayoutControl.GroupBox)groupBox).Content).Children.Cast
                        <FrameworkElement>()
                    where
                        (layoutItem is LayoutItem) && (layoutItem.Tag is DBCTProfileValue) &&
                        ItemsToSave.Contains((layoutItem.Tag as DBCTProfileValue).Oid)
                    select (DBCTProfileValue)layoutItem.Tag;

            q.ToList().ForEach(SaveData);
            if (!NeedToSaveContact) return true;
            var dbc = (DBCTContact)Contact;
            dbc.Save();
            NeedToSaveContact = false;
            return true;
            
        }

        public bool Validate()
        {
            return true;
        }



        private void UpdateSaveStatus(DBCTProfileValue db, bool bb)
        {
            var li = (LayoutItem)db.LayoutItemHolder;
            db.NeedToSave = bb;
            li.LabelStyle = bb ? LableNeedSaveStyle : LableNormalStyle;
            if (bb)
            {
                if (!ItemsToSave.Contains(db.Oid))
                    ItemsToSave.Add(db.Oid);
            }
            else
            {
                if (ItemsToSave.Contains(db.Oid))
                    ItemsToSave.Remove(db.Oid);
            }
        }
    }
}
