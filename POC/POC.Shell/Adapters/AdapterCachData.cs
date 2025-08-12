using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using POL.DB.P30Office;
using POL.Lib.Interfaces;
using POL.DB.P30Office.GL;

namespace POC.Shell.Adapters
{
    internal class AdapterCacheData : ICacheData
    {
        private ILoggerFacade ALog { get; set; }
        private IMessagingClient AMessagingClient { get; set; }
        private POCCore APOCCore { get; set; }
        private IMembership AMembership { get; set; }
        private IDatabase ADatabase { get; set; }
        private IDataFieldManager ADataFieldManager { get; set; }


        private bool NeedUpdateProfileItem { get; set; }
        private bool NeedUpdateRoles { get; set; }
        private bool NeedUpdateContactCat { get; set; }
        private bool NeedUpdateCountry { get; set; }
        private bool NeedUpdateProfileTable { get; set; }


        #region CTOR
        public AdapterCacheData(ILoggerFacade log, IMembership membership, POCCore pocCore, IMessagingClient messageing, IDatabase database, IDataFieldManager dataFieldManager)
        {
            ALog = log;
            AMessagingClient = messageing;
            APOCCore = pocCore;
            AMembership = membership;
            ADatabase = database;
            ADataFieldManager = dataFieldManager;

            NeedUpdateProfileItem = true;
            NeedUpdateRoles = true;
            NeedUpdateContactCat = true;
            NeedUpdateCountry = true;
            NeedUpdateProfileTable = true;

            AMembership.OnMembershipStatusChanged += 
                (s, e) =>
                {
                    try
                    {
                        if (e.Status == EnumMembershipStatus.AfterLogin)
                        {
                            PopulateAll(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        ALog.Log(ex.ToString(), Category.Exception, Priority.High);
                    }
                };

            AMessagingClient.RegisterHookForMessage(
                m =>
                {
                    if (!AMembership.IsAuthorized) return;
                    try
                    {
                        var en = (EnumCachDataType)Enum.Parse(typeof(EnumCachDataType), m.MessageData[0]);
                        if (APOCCore.CTSI.ID == m.From)
                        {
                            var def = Math.Abs((DateTime.Now - m.MessageDate).TotalSeconds);
                            if (def <= 3)
                            {
                                return;
                            }
                        }
                        switch (en)
                        {
                            case EnumCachDataType.ProfileItem:
                                NeedUpdateProfileItem = true;
                                break;
                            case EnumCachDataType.Roles:
                                NeedUpdateRoles = true;
                                break;
                            case EnumCachDataType.ContactCat:
                                NeedUpdateContactCat = true;
                                break;
                            case EnumCachDataType.Country:
                                NeedUpdateCountry = true;
                                break;
                            case EnumCachDataType.ProfileTable:
                                NeedUpdateProfileTable = true;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("en");
                        }
                    }
                    catch (Exception ex)
                    {
                        ALog.Log(ex.ToString(), Category.Exception, Priority.Medium);
                        throw;
                    }
                }, EnumMessageKind.CacheChanged);
        }
        #endregion


        #region ICacheData
        public void PopulateAll()
        {
            ForcePopulateCache(EnumCachDataType.ProfileItem, true, null);
            ForcePopulateCache(EnumCachDataType.Roles, true, null);
            ForcePopulateCache(EnumCachDataType.ContactCat, true, null);
            ForcePopulateCache(EnumCachDataType.Country, true, null);
            ForcePopulateCache(EnumCachDataType.ProfileTable, true, null);
        }
        public void ForcePopulateCache(EnumCachDataType type, bool rebuild, object db)
        {
            switch (type)
            {
                case EnumCachDataType.ProfileItem:
                    NeedUpdateProfileItem = rebuild;
                    PopulateProfileItemList(db);
                    break;
                case EnumCachDataType.Roles:
                    NeedUpdateRoles = rebuild;
                    PopulateRoleList();
                    break;
                case EnumCachDataType.ContactCat:
                    NeedUpdateContactCat = rebuild;
                    PopulateContactCatList(db);
                    break;
                case EnumCachDataType.Country:
                    NeedUpdateCountry = rebuild;
                    PopulateCountryList(db);
                    break;
                case EnumCachDataType.ProfileTable:
                    NeedUpdateProfileTable = rebuild;
                    PopulateProfileTableList(db);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
        public void RaiseCacheChanged(EnumCachDataType type)
        {
            AMessagingClient.SendMessage(new MessagingItem
            {
                From = APOCCore.CTSI.ID,
                MessageData = new string[] { type.ToString() },
                MessageDate = DateTime.Now.AddSeconds(2),
                MessageKind = EnumMessageKind.CacheChanged,
                Oid = Guid.Empty,
                PermissionBaseType = EnumPermissionBaseType.EveryOne,
                PermissionValue = null,
                To = Guid.Empty
            });
        }

        public ObservableCollection<CacheItemProfileItem> GetProfileItemList()
        {
            if (NeedUpdateProfileItem)
                ForcePopulateCache(EnumCachDataType.ProfileItem, true, null);
            return ProfileItemList;
        }
        public void SetProfileItemList(ObservableCollection<CacheItemProfileItem> list)
        {
            ProfileItemList = list;
        }

        public ObservableCollection<CacheItemRoleItem> GetRoleList()
        {
            if (NeedUpdateRoles)
                ForcePopulateCache(EnumCachDataType.Roles, true, null);
            return RoleList;
        }
        public void SetRoleList(ObservableCollection<CacheItemRoleItem> list)
        {
            RoleList = list;
        }

        public ObservableCollection<CacheItemContactCat> GetContactCatList()
        {
            if (NeedUpdateContactCat)
                ForcePopulateCache(EnumCachDataType.ContactCat, true, null);
            return ContactCatList;
        }
        public void SetRoleList(ObservableCollection<CacheItemContactCat> list)
        {
            ContactCatList = list;
        }

        public ObservableCollection<CacheItemCountry> GetCountryList()
        {
            if (NeedUpdateCountry)
                ForcePopulateCache(EnumCachDataType.Country, true, null);
            return CountryList;
        }
        public void SetRoleList(ObservableCollection<CacheItemCountry> list)
        {
            CountryList = list;
        }

        public ObservableCollection<CacheItemProfileTable> GetProfileTableList()
        {
            if (NeedUpdateProfileTable)
                ForcePopulateCache(EnumCachDataType.ProfileTable, true, null);
            return ProfileTableList;
        }
        public void SetProfileTableList(ObservableCollection<CacheItemProfileTable> list)
        {
            ProfileTableList = list;
        }
        #endregion

        private ObservableCollection<CacheItemProfileItem> ProfileItemList { get; set; }
        private ObservableCollection<CacheItemRoleItem> RoleList { get; set; }
        private ObservableCollection<CacheItemContactCat> ContactCatList { get; set; }
        private ObservableCollection<CacheItemCountry> CountryList { get; set; }
        private ObservableCollection<CacheItemProfileTable> ProfileTableList { get; set; }

        private void PopulateProfileItemList(object db)
        {
            if (ProfileItemList == null || NeedUpdateProfileItem)
            {
                ProfileItemList = new ObservableCollection<CacheItemProfileItem>();
                NeedUpdateProfileItem = false;
            }
            List<DBCTProfileRoot> ps;
            if (db == null || db is DBCTProfileRoot)
                ps = DBCTProfileRoot.GetAll(ADatabase.Dxs).ToList();
            else
                ps = (from r in ProfileItemList orderby r.Order, r.Title select ((DBCTProfileRoot)r.Tag)).ToList();

            if (ps.Count < ProfileItemList.Count)
            {
                var li = (from n in ProfileItemList select ((DBCTProfileRoot)n.Tag)).ToList();
                var def = li.Except(ps);

                def.ToList().ForEach(
                    d =>
                    {
                        var delitem = (from n in ProfileItemList where ((DBCTProfileRoot)n.Tag).Oid == d.Oid select n).First();
                        ProfileItemList.Remove(delitem);
                    });
                return;
            }

            ps.ForEach(
                root =>
                {
                    CacheItemProfileItem itemR;
                    var qr = from n in ProfileItemList where ((DBCTProfileRoot)n.Tag).Oid == root.Oid select n;
                    if (!qr.Any())
                    {
                        itemR = new CacheItemProfileItem
                                        {
                                            Title = root.Title,
                                            ImageUriString = DBCTProfileRoot.GetProfileItemUriString(),
                                            Tag = root,
                                            Order = root.Order,
                                            ProfileItemType = EnumProfileItemType.Bool,
                                            TreeLevel = 0,
                                            ChildList = new ObservableCollection<CacheItemProfileItem>(),
                                            RoleEditor = root.RoleEditor,
                                            RoleViewer = root.RoleViewer,
                                            CategoriesString = string.Join(", ", root.ContactCats.ToList().Select(n => n.Title)),
                                        };
                        ProfileItemList.Add(itemR);
                    }
                    else
                    {
                        itemR = qr.First();
                        if (db is DBCTProfileRoot)
                        {
                            itemR.Title = root.Title;
                            itemR.ImageUriString = DBCTProfileRoot.GetProfileItemUriString();
                            itemR.Tag = root;
                            itemR.Order = root.Order;
                            itemR.RoleEditor = root.RoleEditor;
                            itemR.RoleViewer = root.RoleViewer;
                            itemR.CategoriesString = string.Join(", ", root.ContactCats.ToList().Select(n => n.Title));
                        }
                    }

                    if (db is DBCTProfileRoot) return;
                    if (db is DBCTProfileGroup && ((DBCTProfileGroup)db).ProfileRoot.Oid != root.Oid) return;
                    if (db is DBCTProfileItem && ((DBCTProfileItem)db).ProfileGroup.ProfileRoot.Oid != root.Oid) return;

                    var gs = new List<DBCTProfileGroup>();
                    if (db == null || db is DBCTProfileGroup)
                        gs = DBCTProfileGroup.GetAll(ADatabase.Dxs, root.Oid).ToList();
                    else
                        gs = (from g in itemR.ChildList orderby g.Order, g.Title select ((DBCTProfileGroup)g.Tag)).ToList();

                    if (gs.Count < itemR.ChildList.Count)
                    {
                        var li = (from n in itemR.ChildList select ((DBCTProfileGroup)n.Tag)).ToList();
                        var def = li.Except(gs);

                        def.ToList().ForEach(
                            d =>
                            {
                                var delitem = (from n in itemR.ChildList where ((DBCTProfileGroup)n.Tag).Oid == d.Oid select n).First();
                                itemR.ChildList.Remove(delitem);
                            });
                        return;
                    }

                    gs.ForEach(
                        group1 =>
                        {
                            CacheItemProfileItem itemG;
                            var qg = from n in itemR.ChildList where ((DBCTProfileGroup)n.Tag).Oid == group1.Oid select n;
                            if (!qg.Any())
                            {
                                itemG = new CacheItemProfileItem
                                            {
                                                Title = group1.Title,
                                                Tag = group1,
                                                ImageUriString = DBCTProfileGroup.GetProfileItemUriString(),
                                                Order = group1.Order,
                                                ProfileItemType = EnumProfileItemType.Bool,
                                                TreeLevel = 1,
                                                ChildList = new ObservableCollection<CacheItemProfileItem>(),
                                                RoleEditor = string.Empty,
                                                RoleViewer = string.Empty,
                                            };
                                itemR.ChildList.Add(itemG);
                            }
                            else
                            {
                                itemG = qg.First();
                                if (db is DBCTProfileGroup)
                                {
                                    itemG.Title = group1.Title;
                                    itemG.ImageUriString = DBCTProfileGroup.GetProfileItemUriString();
                                    itemG.Tag = group1;
                                    itemG.Order = group1.Order;
                                }
                            }

                            if (db is DBCTProfileGroup) return;
                            if (db is DBCTProfileItem && ((DBCTProfileItem)db).ProfileGroup.Oid != group1.Oid) return;

                            var ms = new List<DBCTProfileItem>();
                            if (db == null || db is DBCTProfileItem)
                                ms = DBCTProfileItem.GetAll(ADatabase.Dxs, group1.Oid).ToList();
                            else
                                ms = (from i in itemG.ChildList orderby i.Order, i.Title select ((DBCTProfileItem)i.Tag)).ToList();

                            if (ms.Count < itemG.ChildList.Count)
                            {
                                var li = (from n in itemG.ChildList select ((DBCTProfileItem)n.Tag)).ToList();
                                var def = li.Except(ms);

                                def.ToList().ForEach(
                                    d =>
                                    {
                                        var delitem = (from n in itemG.ChildList where ((DBCTProfileItem)n.Tag).Oid == d.Oid select n).First();
                                        itemG.ChildList.Remove(delitem);
                                    });
                                return;
                            }
                            ms.ForEach(
                                item =>
                                {
                                    CacheItemProfileItem itemI;
                                    var qi = from n in itemG.ChildList where ((DBCTProfileItem)n.Tag).Oid == item.Oid select n;
                                    if (!qi.Any())
                                    {
                                        var field = ADataFieldManager.FindByType(item.ItemType);
                                        itemI = new CacheItemProfileItem
                                                    {
                                                        Title = item.Title,
                                                        Tag = item,
                                                        ImageUriString = field == null ? string.Empty : field.ImageUriString,
                                                        Order = item.Order,
                                                        ProfileItemType = item.ItemType,
                                                        TreeLevel = 2,
                                                        ChildList = new ObservableCollection<CacheItemProfileItem>(),
                                                        RoleEditor = string.Empty,
                                                        RoleViewer = string.Empty,
                                                    };
                                        itemG.ChildList.Add(itemI);
                                    }
                                    else
                                    {
                                        itemI = qi.First();
                                        if (db is DBCTProfileItem)
                                        {
                                            var field = ADataFieldManager.FindByType(item.ItemType);

                                            itemI.Title = item.Title;
                                            itemI.ImageUriString = field == null ? string.Empty : field.ImageUriString;
                                            itemI.Tag = item;
                                            itemI.Order = item.Order;
                                        }
                                    }
                                });

                        });
                });
        }
        private void PopulateRoleList()
        {
            if (RoleList == null || NeedUpdateRoles)
            {
                RoleList = new ObservableCollection<CacheItemRoleItem>();
                NeedUpdateRoles = false;
            }
            RoleList.Clear();
            var rs = POL.DB.Membership.DBMSRole2.RoleGetAll(ADatabase.Dxs, null);
            foreach (var r in rs)
            {
                RoleList.Add(new CacheItemRoleItem { Title = r.Title, Tag = r });
            }
        }
        private void PopulateContactCatList(object db)
        {
            if (ContactCatList == null || NeedUpdateContactCat)
            {
                ContactCatList = new ObservableCollection<CacheItemContactCat>();
                NeedUpdateContactCat = false;
            }
            if (db == null)
            {
                ContactCatList.Clear();
                var rs = DBCTContactCat.GetAll(ADatabase.Dxs);
                foreach (var r in rs)
                {
                    ContactCatList.Add(new CacheItemContactCat
                                           {
                                               Title = r.Title,
                                               Tag = r,
                                               Role = r.Role,
                                               ProfileRoots = string.Join(", ", r.ProfileRoots.ToList().Select(n => n.Title))
                                           });
                }
            }
            else
            {
                var dbc = (DBCTContactCat)db;
                var q = (from n in ContactCatList where ((DBCTContactCat)n.Tag).Oid == dbc.Oid select n).FirstOrDefault();
                if (q != null)
                {
                    q.Tag = db;
                    q.Title = dbc.Title;
                    q.Role = dbc.Role;
                    if (dbc.IsDeleted)
                        ContactCatList.Remove(q);
                }
                else 
                {
                    ContactCatList.Add(new CacheItemContactCat
                                           {
                                               Title = dbc.Title,
                                               Tag = dbc,
                                               Role = dbc.Role,
                                               ProfileRoots = string.Join(", ", dbc.ProfileRoots.ToList().Select(n => n.Title))
                                           });
                }
            }
        }

        private void PopulateCountryList(object db)
        {
            if (CountryList == null || NeedUpdateCountry)
            {
                CountryList = new ObservableCollection<CacheItemCountry>();
                NeedUpdateCountry = false;
            }
            if (db == null)
            {
                CountryList.Clear();
                var rs = DBGLCountry.GetAll(ADatabase.Dxs);
                foreach (var r in rs)
                {
                    CountryList.Add(new CacheItemCountry { Title = r.TitleXX, Tag = r, ISO3 = r.ISO3, TeleCodeString = r.TeleCodeString });
                }
            }
            else
            {
                if (db is DBGLCountry)
                {
                    var dbc = (DBGLCountry)db;
                    var q = (from n in CountryList where ((DBGLCountry)n.Tag).Oid == dbc.Oid select n).FirstOrDefault();
                    if (q != null)
                    {
                        q.Tag = db;
                        q.Title = dbc.TitleXX;
                        q.ISO3 = dbc.ISO3;
                        q.TeleCodeString = dbc.TeleCodeString;
                    }
                }
            }
        }
        private void PopulateProfileTableList(object db)
        {
            if (ProfileTableList == null || NeedUpdateProfileTable)
            {
                ProfileTableList = new ObservableCollection<CacheItemProfileTable>();
                NeedUpdateProfileTable = false;
            }
            if (db == null)
            {
                ProfileTableList.Clear();
                var rs = DBCTProfileTable.GetAll(ADatabase.Dxs);
                foreach (var r in rs)
                {
                    ProfileTableList.Add(new CacheItemProfileTable { Title = r.Title, Tag = r, ValueDepth = r.ValueDepth, ChildList = GetTValueChildList(r) });
                }
            }
            else
            {
                if (db is DBCTProfileTable)
                {
                    var dbc = (DBCTProfileTable)db;
                    var q = (from n in ProfileTableList where ((DBCTProfileTable)n.Tag).Oid == dbc.Oid select n).FirstOrDefault();
                    if (q != null)
                    {
                        q.Tag = db;
                        q.Title = dbc.Title;
                        if (dbc.IsDeleted)
                            ProfileTableList.Remove(q);
                    }
                    else
                    {
                        ProfileTableList.Add(new CacheItemProfileTable { Title = dbc.Title, Tag = dbc });
                    }
                }

                if (db is DBCTProfileTValue)
                {
                    var dbc = (DBCTProfileTValue)db;
                    var qTable = (from n in ProfileTableList where ((DBCTProfileTable)n.Tag).Oid == dbc.Table.Oid select n).FirstOrDefault();
                    if (qTable == null)
                        return;
                    if (dbc.Parent == null) 
                    {
                        if (dbc.IsDeleted)
                        {
                            var q2 = from n in qTable.ChildList
                                     from m in n.ChildList
                                     where ((DBCTProfileTValue)m.Tag).Oid == dbc.Oid
                                     select new { n, m };
                            if (q2.Any())
                            {
                                var v = q2.First();
                                v.n.ChildList.Remove(v.m);
                                return;
                            }
                        }

                        var q = (from n in qTable.ChildList where ((DBCTProfileTValue)n.Tag).Oid == dbc.Oid select n).FirstOrDefault();

                        if (q != null)
                        {
                            q.Tag = db;
                            q.Title = dbc.Title;
                            q.Order = dbc.Order;
                            if (dbc.IsDeleted)
                                qTable.ChildList.Remove(q);
                        }
                        else
                        {
                            if (qTable.ChildList != null)
                                qTable.ChildList.Add(new CacheItemProfileTValue { Title = dbc.Title, Tag = dbc, Order = dbc.Order });
                            else
                            {
                                qTable.ChildList = new ObservableCollection<CacheItemProfileTValue>
                                                       {
                                                           new CacheItemProfileTValue
                                                               {Title = dbc.Title, Tag = dbc, Order = dbc.Order}
                                                       };
                            }
                        }
                    }
                    else 
                    {
                        var qParent = (from n in qTable.ChildList where ((DBCTProfileTValue)n.Tag).Oid == dbc.Parent.Oid select n).FirstOrDefault();
                        if(qParent==null)return;
                        var q = (from n in qParent.ChildList where ((DBCTProfileTValue)n.Tag).Oid == dbc.Oid select n).FirstOrDefault();

                        if (q != null)
                        {
                            q.Tag = db;
                            q.Title = dbc.Title;
                            q.Order = dbc.Order;
                            if (dbc.IsDeleted)
                                qParent.ChildList.Remove(q);
                        }
                        else
                        {
                            if (qParent.ChildList != null)
                                qParent.ChildList.Add(new CacheItemProfileTValue { Title = dbc.Title, Tag = dbc, Order = dbc.Order });
                            else
                            {
                                qParent.ChildList = new ObservableCollection<CacheItemProfileTValue>
                                                       {
                                                           new CacheItemProfileTValue
                                                               {Title = dbc.Title, Tag = dbc, Order = dbc.Order}
                                                       };
                            }
                        }
                    }

                }
            }
        }

        private ObservableCollection<CacheItemProfileTValue> GetTValueChildList(DBCTProfileTable dbpt)
        {
            var rv = new ObservableCollection<CacheItemProfileTValue>();
            var vals = DBCTProfileTValue.GetAll(dbpt.Session, dbpt.Oid);
            foreach (var v in vals)
            {
                rv.Add(new CacheItemProfileTValue { Title = v.Title, Order = v.Order, Tag = v, ChildList = GetTValueChildList(v) });
            }
            return rv;
        }
        private ObservableCollection<CacheItemProfileTValue> GetTValueChildList(DBCTProfileTValue dbptv)
        {
            var rv = new ObservableCollection<CacheItemProfileTValue>();
            foreach (var v in dbptv.Children)
            {
                rv.Add(new CacheItemProfileTValue { Title = v.Title, Order = v.Order, Tag = v, ChildList = GetTValueChildList(v) });
            }
            return rv;
        }



    }
}
