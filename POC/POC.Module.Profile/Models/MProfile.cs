using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Xml.Serialization;
using DevExpress.Xpf.Grid;
using DevExpress.Xpo;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Win32;
using POC.Module.Profile.Views.ModuleSettings;
using POL.Lib.Interfaces;
using POL.Lib.XOffice;
using POL.WPF.Controles.MVVM;
using POL.WPF.DXControls.MVVM;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using POL.DB.P30Office;
using POL.WPF.DXControls;

namespace POC.Module.Profile.Models
{
    public class MProfile : NotifyObjectBase, IDisposable
    {
        #region Private Properties
        private IPOCContactModule APOCContactModule { get; set; }
        private POCCore APOCCore { get; set; }
        private IDatabase ADatabase { get; set; }
        private IMembership AMembership { get; set; }
        private IPOCMainWindow APOCMainWindow { get; set; }
        private ILoggerFacade ALogger { get; set; }
        private ICacheData ACacheData { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }

        private DispatcherTimer DataUpdateTimer { get; set; }

        private dynamic MainView { get; set; }
        private TreeListControl DynamicTreeListControl { get; set; }
        private Window DynamicOwner { get; set; }
        #endregion



        #region CTOR
        public MProfile(object mainView)
        {
            MainView = mainView;

            APOCContactModule = ServiceLocator.Current.GetInstance<IPOCContactModule>();
            APOCCore = ServiceLocator.Current.GetInstance<POCCore>();
            AMembership = ServiceLocator.Current.GetInstance<IMembership>();
            ADatabase = ServiceLocator.Current.GetInstance<IDatabase>();
            APOCMainWindow = ServiceLocator.Current.GetInstance<IPOCMainWindow>();
            ALogger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            ACacheData = ServiceLocator.Current.GetInstance<ICacheData>();
            ADataFieldManager = ServiceLocator.Current.GetInstance<IDataFieldManager>();

            InitDynamics();

            InitCommands();
            NoSearch = true;
        }
        #endregion




        #region SearchText
        private string _SearchText;
        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                _SearchText = value;
                RaisePropertyChanged("SearchText");
                NoSearch = string.IsNullOrWhiteSpace(_SearchText);
                UpdateSearchWithDelay();
            }
        }
        #endregion

        #region ModulePropertyContent
        public UIElement ModulePropertyContent { get; set; }
        #endregion
        public string ModuleTypeTitle { get; set; }

        private bool NoSearch { get; set; }







        #region ProfileItemList
        public ObservableCollection<CacheItemProfileItem> ProfileItemList { get { return ACacheData.GetProfileItemList(); }  }
        #endregion
        #region SelectedProfileItem
        private CacheItemProfileItem _SelectedProfileItem;
        public CacheItemProfileItem SelectedProfileItem
        {
            get { return _SelectedProfileItem; }
            set
            {
                if (ReferenceEquals(value, _SelectedProfileItem))
                    return;
                _SelectedProfileItem = value;
                RaisePropertyChanged("SelectedProfileItem");
                PopulateModulePropertyContenet();
            }
        }
        #endregion




        #region [METHODS]
        private void InitDynamics()
        {
            DynamicTreeListControl = MainView.DynamicTreeListControl;
            DynamicOwner = MainView.DynamicOwner;

            DynamicTreeListControl.MouseDoubleClick += (s1, e1) =>
            {
                if (SelectedProfileItem == null) return;
                if (CommandEdit.CanExecute(null))
                    CommandEdit.Execute(null);
                e1.Handled = true;
            };

        }
        private void UpdateSearchWithDelay()
        {
            if (DataUpdateTimer == null)
            {
                DataUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                DataUpdateTimer.Tick += (s, e) =>
                {
                    DataUpdateTimer.Stop();
                    PopulateSearchResult();
                };
            }
            DataUpdateTimer.Stop();
            DataUpdateTimer.Start();
        }
        private void PopulateSearchResult()
        {
            foreach (var pi in ProfileItemList)
            {
                pi.InSearch = !string.IsNullOrEmpty(SearchText) && pi.Title.Contains(SearchText);
                foreach (var pi2 in pi.ChildList)
                {
                    pi2.InSearch = !string.IsNullOrEmpty(SearchText) && pi2.Title.Contains(SearchText);
                    foreach (var pi3 in pi2.ChildList)
                    {
                        pi3.InSearch = !string.IsNullOrEmpty(SearchText) && pi3.Title.Contains(SearchText);
                    }
                }
            }
        }
        private void PopulateModulePropertyContenet()
        {
            if (SelectedProfileItem == null)
            {
                ModulePropertyContent = null;
                ModuleTypeTitle = string.Empty;
                RaisePropertyChanged("ModuleTypeTitle");
                RaisePropertyChanged("ModulePropertyContent");
                return;
            }
            if (SelectedProfileItem.Tag is DBCTProfileRoot)
            {
                var r = SelectedProfileItem.Tag as DBCTProfileRoot;
                ModulePropertyContent = new UIProfileRoot { Item = r };
                ModuleTypeTitle = "فرم";
            }
            else if (SelectedProfileItem.Tag is DBCTProfileGroup)
            {
                var r = SelectedProfileItem.Tag as DBCTProfileGroup;
                ModulePropertyContent = new UIProfileGroup { Item = r };
                ModuleTypeTitle = "گروه";
            }
            else
            {
                var item = SelectedProfileItem.Tag as DBCTProfileItem;
                if (item != null)
                {
                    var df = ADataFieldManager.FindByType(item.ItemType);
                    if (df == null)
                    {
                        ModulePropertyContent = null;
                        ModuleTypeTitle = string.Empty;
                    }
                    else
                    {
                        ModulePropertyContent = df.GetUISettingsWpf(item) as UIElement;
                        ModuleTypeTitle = df.Title;
                    }
                }
                
            }
            RaisePropertyChanged("ModuleTypeTitle");
            RaisePropertyChanged("ModulePropertyContent");
        }
        private DBCTProfileGroup GetSelectedProfileGroup()
        {
            if (SelectedProfileItem == null) return null;
            var test = SelectedProfileItem.Tag;
            if (test is DBCTProfileRoot) return null;
            return test is DBCTProfileGroup ? (test as DBCTProfileGroup) : ((DBCTProfileItem)test).ProfileGroup;
        }
        private DBCTProfileRoot GetSelectedProfileRoot()
        {
            if (SelectedProfileItem == null) return null;
            var test = SelectedProfileItem.Tag;
            if (test is DBCTProfileRoot)
                return test as DBCTProfileRoot;
            return test is DBCTProfileGroup
                       ? ((DBCTProfileGroup)test).ProfileRoot
                       : ((DBCTProfileItem)test).ProfileGroup.ProfileRoot;
        }




        private void InitCommands()
        {
            CommandProfileRootNew = new RelayCommand(ProfileRootNew, () => AMembership.HasPermission(PCOPermissions.Profile_RootAdd) && !APOCCore.STCI.IsTamas );
            CommandProfileGroupNew = new RelayCommand(ProfileGroupNew, () => SelectedProfileItem != null && DynamicTreeListControl.SelectedItems.Count == 1 && AMembership.HasPermission(PCOPermissions.Profile_GroupAdd) && !APOCCore.STCI.IsTamas);
            CommandProfileItemNew = new RelayCommand(ProfileItemNew, () => SelectedProfileItem != null && DynamicTreeListControl.SelectedItems.Count == 1 && !(SelectedProfileItem.Tag is DBCTProfileRoot) && AMembership.HasPermission(PCOPermissions.Profile_ItemAdd) && !APOCCore.STCI.IsTamas);

            CommandEdit = new RelayCommand(Edit, () => SelectedProfileItem != null && DynamicTreeListControl.SelectedItems.Count == 1 && !APOCCore.STCI.IsTamas);
            CommandDelete = new RelayCommand(Delete, () => SelectedProfileItem != null && !APOCCore.STCI.IsTamas);
            CommandRefresh = new RelayCommand(() => { APOCMainWindow.ShowBusyIndicator(); Refresh(null); APOCMainWindow.HideBusyIndicator(); }, () => !APOCCore.STCI.IsTamas);



            CommandContactLink = new RelayCommand(ContactLink, () => SelectedProfileItem != null && AMembership.HasPermission(PCOPermissions.Profile_LinkContact));
            CommandContactUnLink = new RelayCommand(ContactUnlink, () => SelectedProfileItem != null && AMembership.HasPermission(PCOPermissions.Profile_UnlinkContact));
            CommandContactReorder = new RelayCommand(ContactReorder, () => SelectedProfileItem != null && (SelectedProfileItem.Tag is DBCTProfileGroup) && AMembership.HasPermission(PCOPermissions.Profile_ReorderContact));
            CommandExportXMLSelected = new RelayCommand(
                () =>
                {
                    var sel = (from n in DynamicTreeListControl.SelectedItems.Cast<CacheItemProfileItem>()
                               where  ((CacheItemProfileItem)n).Tag is DBCTProfileRoot
                               select (CacheItemProfileItem)n).ToList();
                    ExportXML(sel);
                },
                () => SelectedProfileItem != null && AMembership.HasPermission(PCOPermissions.Profile_ExportXML)
                );
            CommandExportXML = new RelayCommand(() => ExportXML(null), () => AMembership.HasPermission(PCOPermissions.Profile_ExportXML));
            CommandImportXML = new RelayCommand(ImportXML, () => AMembership.HasPermission(PCOPermissions.Profile_ImportXML) && !APOCCore.STCI.IsTamas);

            CommandClearSearchText = new RelayCommand(ClearSearchText, () => !NoSearch);

            CommandExpandAll = new RelayCommand(ExpandAll, () => true);
            CommandCollapseAll = new RelayCommand(CollapseAll, () => true);
            CommandHelp = new RelayCommand(Help, () => ConstantPOCHelpURL.POCHelp53 != "");
        }



        private void ProfileRootNew()
        {
            var w = new Views.WProfileRootAddEdit(null) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            APOCMainWindow.ShowBusyIndicator();
            Refresh(w.DynamicSelectedData);
            SelectedProfileItem = (from n in ProfileItemList where n.Tag is DBCTProfileRoot && ((DBCTProfileRoot)n.Tag).Title == w.DynamicSelectedData.Title select n).FirstOrDefault();
            APOCMainWindow.HideBusyIndicator();
        }
        private void ProfileGroupNew()
        {
            var root = GetSelectedProfileRoot();
            if (root == null) return;
            var w = new Views.WProfileGroupAddEdit(root, null) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            APOCMainWindow.ShowBusyIndicator();
            Refresh(w.DynamicSelectedData);
            SelectedProfileItem = (from r in ProfileItemList from g in r.ChildList where ((DBCTProfileGroup)g.Tag).Oid == w.DynamicSelectedData.Oid select g).FirstOrDefault();
            APOCMainWindow.HideBusyIndicator();
        }
        private void ProfileItemNew()
        {
            var group = GetSelectedProfileGroup();
            if (group == null) return;
            var w = new Views.WProfileItemAddEdit(group, null) { Owner = DynamicOwner };
            if (w.ShowDialog() != true) return;
            APOCMainWindow.ShowBusyIndicator();
            Refresh(w.DynamicSelectedData);
            SelectedProfileItem = (from r in ProfileItemList from g in r.ChildList from i in g.ChildList where ((DBCTProfileItem)i.Tag).Oid == w.DynamicSelectedData.Oid select i).FirstOrDefault();
            APOCMainWindow.HideBusyIndicator();
        }

        private void Edit()
        {
            if (SelectedProfileItem == null) return;
            if (SelectedProfileItem.Tag == null) return;

            if (SelectedProfileItem.Tag is DBCTProfileRoot)
            {
                if (!AMembership.HasPermission(PCOPermissions.Profile_RootEdit))
                {
                    POLMessageBox.ShowWarning("خطا : عدم دریافت مجوز جهت اجرای فرمان.", DynamicOwner);
                    return;
                }
                var w = new Views.WProfileRootAddEdit(SelectedProfileItem.Tag as DBCTProfileRoot) { Owner = DynamicOwner };
                if (w.ShowDialog() == true)
                {
                    SelectedProfileItem.Title = w.DynamicSelectedData.Title;
                }
            }
            if (SelectedProfileItem.Tag is DBCTProfileGroup)
            {
                if (!AMembership.HasPermission(PCOPermissions.Profile_GroupEdit))
                {
                    POLMessageBox.ShowWarning("خطا : عدم دریافت مجوز جهت اجرای فرمان.", DynamicOwner);
                    return;
                }
                var group = SelectedProfileItem.Tag as DBCTProfileGroup;
                var w = new Views.WProfileGroupAddEdit(group.ProfileRoot, group) { Owner = DynamicOwner };
                if (w.ShowDialog() == true)
                    SelectedProfileItem.Title = w.DynamicSelectedData.Title;
            }
            if (SelectedProfileItem.Tag is DBCTProfileItem)
            {
                if (!AMembership.HasPermission(PCOPermissions.Profile_ItemEdit))
                {
                    POLMessageBox.ShowWarning("خطا : عدم دریافت مجوز جهت اجرای فرمان.", DynamicOwner);
                    return;
                }
                var item = SelectedProfileItem.Tag as DBCTProfileItem;
                var w = new Views.WProfileItemAddEdit(item.ProfileGroup, item) { Owner = DynamicOwner };
                if (w.ShowDialog() == true)
                    SelectedProfileItem.Title = w.DynamicSelectedData.Title;
            }
        }
        private void Delete()
        {
            if (SelectedProfileItem == null) return;
            if (SelectedProfileItem.Tag == null) return;

            var list = (from n in DynamicTreeListControl.SelectedItems.Cast<CacheItemProfileItem>() select n).ToList();
            var itemcount = list.Count(n => n.Tag is DBCTProfileItem);
            var profilecount = list.Count(n => n.Tag is DBCTProfileRoot);
            var groupcount = list.Count(n => n.Tag is DBCTProfileGroup);
            if (Math.Max(profilecount, Math.Max(itemcount, groupcount)) < (itemcount + profilecount + groupcount))
            {
                POLMessageBox.ShowWarning("خطا : لطفا گزینه های انتخاب شده را از یك نوع انتخاب كنید.", DynamicOwner);
                return;
            }

            if (profilecount > 0)
                if (POLMessageBox.ShowQuestionYesNo("فرم انتخاب شده حذف شود؟", DynamicOwner) != MessageBoxResult.Yes) return;
            if (groupcount > 0)
                if (POLMessageBox.ShowQuestionYesNo("گروه انتخاب شده حذف شود؟", DynamicOwner) != MessageBoxResult.Yes) return;
            if (itemcount > 0)
                if (POLMessageBox.ShowQuestionYesNo("فیلد انتخاب شده حذف شود؟", DynamicOwner) != MessageBoxResult.Yes) return;
            foreach (var spi in list)
            {
                #region DBCTProfileRoot

                if (spi.Tag is DBCTProfileRoot)
                {
                    if (!AMembership.HasPermission(PCOPermissions.Contact_Profile_DelRoot))
                    {
                        POLMessageBox.ShowWarning("خطا : عدم دریافت مجوز جهت اجرای فرمان.", DynamicOwner);
                        return;
                    }
                    var dbRoot = spi.Tag as DBCTProfileRoot;
                    var xpc =
                    (from n in ACacheData.GetProfileItemList()
                     where ((DBCTProfileRoot)n.Tag).Oid == dbRoot.Oid
                     select n.ChildList.Select(m => (DBCTProfileGroup)m.Tag).ToList()).FirstOrDefault();

                    if (xpc != null && xpc.Count > 0)
                    {
                        POLMessageBox.ShowError("فرم دارای تعدادی گروه می باشد. امكان حذف وجود ندارد.", DynamicOwner);
                        return;
                    }
                    try
                    {
                        dbRoot.Delete();
                        dbRoot.Save();
                        Refresh(dbRoot);
                    }
                    catch (Exception ex)
                    {
                        POLMessageBox.ShowError(ex.Message);
                        return;
                    }
                }

                #endregion

                #region DBCTProfileGroup

                if (spi.Tag is DBCTProfileGroup)
                {
                    if (!AMembership.HasPermission(PCOPermissions.Contact_Profile_DelGroup))
                    {
                        POLMessageBox.ShowWarning("خطا : عدم دریافت مجوز جهت اجرای فرمان.", DynamicOwner);
                        return;
                    }
                    var dbGroup = spi.Tag as DBCTProfileGroup;
                    var xpc =
                    (from r in ACacheData.GetProfileItemList()
                     from g in r.ChildList
                     where ((DBCTProfileGroup)g.Tag).Oid == dbGroup.Oid
                     select g.ChildList.Select(m => (DBCTProfileItem)m.Tag).ToList()).FirstOrDefault();

                    if (xpc != null && xpc.Count > 0)
                    {
                        POLMessageBox.ShowError("گروه دارای تعدادی فیلد می باشد. امكان حذف وجود ندارد.", DynamicOwner);
                        return;
                    }
                    try
                    {
                        dbGroup.Delete();
                        dbGroup.Save();
                        Refresh(dbGroup);
                    }
                    catch (Exception ex)
                    {
                        POLMessageBox.ShowError(ex.Message);
                        return;
                    }
                }

                #endregion

                #region DBCTProfileItem
                if (spi.Tag is DBCTProfileItem)
                {
                    if (!AMembership.HasPermission(PCOPermissions.Contact_Profile_DelItem))
                    {
                        POLMessageBox.ShowWarning("خطا : عدم دریافت مجوز جهت اجرای فرمان.", DynamicOwner);
                        return;
                    }
                    var dbItem = spi.Tag as DBCTProfileItem;


                    var xpq = new XPQuery<DBCTProfileValue>(ADatabase.Dxs);
                    var count = xpq.Count(n => n.ProfileItem.Oid == dbItem.Oid);
                    if (count > 0)
                    {
                        POLMessageBox.ShowError(string.Format("خطا : فیلد ({0}) در {1} پرونده مورد استفاده قرار گرفته.", dbItem.Title, count));
                        return;
                    }
                    if (dbItem.ItemType == EnumProfileItemType.List)
                    {
                    }
                    try
                    {
                        dbItem.Delete();
                        dbItem.Save();
                        Refresh(dbItem);
                    }
                    catch (Exception ex)
                    {
                        POLMessageBox.ShowError(ex.Message);
                        return;
                    }
                }
                #endregion
            }
        }
        private void Refresh(object db)
        {
            ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileItem);
            ACacheData.ForcePopulateCache(EnumCachDataType.ProfileItem, false, db);
        }

        private void ExportXML(List<CacheItemProfileItem> selectedRoots)
        {
            var sf = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = "Form.xml"
            };
            if (sf.ShowDialog() != true) return;
            if (selectedRoots == null)
            {
                selectedRoots = ACacheData.GetProfileItemList().ToList();
            }

            try
            {
                var ioRoots = new PackIOProfileRootArray()
                    {
                        ProfileRoots = (from r in selectedRoots
                                        select new PackIOProfileRoot
                                        {
                                            Title = r.Title,
                                            Order = r.Order,
                                            RoleEdit = r.RoleEditor,
                                            RoleView = r.RoleViewer,
                                            Cats = ((DBCTProfileRoot)r.Tag).ContactCats.ToList().Select(n => n.Title).ToArray(),
                                            Groups = (from g in r.ChildList
                                                      select new PackIOProfileGroup
                                                      {
                                                          Title = g.Title,
                                                          Order = g.Order,
                                                          Items = (from item in g.ChildList
                                                                   let i = item.Tag as DBCTProfileItem
                                                                   select new PackIOProfileItem
                                                                   {
                                                                       Title = item.Title,
                                                                       Order = item.Order,
                                                                       Double1 = i.Double1,
                                                                       Double2 = i.Double2,
                                                                       Guid1 = i.Guid1,
                                                                       Int1 = i.Int1,
                                                                       Int2 = i.Int2,
                                                                       Int3 = i.Int3,
                                                                       ItemType = i.ItemType,
                                                                       String1 = i.String1,
                                                                       String2 = i.String2,
                                                                       String3 = i.String3,
                                                                       TableName = GetTableNameForExport(i),
                                                                       UnicCode = i.UnicCode,
                                                                   }).ToArray(),
                                                      }).ToArray(),
                                        }).ToArray(),
                    };

                var serializer = new XmlSerializer(ioRoots.GetType());
                using (var f = new StreamWriter(sf.FileName))
                {
                    serializer.Serialize(f, ioRoots);
                }
                POLMessageBox.ShowInformation("عملیات با موفقیت انجام شد.", DynamicOwner);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }
        private string GetTableNameForExport(DBCTProfileItem i)
        {
            if (i.ItemType == EnumProfileItemType.StringCombo || i.ItemType == EnumProfileItemType.StringCheckList)
            {
                var db = DBCTProfileTable.FindByOid(i.Session, i.Guid1);
                if (db != null)
                    return db.Title;
            }
            return string.Empty;
        }
        private void ImportXML()
        {
            var sf = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "xml",
                Filter = "XML (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
            };
            if (sf.ShowDialog() != true) return;
            try
            {
                PackIOProfileRootArray roots;
                var serializer = new XmlSerializer(typeof(PackIOProfileRootArray));
                using (var f = new StreamReader(sf.FileName))
                {
                    roots = serializer.Deserialize(f) as PackIOProfileRootArray;
                    if (roots == null)
                        throw new Exception("محتوای فایل معتبر نمی باشد.");
                    if (roots.ProfileRoots == null || !roots.ProfileRoots.Any())
                        throw new Exception("محتوای فایل معتبر نمی باشد : فرم خالی می باشد.");

                    foreach (var root in roots.ProfileRoots)
                    {
                        if (string.IsNullOrWhiteSpace(root.Title))
                            throw new Exception("محتوای فایل معتبر نمی باشد : نام فرم خالی می باشد.");
                        var dbr = DBCTProfileRoot.FindDuplicateTitleExcept(ADatabase.Dxs, null, root.Title);
                        if (dbr != null)
                            throw new Exception(string.Format("نام فرم تكراری می باشد :{0}{0}نام : {1}",
                                                              Environment.NewLine, dbr.Title));
                        using (var uow = new UnitOfWork(ADatabase.Dxs.DataLayer))
                        {
                            var dbroot = new DBCTProfileRoot(uow)
                                             {
                                                 Title = root.Title,
                                                 Order = root.Order,
                                                 RoleEditor =
                                                     (from n in ACacheData.GetRoleList()
                                                      where n.Title == root.RoleEdit
                                                      select n.Title).FirstOrDefault(),
                                                 RoleViewer =
                                                     (from n in ACacheData.GetRoleList()
                                                      where n.Title == root.RoleView
                                                      select n.Title).FirstOrDefault(),
                                             };
                            root.Cats.ToList().ForEach(cat =>
                                                           {
                                                               var c = (from n in ACacheData.GetContactCatList()
                                                                        where n.Title == cat
                                                                        select
                                                                            DBCTContactCat.FindDuplicateTitleExcept(
                                                                                uow, null, n.Title)
                                                                       ).FirstOrDefault();
                                                               if (c != null)
                                                                   dbroot.ContactCats.Add(c);
                                                           });

                            foreach (var group in root.Groups)
                            {
                                var dbgroup = new DBCTProfileGroup(uow)
                                                  {
                                                      ProfileRoot = dbroot,
                                                      Title = group.Title,
                                                      Order = group.Order,
                                                  };
                                if (string.IsNullOrWhiteSpace(group.Title))
                                    throw new Exception("محتوای فایل معتبر نمی باشد : نام گروه خالی می باشد.");

                                foreach (var item in group.Items)
                                {
                                    var dbitem = new DBCTProfileItem(uow)
                                    {
                                        ProfileGroup = dbgroup,
                                        Title = item.Title,
                                        Order = item.Order,
                                        ItemType = item.ItemType,
                                        Double1 = item.Double1,
                                        Double2 = item.Double2,
                                        Guid1 = item.Guid1,
                                        Int1 = item.Int1,
                                        Int2 = item.Int2,
                                        Int3 = item.Int3,
                                        String1 = item.String1,
                                        String2 = item.String2,
                                        String3 = item.String3,
                                        UnicCode = item.UnicCode,
                                    };
                                    dbitem.Guid1 = GetGuidFromImport(uow, item.ItemType, item.Guid1, item.TableName);
                                    if (string.IsNullOrWhiteSpace(item.Title))
                                        throw new Exception("محتوای فایل معتبر نمی باشد : عنوان فیلد خالی می باشد.");
                                    dbitem.Save();
                                }
                                dbgroup.Save();
                            }
                            dbroot.Save();
                            uow.CommitChanges();
                        }
                    }
                }
                POLMessageBox.ShowInformation(string.Format("عملیات با موفقیت انجام شد.{0}{0}تعداد فرم : {1}", Environment.NewLine, roots.ProfileRoots.Count()), DynamicOwner);
                Refresh(null);
            }
            catch (Exception ex)
            {
                ALogger.Log(ex.ToString(), Category.Exception, Priority.Medium);
                POLMessageBox.ShowError(ex.Message, DynamicOwner);
            }
        }

        private Guid GetGuidFromImport(UnitOfWork uow, EnumProfileItemType itemType, Guid guid, string name)
        {
            if (!(itemType == EnumProfileItemType.StringCombo || itemType == EnumProfileItemType.StringCheckList))
                return Guid.Empty;

            if (guid == Guid.Empty) return guid;
            if (string.IsNullOrWhiteSpace(name)) return Guid.Empty;

            var db = DBCTProfileTable.FindDuplicateTitleExcept(uow, null, name);
            if (db != null)
                return db.Oid;

            return Guid.Empty;
        }
        private void ClearSearchText()
        {
            SearchText = string.Empty;
        }
        private void ContactLink()
        {
            if (SelectedProfileItem == null) return;

            if (SelectedProfileItem.TreeLevel < 2)
            {
                var fields = 0;
                if (SelectedProfileItem.TreeLevel == 0)
                    fields = (from g in SelectedProfileItem.ChildList
                              from i in g.ChildList
                              select i).Count();
                if (SelectedProfileItem.TreeLevel == 1)
                    fields = (from i in SelectedProfileItem.ChildList select i).Count();

                if (fields == 0)
                {
                    POLMessageBox.ShowWarning("خطا : هیچگونه فیلدی در زیرمجموعه های فرم/گروه انتخاب شده موجود نمی باشد." +
                                              Environment.NewLine + "حضور حداقل یك فیلد الزامی می باشد.");
                    return;
                }
            }

            var rv = APOCMainWindow.ShowContactSelectByResult(null);
            if (rv == null) return;
            var s1 = "فرم";
            if (SelectedProfileItem.Tag is DBCTProfileGroup) s1 = "گروه";
            if (SelectedProfileItem.Tag is DBCTProfileItem) s1 = "فیلد";
            var s2 = "تمام پرونده ها";
            if (rv.SelectionType == EnumContactSelectType.SelectionBasket) s2 = "سبد انتخاب شده";
            if (rv.SelectionType == EnumContactSelectType.Category) s2 = "دسته انتخاب شده";

            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("{0} {1} به {2} الحاق شود؟", s1, SelectedProfileItem.Title, s2),
                                            APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;

            var dxs = ADatabase.GetNewSession();
            var moduleList = new List<DBCTProfileItem>();
            if (SelectedProfileItem.Tag is DBCTProfileItem)
                moduleList.Add(DBCTProfileItem.FindByOid(dxs, (SelectedProfileItem.Tag as DBCTProfileItem).Oid));
            if (SelectedProfileItem.Tag is DBCTProfileGroup)
            {
                (from i in SelectedProfileItem.ChildList
                 select i).ToList().ForEach(i =>
                 {
                     var pi = DBCTProfileItem.FindByOid(dxs, ((DBCTProfileItem)i.Tag).Oid);
                     if (pi != null)
                         moduleList.Add(pi);
                 });
            }

            if (SelectedProfileItem.Tag is DBCTProfileRoot)
            {

                (from g in SelectedProfileItem.ChildList
                 from i in g.ChildList
                 select i).ToList().ForEach(i =>
                                                {
                                                    var pi = DBCTProfileItem.FindByOid(dxs, ((DBCTProfileItem)i.Tag).Oid);
                                                    if (pi != null)
                                                        moduleList.Add(pi);
                                                });
            }

            if (moduleList.Count == 0)
            {
                POLMessageBox.ShowWarning("هیچ فیلدی جهت الحاق پیدا نشد.", APOCMainWindow.GetWindow());
                return;
            }

            var contactOidList = new List<Guid>();
            if (rv.SelectionType == EnumContactSelectType.All)
                contactOidList = (from n in new XPQuery<DBCTContact>(dxs) select n.Oid).ToList();
            if (rv.SelectionType == EnumContactSelectType.Category)
                contactOidList = (from n in new XPCollection<DBCTContact>(dxs,
                                                new DevExpress.Data.Filtering.ContainsOperator(
                                                    "Categories",
                                                    new DevExpress.Data.Filtering.BinaryOperator(
                                                        "Oid",
                                                        rv.SelectionOid)),
                                                    null)
                                  select n.Oid).ToList();

            if (rv.SelectionType == EnumContactSelectType.SelectionBasket)
                contactOidList = (from n in new XPCollection<DBCTContact>(dxs,
                                                new DevExpress.Data.Filtering.ContainsOperator(
                                                   "Selections",
                                                   new DevExpress.Data.Filtering.BinaryOperator(
                                                       "Oid",
                                                       rv.SelectionOid)),
                                                   null)
                                  select n.Oid).ToList();

            if (contactOidList.Count == 0)
            {
                POLMessageBox.ShowWarning("هیچ پرونده ای جهت الحاق پیدا نشد.", APOCMainWindow.GetWindow());
                return;
            }
            var linked = 0;
            var failed = 0;
            var contactcount = 0;
            POLProgressBox.Show("الحاق فیلد به پرونده", true, 0, contactOidList.Count, 1,
            pb =>
            {

                foreach (var guid in contactOidList)
                {
                    if (pb.NeedToCancel) return;
                    var dbc = DBCTContact.FindByOid(dxs, guid);
                    pb.AsyncSetText(1, dbc.Title);
                    pb.AsyncSetValue(contactcount);
                    contactcount++;


                    foreach (var module in moduleList)
                    {
                        var dataField = ADataFieldManager.FindByType(module.ItemType);
                        var dbv = DBCTProfileValue.FindByContactAndItem(dxs, guid, module.Oid);
                        if (dbv != null) continue;
                        try
                        {

                            dbv = new DBCTProfileValue(dxs)
                            {
                                Contact = dbc,
                                Order = 1000,
                                ProfileItem = module,
                            };
                            dataField.SetValueToDefault(dbv, module);
                            dbv.Save();
                            linked++;
                        }
                        catch
                        {
                            failed++;
                        }
                    }
                }
            },
            pb => POLMessageBox.ShowInformation(string.Format("تعداد پرونده پردازش شده : {0}{1}تعداد فیلد های الحاق شده : {2}{1}تعداد خطا ها : {3}", contactcount, Environment.NewLine, linked, failed), pb),
            APOCMainWindow.GetWindow());

        }
        private void ContactUnlink()
        {
            if (SelectedProfileItem == null) return;
            var rv = APOCMainWindow.ShowContactSelectByResult(null);
            if (rv == null) return;
            var s1 = "فرم";
            if (SelectedProfileItem.Tag is DBCTProfileGroup) s1 = "گروه";
            if (SelectedProfileItem.Tag is DBCTProfileItem) s1 = "فیلد";
            var s2 = "تمام پرونده ها";
            if (rv.SelectionType == EnumContactSelectType.SelectionBasket) s2 = "سبد انتخاب شده";
            if (rv.SelectionType == EnumContactSelectType.Category) s2 = "دسته انتخاب شده";

            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("{0} {1} از {2} برداشته شود؟{3}توجه : تمامی اطلاعات وابسته نیز حذف خواهد شد.",
                                                        s1, SelectedProfileItem.Title, s2, Environment.NewLine),
                                            APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;

            var dxs = ADatabase.GetNewSession();
            var moduleList = new List<DBCTProfileItem>();
            if (SelectedProfileItem.Tag is DBCTProfileItem)
                moduleList.Add((DBCTProfileItem)SelectedProfileItem.Tag);
            if (SelectedProfileItem.Tag is DBCTProfileGroup)
            {
                (from i in SelectedProfileItem.ChildList
                 select i).ToList().ForEach(i =>
                 {
                     var pi = (DBCTProfileItem)i.Tag;
                     if (pi != null)
                         moduleList.Add(pi);
                 });
            }

            if (SelectedProfileItem.Tag is DBCTProfileRoot)
            {

                (from g in SelectedProfileItem.ChildList
                 from i in g.ChildList
                 select i).ToList().ForEach(i =>
                 {
                     var pi = (DBCTProfileItem)i.Tag;
                     if (pi != null)
                         moduleList.Add(pi);
                 });
            }


            if (moduleList.Count == 0)
            {
                POLMessageBox.ShowWarning("هیچ فیلدی جهت تفكیك پیدا نشد.", APOCMainWindow.GetWindow());
                return;
            }

            var contactOidList = new List<Guid>();
            if (rv.SelectionType == EnumContactSelectType.All)
                contactOidList = (from n in new XPQuery<DBCTContact>(dxs) select n.Oid).ToList();
            if (rv.SelectionType == EnumContactSelectType.Category)
                contactOidList = (from n in new XPCollection<DBCTContact>(dxs,
                                                new DevExpress.Data.Filtering.ContainsOperator(
                                                    "Categories",
                                                    new DevExpress.Data.Filtering.BinaryOperator(
                                                        "Oid",
                                                        rv.SelectionOid)),
                                                    null)
                                  select n.Oid).ToList();

            if (rv.SelectionType == EnumContactSelectType.SelectionBasket)
                contactOidList = (from n in new XPCollection<DBCTContact>(dxs,
                                                new DevExpress.Data.Filtering.ContainsOperator(
                                                   "Selections",
                                                   new DevExpress.Data.Filtering.BinaryOperator(
                                                       "Oid",
                                                       rv.SelectionOid)),
                                                   null)
                                  select n.Oid).ToList();

            if (contactOidList.Count == 0)
            {
                POLMessageBox.ShowWarning("هیچ پرونده ای جهت تفكیك پیدا نشد.", APOCMainWindow.GetWindow());
                return;
            }
            var linked = 0;
            var failed = 0;
            var contactcount = 0;
            POLProgressBox.Show("حذف فیلد از پرونده", true, 0, contactOidList.Count, 1,
            pb =>
            {
                foreach (var guid in contactOidList)
                {
                    if (pb.NeedToCancel) return;
                    var dbc = DBCTContact.FindByOid(dxs, guid);
                    pb.AsyncSetText(1, dbc.Title);
                    pb.AsyncSetValue(contactcount);
                    contactcount++;
                    foreach (var module in moduleList)
                    {
                        var dbv = DBCTProfileValue.FindByContactAndItem(dxs, guid, module.Oid);
                        if (dbv != null)
                            try
                            {
                                dbv.Delete();
                                dbv.Save();
                                linked++;
                            }
                            catch
                            {
                                failed++;
                            }
                    }
                }
            },
            pb => POLMessageBox.ShowInformation(string.Format("تعداد پرونده پردازش شده : {0}{1}تعداد فیلد های حذف شده : {2}{1}تعداد خطا ها : {3}", contactcount, Environment.NewLine, linked, failed), pb),
            APOCMainWindow.GetWindow());
        }
        private void ContactReorder()
        {
            if (SelectedProfileItem == null) return;
            var rv = APOCMainWindow.ShowContactSelectByResult(null);
            if (rv == null) return;
            var s2 = "تمام پرونده ها";
            if (rv.SelectionType == EnumContactSelectType.SelectionBasket) s2 = "سبد انتخاب شده";
            if (rv.SelectionType == EnumContactSelectType.Category) s2 = "دسته انتخاب شده";

            var dr = POLMessageBox.ShowQuestionYesNo(string.Format("{0} {1} در {2} مرتب شود؟", "گروه", SelectedProfileItem.Title, s2),
                                            APOCMainWindow.GetWindow());
            if (dr != MessageBoxResult.Yes) return;

            var dxs = ADatabase.GetNewSession();


            var moduleList = new List<DBCTProfileItem>();
            if (SelectedProfileItem.Tag is DBCTProfileItem)
                moduleList.Add(DBCTProfileItem.FindByOid(dxs, (SelectedProfileItem.Tag as DBCTProfileItem).Oid));
            if (SelectedProfileItem.Tag is DBCTProfileGroup)
            {
                var v = (from r in ACacheData.GetProfileItemList()
                         from g in r.ChildList
                         where ((DBCTProfileGroup)g.Tag).Oid == ((DBCTProfileGroup)SelectedProfileItem.Tag).Oid
                         select g.ChildList.Select(m => (DBCTProfileItem)m.Tag).ToList()).FirstOrDefault();
                if (v != null && v.Any())
                    moduleList.AddRange(v);
            }

            if (SelectedProfileItem.Tag is DBCTProfileRoot)
            {
                var groups = (from n in ACacheData.GetProfileItemList()
                              where ((DBCTProfileRoot)n.Tag).Oid == ((DBCTProfileRoot)SelectedProfileItem.Tag).Oid
                              select n.ChildList.Select(m => (DBCTProfileGroup)m.Tag).ToList()).FirstOrDefault();
                var v = (from r in ACacheData.GetProfileItemList()
                         from g in r.ChildList
                         where ((DBCTProfileGroup)g.Tag).Oid == ((DBCTProfileGroup)SelectedProfileItem.Tag).Oid
                         select g.ChildList.Select(m => (DBCTProfileItem)m.Tag).ToList()).FirstOrDefault();
                if (v != null && v.Any())
                    moduleList.AddRange(
                        from g in groups 
                        from i in v
                        select i
                        );
            }

            if (moduleList.Count == 0)
            {
                POLMessageBox.ShowWarning("هیچ فیلدی جهت مرتب سازی پیدا نشد.", APOCMainWindow.GetWindow());
                return;
            }

            var contactOidList = new List<Guid>();
            if (rv.SelectionType == EnumContactSelectType.All)
                contactOidList = (from n in new XPQuery<DBCTContact>(dxs) select n.Oid).ToList();
            if (rv.SelectionType == EnumContactSelectType.Category)
                contactOidList = (from n in new XPCollection<DBCTContact>(dxs,
                                                new DevExpress.Data.Filtering.ContainsOperator(
                                                    "Categories",
                                                    new DevExpress.Data.Filtering.BinaryOperator(
                                                        "Oid",
                                                        rv.SelectionOid)),
                                                    null)
                                  select n.Oid).ToList();

            if (rv.SelectionType == EnumContactSelectType.SelectionBasket)
                contactOidList = (from n in new XPCollection<DBCTContact>(dxs,
                                                new DevExpress.Data.Filtering.ContainsOperator(
                                                   "Selections",
                                                   new DevExpress.Data.Filtering.BinaryOperator(
                                                       "Oid",
                                                       rv.SelectionOid)),
                                                   null)
                                  select n.Oid).ToList();

            if (contactOidList.Count == 0)
            {
                POLMessageBox.ShowWarning("هیچ پرونده ای جهت مرتب كردن پیدا نشد.", APOCMainWindow.GetWindow());
                return;
            }
            var linked = 0;
            var failed = 0;
            var contactcount = 0;
            POLProgressBox.Show("مرتب كردن فیلد ها", true, 0, contactOidList.Count, 1,
            pb =>
            {
                foreach (var guid in contactOidList)
                {
                    if (pb.NeedToCancel) return;
                    var dbc = DBCTContact.FindByOid(dxs, guid);
                    pb.AsyncSetText(1, dbc.Title);
                    pb.AsyncSetValue(contactcount);
                    contactcount++;
                    foreach (var module in moduleList)
                    {
                        var dbv = DBCTProfileValue.FindByContactAndItem(dxs, guid, module.Oid);
                        if (dbv == null) continue;
                        if (dbv.Order == module.Order) continue;

                        try
                        {
                            dbv.Order = module.Order;
                            dbv.Save();
                            linked++;
                        }
                        catch
                        {
                            failed++;
                        }
                    }
                }
            },
            pb => POLMessageBox.ShowInformation(string.Format("تعداد پرونده پردازش شده : {0}{1}تعداد فیلد های مرتب شده : {2}{1}تعداد خطا ها : {3}", contactcount, Environment.NewLine, linked, failed), pb),
            APOCMainWindow.GetWindow());
        }

        private void ExpandAll()
        {
            DynamicTreeListControl.View.ExpandAllNodes();
        }
        private void CollapseAll()
        {
            DynamicTreeListControl.View.CollapseAllNodes();
        }
        public void Drop(object sender, DevExpress.Xpf.Grid.DragDrop.TreeListDropEventArgs e)
        {
            if (e.TargetNode == null)
            {
                e.Handled = true;
                return;
            }
            if (!(e.TargetNode.Content is CacheItemProfileItem)) { e.Handled = true; return; }
            if (e.DraggedRows.Count == 0) { e.Handled = true; return; }

            APOCMainWindow.ShowBusyIndicator();

            var tcach = (CacheItemProfileItem)e.TargetNode.Content;
            #region Profile
            if (tcach.Tag is DBCTProfileRoot)
            {
                if (((CacheItemProfileItem)((TreeListNode)e.DraggedRows[0]).Content).Tag is DBCTProfileRoot) 
                {
                    var order = tcach.Order;
                    if (e.DropTargetType == DropTargetType.InsertRowsBefore)
                        order--;

                    var other = (from n in ProfileItemList where n.Order > order select n).ToList();
                    other.ForEach(o => o.Order += 10000);
                    var moved = (from n in e.DraggedRows.Cast<TreeListNode>() orderby ((CacheItemProfileItem)n.Content).Order select n.Content as CacheItemProfileItem).ToList();
                    var i = 1;
                    moved.ForEach(n => n.Order = order + (i++));

                    i = 0;
                    ProfileItemList.OrderBy(n => n.Order).ToList().ForEach(n => n.Order = i++);
                    ProfileItemList.ToList().ForEach(
                        n =>
                        {
                            var r = (DBCTProfileRoot)n.Tag;
                            if (r.Order != n.Order)
                            {
                                r.Order = n.Order;
                                r.Save();
                            }
                        });
                    ACacheData.SetProfileItemList(new ObservableCollection<CacheItemProfileItem>(ProfileItemList.OrderBy(n => n.Order)));
                    ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileItem);
                    RaisePropertyChanged("ProfileItemList");
                }
            }
            #endregion

            #region Group
            if (tcach.Tag is DBCTProfileGroup)
            {
                if (((CacheItemProfileItem)((TreeListNode)e.DraggedRows[0]).Content).Tag is DBCTProfileGroup) 
                {
                    var glist = from n in e.DraggedRows.Cast<TreeListNode>()
                                select ((DBCTProfileGroup)((CacheItemProfileItem)n.Content).Tag);
                    var gCount = glist.GroupBy(n => n.ProfileRoot.Oid).Count();
                    if (gCount == 1) 
                    {
                        var order = tcach.Order;
                        if (e.DropTargetType == DropTargetType.InsertRowsBefore)
                            order--;

                        var list =
                            (from r in ProfileItemList
                             where ((DBCTProfileRoot)r.Tag).Oid == glist.First().ProfileRoot.Oid
                             select r).First().ChildList;


                        var other = (from n in list where n.Order > order select n).ToList();
                        other.ForEach(o => o.Order += 10000);
                        var moved = (from n in e.DraggedRows.Cast<TreeListNode>() orderby ((CacheItemProfileItem)n.Content).Order select n.Content as CacheItemProfileItem).ToList();
                        var i = 1;
                        moved.ForEach(n => n.Order = order + (i++));

                        i = 0;
                        list.OrderBy(n => n.Order).ToList().ForEach(n => n.Order = i++);
                        list.ToList().ForEach(
                            n =>
                            {
                                var r = (DBCTProfileGroup)n.Tag;
                                if (r.Order != n.Order)
                                {
                                    r.Order = n.Order;
                                    r.Save();
                                }
                            });

                        var ll = (from r in ProfileItemList
                                  where ((DBCTProfileRoot)r.Tag).Oid == glist.First().ProfileRoot.Oid
                                  select r).First();
                        var vv = list.OrderBy(n => n.Order).ToList();
                        ll.ChildList.Clear();
                        vv.ForEach(n => ll.ChildList.Add(n));
                        ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileItem);
                    }
                }
            }
            #endregion

            #region Item
            if (tcach.Tag is DBCTProfileItem)
            {
                if (((CacheItemProfileItem)((TreeListNode)e.DraggedRows[0]).Content).Tag is DBCTProfileItem) 
                {
                    var glist = from n in e.DraggedRows.Cast<TreeListNode>()
                                select ((DBCTProfileItem)((CacheItemProfileItem)n.Content).Tag);
                    var gCount = glist.GroupBy(n => n.ProfileGroup.Oid).Count();
                    if (gCount == 1) 
                    {
                        var order = tcach.Order;
                        if (e.DropTargetType == DropTargetType.InsertRowsBefore)
                            order--;

                        var list =
                            (from r in ProfileItemList
                             from g in r.ChildList
                             where ((DBCTProfileRoot)r.Tag).Oid == glist.First().ProfileGroup.ProfileRoot.Oid
                             && ((DBCTProfileGroup)g.Tag).Oid == glist.First().ProfileGroup.Oid
                             select g).First().ChildList;


                        var other = (from n in list where n.Order > order select n).ToList();
                        other.ForEach(o => o.Order += 10000);
                        var moved = (from n in e.DraggedRows.Cast<TreeListNode>() orderby ((CacheItemProfileItem)n.Content).Order select n.Content as CacheItemProfileItem).ToList();
                        var i = 1;
                        moved.ForEach(n => n.Order = order + (i++));

                        i = 0;
                        list.OrderBy(n => n.Order).ToList().ForEach(n => n.Order = i++);
                        list.ToList().ForEach(
                            n =>
                            {
                                var r = (DBCTProfileItem)n.Tag;
                                if (r.Order != n.Order)
                                {
                                    r.Order = n.Order;
                                    r.Save();
                                }
                            });

                        var ll = (from r in ProfileItemList
                                  from g in r.ChildList
                                  where ((DBCTProfileRoot)r.Tag).Oid == glist.First().ProfileGroup.ProfileRoot.Oid
                                  && ((DBCTProfileGroup)g.Tag).Oid == glist.First().ProfileGroup.Oid
                                  select g).First();
                        var vv = list.OrderBy(n => n.Order).ToList();
                        ll.ChildList.Clear();
                        vv.ForEach(n => ll.ChildList.Add(n));
                        ACacheData.RaiseCacheChanged(EnumCachDataType.ProfileItem);
                    }
                }
            }
            #endregion
            e.Handled = true;

            APOCMainWindow.HideBusyIndicator();
        }
        private void Help()
        {
            System.Diagnostics.Process.Start(ConstantPOCHelpURL.POCHelp53);
        }
        #endregion

        #region [COMMANDS]
        public RelayCommand CommandProfileRootNew { get; set; }
        public RelayCommand CommandProfileGroupNew { get; set; }
        public RelayCommand CommandProfileItemNew { get; set; }

        public RelayCommand CommandEdit { get; set; }
        public RelayCommand CommandDelete { get; set; }
        public RelayCommand CommandRefresh { get; set; }



        public RelayCommand CommandContactLink { get; set; }
        public RelayCommand CommandContactUnLink { get; set; }
        public RelayCommand CommandContactReorder { get; set; }

        public RelayCommand CommandExportXML { get; set; }
        public RelayCommand CommandExportXMLSelected { get; set; }
        public RelayCommand CommandImportXML { get; set; }

        public RelayCommand CommandClearSearchText { get; set; }

        public RelayCommand CommandExpandAll { get; set; }
        public RelayCommand CommandCollapseAll { get; set; }
        public RelayCommand CommandHelp { get; set; }

        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion
    }
}
