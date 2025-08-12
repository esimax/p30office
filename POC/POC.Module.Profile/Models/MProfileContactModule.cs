using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using Microsoft.Practices.ServiceLocation;
using DevExpress.Xpf.Core;
using POL.DB.P30Office;
using System.Collections.Generic;
using POL.WPF.DXControls;
using POL.Lib.Utils;

namespace POC.Module.Profile.Models
{
    public class MProfileContactModule : NotifyObjectBase, IDisposable, ISave, IRefrashable
    {
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private IPOCContactModule AContactModule { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private POCCore APOCCore { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }
        private ICacheData ACacheData { get; set; }


        private dynamic MainView { get; set; }
        private Window DynamicOwner { get; set; }
        private DXTabControl DynamicTabControl { get; set; }
        private Style LableNormalStyle { get; set; }
        private  Style LableNeedSaveStyle { get; set; }
        private ControlTemplate CountryPopupTemplate { get; set; }
        private ControlTemplate CityPopupTemplate { get; set; }
        private  List<Guid> ItemsToSave { get; set; }
        private UserControl DynamicView { get; set; }

        private Dictionary<int, Guid> CustomColGuids { get; set; }
        private bool NeedToSaveContact { get; set; }
        private DBCTContact CurrentContact { get; set; }


        public MProfileContactModule(object mainView)
        {
            MainView = mainView;

            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            AContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            ADataFieldManager = ServiceLocator.Current.GetInstance<IDataFieldManager>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();

            AContactModule.OnSelectedContactChanged += AContactModule_OnSelectedContactChanged;

            InitCommands();
            GetDynamicData();
            DataRefresh();
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
        }

        void AContactModule_OnSelectedContactChanged(object sender, EventArgs e)
        {
            if (POCContactModuleItem.LasteSelectedVmType == null || POCContactModuleItem.LasteSelectedVmType != GetType())
            {
                RequiresRefresh = true;
                return;
            }
            if (ReferenceEquals(CurrentContact, (AContactModule.SelectedContact as DBCTContact)))
                return;

            CheckToSave();
            DoRefresh();
        }



        #region RootEnable
        public bool RootEnable { get { return AContactModule.SelectedContact != null; } }
        #endregion




        #region [METHODS]
        private void GetDynamicData()
        {
            DynamicOwner = MainView.DynamicOwner;
            DynamicView = MainView as UserControl;
            DynamicTabControl = MainView.DynamicTabControl;
            if (LableNormalStyle == null)
                LableNormalStyle = MainView.FindResource("LayoutItemLabelStyle");
            if (LableNeedSaveStyle == null)
                LableNeedSaveStyle = MainView.FindResource("LILStyle_NeedSave");
            CountryPopupTemplate = MainView.FindResource("CountryPopupTemplate");
            CityPopupTemplate = MainView.FindResource("CityPopupTemplate");
        }
        private void CheckToSave()
        {
            if (ItemsToSave != null && ItemsToSave.Count > 0)
            {
                var needToFill = CheckForIsRequired();
                if (needToFill != null)
                {
                    POLMessageBox.ShowError("ورود اطلاعات برای :"+Environment.NewLine+Environment.NewLine+ needToFill.ProfileItem.Title +Environment.NewLine+Environment.NewLine+"اجباری می باشد.", DynamicOwner);
                    return;
                }
                    
                if (APOCCore.AutoSaveProfile)
                {
                    DataSave();
                }
                else
                {
                    if (POLMessageBox.ShowQuestionYesNo("تغییرات ذخیره شوند؟", DynamicOwner) == MessageBoxResult.Yes)
                        DataSave();
                }
            }
        }

        private DBCTProfileValue CheckForIsRequired()
        {
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


                foreach (var dbpv in q.ToList())
                {
                    var df = ADataFieldManager.FindByType(dbpv.ProfileItem.ItemType);
                    if (!df.IsRequiredSatisfied(dbpv))
                        return dbpv;
                }
            return null;
        }

        private void InitCommands()
        {
            CommandNew = new RelayCommand(DataNew, () => AContactModule.SelectedContact != null && AMembership.HasPermission(PCOPermissions.Contact_Profile_Add));
            CommandDelete = new RelayCommand(DataDelete, () => DynamicTabControl.SelectedTabItem != null && AMembership.HasPermission(PCOPermissions.Contact_Profile_DelRoot));
            CommandRefresh = new RelayCommand(DataRefresh, () => AContactModule.SelectedContact != null);
            CommandSave = new RelayCommand(DataSave2, () => ItemsToSave != null && ItemsToSave.Count > 0);

            CommandDeleteValue = new RelayCommand<object>(DeleteProfileValue, o => AMembership.HasPermission(PCOPermissions.Contact_Profile_DelItem));
            CommandDeleteGroup = new RelayCommand<object>(DeleteProfileGroup, o => AMembership.HasPermission(PCOPermissions.Contact_Profile_DelGroup));
            CommandDeleteRoot = new RelayCommand<object>(DeleteProfileRoot, o => AMembership.HasPermission(PCOPermissions.Contact_Profile_DelRoot));
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp54 != "");
        }

        private void DataSave2()
        {
            var needToFill = CheckForIsRequired();
            if (needToFill != null)
            {
                POLMessageBox.ShowError("ورود اطلاعات برای :" + Environment.NewLine + Environment.NewLine + needToFill.ProfileItem.Title + Environment.NewLine + Environment.NewLine + "اجباری می باشد.", DynamicOwner);
                return;
            }
            DataSave();
        }

        private void DataNew()
        {
            var dt = APOCMainWindow.ShowSelectProfileItem(DynamicOwner, null);

            var dbc = AContactModule.SelectedContact as DBCTContact;
            if (dbc == null) return;

            var addedCount = dbc.AddProfileObjectToContact(dt);

            DataRefresh();
            POLMessageBox.ShowInformation(string.Format("{0} فیلد به فرم پرونده اضافه شد.", addedCount), DynamicOwner);
        }
        private void DataDelete()
        {
            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("كلیه اطلاعات فرم زیر حذف شود؟{0}{0}{1}", Environment.NewLine, ((DBCTContact)AContactModule.SelectedContact).Title), DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            try
            {
                using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                {
                    var allVals = DBCTProfileValue.GetAll(uow, ((DBCTContact)AContactModule.SelectedContact).Oid);
                    allVals.ToList().ForEach(item =>
                    {
                        item.Delete();
                    });

                    uow.CommitChanges();
                }
                DynamicTabControl.Items.Clear();
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }
        private void DataRefresh()
        {
            DynamicTabControl.Items.Clear();
            CurrentContact = AContactModule.SelectedContact as DBCTContact;
            if (CurrentContact == null) return;
            var dbc = CurrentContact;

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
                        canEdit = AMembership.ActiveUser.Roles.Select(r => HelperConvert.CorrectPersianBug(r.ToLower())).Contains(root.RoleEditor == null ? string.Empty : root.RoleEditor.ToLower());
                        canView = canEdit || AMembership.ActiveUser.Roles.Select(r => HelperConvert.CorrectPersianBug(r.ToLower())).Contains(root.RoleViewer == null ? string.Empty : root.RoleViewer.ToLower());
                    }

                    var dxti = new DXTabItem
                    {
                        Padding = new Thickness(6),
                    };
                    dxti.Header = new TextBlock { Text = root.Title, ContextMenu = GetRootContextMenu(dxti, root) };

                    if (!canView)
                    {
                        dxti.IsEnabled = false;
                        DynamicTabControl.Items.Add(dxti);
                        continue;
                    }

                    var flc = new FlowLayoutControl
                    {
                        AllowItemMoving = AMembership.HasPermission(PCOPermissions.Contact_Profile_Reorder),
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
                    flc.ItemPositionChanged +=
                        (s, e) =>
                        {
                            var self = s as FlowLayoutControl;
                            if (self == null) return;
                            var order = 0;
                            foreach (var fe in self.Children.Cast<FrameworkElement>())
                            {
                                if (!(fe is DevExpress.Xpf.LayoutControl.GroupBox)) continue;
                                var groupBox = fe as DevExpress.Xpf.LayoutControl.GroupBox;
                                var db = groupBox.Tag as DBCTProfileGroup;
                                if (db != null && db.Order != order)
                                {
                                    db.Order = order;
                                    HelperUtils.Try(db.Save);
                                }
                                order++;
                            }
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
                                Background = new SolidColorBrush(Color.FromArgb(0xff, 0xe3, 0xef, 0xff)),// Brushes.LightSteelBlue,
                            };
                            gb.Header = new TextBlock { Text = group1.Title, ContextMenu = GetGroupContextMenu(flc, gb, group1) };
                            flc.Children.Add(gb);

                            var flcGroup = new FlowLayoutControl
                            {
                                AllowItemMoving = true,
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

                            flcGroup.ItemPositionChanged +=
                                (s, e) =>
                                {
                                    var self = s as FlowLayoutControl;
                                    if (self == null) return;
                                    var order = 0;
                                    foreach (var fe in self.Children.Cast<FrameworkElement>())
                                    {
                                        if (!(fe is LayoutItem)) continue;
                                        var li = fe as LayoutItem;
                                        var db = li.Tag as DBCTProfileValue;
                                        if (db != null && db.Order != order)
                                        {
                                            db.Order = order;
                                            HelperUtils.Try(db.Save);
                                        }
                                        order++;
                                    }
                                };



                            var currentValues = (from n in valueXPC
                                                 where n.ProfileItem.ProfileGroup.Oid == group1.Oid
                                                 orderby n.Order
                                                 select n).ToList();
                            foreach (var dbpv in currentValues)
                            {
                                var li = new LayoutItem
                                             {
                                                 Label = (dbpv.ProfileItem.IsRequired?"* ":string.Empty) + dbpv.ProfileItem.Title,
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
                ALogger.Log(ex.ToString(), Category.Exception, Priority.High);
            }
            DynamicTabControl.EndInit();
        }
        private void DataSave()
        {
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
            if (!NeedToSaveContact) return;
            var dbc = (DBCTContact)AContactModule.SelectedContact;
            dbc.Save();
            NeedToSaveContact = false;
        }
        private void DeleteProfileValue(object obj)
        {
            var v1 = (LayoutItemLabel)obj;
            var v2 = (LayoutItemPanel)v1.Parent;
            var v3 = (DXContentPresenter)v2.Content;
            var v4 = (FrameworkElement)v3.Content;
            var layoutItem = (LayoutItem)v4.Parent;
            var flc = (FlowLayoutControl)layoutItem.Parent;
            var db = (DBCTProfileValue)layoutItem.Tag;

            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("فیلد زیر حذف شود؟{0}{0}{1}", Environment.NewLine, db.ProfileItem.Title), DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            try
            {
                db.Delete();
                db.Save();
                flc.Children.Remove(layoutItem);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }

        }
        private void DeleteProfileGroup(object obj)
        {
            var v1 = (Tuple<FlowLayoutControl, DevExpress.Xpf.LayoutControl.GroupBox, DBCTProfileGroup>)obj;
            var flc = v1.Item1;
            var gb = v1.Item2;
            var db = v1.Item3;

            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("گروه زیر حذف شود؟{0}{0}{1}", Environment.NewLine, db.Title), DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            try
            {
                using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                {
                    var allVals = DBCTProfileValue.GetAll(uow, ((DBCTContact)AContactModule.SelectedContact).Oid);
                    var valsInGroup = from n in allVals where n.ProfileItem.ProfileGroup.Oid == db.Oid select n;
                    if (valsInGroup.Any())
                        valsInGroup.ToList().ForEach(item =>
                        {
                            item.Delete();
                        }
                    );

                    uow.CommitChanges();
                }
                flc.Children.Remove(gb);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }

        }
        private void DeleteProfileRoot(object obj)
        {
            var v1 = (Tuple<DXTabItem, DBCTProfileRoot>)obj;
            var tab = v1.Item1;
            var root = v1.Item2;

            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("فرم زیر حذف شود؟{0}{0}{1}", Environment.NewLine, root.Title), DynamicOwner);
            if (dr != MessageBoxResult.Yes) return;

            try
            {
                using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                {
                    var allVals = DBCTProfileValue.GetAll(uow, ((DBCTContact)AContactModule.SelectedContact).Oid);
                    var groups =
                        (from n in ACacheData.GetProfileItemList()
                         where ((DBCTProfileRoot)n.Tag).Oid == root.Oid
                         select n.ChildList.Select(m => (DBCTProfileGroup)m.Tag).ToList()).FirstOrDefault();
                    if (groups != null)
                        groups.ToList().ForEach(
                            g =>
                            {
                                var valsInGroup = from n in allVals where n.ProfileItem.ProfileGroup.Oid == g.Oid select n;
                                if (valsInGroup.Any())
                                    valsInGroup.ToList().ForEach(item =>
                                    {
                                        item.Delete();
                                    });
                            });
                    uow.CommitChanges();
                }
                DynamicTabControl.Items.Remove(tab);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }

        private ContextMenu GetRootContextMenu(DXTabItem dxti, DBCTProfileRoot root)
        {
            var rv = new ContextMenu
            {
                FlowDirection = HelperLocalize.ApplicationFlowDirection,
                FontFamily = new FontFamily(HelperLocalize.ApplicationFontName),
                FontSize = HelperLocalize.ApplicationFontSize,
            };
            var miDeleteRoot = new MenuItem
            {
                Header = "حذف فرم",
                Icon = new Image { Width = 16, Height = 16, Stretch = Stretch.Fill, Source = HelperImage.GetStandardImage16("_16_delete2.png") },
                Command = CommandDeleteRoot,
                CommandParameter = new Tuple<DXTabItem, DBCTProfileRoot>(dxti, root),
            };
            rv.Items.Add(miDeleteRoot);
            return rv;
        }
        private ContextMenu GetGroupContextMenu(FlowLayoutControl flc, DevExpress.Xpf.LayoutControl.GroupBox gb, DBCTProfileGroup db)
        {
            var rv = new ContextMenu
            {
                FlowDirection = HelperLocalize.ApplicationFlowDirection,
                FontFamily = new FontFamily(HelperLocalize.ApplicationFontName),
                FontSize = HelperLocalize.ApplicationFontSize,
            };
            var miDeleteGroup = new MenuItem
            {
                Header = "حذف گروه",
                Icon = new Image { Width = 16, Height = 16, Stretch = Stretch.Fill, Source = HelperImage.GetStandardImage16("_16_delete2.png") },
                Command = CommandDeleteGroup,
                CommandParameter = new Tuple<FlowLayoutControl, DevExpress.Xpf.LayoutControl.GroupBox, DBCTProfileGroup>(flc, gb, db),
            };
            rv.Items.Add(miDeleteGroup);
            return rv;
        }

        internal  void UpdateSaveStatus(DBCTProfileValue db, bool bb)
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

            if (db.ProfileItem != null)
            {
                if (db.ProfileItem.ProfileGroup != null)
                {
                    if (db.ProfileItem.ProfileGroup.IsRequired)
                    {
                        var q = from tab in DynamicTabControl.Items.Cast<DXTabItem>()
                                where tab.Content != null
                                from groupBox in ((FlowLayoutControl)tab.Content).Children.Cast<FrameworkElement>()
                                where
                                    (groupBox is DevExpress.Xpf.LayoutControl.GroupBox &&
                                    groupBox.Tag is DBCTProfileGroup &&
                                    ((DBCTProfileGroup)groupBox.Tag).Oid == db.ProfileItem.ProfileGroup.Oid &&
                                     ((DevExpress.Xpf.LayoutControl.GroupBox)groupBox).Content is FlowLayoutControl)
                                from layoutItem in
                                    ((FlowLayoutControl)((DevExpress.Xpf.LayoutControl.GroupBox)groupBox).Content).Children.Cast
                                    <FrameworkElement>()
                                where
                                    (layoutItem is LayoutItem) && (layoutItem.Tag is DBCTProfileValue) 
                                select (DBCTProfileValue)layoutItem.Tag;

                        if (q.Any())
                        {
                            q.ForEach(item =>
                            {
                                var li2 = (LayoutItem)item.LayoutItemHolder;
                                item.NeedToSave = true;
                                li2.LabelStyle =  LableNeedSaveStyle ;
                                if (!ItemsToSave.Contains(item.Oid))
                                    ItemsToSave.Add(item.Oid);
                            });
                        }
                                
                    }
                }
            }
        }
        private void SaveData(DBCTProfileValue dbpv)
        {
            try
            {
                var df = ADataFieldManager.FindByType(dbpv.ProfileItem.ItemType);
                df.Save(dbpv);
                if (ItemsToSave.Contains(dbpv.Oid))
                    ItemsToSave.Remove(dbpv.Oid);
                var li = (LayoutItem)dbpv.LayoutItemHolder;
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
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp54);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandNew { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }
        public RelayCommand CommandSave { get; set; }

        public RelayCommand<object> CommandDeleteValue { get; set; }
        public RelayCommand<object> CommandDeleteGroup { get; set; }
        public RelayCommand<object> CommandDeleteRoot { get; set; }
        public RelayCommand CommandHelp { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            AContactModule.OnSelectedContactChanged -= AContactModule_OnSelectedContactChanged;
            DynamicTabControl.Items.Clear();
        }
        #endregion

        #region ISave
        public void Save()
        {
            CheckToSave();
        }
        #endregion

        #region IRefrashable
        public void DoRefresh()
        {
            APOCMainWindow.ShowBusyIndicator();
            ItemsToSave = new List<Guid>();
            DataRefresh();
            RaisePropertyChanged("RootEnable");
            APOCMainWindow.HideBusyIndicator();
        }
        public bool RequiresRefresh { get; set; }
        #endregion
    }
}
